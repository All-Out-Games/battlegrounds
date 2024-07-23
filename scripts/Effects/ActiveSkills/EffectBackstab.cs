using AO;
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
    
    public override float Cooldown => EffectConfig.BackStabConfig.Cooldown;
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
    private FightPlayer _fp;

    public override void OnEffectStart()
    {
        base.OnEffectStart();
        _fp = Caster as FightPlayer;
        if (_fp == null)
        {
            FightPlayer.RemoveEffect<EffectBackstab>(false);
            return;
        }
        FightPlayer.SetAnimTrigger("backstabbed");
        DurationRemaining = MainLayer.GetCurrentStateLength();
    }
    

    public override void OnEffectUpdate()
    {
        base.OnEffectUpdate();
        if (Util.OneTime(ElapsedTime > EffectConfig.BackStabConfig.DamageDelay, ref _damaged))
        {
            _config = EffectConfig.BackStabConfig.GetDefault(_fp.CurrentAttack);
            FightPlayer.DamageInfo info = FightPlayer.DamageInfo.CreateDamageInfo(_config.Damage);
            info.ReactionInfo.Flinch = false;
            FightPlayer.TakeDamage(_fp, info);
        }
    }
}

public class EffectBackstabCaster : FightEffectWithImmunity
{
    // This effect will teleport the caster of the Kunai to the back of the victim
    public override bool IsActiveEffect => true;
    public override bool BlockAbilityActivation => true;
    public override bool FreezePlayer => true;

    protected override string InvincibilityReason => "BacstabCast";

    public override void OnEffectStart()
    {
        base.OnEffectStart();
        FightPlayer victimFp = Caster as FightPlayer;
        if (victimFp == null)
        {
            FightPlayer.RemoveEffect<EffectBackstabCaster>(false);
            return;
        }
        Vector2 casterPos = Caster.Entity.Position - victimFp.GetFacingDirectionAsVector();
        FightPlayer.Teleport(casterPos);
        FightPlayer.SetAnimTrigger("backstab");
        DurationRemaining = MainLayer.GetCurrentStateLength();
        FightPlayer.SetFacingDirection(victimFp.GetFacingDirection());
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
    }

}