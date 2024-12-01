using AO;
using Assembly.scripts.SceneObjects;
using StreamReader = AO.StreamReader;

namespace Assembly.scripts.Effects.ActiveSkills;

public class AbilityHypnotize : FightAbility
{
    public override string SkillKey => "Hypnotize";

    public override Type Effect => typeof(EffectHypnotize);
    public override bool MonitorEffectDuration => false;
    public override TargettingMode TargettingMode => TargettingMode.Select;
    public override float MaxDistance => EffectConfig.HypnotizeConfig.HypnotizeRange;
    public override int MaxTargets => 1;
    
    public override float Cooldown => GetCooldown(FightPlayer);
    public static float GetCooldown(FightPlayer fp)
    {
        int lv = int.Min(4, fp.GetSkillTree().GetSkillLevel("Hypnotize"));
        return EffectConfig.HypnotizeConfig.Cooldown + 1 - lv;
    }

    public override bool CanTarget(Player player)
    {
        return GenericCanTarget(player as FightPlayer) && !player.HasEffect<EffectHypnotize>() && Vector2.Distance(FightPlayer.Position, player.Position) < MaxDistance;
    }
}

public class EffectHypnotizeCaster : FightEffect
{
    public override bool IsActiveEffect => true;

    public override bool BlockAbilityActivation => true;

    protected override bool PreventMovement => true;

    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        FightPlayer.SetAnimTrigger("hypnotize");
        DurationRemaining = MainLayer.GetCurrentStateLength();
        SoundId = SFX.Play(SFXKeys.HypnotizeAudio, DefaultSoundDesc);
    }
}

public class EffectHypnotize : FightEffectWithNoFlinch
{
    public override bool IsActiveEffect => true;
    protected override int InterruptLevel => 1000;

    protected override bool PreventMovement => true;

    public override bool BlockAbilityActivation => true;

    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        FightPlayer.UnsetAnimTrigger("knockdown_end");
        FightPlayer.SetAnimTrigger("knockdown");
        FightPlayer.OnReceiveDamage += OnDamageEvent;

        Caster.AddEffect<EffectHypnotizeCaster>();
        
        if (!isDropIn)
        {
            DurationRemaining = EffectConfig.HypnotizeConfig.HypnotizeTime;
            if((Caster as FightPlayer)?.GetSkillTree().GetSkillLevel("Hypnotize") > 4)
            {
                DurationRemaining += 1;
            }
        }
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        FightPlayer.OnReceiveDamage -= OnDamageEvent;
        FightPlayer.UnsetAnimTrigger("knockdown");
        FightPlayer.SetAnimTrigger("knockdown_end");
        FightPlayer.AddEffect<EffectGenericPostActionDelay>(Caster, 1f);
        SFX.Play(SFXKeys.HypnotizeGetupAudio, DefaultSoundDesc);
    }

    public override void NetworkDeserialize(StreamReader reader)
    {
        base.NetworkDeserialize(reader);
        FightPlayer.OnReceiveDamage += OnDamageEvent;
    }
}
