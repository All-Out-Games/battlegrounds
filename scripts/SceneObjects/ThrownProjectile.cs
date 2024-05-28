using AO;
namespace Assembly.scripts.SceneObjects;

public class ThrownProjectile : Projectile
{
    protected Vector2 Direction;
    
    public override void Update()
    {
        Entity.Position += Direction * Time.DeltaTime * Speed;
    }

    public void SetDirection(Vector2 dir)
    {
        Direction = dir;
    }
}