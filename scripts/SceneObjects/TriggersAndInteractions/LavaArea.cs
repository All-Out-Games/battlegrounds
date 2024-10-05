using AO;

namespace Assembly.scripts.SceneObjects.TriggersAndInteractions;

public class LavaArea : Component
{
    [Serialized] private Box_Collider _collider;

    public override void Start()
    {
        base.Start();
        _collider.OnCollisionEnter += OnLavaDamage;
    }

    public void OnLavaDamage(Entity other)
    {
        FightPlayer fp = other.GetComponent<FightPlayer>();
        if (fp.Alive())
        {
            FightPlayer.DamageInfo info = FightPlayer.DamageInfo.CreateDamageInfo(9999, DamageType.None, 10000);
            info.SpecialDeathAnimation = true;
            info.SkillKey = "SelfDestruct";
            info.DamageNumberColor = GlobalData.CritNumberColor;
            info.AwardCoin = false;
            fp.TakeDamage(fp, info);
        }
    }
}