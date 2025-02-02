using AO;
using Assembly.scripts.SceneObjects;

namespace Assembly.scripts.Effects.ActiveSkills;

public class AbilityFlashOfSteel : FightAbility
{
    public override string SkillKey => "FlashOfSteel";

    public override Type Effect => typeof(EffectFlashOfSteel);
    public override bool MonitorEffectDuration => false;
    public override TargettingMode TargettingMode => TargettingMode.Line;
    public override float MaxDistance => 5f;

    public override float Cooldown => GetCooldown(FightPlayer);

    public static float GetCooldown(FightPlayer fp)
    {
        return fp.GetSkillTree().GetSkillLevel("FlashOfSteel") > 1 ? EffectConfig.FlashOfSteelConfig.Cooldown - 1 : EffectConfig.FlashOfSteelConfig.Cooldown;
    }
}

public class TrailEffect : FightEffect
{
    public override bool IsActiveEffect => false;
    public override float DefaultDuration => 1f;
    Trail_Renderer _trail;
    Entity _entity;

    public static Texture TrailTexture = Assets.GetAsset<Texture>("Props/ParticleA.png");
    public override void OnEffectEnd(bool interrupt)
    {
        if (_entity != null)
        {
            _entity.Destroy();
        }
    }

    public override void OnEffectStart(bool isDropIn)
    {
        _entity = Entity.Create();
        _entity.SetParent(Player.Entity, false);
        _entity.LocalPosition = new Vector2(0,.4f);
        _trail = _entity.AddComponent<Trail_Renderer>();
        _trail.TargetLength = 20;
        _trail.Width = 2;
        _trail.Tint = Vector4.Red;
        _trail.Texture = TrailTexture;
        _trail.DepthOffset = 1;
    }
}

public class FosHitEffect : FightEffect
{
    public static float DamageDelay = 1.2f;
    public static string SlashPrefabPath = "FoSSlash.prefab";
    public static string SoulPrefabPath = "FoSEffect.prefab";

    private bool _damaged;
    public override bool IsActiveEffect => false;
    public override bool IsCC => false;
    public override bool BlockAbilityActivation => true;
    protected override bool PreventMovement => true;

    private Entity _slashEffect;
    private Entity _soulEffect;
    private Spine_Animator _soulAnimator;

    public int Damage;
    public bool Bleed;

    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        FightPlayer.SetAnimTrigger("fos_victim", true);
        FightClubGameManager.Instance.ClientSpawn(SoulPrefabPath, Position + new Vector2(0, 1.35f), entity =>
        {
            _soulEffect = entity;
            _soulAnimator = entity.GetComponent<Spine_Animator>();
            var stateMachine = StateMachine.Make();
            var mainLayer = stateMachine.CreateLayer("main");
        
            var emptyState = mainLayer.CreateState("__CLEAR_TRACK__", 0, true);
            var appearState = mainLayer.CreateState("appear", 0, false);
            var idleState = mainLayer.CreateState("loop", 0, true);
            var disappearState = mainLayer.CreateState("disappear", 0, false);

            var appearTrigger = stateMachine.CreateVariable("fos_soul_start", StateMachineVariableKind.TRIGGER);
            var disappearTrigger = stateMachine.CreateVariable("fos_soul_crack", StateMachineVariableKind.TRIGGER);
            mainLayer.CreateGlobalTransition(appearState).CreateTriggerCondition(appearTrigger);
            mainLayer.CreateTransition(appearState, idleState, true);
            mainLayer.CreateGlobalTransition(disappearState).CreateTriggerCondition(disappearTrigger);
            mainLayer.CreateTransition(disappearState, emptyState, true);
            mainLayer.InitialState = emptyState;
            
            _soulAnimator.SpineInstance.SetStateMachine(stateMachine, _soulEffect);

            _soulAnimator.SpineInstance.StateMachine.SetTrigger("fos_soul_start");
            //_soulEffect.SetParent(FightPlayer.Entity, false);
        });
    }

    public override void Update()
    {
        base.Update();
        if(Util.OneTime(ElapsedTime > DamageDelay, ref _damaged))
        {
            if (Caster.Alive())
            {
                var info = FightPlayer.DamageInfo.CreateDamageInfo(Damage, DamageType.Melee, FightPlayer.DamageInfo.StunInterruptLevel);
                info.SkillKey = "FlashOfSteel";
                info.SpecialDeathAnimation = true;

                if (Bleed && Damage < FightPlayer.CurrentHealth)
                {
                    EffectBleed.AddOrStackBleed((FightPlayer)Player, Caster.Entity, 4, EffectConfig.FlashOfSteelConfig.BleedDps);
                }
                FightPlayer.TakeDamage((FightPlayer)Caster,info);
                

                if (Network.IsClient && _soulAnimator.Alive())
                {
                    FightClubGameManager.Instance.ClientSpawn(SlashPrefabPath, Position + new Vector2(0, 0.55f), entity => _slashEffect = entity);
                    SFX.Play(SFXKeys.FoSHitAudio, DefaultSoundDesc);
                    _soulAnimator.SpineInstance.StateMachine.SetTrigger("fos_soul_crack");
                }
            }
        }
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        //FightPlayer.SetAnimTrigger("RESET");
        if (_slashEffect.Alive())
        {
            _slashEffect.Destroy();
        }

        if (_soulEffect.Alive())
        {
            _soulEffect.Destroy();
        }
    }
}

