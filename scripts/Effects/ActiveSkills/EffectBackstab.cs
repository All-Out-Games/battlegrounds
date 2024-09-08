using AO;
using Assembly.scripts.SceneObjects;
using Assembly.scripts.SceneObjects.Projectiles;

namespace Assembly.scripts.Effects.ActiveSkills;

public class AbilityBackstab : FightAbility
{
    public override string SkillKey => "Backstab";

    public override Type Effect => typeof(EffectKunaiThrow);
    public override bool MonitorEffectDuration => false;
    public override TargettingMode TargettingMode => TargettingMode.Line;
    public override float MaxDistance => EffectConfig.BackStabConfig.KunaiRange;
    public override int MaxTargets => 1;
    
    public override float Cooldown => GetCooldown(FightPlayer);

    public static float GetCooldown(FightPlayer fp)
    {
        int lv = fp.GetSkillTree().GetSkillLevel("Backstab");
        if (lv > 3)
        {
            return EffectConfig.BackStabConfig.Cooldown - 1;
        }
        else
        {
            return EffectConfig.BackStabConfig.Cooldown;
        }
    }
}
public class EffectBackstab : EffectNoMovement
{
    // This effect triggers after the kunai hits
    // Play the victim animation and takes damage from the caster
    public override bool IsActiveEffect => false;
    public override bool BlockAbilityActivation => true;
    public override bool FreezePlayer => true;

    private bool _damaged = false;
    private EffectConfig.BackStabConfig _config;
    private FightPlayer _fp; // Caster FP

    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        _fp = Caster as FightPlayer;
        if (_fp == null)
        {
            FightPlayer.RemoveEffect<EffectBackstab>(false);
            return;
        }
        FightPlayer.SetAnimTrigger("backstabbed");
        DurationRemaining = EffectConfig.BackStabConfig.BackstabTime + 0.4f;
        SoundId = SFX.Play(SFXKeys.BackStabVictimAudio, DefaultSoundDesc);
    }
    

    public override void OnEffectUpdate()
    {
        base.OnEffectUpdate();
        if (Util.OneTime(ElapsedTime > EffectConfig.BackStabConfig.DamageDelay, ref _damaged))
        {
            _config = EffectConfig.BackStabConfig.GetDefault(_fp.CurrentAttack, _fp.GetSkillTree().GetSkillLevel("Backstab"));
            FightPlayer.DamageInfo info = FightPlayer.DamageInfo.CreateDamageInfo(_config.Damage);
            info.ReactionInfo.Flinch = false;
            info.SkillKey = SkillConfig.BackstabConfig.SkillKey;
            FightPlayer.TakeDamage(_fp, info);
            if (_config.LifeSteal)
            {
                info.ReactionInfo.Amount = (int)(info.ReactionInfo.Amount * -EffectConfig.BackStabConfig.FourStarLifeStealModifier);
                info.DamageNumberColor = GlobalData.HealNumberColor;
                _fp.TakeDamage(_fp, info);
            }
        }
    }
}

public class EffectBackstabCaster : FightEffectWithImmunity
{
    // This effect will teleport the owner of the Kunai to the back of the victim.
    // It is casted by the victim after they are hit by the Kunai, so the Caster is the victim and the FightPlayer field is the Kunai owner.
    public override bool IsActiveEffect => true;
    public override bool BlockAbilityActivation => true;
    public override bool FreezePlayer => true;

    protected override string InvincibilityReason => "BacstabCast";

    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        FightPlayer victimFp = Caster as FightPlayer;
        if (victimFp == null || !victimFp.Damageable())
        {
            FightPlayer.RemoveEffect<EffectBackstabCaster>(false);
            return;
        }
        
        FightPlayer.SetAnimTrigger("backstab");

        Vector2 casterPos = victimFp.Entity.Position;
        Vector2 facing = victimFp.GetFacingDirectionAsVector();
        
        
        Physics.RaycastHit rc;
        var hit = Physics.RaycastWithWhitelist(victimFp.Entity.Position, -victimFp.GetFacingDirectionAsVector(),
            EffectConfig.ShadowStepConfig.MovementDistance, new Entity[]{ FightClubGameManager.References.PvpZoneEdge.Entity }, 
            new Entity[]{ },out rc);
        if (hit)
        {
            // Modify this position so that we don't end up off the map
            casterPos +=  facing;
            // Also set the victim's facing direction to the opposite side
            victimFp.SetFacingDirection(!victimFp.GetFacingDirection());
        }
        else
        {
            casterPos -= facing;
        }

        if (Network.IsServer)
        {
            FightPlayer.Teleport(casterPos);
        }
        SFX.Play(SFXKeys.BackStabTeleportAudio, DefaultSoundDesc);

        DurationRemaining = EffectConfig.BackStabConfig.BackstabTime;
        FightPlayer.SetFacingDirection(victimFp.GetFacingDirection());
        SoundId = SFX.Play(SFXKeys.BackStabCasterAudio, DefaultSoundDesc);
    }
    
    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        FightPlayer.SetAnimTrigger("RESET");
    }
}

public class EffectKunaiThrow : EffectProjectileThrow
{
    public override void AssignConfig()
    {
        Config = EffectConfig.BackStabConfig.GetKunaiConfig();
        if (FightPlayer.HasSkill("NinjaMastery"))
        {
            Config.ProjectileLifetime *= EffectConfig.BackStabConfig.NinjaMasteryRangeModifier;
        }
    }

}