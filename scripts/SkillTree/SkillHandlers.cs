
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
            Log.Debug($"AddSkill Called. {skillKey} = Lv. {level}");
            if (!SkillConfig.STConfigQueryDict.TryGetValue(skillKey, out SkillConfig.SkillTreeNodeConfig cfg))
            {
                Log.Error($"{skillKey} not found in STConfig!");
                return;
            }
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
        if (Network.IsServer)
        {
            Log.Debug($"RemoveSkill Called. {skillKey} ");
            if (!SkillConfig.STConfigQueryDict.TryGetValue(skillKey, out SkillConfig.SkillTreeNodeConfig cfg))
            {
                Log.Error($"{skillKey} not found in STConfig!");
                return;
            }

            if (cfg.NeedRemover)
            {
                switch (cfg.NType)
                {
                    case SkillConfig.NodeType.SkillUnlock:
                        // Nothing need to be done as of now. The skill tree will remove the skill from dictionary and they can no longer be casted
                        break;
                    case SkillConfig.NodeType.SkillReplace:
                        CallClient_ReplacementRemover(skillKey); // Currently, punch are the only slot that need replacement
                        break;
                    case SkillConfig.NodeType.AttrBoost: // Only handles single stat buff. If we need multiple stats write a special handler for that
                        CallClient_StatRemover(skillKey, cfg.Buff);
                        break;
                }
            }
        }
    }
    
    // *IMPORTANT*
    // Write handlers - Both the adder and the remover have to be ClientRPCs. They are only allowed to be called from server.
    // The naming convention is [SkillKey]_Adder/Remover. The adder must have a level input (as int) and the remover must not have any parameters.
    // Reflection will be used to call these functions
    // Note that FightPlayerComponent provides access to player directly (call attribute _player)

    #region Generic Handlers

    [ClientRpc]
    public void UnlockAdder(int level, string skillKey)
    {
        
    }

    [ClientRpc]
    public void ReplacementAdder(int level, string skillKey, string slotKey)
    {
        if (slotKey == "Punch")
        {
            if (skillKey == "Punch")
            {
                if (_player.PunchLevel < 1)
                {
                    _player.PunchLevel = 1;
                }
            }
            else if (skillKey == "Punch2")
            {
                if (_player.PunchLevel < 2)
                {
                    _player.PunchLevel = 2;
                }
            }
            else if (skillKey == "Punch3")
            {
                if (_player.PunchLevel < 3)
                {
                    _player.PunchLevel = 3;
                }
            }
        }
    }

    [ClientRpc]
    public void StatAdder(int level, string skillKey, SkillConfig.StatBuff buff)
    {
        switch (buff.BoostType)
        {
            case SkillConfig.StatType.AttackPower:
                _player.CurrentAttack += buff.BoostValue;
                break;
            case SkillConfig.StatType.MaxHealth:
                _player.MaxHealth += buff.BoostValue;
                break;
            case SkillConfig.StatType.BaseSpeed:
                _player.CombatSpeedPercentage += buff.BoostValue;
                break;
            case SkillConfig.StatType.None:
                break;
        }
    }

    [ClientRpc]
    public void ReplacementRemover(string skillKey)
    {
        if (skillKey == "Punch")
        {
            Log.Error("You shouldn't remove punch! Your punch level is now set to 1");
            _player.PunchLevel = 1;
        }
        else if (skillKey == "Punch2")
        {
            if (_player.PunchLevel > 1)
            {
                _player.PunchLevel = 1;
            }
        }
        else if (skillKey == "Punch3")
        {
            if (_player.PunchLevel > 2)
            {
                _player.PunchLevel = 2;
            }
        }
    }

    [ClientRpc]
    public void StatRemover(string skillKey, SkillConfig.StatBuff buff)
    {
        switch (buff.BoostType)
        {
            case SkillConfig.StatType.AttackPower:
                _player.CurrentAttack -= buff.BoostValue;
                break;
            case SkillConfig.StatType.MaxHealth:
                _player.MaxHealth -= buff.BoostValue;
                break;
            case SkillConfig.StatType.BaseSpeed:
                _player.CombatSpeedPercentage -= buff.BoostValue;
                break;
            case SkillConfig.StatType.None:
                break;
        }
    }

    #endregion
    
}