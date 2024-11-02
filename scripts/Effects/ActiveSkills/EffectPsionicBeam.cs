using System.Collections;
using AO;
using Assembly.scripts.SceneObjects;
using Assembly.scripts.VFX;

namespace Assembly.scripts.Effects.ActiveSkills;


public class AbilityPsionicBeam : FightAbility
{
    public override Type Effect => typeof(EffectPsionicBeam);
    public override string SkillKey => "PsionicBeam";
    public override bool MonitorEffectDuration => false;
    public override TargettingMode TargettingMode => TargettingMode.CircleAOE;
    public override float MaxDistance => EffectConfig.PsionicBeamConfig.MaximumRange;

    public override float Cooldown => GetCooldown(FightPlayer);

    public static float GetCooldown(FightPlayer fp)
    {
        if (fp.GetSkillTree().GetSkillLevel("PsionicBeam") > 2)
        {
            return EffectConfig.PsionicBeamConfig.Cooldown - 1;
        }
        else
        {
            return EffectConfig.PsionicBeamConfig.Cooldown;
        }
    }
}

public class EffectPsionicBeam : FightEffectWithNoFlinch
{
    public override bool IsActiveEffect => false;
    public override bool FreezePlayer => true;
    public override bool BlockAbilityActivation => true;
    

    private EffectConfig.PsionicBeamConfig _cfg;
    
    private float _angleLow;
    private float _angleHigh;
    private Vector2 _eyePos;
    private float _rayLength;
    private List<Collider> _interactedEntities;
    private bool _enhanced;
    
    // Carve Trail
    private float _nextCarveTick = 0;
    private float _tickTime = 0.1f;
    private bool _ticked = false;
    private int _repeatTimes = 1;
    private Vector2 _rayEnd;
    private float _currentAngle = 0;
    private float _angleInterval = 1;
    private int _currentInterval = 0;
    private Prefab _fissueEndPrefab;
    private Prefab _fissuePrefab;
    
    // VFX
    private BeamVFX _vfx;

    public void AssignConfig(EffectConfig.PsionicBeamConfig cfg)
    {
        _cfg = cfg;
        DurationRemaining = EffectConfig.PsionicBeamConfig.CarveTime;
        _enhanced = FightPlayer.HasSkill("Psychic");
    }
    
    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        AssignConfig(EffectConfig.PsionicBeamConfig.GetDefault(FightPlayer.CurrentAttack, FightPlayer.GetSkillTree().GetSkillLevel("PsionicBeam")));
        FightPlayer.SetFacingDirection(AbilityPositionOrDirection.X >= 0);
        FightPlayer.UnsetAnimTrigger("psibeam_end");
        FightPlayer.SetAnimTrigger("psibeam");
        _interactedEntities = new List<Collider>();
        
        float targetAngle = FightClubUtils.AngleBetween(Vector2.Right,AbilityPositionOrDirection);
        _angleHigh = targetAngle + EffectConfig.PsionicBeamConfig.Degrees;
        _angleLow = targetAngle - EffectConfig.PsionicBeamConfig.Degrees;
        
        _eyePos = FightPlayer.Entity.Position + new Vector2(FightPlayer.GetFacingDirection()? 0 : EffectConfig.PsionicBeamConfig.EyeOffsetX, 
            EffectConfig.PsionicBeamConfig.EyeOffsetY);
        Entity vfxEntity = VFXPrefabs.PsionicRayVFX.Instantiate();
        _vfx = vfxEntity.GetComponent<BeamVFX>();
        vfxEntity.Position = _eyePos;

        _rayLength = AbilityPositionOrDirection.Length;
        //Log.Warn($"{AbilityPositionOrDirection.ToString()}, L = {_rayLength}, M = {AbilityMagnitude}");
        if (_rayLength < EffectConfig.PsionicBeamConfig.MinimumRange)
            _rayLength = EffectConfig.PsionicBeamConfig.MinimumRange;
        
        // Ground Carve Visual
        _fissuePrefab = Assets.GetAsset<Prefab>(EffectConfig.PsionicBeamConfig.FissuePrefabPath);
        _fissueEndPrefab = Assets.GetAsset<Prefab>(EffectConfig.PsionicBeamConfig.FissueEndPrefabPath);
        
        float curveLength = (4 * (float)Math.PI * _rayLength * EffectConfig.PsionicBeamConfig.Degrees) / 360;
        var repeatTimes = float.Round(curveLength / EffectConfig.PsionicBeamConfig.BeamCarveInterval);
        _angleInterval = 2 * EffectConfig.PsionicBeamConfig.Degrees / repeatTimes;
        _tickTime = EffectConfig.PsionicBeamConfig.CarveTime / repeatTimes;
        _repeatTimes = (int)repeatTimes + 1;

        //Log.Warn($"R: {_repeatTimes}"); // 8-15 ticks
        
        // First carve starts here, others are in OnEffectUpdate
        _currentAngle = _angleLow;
        CarveGround();

