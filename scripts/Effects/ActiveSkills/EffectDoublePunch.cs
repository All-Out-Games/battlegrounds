namespace Assembly.scripts.Effects.ActiveSkills;
using System.Collections;
using AO;

public class AbilityDoublePunch : FightAbility
{
    public override string SkillKey => "DoublePunch";
    public override Type Effect => typeof(EffectDoublePunch);
    public override bool MonitorEffectDuration => false;
    public override TargettingMode TargettingMode => TargettingMode.Self;
    public override float Cooldown => EffectConfig.DoublePunchConfig.Cooldown;
}

public class EffectDoublePunch : FightEffect
{
    protected EffectConfig.DoublePunchConfig Config;
    public override bool IsActiveEffect => false;
    public override bool BlockAbilityActivation => true;
    public override bool IsValidTarget => true;

    protected List<Tuple<float, string>> EventTimeline;
    public override void OnEffectStart()
    {
        base.OnEffectStart();
        
        AssignConfig(EffectConfig.DoublePunchConfig.GetDefault(FightPlayer.CurrentAttack));

        EventTimeline = new List<Tuple<float, string>>();
        
        EventTimeline.Add(new(0, "PunchAnimation"));
        EventTimeline.Add(new(EffectConfig.DoublePunchConfig.PunchActivationTime, "FirstPunch"));
        EventTimeline.Add(new(EffectConfig.DoublePunchConfig.PunchAnimationTime, "PunchAnimation"));
        EventTimeline.Add(new(EffectConfig.DoublePunchConfig.PunchActivationTime + EffectConfig.DoublePunchConfig.PunchAnimationTime, "SecondPunch"));

        EventTimeline = EventTimeline.OrderBy(tuple => tuple.Item1).ToList();
    }

    public override void OnEffectEnd(bool interrupt)
    {
        
    }

    public override void OnEffectUpdate()
    {
        if (EventTimeline.Count > 0)
        {
            if (ElapsedTime > EventTimeline[0].Item1)
            {
                ProcessEvent(EventTimeline[0].Item2);
            }
        }
    }

    protected void ProcessEvent(string evt)
    {
        switch (evt)
        {
            case "PunchAnimation":
                FightPlayer.SetAnimTrigger("punch");
                break;
            case "FirstPunch":
                DoublePunch(1);
                break;
            case "SecondPunch":
                DoublePunch(2);
                break;
        }
        EventTimeline.RemoveAt(0);
    }

    public void AssignConfig(EffectConfig.DoublePunchConfig cfg)
    {
        DurationRemaining = EffectConfig.DoublePunchConfig.PunchAnimationTime * 2;
        Config = cfg;
    }
    
    public void DoublePunch(int punchType)
    {
        // The first punch stuns the enemy if hit. The second punch knock them back
        Physics.RaycastHit rc;
        var hit = Physics.RaycastWithWhitelist(Entity.Position, FightPlayer.GetPunchDirection(),
            EffectConfig.DoublePunchConfig.PunchRange, FightClubGameManager.Instance.GetCombatPlayersCollisionEntities(), out rc);

        /*hit = Physics.Raycast(Entity.Position, FightPlayer.GetPunchDirection(),
            EffectConfig.PunchConfig.PunchRange, out rc);*/

        if (hit && rc.Entity != null)
        {
            PlayerCollisionChild other = rc.Entity.GetComponent<PlayerCollisionChild>();
                
            FightPlayer.DamageInfo info = new FightPlayer.DamageInfo();

            if (punchType == 1)
            {
                // Stunning Punch
                other.Player.TakeDamage(Config.PunchDamage, FightPlayer, info);
                other.Player.GetEffectMgr().AddStun(FightPlayer.Entity, EffectConfig.DoublePunchConfig.PunchAnimationTime);
            }
            else
            {
                // Bumping Punch
                other.Player.TakeDamage(Config.PunchDamage, FightPlayer, info);
                Vector2 bumpDir = other.Entity.Position - FightPlayer.Entity.Position;
                other.Player.AddBumpFrom(FightPlayer, bumpDir * Config.BumpStrength, false);
            }
                
        }
    }

    IEnumerator DelayActivePunchHitbox(float delayTime, int punchType = 0)
    {
        yield return new WaitForSeconds(delayTime);
        Physics.RaycastHit rc;
        var hit = Physics.RaycastWithWhitelist(Entity.Position, FightPlayer.GetPunchDirection(),
            EffectConfig.DoublePunchConfig.PunchRange, FightClubGameManager.Instance.GetCombatPlayersAsEntities(), out rc);

        /*hit = Physics.Raycast(Entity.Position, FightPlayer.GetPunchDirection(),
            EffectConfig.PunchConfig.PunchRange, out rc);*/

        if (hit)
        {
            FightPlayer other = rc.Entity.GetComponent<FightPlayer>();
                
            FightPlayer.DamageInfo info = new FightPlayer.DamageInfo();

            if (punchType == 0)
            {
                // Stunning Punch
                other.TakeDamage(Config.PunchDamage, FightPlayer, info);
                other.GetEffectMgr().AddStun(FightPlayer.Entity, EffectConfig.DoublePunchConfig.PunchAnimationTime);
            }
            else
            {
                // Bumping Punch
                other.TakeDamage(Config.PunchDamage, FightPlayer, info);
                Vector2 bumpDir = other.Entity.Position - FightPlayer.Entity.Position;
                other.AddBumpFrom(FightPlayer, bumpDir * Config.BumpStrength, false);
            }
                
        }

        
        //FightPlayer.AddPlayerPunchCollisionFunction(OnPunchCollisionEnter);
    }

    IEnumerator SecondPunch(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        FightPlayer.SetAnimTrigger("punch"); // Animation can be done locally first...
        Coroutine.Start(Entity, DelayActivePunchHitbox(EffectConfig.DoublePunchConfig.PunchActivationTime, 1));
    }
}