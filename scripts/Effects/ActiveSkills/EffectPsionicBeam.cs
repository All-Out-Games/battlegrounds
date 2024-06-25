using System.Collections;
using AO;
using Assembly.scripts.VFX;

namespace Assembly.scripts.Effects.ActiveSkills;


public class AbilityPsionicBeam : FightAbility
{
    public override Type Effect => typeof(EffectPsionicBeam);
    public override string SkillKey => "PsionicBeam";
    public override bool MonitorEffectDuration => false;
    public override TargettingMode TargettingMode => TargettingMode.CircleAOE;
    public override float MaxDistance => EffectConfig.PsionicBeamConfig.MaximumRange;

    public override float Cooldown => EffectConfig.PsionicBeamConfig.Cooldown;
}

public class EffectPsionicBeam : FightEffect
{
    public override bool IsActiveEffect => false;
    public override bool FreezePlayer => true;
    public override bool BlockAbilityActivation => true;

    private EffectConfig.PsionicBeamConfig _cfg;
    
    private float _angleLow;
    private float _angleHigh;
    private Vector2 _eyePos;
    private float _rayLength;
    
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

    public void AssignConfig(EffectConfig.PsionicBeamConfig cfg)
    {
        _cfg = cfg;
        DurationRemaining = EffectConfig.PsionicBeamConfig.CarveTime;
    }
    
    public override void OnEffectStart()
    {
        base.OnEffectStart();
        AssignConfig(EffectConfig.PsionicBeamConfig.GetDefault(FightPlayer.CurrentAttack));
        
        
        
        float targetAngle = FightClubUtils.AngleBetween(Vector2.Right,AbilityPositionOrDirection);
        _angleHigh = targetAngle + EffectConfig.PsionicBeamConfig.Degrees;
        _angleLow = targetAngle - EffectConfig.PsionicBeamConfig.Degrees;
        
        _eyePos = FightPlayer.Entity.Position + new Vector2(0, EffectConfig.PsionicBeamConfig.EyeOffsetY);
        
        _rayLength = AbilityMagnitude * EffectConfig.PsionicBeamConfig.MaximumRange;
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

        // First carve starts here, others are in OnEffectUpdate
        _currentAngle = _angleLow;
        CarveGround(); 
    }

    public override void OnEffectUpdate()
    {
        
        if (Util.OneTime(ElapsedTime > _nextCarveTick, ref _ticked))
        {
            //_rayEnd = FightClubUtils.PolarCirclePoint(FightPlayer.Entity.Position, _rayLength, _currentAngle);
            _currentAngle += _angleInterval;
            _currentInterval += 1;
            CarveGround();
            
            _nextCarveTick += _tickTime;
            _ticked = false;
        }
        
        // Debug
        
        IM.PushZ(-1);
        DebugLine(FightClubUtils.PolarCirclePoint(FightPlayer.Entity.Position, _rayLength, _angleLow), Vector4.Blue);
        DebugLine(FightClubUtils.PolarCirclePoint(FightPlayer.Entity.Position, _rayLength, _angleHigh), Vector4.Red);
        DebugLine(_rayEnd, Vector4.White);
        IM.PopZ();
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        BeamDamage();
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
        List<FightPlayer> fpInRadius =
            FightClubGameManager.Instance.OverlapCircleForCombatPlayers(FightPlayer.Entity.Position, _rayLength);
        foreach (var fp in fpInRadius)
        {
            if (fp != FightPlayer)
            {
                Vector2 fpVector = fp.Entity.Position - FightPlayer.Entity.Position;
                //Log.Warn($"Angle1 = {FightClubUtils.AngleBetween(Vector2.Right, fpVector)}");
                //Log.Warn($"Low {_angleLow}, High {_angleHigh}");
                float angle1 = FightClubUtils.AngleBetween(Vector2.Right, fpVector);
                if (angle1 > _angleLow && angle1 < _angleHigh)
                {
                    FightPlayer.DamageInfo info = FightPlayer.DamageInfo.CreateDamageInfo(_cfg.Damage, DamageType.Ranged);
                    fp.TakeDamage(FightPlayer, info);
                    
                    var hit = VFXPrefabs.PsionicBeamHitVFX.Instantiate();
                    hit.Position = fp.Entity.Position - fpVector * 0.01f;
                }
            }
            
        }
    }

    private void CarveGround()
    {
        _rayEnd = FightClubUtils.PolarCirclePoint(FightPlayer.Entity.Position, _rayLength, _currentAngle);
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
        Sprite_Renderer rdr = fissure.GetComponent<Sprite_Renderer>();
        float acc = t;
        while (acc > 0)
        {
            acc -= Time.DeltaTime;
            rdr.Tint = rdr.Tint with { W = Util.Lerp(0,1, acc/t) };
            yield return null;
        }
        fissure.Destroy();
    }
}