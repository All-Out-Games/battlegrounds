using AO;


public partial class EffectPunch : FightEffect
{
    protected List<Entity> DamagedPlayer = new();
    private AbilityConfig.PunchConfig _config;

    EffectPunch()
    {
        IsActiveEffect = false;
        BlockAbilityActivation = true;
        IsValidTarget = false;
    }
    
    public override void OnEffectStart()
    {
        base.OnEffectStart();
        
        Punch();
    }

    public override void OnEffectEnd(bool interrupt)
    {
        
    }
    
    public void AssignConfig(AbilityConfig.PunchConfig cfg)
    {
        _config = cfg;
    }

    [ClientRpc]
    public void Punch()
    {
        FightPlayer.SetAnimTrigger("punch");
    }

    protected void OnPunchCollisionEnter(Entity other)
    {
        
    }

    public override bool IsActiveEffect { get; }
    public override bool BlockAbilityActivation { get; }
    public override bool IsValidTarget { get; }
}