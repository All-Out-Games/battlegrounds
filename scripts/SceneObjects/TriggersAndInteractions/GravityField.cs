using Assembly.scripts.VFX;
using AO;

namespace Assembly.scripts.SceneObjects.TriggersAndInteractions;

public class GravityField : Component
{

    private Circle_Collider _circleCollider;
    
    
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
        }
    }

    private void OnGravityFieldEnter(Entity other)
    {
        // TODO
    }
}