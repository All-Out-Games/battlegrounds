using AO;
using System.Reflection;

public class EquipSkillSlot : SkillSlot
{
    public override void UseSkill()
    {
        FightPlayer owner = SlotsMgr.GetPlayer();
        
        // Use Reflection to call server RPC and cast the skill
        FightPlayerEffectManager mgr = owner.GetEffectMgr();
            
        MethodInfo skillCaster = EffectManagerType.GetMethod($"CallServer_Cast{CurrentSkillKey}");
        if (skillCaster == null)
        {
            TestServerRPC.LogSomethingOnServer($"{MainSkillKey}: Skill Caster for {CurrentSkillKey} NOT FOUND!");
            return;
        }

        skillCaster.Invoke(mgr, BindingFlags.Public | BindingFlags.Instance, null, new object[] {MainSkillKey}, null);
    }
}