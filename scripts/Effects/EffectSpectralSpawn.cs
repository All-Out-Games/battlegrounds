using AO;
using Assembly.scripts.SceneObjects;
using Assembly.scripts.UI;

namespace Assembly.scripts.Effects;

public class AbilitySpectreMaterialze : FightAbility
{
    public override string SkillKey => "Punch";
    public override bool CanUse() => Player.HasEffect<EffectSpectralSpawn>();

    public override Type Effect => typeof(EffectSpectralEnd);

    public override TargettingMode TargettingMode => TargettingMode.Self;
    
    public override string SkillIconPath => "AbilityIcon_Separate/spectral_spawn.png";
}

public class EffectSpectralSpawn : FightEffect
{
    // This is our spectator mode.
    // Player can roam around and get XP, and they will have an ability that can be used to immediately spawn into the arena
    public override bool IsActiveEffect => true;

    public override List<Type> AbilityWhitelist => new() { typeof(AbilitySpectreMaterialze)};
    
    protected float NextDmgTick = 1;
    protected bool Ticked = false;
    protected ResourceOverlayWindow _overlay;
    

    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        FightPlayer.SetAnimBool("ghost_form", true);
        EnterSpectre();
        
        
        if (FightPlayer.IsLocal)
        {
            var slotsMgr = FightPlayer.GetSkillSlots();
            slotsMgr.ReplaceSlot(0, slotsMgr.GetAbilityInstance(typeof(AbilitySpectreMaterialze)), 2); // Changed from 5 to 2s in KoH
            _overlay =
                UIManager.Instance.GetOverlayWindow<ResourceOverlayWindow>("ResourcesOverlayWindow.prefab");
        }
    }

    public override void OnEffectUpdate()
    {
        base.OnEffectUpdate();
        if (FightPlayer.PlayerStatus != PlayerStatus.Spectator)
        {
            // Just in case
            FightPlayer.RemoveEffect<EffectSpectralSpawn>(false);
        }
        
        if (FightPlayer.IsLocal && Util.OneTime(ElapsedTime > NextDmgTick, ref Ticked))
        {
            NextDmgTick += 1;
            Ticked = false;
            if (_overlay.Alive())
            {
                _overlay.SpectralText.Text = $"{(int)DurationRemaining}s";
            }
        }
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        FightPlayer.SetAnimBool("ghost_form", false);
        FightPlayer.EnterCombatFromSpectator();
        ExitSpectre();
        if (!interrupt)
        {
            FightPlayer.AddEffect<EffectSpectralEnd>(FightPlayer, 1.2f);
        }
        if (FightPlayer.IsLocal)
        {
            var slotsMgr = FightPlayer.GetSkillSlots();
            slotsMgr.ReplaceSlot(0, slotsMgr.GetAbilityInstance(typeof(AbilityPunch)));
            if (_overlay.Alive())
            {
                _overlay.SpectralText.Text = $"{FightPlayer.SpectralCount}";
            }
        }
    }

    private void EnterSpectre()
    {
        if (FightPlayer.IsLocal)
        {
            FightPlayer.SpineAnimator.SpineInstance.ColorMultiplier =
                FightPlayer.SpineAnimator.SpineInstance.ColorMultiplier with { W = 0.5f };
        }
        else
        {
            FightPlayer.AddInvisibilityReason("spectator");
            FightPlayer.AddNameInvisibilityReason("spectator");
            FightPlayer.GetPlayerUIComp().AddPlayerUIInvisibleReason("spectator");
        }
    }

    private void ExitSpectre()
    {
        if (FightPlayer.IsLocal)
        {
            FightPlayer.SpineAnimator.SpineInstance.ColorMultiplier =
                FightPlayer.SpineAnimator.SpineInstance.ColorMultiplier with { W = 1f };
        }
        else
        {
            FightPlayer.RemoveInvisibilityReason("spectator");
            FightPlayer.GetPlayerUIComp().RemovePlayerUIInvisibleReason("spectator");
            FightPlayer.RemoveNameInvisibilityReason("spectator");
        }
    }
}

public class EffectSpectralEnd : FightEffect
{
    // This is our spectator mode.
    // Player can roam around and get XP, and they will have an ability that can be used to immediately spawn into the arena
    public override bool IsActiveEffect => true;

    protected override bool PreventMovement => true;

    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        FightPlayer.SetAnimTrigger("meteor_land");
        DurationRemaining = 1.2f;
        SFX.Play(SFXKeys.SpectralSpawnAudio, DefaultSoundDesc);
    }
}