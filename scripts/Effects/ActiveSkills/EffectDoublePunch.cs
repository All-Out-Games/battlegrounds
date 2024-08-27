using Assembly.scripts.SceneObjects;

namespace Assembly.scripts.Effects.ActiveSkills;
using System.Collections;
using AO;

public class AbilityDoublePunch : FightAbility
{
    public override string SkillKey => "DoublePunch";
    public override Type Effect => typeof(EffectDoublePunch);
    public override bool MonitorEffectDuration => false;
    public override TargettingMode TargettingMode => TargettingMode.Self;
    public override float Cooldown => GetCooldown(FightPlayer);

    public static float GetCooldown(FightPlayer fp)
    {
        return fp.GetSkillTree().GetSkillLevel("DoublePunch") > 4
            ? EffectConfig.DoublePunchConfig.Cooldown - 1
            : EffectConfig.DoublePunchConfig.Cooldown;
    }
}

public class EffectDoublePunch : FightEffect
{
    protected EffectConfig.DoublePunchConfig Config;
    public override bool IsActiveEffect => true;
    public override bool BlockAbilityActivation => true;
    public override bool IsValidTarget => true;

    private int _punchIndex;

    private Vector2 punchDir;
    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        
        AssignConfig(EffectConfig.DoublePunchConfig.GetDefault(FightPlayer.CurrentAttack, FightPlayer.GetSkillTree().GetSkillLevel("DoublePunch")));
        
        FightPlayer.SetAnimTrigger("doublepunch");
        FightPlayer.SpineAnimator.OnEvent += OnAnimationEvent;
        
        SoundId = SFX.Play(SFXKeys.DoublePunchAudio, DefaultSoundDesc);
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        FightPlayer.SpineAnimator.OnEvent -= OnAnimationEvent;
    }

    public override void OnAnimationEvent(string eventName)
    {
        base.OnAnimationEvent(eventName);
        if (eventName == "Attack")
        {
            _punchIndex++;
            DoublePunch(_punchIndex);
        }
    }

    public void AssignConfig(EffectConfig.DoublePunchConfig cfg)
    {
        DurationRemaining = EffectConfig.DoublePunchConfig.PunchAnimationTime * 2;
        Config = cfg;
    }
    
    public void DoublePunch(int punchType)
    {
        // The first punch stuns the enemy if hit. The second punch knock them back
        
        // Re-adjust aiming
        punchDir = FightPlayer.GetPunchDirection();
        FightPlayer.SetAimTarget(Entity.Position + punchDir);
        
        
        Physics.RaycastHit rc;
        var hit = Physics.RaycastWithWhitelist(Entity.Position, FightPlayer.GetPunchDirection(),
            EffectConfig.DoublePunchConfig.PunchRange, FightClubGameManager.Instance.GetCombatPlayersCollisionEntities(Player),new Entity[]{ }, out rc);



        /*hit = Physics.Raycast(Entity.Position, FightPlayer.GetPunchDirection(),
            EffectConfig.PunchConfig.PunchRange, out rc);*/

        if (hit)
        {
            DamageableObject other = rc.Collider.GetComponent<DamageableObject>();
            FightPlayer.DamageInfo info = FightPlayer.DamageInfo.CreateDamageInfo(Config.PunchDamage);
            // Effect
            if (other is PlayerCollisionChild fdb)
            {
                if (punchType == 1)
                {
                    // Stunning Punch
                    fdb.Player.GetEffectMgr().AddStun(FightPlayer.Entity, EffectConfig.DoublePunchConfig.PunchAnimationTime);
                }
                else
                {
                    // Bumping Punch
                    info.InterruptLevel = FightPlayer.DamageInfo.KnockBackInterruptLevel;
                    Vector2 bumpDir = other.Entity.Position - FightPlayer.Entity.Position;
                    fdb.Player.AddBumpFrom(FightPlayer, bumpDir * Config.BumpStrength, false);
                }
            }
            // Damage
            info.SkillKey = SkillConfig.DoublePunchConfig.SkillKey;
            other.TakeDamage(FightPlayer, info);
        }
    }
    
    

}