public partial class EffectFlashOfSteel : FightEffectWithImmunity
{
    public override bool IsActiveEffect => true;
    public override bool BlockAbilityActivation => true;
    public override bool IsValidTarget => false;

    public EffectConfig.FlashOfSteelConfig _config;

    protected override bool PreventMovement => true;
    protected override string InvincibilityReason => "FlashOfSteel";

    private bool _dash;

    private Vector2 _startPos;
    private Vector2 _endPos;
    
    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        _config = EffectConfig.FlashOfSteelConfig.GetDefault(FightPlayer.CurrentAttack,
            FightPlayer.GetSkillTree().GetSkillLevel("FlashOfSteel"));

        // The player is invincible and not allowed to input movement during the dash
        FightPlayer.SetFacingDirection(AbilityDirection.X > 0);
        FightPlayer.SetKatana(true);
        if (!isDropIn)
        {
            //SFX.Play(SFXKeys.ShoulderCrashAudio, DefaultSoundDesc);
            DurationRemaining = EffectConfig.FlashOfSteelConfig.DashTime + EffectConfig.FlashOfSteelConfig.DashDelay + 0.1f;
            FightPlayer.SetAnimTrigger("fos_start", true);
            FightPlayer.AddEffect<TrailEffect>(null, 1f);
            SFX.Play(SFXKeys.FoSPrepareAudio, DefaultSoundDesc);
        }
        
    }

    public override void OnEffectUpdate()
    {
        if (Util.OneTime(ElapsedTime > EffectConfig.FlashOfSteelConfig.DashDelay, ref _dash))
        {
            Vector2 dir = AbilityDirection;
            FightPlayer.SetFacingDirection(dir.X > 0);
            FightPlayer.AddDash(dir * EffectConfig.FlashOfSteelConfig.DashSpeed, EffectConfig.FlashOfSteelConfig.DashTime);
            _startPos = Position;
            SFX.Play(SFXKeys.FoSChargeAudio, DefaultSoundDesc);
        }
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        FightPlayer.SetAnimTrigger("RESET");
        FightPlayer.RemoveEffect<TrailEffect>(false);
        _endPos = Position;
        
        
        if (Network.IsServer)
        {
            // DAMAGE
            Vector2 damageLine = _endPos - _startPos;
            Vector2 damageDirIncrement = damageLine.Normalized;
            int incrementCount = (int)(damageLine.Length * 2);
            HashSet<FightPlayer> hitPlayers = new HashSet<FightPlayer>();
            float radius = 1.6f;
            for (int i = 0; i < incrementCount; i++)
            {
                Vector2 center = _startPos + damageDirIncrement * i * 0.5f;
                var lst = FightClubGameManager.Instance.OverlapCircleForCombatPlayers(center, radius, FightPlayer);
                foreach (var fp in lst)
                {
                    hitPlayers.Add(fp);
                }
            }
            var endLst = FightClubGameManager.Instance.OverlapCircleForCombatPlayers(_endPos, radius, FightPlayer);
            foreach (var fp in endLst)
            {
                hitPlayers.Add(fp);
            }
            foreach (var fp in hitPlayers)
            {
                if (fp.Damageable())
                {
                    CallClient_AddFosHit(fp, FightPlayer, _config.Damage, _config.ApplyBleed);
                }
            }
        }

        if (!FightPlayer.HasEffect<EffectBladeFrenzy>())
        {
            FightPlayer.SetKatana(false);
        }
    }

    [ClientRpc]
    public static void AddFosHit(FightPlayer victim, FightPlayer source, int damage, bool bleed)
    {
        if (victim.Alive() && source.Alive())
        {
            victim.AddEffect<FosHitEffect>(source,1.5f, (effect =>
            {
                effect.Damage = damage;
                effect.Bleed = bleed;
            }));
        }
    }
}