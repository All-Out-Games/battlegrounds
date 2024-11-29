using AO;
using Assembly.scripts.VFX;

namespace Assembly.scripts.Effects;

public class EffectBleed : FightEffect
{
    
    public override bool IsActiveEffect => false;

    public int PerSecondDmg = 0;
    protected float NextDmgTick = 1;
    protected bool Ticked = false;

    private AttachmentObject _aura;

    public override void OnEffectUpdate()
    {
        if (Util.OneTime(ElapsedTime > NextDmgTick, ref Ticked))
        {
            Bleed();
            NextDmgTick += 1;
            Ticked = false;
        }
    }

    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        AddAura();
    }
    

    protected void Bleed()
    {
        FightPlayer.DamageInfo selfDmgInfo = FightPlayer.DamageInfo.CreateSelfDamageInfo(PerSecondDmg);
        selfDmgInfo.SkillKey = SkillConfig.ClawSlashConfig.SkillKey;
        FightPlayer.TakeDamage(Caster as FightPlayer, selfDmgInfo); // You can create self damage that comes from other players.
    }
    
    private void AddAura()
    {
        Prefab auraPrefab = VFXPrefabs.BloodSplurt;
        _aura = auraPrefab.Instantiate().GetComponent<AttachmentObject>();
        _aura.Spawn(FightPlayer.Entity,new Vector2(-0.3f, 0.9f), false, DurationRemaining);

        var auraVfx = _aura.Entity.GetComponent<BaseVFX>();
        auraVfx?.SetLifetime(DurationRemaining);

        var auraFade = _aura.Entity.GetComponent<FadeAfterStart>();
        if (auraFade != null)
        {
            auraFade.SetPersistFadeTime(DurationRemaining-1.0f, DurationRemaining-0.5f);
        }
        
    }

    /// <summary>
    /// Called on bleed effect when another bleed effect is about to be applied.
    /// Refresh duration and add on dps
    /// </summary>
    public void Stack(float duration, int dps)
    {
        if (_aura.Alive() && _aura.Entity.Alive())
        {
            var fade = _aura.Entity.GetComponent<FadeAfterStart>();
            if (fade.Alive() && DurationRemaining > fade.ElapsedTime)
            {
                fade.ExtendLifetime(DurationRemaining - fade.ElapsedTime + 0.15f);
            }
        }
        
        DurationRemaining = float.Max(duration, DurationRemaining);
        PerSecondDmg += dps;
    }

    public static void AddOrStackBleed(FightPlayer fp, Entity source, float time, int dps)
    {
        if (fp.Alive())
        {
            var bld = fp.GetEffect<EffectBleed>();
            if (bld.Alive())
            {
                bld.Stack(time, dps);
            }
            else
            {
                fp.GetEffectMgr().AddBleed(source, time, dps);
            }
        }
    }
}