using AO;
using Assembly.scripts.VFX;

namespace Assembly.scripts.Effects.ActiveSkills;

public class AbilityShadowStep : FightAbility
{
    public override string SkillKey => "ShadowStep";
    
    public override Type Effect => typeof(EffectShadowStep);
    public override bool MonitorEffectDuration => true;
    public override TargettingMode TargettingMode => TargettingMode.Self;

    public override float Cooldown => EffectConfig.ShadowStepConfig.Cooldown;
}

public class EffectShadowStep : FightEffect
{
    public override bool IsActiveEffect => true;

    public override bool BlockAbilityActivation => true;

    private EffectConfig.ShadowStepConfig _cfg;

    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        var fx = VFXPrefabs.ShadowStepVFX.Instantiate();
        fx.Position = FightPlayer.Entity.Position;
        fx.GetComponent<SelectionVFX>()?.StartVFX("shadow_step_effect", false);
        
        DurationRemaining = 0.1f;
        _cfg = EffectConfig.ShadowStepConfig.GetDefault(FightPlayer.GetSkillTree().GetSkillLevel("ShadowStep"));
        if (Network.IsServer)
        {
            // If it's not wrapped in IsServer, a rare crash might happen when the server fails frame validation.
            
            Vector2 dir = FightPlayer.Velocity.Length < 0.1f ? FightPlayer.GetFacingDirectionAsVector() : FightPlayer.Velocity.Normalized;
            Vector2 tlePosition = FightPlayer.Entity.Position + dir * EffectConfig.ShadowStepConfig.MovementDistance;
            // Raycast. Try to detect edges
            Physics.RaycastHit rc;
            var hit = Physics.RaycastWithWhitelist(FightPlayer.Entity.Position, dir,
                EffectConfig.ShadowStepConfig.MovementDistance, new Entity[]{ FightClubGameManager.References.PvpZoneEdge.Entity }, 
                new Entity[]{ },out rc);
            
            /*var hit = Physics.Raycast(FightPlayer.Entity.Position, dir,
                EffectConfig.ShadowStepConfig.MovementDistance, out rc);*/
            //Log.Warn($"Hit = {hit}, Entity = {rc.Entity?.Name}");
            if (hit)
            {
                Collider eg = rc.Collider;
                if (eg != null)
                {
                    dir *= 0.1f;
                    tlePosition = rc.point - dir;
                }
            }
            
            
            FightPlayer.Teleport(tlePosition);
        }

        FightPlayer.AddEffect<EffectShadowArmor>(FightPlayer, EffectConfig.ShadowStepConfig.ShadowArmorBuffTime,
            armor =>
            {
                armor.TriggerForTime = _cfg.ShadowArmorEffectiveTime;
                armor.ArmorAmount = _cfg.ShadowArmorAmount;
            });
    }
}

public class EffectShadowArmor : FightEffect
{
    public override bool IsActiveEffect => false;

    private int _triggerCount;
    public int TriggerForTime;
    public int ArmorAmount;
    
    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        FightPlayer.RegisterPreDamageEvent(this);
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        FightPlayer.RemovePreDamageEvent(this);
    }

    public override void PreDamageMod(ref FightPlayer.DamageInfo info)
    {
        base.PreDamageMod(ref info);
        if (Network.IsServer && _triggerCount < TriggerForTime)
        {
            info.ReactionInfo.Amount -= ArmorAmount;
            info.ReactionInfo.Amount = int.Max(0, info.ReactionInfo.Amount);
        }
        _triggerCount++;
        if (_triggerCount >= TriggerForTime)
        {
            DurationRemaining = 0.05f;
        }

    }
}