        SoundId = SFX.Play(SFXKeys.PsiRayAudio, new SFX.PlaySoundDesc() {Volume = 0.6f, EntityToFollow = FightPlayer.Entity});
    }

    public override void OnEffectUpdate()
    {
        
        if (Util.OneTime(ElapsedTime > _nextCarveTick, ref _ticked))
        {
            _currentAngle += _angleInterval;
            _currentInterval += 1;
            CarveGround();
            
            _nextCarveTick += _tickTime;
            _ticked = false;
        }
        
        // Debug
        
        // IM.PushZ(-1);
        // DebugLine(FightClubUtils.PolarCirclePoint(_eyePos, _rayLength, _angleLow), Vector4.Blue);
        // DebugLine(FightClubUtils.PolarCirclePoint(_eyePos, _rayLength, _angleHigh), Vector4.Red);
        // DebugLine(_rayEnd, Vector4.White);
        // IM.PopZ();
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        if (_vfx != null)
        {
            _vfx.Despawn();
        }
        FightPlayer.SetAnimTrigger("psibeam_end");
        FightPlayer.AddEffect<EffectGenericPostActionDelay>();
        SFX.FadeOutAndStop(SoundId, 0.5f);
    }


    private void DebugLine(Vector2 point, Vector4 color)
    {
        if (Network.IsClient)
        {
            IM.Line(Camera.WorldToScreen(_eyePos), Camera.WorldToScreen(point), 5, color, null, true);
        }
    }

    private void BeamDamage()
    {
        var fpInRadius =
            FightClubGameManager.Instance.GetAllDamagableEntities(Player);
        bool explode = FightPlayer.GetSkillTree().GetSkillLevel("PsionicBeam") > 4;
        if (AO.Physics.RaycastWithWhitelist(_eyePos, _rayEnd - _eyePos, _rayLength, fpInRadius, new Entity[] { },
                out Physics.RaycastHit hit))
        {
            if (_interactedEntities.Contains(hit.Collider))
            {
                return;
            }
            _interactedEntities.Add(hit.Collider);

            DamageableObject dmg = hit.Collider.GetComponent<DamageableObject>();
            FightPlayer.DamageInfo info =
                FightPlayer.DamageInfo.CreateDamageInfo(_cfg.Damage, DamageType.Ranged);
            info.SkillKey = SkillConfig.PsionicBeamConfig.SkillKey;
            info.SpecialDeathAnimation = true;
            info.CrateImmediateDestroy = true;
            
            if (dmg is PlayerCollisionChild fp)
            {
                if (fp.Player == FightPlayer)
                {
                    return;
                }
                fp.Player.TakeDamage(FightPlayer, info);

                var hitVfx = VFXPrefabs.PsionicBeamHitVFX.Instantiate();
                hitVfx.Position = hit.point;
                // Psychic Enhancement: Heal for a certain amount
                if (_enhanced)
                {
                    FightPlayer.DamageInfo selfHealInfo = FightPlayer.DamageInfo.CreateHealInfo(EffectConfig.PsionicBeamConfig.PsychicHeal);
                    FightPlayer.TakeDamage(FightPlayer, selfHealInfo);
                }
                // Lv.4 Enhancement: Attach PsyExplosion for 20% Damage
                if (explode)
                {
                   
                    fp.Player.AddEffect<EffectPsyExplosion>(FightPlayer, 2f, explosion =>
                    {
                        explosion.Damage = (int)float.Ceiling(0.1f * info.ReactionInfo.Amount);
                        explosion.Radius = 3;
                        explosion.SkillKey = "PsionicBeam";

                    });
                }
            }
            else
            {
                dmg.TakeDamage(FightPlayer, info);
            }
        }
    }

    private void CarveGround()
    {
        _rayEnd = FightClubUtils.PolarCirclePoint(_eyePos, _rayLength, _currentAngle);
        _vfx.SetBonePosition("end", _rayEnd - _eyePos);
        BeamDamage();
        
        if (Network.IsClient)
        {
            Entity fissue;
            // Pure visual, so don't bother the server...
            if (_currentInterval == 0) // Start fissue
            {
                fissue = _fissueEndPrefab.Instantiate();
                fissue.Position = _rayEnd;
                fissue.Rotation = _currentAngle;
            }
            else if (_currentInterval < _repeatTimes-1)
            {
                fissue = _fissuePrefab.Instantiate();
                fissue.Position = _rayEnd;
                fissue.Rotation = _currentAngle;
            }
            else // End fissue
            {
                fissue = _fissueEndPrefab.Instantiate();
                fissue.Position = _rayEnd;
                fissue.Rotation = _currentAngle - 180;
            }

            Coroutine.Start(FightPlayer.Entity, FissureDissipate(fissue, EffectConfig.PsionicBeamConfig.CarveFadeTime));
        }
    }

    private IEnumerator FissureDissipate(Entity fissure, float t)
    {
        yield return new WaitForSeconds(t);
        if (!fissure.Alive())
        {
            yield break;
        }
        Sprite_Renderer rdr = fissure.GetComponent<Sprite_Renderer>();
        float acc = t;
        while (acc > 0)
        {
            if (!fissure.Alive())
            {
                yield break;
            }
            acc -= Time.DeltaTime;
            rdr.Tint = rdr.Tint with { W = Util.Lerp(0,1, acc/t) };
            yield return null;
        }
        fissure.Destroy();
    }
}