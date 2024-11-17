using AO;
using Assembly.scripts.SceneObjects;
using Assembly.scripts.VFX;
namespace Assembly.scripts.Effects;

public class EffectBurn: FightEffect
{
    
    public override bool IsActiveEffect => false;

    public int PerSecondDmg = 0;
    protected float NextDmgTick = 1;
    protected bool Ticked = false;
    

    public override void OnEffectUpdate()
    {
        if (Util.OneTime(ElapsedTime > NextDmgTick, ref Ticked))
        {
            Burn();
            NextDmgTick += 1;
            Ticked = false;
        }
    }
    
    protected void Burn()
    {
        FightPlayer.DamageInfo selfDmgInfo = FightPlayer.DamageInfo.CreateSelfDamageInfo(PerSecondDmg);
        selfDmgInfo.SkillKey = SkillConfig.FireballNodeConfig.SkillKey;
        selfDmgInfo.SpecialDeathAnimation = true;
        FightPlayer.TakeDamage(Caster as FightPlayer, selfDmgInfo); // You can create self damage that comes from other players.
        
        SFX.Play(SFXKeys.FireballHitAudio, new SFX.PlaySoundDesc() { EntityToFollow = Entity, Volume = 0.35f});
        FightClubGameManager.Instance.ClientSpawn(VFXPrefabs.HitVFX, FightPlayer.Position,
            entity =>
            {
                SelectionVFX vfx = entity.GetComponent<SelectionVFX>();
                vfx.StartVFX("hit_fire", false);
            }
        );
    }
    

    /// <summary>
    /// Called on bleed effect when another bleed effect is about to be applied.
    /// Refresh duration and add on dps
    /// </summary>
    public void Stack(float duration, int dps)
    {
        DurationRemaining = float.Max(duration, DurationRemaining);
        PerSecondDmg += dps;
    }
    
    public static void AddOrStackBurn(FightPlayer fp, Entity source, float time, int dps)
    {
        var bld = fp.GetEffect<EffectBurn>();
        if (bld.Alive())
        {
            bld.Stack(time, dps);
        }
        else
        {
            fp.GetEffectMgr().AddBurn(source, time, dps);
        }
    }
}