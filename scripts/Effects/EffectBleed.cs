using AO;
using Assembly.scripts.VFX;
using StreamReader = AO.StreamReader;

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

    public override void OnEffectStart()
    {
        base.OnEffectStart();
        AddAura();
    }

    public override void NetworkDeserialize(StreamReader reader)
    {
        base.NetworkDeserialize(reader);
        AddAura();
    }

    protected void Bleed()
    {
        FightPlayer.DamageInfo selfDmgInfo = FightPlayer.DamageInfo.CreateSelfDamageInfo(PerSecondDmg);
        FightPlayer.TakeDamage(Caster as FightPlayer, selfDmgInfo); // You can create self damage that comes from other players.
    }
    
    private void AddAura()
    {
        Prefab auraPrefab = VFXPrefabs.BloodSplurt;
        _aura = auraPrefab.Instantiate().GetComponent<AttachmentObject>();
        _aura.Spawn(FightPlayer.Entity,new Vector2(-0.3f, 0.9f), false, DurationRemaining);

        var auraFade = _aura.Entity.GetComponent<FadeAfterStart>();
        if (auraFade != null)
        {
            auraFade.SetPersistFadeTime(DurationRemaining-1.0f, DurationRemaining-0.5f);
        }
        
    }
}