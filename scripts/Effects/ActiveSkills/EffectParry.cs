using AO;
using Assembly.scripts.SceneObjects;

namespace Assembly.scripts.Effects.ActiveSkills;

public class AbilityParry : FightAbility
{
    public override string SkillKey => "Parry";
    
    public override Type Effect => typeof(EffectParry);
    public override bool MonitorEffectDuration => true;
    public override TargettingMode TargettingMode => TargettingMode.Self;

    public override float Cooldown => GetCooldown(FightPlayer);
    
    public static float GetCooldown(FightPlayer fp)
    {
        int lv = fp.GetSkillTree().GetSkillLevel("Parry"); // Reduce 1 cooldown for the first level
        return lv > 1 ? EffectConfig.ParryConfig.Cooldown - 1 : EffectConfig.ParryConfig.Cooldown;
    }
}

public class EffectParry : FightEffect
{
    public override bool IsActiveEffect => true;
    private EffectConfig.ParryConfig _config;

    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        _config = EffectConfig.ParryConfig.GetDefault(FightPlayer.CurrentAttack, FightPlayer.GetSkillTree().GetSkillLevel("Parry"));
        if (!isDropIn)
        {
            FightPlayer.SetAnimTrigger("parry_start"); // We can omit this trigger for drop-in, this effect should be < 2s
            SFX.Play(SFXKeys.ParryStartAudio, DefaultSoundDesc);
            DurationRemaining = _config.ParryTime;
        }
        
        FightPlayer.RegisterPreDamageEvent(this);
        FightPlayer.AddSpeedModifier(0);
        

        // TODO: Dynamically add a projectile reflector on the player. it should be smaller than the hitbox to trigger the pre-damage mod
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        if (!interrupt)
        {
            FightPlayer.SetAnimTrigger("parry_end");
        }
        FightPlayer.RemoveSpeedModifier(0);
        FightPlayer.RemovePreDamageEvent(this);
    }

    public override void PreDamageMod(ref FightPlayer.DamageInfo info)
    {
        base.PreDamageMod(ref info);
        if (info.DmgType == DamageType.Melee || info.DmgType == DamageType.Ranged)
        {
            info.ReactionInfo.Amount = 0;
            info.OverrideDamageNumber = FightPlayer.DamageInfo.DamageNumberOverrideType.Parry;
            info.ReactionInfo.Flinch = false;
            // Player.RemoveEffect(this, true);
            CounterAttack();
            DurationRemaining = 0.01f; // We can't call RemoveEffect because this function is called from an iterator
        }
    }

    public void CounterAttack()
    {
        FightPlayer.SetAnimTrigger("parry_attack");
        SFX.Play(SFXKeys.ParryAttackAudio, DefaultSoundDesc);
        Vector2 selfPos = FightPlayer.Entity.Position;
        var cbPlayers = FightClubGameManager.Instance.OverlapCircleForDamageables(selfPos, EffectConfig.ParryConfig.CounterAttackRange, Player);
        FightPlayer.DamageInfo info = FightPlayer.DamageInfo.CreateDamageInfo(_config.Dmg, DamageType.AOE);
        info.SkillKey = "Parry";
        info.CrateImmediateDestroy = true;
        
        foreach (var dmg in cbPlayers)
        {
            if(!dmg.Damageable()) continue;
            if (dmg is PlayerCollisionChild fp)
            {
                if (fp.Player != FightPlayer)
                {
                    Vector2 bumpDir = fp.Entity.Position - selfPos;
                    fp.Player.AddBumpFrom(FightPlayer, bumpDir.Normalized * EffectConfig.ParryConfig.BumpStrength, false);
                }
            }
            dmg.TakeDamage(FightPlayer, info);
        }

        FightPlayer.AddEffect<EffectNoMovement>(Player, 0.25f);


    }
}