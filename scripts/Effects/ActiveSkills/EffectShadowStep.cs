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

    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        var fx = VFXPrefabs.ShadowStepVFX.Instantiate();
        fx.Position = FightPlayer.Entity.Position;
        fx.GetComponent<SelectionVFX>()?.StartVFX("shadow_step_effect", false);
        
        DurationRemaining = 0.1f;
        if (Network.IsServer)
        {
            Vector2 dir = FightPlayer.Velocity.Length < 0.1f ? FightPlayer.GetFacingDirectionAsVector() : FightPlayer.Velocity.Normalized;
            Vector2 tlePosition = FightPlayer.Entity.Position + dir * EffectConfig.ShadowStepConfig.MovementDistance;
            // Raycast. Try to detect edges
            Physics.RaycastHit rc;
            var hit = Physics.RaycastWithWhitelist(FightPlayer.Entity.Position, dir,
                EffectConfig.ShadowStepConfig.MovementDistance, new Entity[]{ FightClubGameManager.References.PvpZoneEdge.Entity }, 
                new Entity[]{ },out rc);
            
            /*var hit = Physics.Raycast(FightPlayer.Entity.Position, dir,
                EffectConfig.ShadowStepConfig.MovementDistance, out rc);*/
            Log.Warn($"Hit = {hit}, Entity = {rc.Entity?.Name}");
            if (hit)
            {
                Edge_Collider eg = rc.Entity.GetComponent<Edge_Collider>();
                if (eg != null)
                {
                    dir *= 0.1f;
                    tlePosition = rc.point - dir;
                }
            }
            
            
            FightPlayer.Teleport(tlePosition);
        }
        
    }
}