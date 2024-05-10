
using System.Reflection;
using AO;

public partial class FightPlayerSkillTree : FightPlayerComponent
{
    // This file is dedicated for skill handlers. They are client RPCs that make modifications to the local clients
    // E.g. unlock abilities, add attributes, enhance or replace abilities.
    // All abilities must implement an Adder and a Remover here.
    protected static Type SkillTreeCompType = Type.GetType("FightPlayerSkillTree");
    
    public void AddSkill(string skillKey, int level)
    {
        if (Network.IsServer)
        {
            MethodInfo skillAdder = SkillTreeCompType.GetMethod($"CallClient_{skillKey}_Adder");
            if (skillAdder == null)
            {
                Log.Error($"SkillAdder for {skillKey} was not found in SkillHandlers.cs!");
                return;
            }
            
            skillAdder.Invoke(this, BindingFlags.Public | BindingFlags.Instance, null, new object[] {level}, null);
        }
    }
    
    public void RemoveSkill(string skillKey)
    {
        if (Network.IsServer)
        {
            MethodInfo skillRemover = SkillTreeCompType.GetMethod($"CallClient_{skillKey}_Remover");
            if (skillRemover == null)
            {
                Log.Error($"SkillRemover for {skillKey} was not found in SkillHandlers.cs!");
                return;
            }

            skillRemover.Invoke(this, BindingFlags.Public | BindingFlags.Instance, null, new object[] {}, null);
        }
    }
    
    // *IMPORTANT*
    // Write handlers - Both the adder and the remover have to be ClientRPCs. They are only allowed to be called from server.
    // The naming convention is [SkillKey]_Adder/Remover. The adder must have a level input (as int) and the remover must not have any parameters.
    // Reflection will be used to call these functions
    // Note that FightPlayerComponent provides access to player directly (call attribute _player)

    [ClientRpc]
    public void Punch_Adder(int level)
    {
        Log.Debug($"Punch Ability is available by default, cur lvl = {level}");
        _player.GetSkillSlots().UpdateSlot("Punch", level, "Punch");
    }

    [ClientRpc]
    public void Punch_Remover()
    {
        // Active & Replacement Ability do not need to be removed. Their upgrade function will just change the abilities' level
        // Stat & Enhancements (normally) need a remover when upgrading. 
    }
}