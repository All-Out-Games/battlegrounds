using AO;

namespace Assembly.Koh;

public struct KohClass
{
    public int Id;
    public string Name;
    public int Chance;
    public ItemRarity Rarity;
    public SkillConfig.StatBuff Passive;
    public List<string> SkillKeys;
    public string IconPath;
    public string Description;
}

public struct KohRandomSkill
{
    public int Id;
    public string SkillKey;
    public int Chance;
}

public static class KoHClassData
{
    #region Classes

    public static KohClass RandomClass = new KohClass()
    {
        Id = 0,
        Name = "Random",
        Chance = 0,
        Rarity = ItemRarity.Common,
        SkillKeys = {},
        Description = "Try your luck and equip 4 random skills."
    };

    public static KohClass Brawler = new KohClass()
    {
        Id = 1,
        Name = "Brawler",
        Chance = 12,
        Rarity = ItemRarity.Common,
        SkillKeys = {"ShoulderCrash", "DoublePunch", "ClawSlash", "LeapSlam"},
        Description = "Fists and claws, simple and effective. Passively gain 10 Health. ",
        Passive = new SkillConfig.StatBuff()
        {
            BoostType = SkillConfig.StatType.MaxHealth,
            BoostValue = 10
        }
    };

    public static KohClass Defender = new KohClass()
    {
        Id = 2,
        Name = "Defender",
        Chance = 12,
        Rarity = ItemRarity.Common,
        SkillKeys = { "Shield", "Rollout", "IronSkin", "Parry"},
        Description = "Take the defensive stance. Passively gain 15 Health.",
        Passive = new SkillConfig.StatBuff()
        {
            BoostType = SkillConfig.StatType.MaxHealth,
            BoostValue = 15
        }
    };

    public static KohClass Destroyer = new KohClass()
    {
        Id = 3,
        Name = "Destroyer",
        Chance = 6,
        Rarity = ItemRarity.Rare,
        SkillKeys = { "GroundStomp", "Rage", "BattleCry", "SelfDestruct" },
        Description = "Use your offense capability to induce chaos. Passively gain 5 speed."
    };

    #endregion

    public static KohRandomSkill ShoulderCrash = new()
    {
        Id = 0,
        SkillKey = "ShoulderCrash",
        Chance = 12
    };
    
    
    
    
    
    /// <summary>
    /// When a player joins, give them "None" class and draw two different Ids from these classes,
    /// Also generates a random class where 4 skills are drawn from the skill pool
    /// </summary>
    public static KohClass[] Classes = new[] { RandomClass, Brawler, Defender, Destroyer};


    public static KohRandomSkill[] RngSkills = { };
}