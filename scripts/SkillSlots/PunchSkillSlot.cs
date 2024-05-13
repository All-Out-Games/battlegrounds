

using System.Reflection;
using AO;

public class PunchSkillSlot : SkillSlot
{
    
    public override void UseSkill()
    {
        // TODO: Find Skill using CurrentSkillKey, fire an RPC to server and cast
        FightPlayer owner = SlotsMgr.GetPlayer();
        
        // Use Reflection to call server RPC and cast the skill
        FightPlayerEffectManager mgr = owner.GetEffectMgr();
            
        MethodInfo skillCaster = EffectManagerType.GetMethod($"CallServer_Cast{CurrentSkillKey}");
        if (skillCaster == null)
        {
            TestServerRPC.LogSomethingOnServer($"PunchSkillSlot: Skill Caster for {CurrentSkillKey} NOT FOUND!");
            return;
        }

        skillCaster.Invoke(mgr, BindingFlags.Public | BindingFlags.Instance, null, null, null);
        TestServerRPC.LogSomethingOnServer($"{CurrentSkillKey} Casted!");
    }
}