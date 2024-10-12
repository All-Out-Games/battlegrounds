using AO;
using Assembly.scripts.SceneObjects;
using Assembly.scripts.SceneObjects.TriggersAndInteractions;
using Assembly.scripts.VFX;

namespace Assembly.scripts.Effects.ActiveSkills;

public class AbilityGravityCrush : FightAbility
{
    public override string SkillKey => "GravityCrush";

    public override Type Effect => typeof(EffectGravityCrush);
    public override bool MonitorEffectDuration => true;
    public override TargettingMode TargettingMode => TargettingMode.Self;
    
    public override float Cooldown => GetCooldown(FightPlayer);

    public static float GetCooldown(FightPlayer fp)
    {
        float cd = EffectConfig.GravityCrushConfig.Cooldown;
        int lv = fp.GetSkillTree().GetSkillLevel("GravityCrush");
        if (lv > 1)
        {
            cd -= 1;
        }

        return cd;
    }
}

public class EffectGravityCrush : FightEffect
{
    public override bool IsActiveEffect => false;
    public override bool BlockAbilityActivation => false;
    public override bool IsValidTarget => true;

    private GravityFieldVFX _gravityFieldVfx;
    // NOTE: The class GravityField.cs has an OnCollision Function that handles the slowdown of projectiles
    private GravityField _gravityField;

    private EffectConfig.GravityCrushConfig _config;

    private float NextDmgTick;
    private bool Ticked;


    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        
        AssignConfig(EffectConfig.GravityCrushConfig.GetDefault(FightPlayer.GetSkillTree().GetSkillLevel("GravityCrush")));
        if (!isDropIn)
        {
            DurationRemaining = _config.Lifetime;
            SoundId = SFX.Play(SFXKeys.GravityCrushAudio, DefaultSoundDesc with{ Loop = true, LoopTimeout = DurationRemaining + 3f, Volume = 0.35f}); 
        }
        AddGravityFx();
    }

    public override void OnEffectUpdate()
    {
        base.OnEffectUpdate();
        if (Util.OneTime(ElapsedTime > NextDmgTick, ref Ticked))
        {
            foreach (var fp in FightClubGameManager.Instance.OverlapCircleForCombatPlayers(Entity.Position, _config.FieldSize * 2, Player))
            {
                var ef = fp.GetEffect<EffectMovementSpeedChange>();
                if (ef.Alive())
                {
                    ef.SpdModifier = EffectConfig.GravityCrushConfig.PlayerSpeedMultiplier;
                    ef.DurationRemaining += 0.5f;
                    
                }
                else
                {
                    fp.AddEffect<EffectMovementSpeedChange>(FightPlayer, 0.5f, change => change.SpdModifier = EffectConfig.GravityCrushConfig.PlayerSpeedMultiplier);
                }
            }
            NextDmgTick += 0.5f;
            Ticked = false;
        }
        
    }

    public void AssignConfig(EffectConfig.GravityCrushConfig cfg)
    {
        _config = cfg;
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        if (_gravityField.Alive())
        {
            _gravityField.LocalEnabled = false;
        }
        _gravityFieldVfx.SetAnimTrigger("disappear");

        if (SoundId != default)
        {
            SFX.FadeOutAndStop(SoundId, 1f);
        }
    }
    

    protected virtual void AddGravityFx()
    {
        var attachment = VFXPrefabs.GravityCrushFx.Instantiate();
        _gravityFieldVfx = attachment.GetComponent<GravityFieldVFX>();
        _gravityField = attachment.GetComponent<GravityField>();
        _gravityFieldVfx.Entity.LocalScale = new Vector2(_config.FieldSize, _config.FieldSize);
        if (_gravityField.Alive() && _gravityFieldVfx.Alive())
        {
            _gravityFieldVfx.Spawn(FightPlayer.Entity, new Vector2(0, 0.22f), false, DurationRemaining+1.5f);
            _gravityFieldVfx.SetAnimTrigger("appear");
        }
        else
        {
            Log.Error("Gravity Field components NOT FOUND on GravityCrush FX prefab!");
        }
        
        
    }
}