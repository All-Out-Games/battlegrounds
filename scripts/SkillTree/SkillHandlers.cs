
using System.Reflection;
using AO;

public partial class FightPlayerSkillTree : FightPlayerComponent
{
    // This file is dedicated for skill handlers. They are client RPCs that make modifications to the local clients
    // E.g. unlock abilities, add attributes, enhance or replace abilities.
    // All abilities must have an Adder and a Remover here.
    protected static Type SkillTreeCompType = Type.GetType("FightPlayerSkillTree");
    
    public void AddSkill(string skillKey, int level)
    {
        if (Network.IsServer)
        {
            SkillConfig.SkillTreeNodeConfig cfg = SkillConfig.STConfigQueryDict[skillKey];
            if (cfg.NeedSpecialHandler)
            {
                // Special handlers will be called using Reflection
                MethodInfo skillAdder = SkillTreeCompType.GetMethod($"CallClient_{skillKey}_Adder");
                if (skillAdder == null)
                {
                    Log.Error($"SkillAdder for {skillKey} was not found in SkillHandlers.cs!");
                    return;
                }
                skillAdder.Invoke(this, BindingFlags.Public | BindingFlags.Instance, null, new object[] {level}, null);
            }
            else
            {
                // Or we use generic handlers by skill type
                switch (cfg.NType)
                {
                    case SkillConfig.NodeType.SkillUnlock:
                        CallClient_UnlockAdder(level, skillKey);
                        break;
                    case SkillConfig.NodeType.SkillReplace:
                        CallClient_ReplacementAdder(level, skillKey, "Punch"); // Currently, punch are the only slot that need replacement
                        break;
                    case SkillConfig.NodeType.AttrBoost: // Only handles single stat buff. If we need multiple stats write a special handler for that
                        CallClient_StatAdder(level, skillKey, cfg.Buff);
                        break;
                }
            }
            

        }
    }
    
    public void RemoveSkill(string skillKey)
    {
        return; // Shin: Design redundancy. Currently not useful. When skills become upgradable we'll need to use this
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

    #region Generic Adders

    [ClientRpc]
    public void UnlockAdder(int level, string skillKey)
    {
        // TODO: TryFindEmptySlot
        // If the slot is edited by player, we don't automatically replace it
        // We'll equip the newly acquired skill automatically if the slot is untouched.
        
    }

    [ClientRpc]
    public void ReplacementAdder(int level, string skillKey, string slotKey)
    {
        _player.GetSkillSlots().UpdateSlot(slotKey, level, skillKey);
    }

    [ClientRpc]
    public void StatAdder(int level, string skillKey, SkillConfig.StatBuff buff)
    {
        // TODO
    }

    #endregion
    
}