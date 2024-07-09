using System.Collections;
using AO;

public class AbilityPunch : FightAbility
{
    public override string SkillKey => "Punch";
    public override Type Effect => typeof(EffectPunch);
    public override bool MonitorEffectDuration => false;
    public override TargettingMode TargettingMode => TargettingMode.Self;
    
}

public class EffectPunch : FightEffect
{
    protected EffectConfig.PunchConfig Config;
    public override bool IsActiveEffect => false;
    public override bool BlockAbilityActivation => true;
    public override bool IsValidTarget => true;

    protected bool Activated = false;
    
    public override void OnEffectStart()
    {
        base.OnEffectStart();
        Vector2 punchAim = Entity.Position + FightPlayer.GetPunchDirection();
        FightPlayer.SetMouseIKPosition(punchAim);
        
        AssignConfig(EffectConfig.GetPlayerPunchConfig(FightPlayer.PunchLevel, FightPlayer.CurrentAttack));
        FightPlayer.SetAnimTrigger(Config.AnimationTrigger); ;
    }
    

    public override void OnEffectUpdate()
    {
        if (Util.OneTime(ElapsedTime > EffectConfig.PunchConfig.PunchActivationTime, ref Activated))
        {
            Punch();
            Activated = true;
        }
    }

    public void AssignConfig(EffectConfig.PunchConfig cfg)
    {
        DurationRemaining = EffectConfig.PunchConfig.PunchAnimationTime;
        Config = cfg;
    }
    public void Punch()
    {
        Physics.RaycastHit rc;

        
        var hit = Physics.RaycastWithWhitelist(Entity.Position, FightPlayer.GetPunchDirection(),
            EffectConfig.PunchConfig.PunchRange, FightClubGameManager.Instance.GetCombatPlayersCollisionEntities(), new Entity[]{ },out rc);

        
        if (hit)
        {
            Log.Debug($"{rc.Entity.Name}");
            var other = rc.Entity.GetComponent<PlayerCollisionChild>();
            if (other != null)
            {
                FightPlayer.DamageInfo info = FightPlayer.DamageInfo.CreateDamageInfo(Config.PunchDamage);
                other.Player.TakeDamage(FightPlayer, info);
            }
            
        }
    }
    


}