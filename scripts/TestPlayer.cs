using AO;

public partial class TestPlayer : Player
{
    public override void Awake()
    {

    }

    public override void Update()
    {
        if (IsLocal) 
        {
            DrawDefaultAbilityUI(new AbilityDrawOptions()
            {
                Abilities = new Ability[] {
                    GetAbility<TestDirectionalAbility>(),
                    GetAbility<TestDirectionOnNearestAbility>(),
                    GetAbility<TestDirectionalAbility>(),
                    GetAbility<TestSelectAbility>(),
                    GetAbility<TestAOEAbility>(),
                },
                AbilityElementSize = 100,
            });
        }
    }
}

public class TestDirectionalAbility : Ability
{
    public override Type Effect => typeof(TestEffect);
    public override bool MonitorEffectDuration => true;
    public override TargettingMode TargettingMode => TargettingMode.Line;
    public override float MaxDistance => 3.0f;

    public override bool CanUse()
    {
        return true;
    }

    public override bool CanTarget(Player player)
    {
        return true;
    }

    public override void OnActivate(Player targetPlayer, Vector2 positionOrDirection, float magnitude)
    {
        base.OnActivate(targetPlayer, positionOrDirection, magnitude);
    }
}

public class TestSelectAbility : Ability
{
    public override Type Effect => typeof(TestEffect);
    public override bool MonitorEffectDuration => true;
    public override TargettingMode TargettingMode => TargettingMode.Select;
    public override float MaxDistance => 3.0f;
    public override int MaxTargets => 3;

    public override bool CanUse()
    {
        return true;
    }

    public override bool CanTarget(Player player)
    {
        return true;
    }

    public override void OnActivate(Player targetPlayer, Vector2 positionOrDirection, float magnitude)
    {
        base.OnActivate(targetPlayer, positionOrDirection, magnitude);
    }
}

public class TestAOEAbility : Ability
{
    public override Type Effect => typeof(TestEffect);
    public override bool MonitorEffectDuration => true;
    public override TargettingMode TargettingMode => TargettingMode.CircleAOE;
    public override float MaxDistance => 3.0f;
    public override int MaxTargets => 3;

    public override bool CanUse()
    {
        return true;
    }

    public override bool CanTarget(Player player)
    {
        return true;
    }

    public override void OnActivate(Player targetPlayer, Vector2 positionOrDirection, float magnitude)
    {
        base.OnActivate(targetPlayer, positionOrDirection, magnitude);
    }
}

public class TestDirectionOnNearestAbility : Ability
{
    public override Type Effect => typeof(TestEffect);
    public override bool MonitorEffectDuration => true;
    public override TargettingMode TargettingMode => TargettingMode.DirectionOnNearest;
    public override float MaxDistance => 3.0f;
    public override int MaxTargets => 1;

    public override bool CanUse()
    {
        return true;
    }

    public override bool CanTarget(Player player)
    {
        return true;
    }

    public override void OnActivate(Player targetPlayer, Vector2 positionOrDirection, float magnitude)
    {
        base.OnActivate(targetPlayer, positionOrDirection, magnitude);
    }
}

public class TestEffect : AEffect
{
    public override bool IsActiveEffect => true;
    public override bool BlockAbilityActivation => true;
    public override bool IsValidTarget => false;
    
    public override void OnEffectStart()
    {

    }

    public override void OnEffectUpdate()
    {

    }

    public override void OnEffectEnd(bool interrupt)
    {

    }
}