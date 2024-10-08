using Assembly.scripts.VFX;
using AO;
using Assembly.scripts.SceneObjects.Projectiles;

namespace Assembly.scripts.SceneObjects.TriggersAndInteractions;

public class GravityField : Component
{

    private Circle_Collider _circleCollider;

    private List<Entity> _interactedEntity;
    public override void Awake()
    {
        base.Awake();
        _circleCollider = Entity.GetComponent<Circle_Collider>();
        if (_circleCollider == null)
        {
            Log.Error("Gravity Field Prefab does not have a Circle Collider!");
        }
        else
        {
            _circleCollider.OnCollisionEnter += OnGravityFieldEnter;
            _circleCollider.OnCollisionExit += OnGravityFieldExit;
            _interactedEntity = new List<Entity>();
        }
        
    }

    private void OnGravityFieldEnter(Entity other)
    {
        if(_interactedEntity.Contains(other)) return;
        _interactedEntity.Add(other);
        var bp = other.GetComponent<BaseProjectile>();
        bp?.ModifySpeed(EffectConfig.GravityCrushConfig.ProjectileSpeedMultiplier);
    }
    
    private void OnGravityFieldExit(Entity other)
    {
        if (_interactedEntity.Contains(other))
        {
            var bp = other.GetComponent<BaseProjectile>();
            bp?.ResumeSpeed();
        }
    }
}