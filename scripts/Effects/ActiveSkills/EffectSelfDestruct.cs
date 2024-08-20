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

public class EffectSelfDestruct : FightEffectWithNoFlinch
{
    protected EffectConfig.SelfDestructConfig Config;
    
    public override bool IsActiveEffect => true;
    public override bool BlockAbilityActivation => true;
    public override bool IsValidTarget => false;
    
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
        var cbPlayers = FightClubGameManager.Instance.OverlapCircleForCombatPlayers(selfPos, EffectConfig.SelfDestructConfig.BlastRange);
            
        foreach (var fp in cbPlayers)
        {
            if (fp == FightPlayer)
            {
                // Self damage
                FightPlayer.DamageInfo selfDmgInfo = FightPlayer.DamageInfo.CreateSelfDamageInfo(Config.SelfDamage) with{SkillKey = SkillConfig.SelfDestructConfig.SkillKey};
                FightPlayer.TakeDamage(FightPlayer, selfDmgInfo);
            }
            else
            {
                if (fp.Damageable())
                {
                    FightPlayer.DamageInfo info = FightPlayer.DamageInfo.CreateDamageInfo(Config.BlastDamage, DamageType.AOE);
                    info.SkillKey = SkillConfig.SelfDestructConfig.SkillKey;
                    fp.TakeDamage(FightPlayer, info);
                        
                    Vector2 bumpDir = fp.Entity.Position - selfPos;
                    fp.AddBumpFrom(FightPlayer, bumpDir.Normalized * EffectConfig.SelfDestructConfig.BumpStrength, false);
                }
            }
        }
    }
}