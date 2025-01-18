using AO;
using Assembly.scripts.SceneObjects;
using Assembly.scripts.VFX;

namespace Assembly.scripts.Effects.ActiveSkills;



public class AbilitySelfDestruct : FightAbility
{
    public override string SkillKey => "SelfDestruct";
    public override Type Effect => typeof(EffectSelfDestruct);
    public override bool MonitorEffectDuration => true;
    public override TargettingMode TargettingMode => TargettingMode.Self;
    public override float Cooldown => GetCooldown(FightPlayer);

    public static float GetCooldown(FightPlayer fp)
    {
        int lv = fp.GetSkillTree().GetSkillLevel("SelfDestruct");
        float cd = EffectConfig.SelfDestructConfig.Cooldown;
        if (lv > 1)
        {
            cd -= 1;
        }

        if (lv > 3)
        {
            cd -= 1;
        }

        return cd;
    }
}

public class EffectSelfDestruct : FightEffectWithImmunity
{
    protected EffectConfig.SelfDestructConfig Config;
    
    public override bool IsActiveEffect => true;
    public override bool BlockAbilityActivation => true;
    public override bool IsValidTarget => false;

    protected override string InvincibilityReason => "SelfDestruct";

    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        FightStateMachine.SetTrigger("self_destruct");
        AssignConfig(EffectConfig.SelfDestructConfig.GetDefault(FightPlayer.CurrentAttack, FightPlayer.GetSkillTree().GetSkillLevel("SelfDestruct")));
        if (!isDropIn)
        {
            DurationRemaining = FightLayer.GetCurrentStateLength();
            SoundId = SFX.Play(SFXKeys.SelfDestructAudio, DefaultSoundDesc);
        }
        
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);

        SFX.Play(SFXKeys.SelfDestructExplodeAudio, DefaultSoundDesc);
        KnockingBlast();
        FightClubGameManager.Instance.ClientSpawn(VFXPrefabKeys.SelfDestructExplosionPath, FightPlayer.Entity.Position);
        SFX.FadeOutAndStop(SoundId, 0.25f);
    }
    

    protected void AssignConfig(EffectConfig.SelfDestructConfig cfg)
    {
        Config = cfg;
    }

    private void KnockingBlast()
    {
        Vector2 selfPos = FightPlayer.Entity.Position;
        var cbPlayers = FightClubGameManager.Instance.OverlapCircleForDamageables(selfPos, EffectConfig.SelfDestructConfig.BlastRange, Player);
        FightPlayer.DamageInfo info = FightPlayer.DamageInfo.CreateDamageInfo(Config.BlastDamage, DamageType.AOE);
        info.SkillKey = SkillConfig.SelfDestructConfig.SkillKey;
        info.SpecialDeathAnimation = true;
        info.CrateImmediateDestroy = true;
        
        foreach (var dmg in cbPlayers)
        {
            if(!dmg.Damageable()) continue;
            if (dmg is PlayerCollisionChild fp)
            {
                if (fp.Player != FightPlayer)
                {
                    Vector2 bumpDir = fp.Entity.Position - selfPos;
                    fp.Player.AddBumpFrom(FightPlayer, bumpDir.Normalized * EffectConfig.SelfDestructConfig.BumpStrength, false);
                }
            }
            dmg.TakeDamage(FightPlayer, info);
        }
        
        // Self damage
        FightPlayer.DamageInfo selfDmgInfo = FightPlayer.DamageInfo.CreateSelfDamageInfo(Config.SelfDamage) with{SkillKey = SkillConfig.SelfDestructConfig.SkillKey};
        FightPlayer.TakeDamage(FightPlayer, selfDmgInfo);
    }
}