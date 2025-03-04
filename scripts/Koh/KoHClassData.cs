using AO;

namespace Assembly.Koh;

public struct KohClass
{
    public int Id;
    public string Name;
    public int Chance;
    public ItemRarity Rarity;
    public SkillConfig.StatBuff Passive;
    public string[] SkillKeys;
    public string IconPath;
    public string Description;
}

public struct KohRandomSkill
{
    public int Id;
    public string SkillKey;
    public int Chance;
}

public struct SkillPackage
{
    // Generate two class Ids and 4 random skills per roll
    public int ClassId0;
    public int ClassId1;
    public int[] Rng;
}

public static class KohClassData
{
    #region Classes

    public static KohClass RandomClass = new KohClass()
    {
        Id = 0,
        Name = "Random",
        Chance = 0,
        Rarity = ItemRarity.Common,
        SkillKeys = new string[]{},
        Description = "Try your luck and equip 4 random skills."
    };

    public static KohClass Brawler = new KohClass()
    {
        Id = 1,
        Name = "Brawler",
        Chance = 12,
        Rarity = ItemRarity.Common,
        SkillKeys = new string[]{"ShoulderCrash", "DoublePunch", "ClawSlash", "LeapSlam"},
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
        SkillKeys = new string[]{ "Shield", "RollOut", "IronSkin", "Parry"},
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
        Chance = 4,
        Rarity = ItemRarity.Epic,
        SkillKeys = new string[]{ "GroundStomp", "Rage", "BattleCry", "SelfDestruct" },
        Description = "Use your AoE capability to induce chaos. Passively gain 5 health.",
        Passive = new SkillConfig.StatBuff()
        {
            BoostType = SkillConfig.StatType.MaxHealth,
            BoostValue = 5
        }
    };

    public static KohClass DarkKnight = new KohClass()
    {
        Id = 4,
        Name = "Dark Knight",
        Chance = 8,
        Rarity = ItemRarity.Rare,
        SkillKeys = new string[] { "GravityCrush", "SpikeShield", "TotalDarkness", "ShadowStep" },
        Description = "The last one standing in the shadows. Passively gain 3 attack.",
        Passive = new SkillConfig.StatBuff()
        {
            BoostType = SkillConfig.StatType.AttackPower,
            BoostValue = 3
        }
    };

    public static KohClass Ninja = new KohClass()
    {
        Id = 5,
        Name = "Ninja",
        Chance = 4,
        Rarity = ItemRarity.Epic,
        SkillKeys = new string[] { "Shuriken", "Backstab", "BearTrap", "Invisibility" },
        Description = "Live for the perfect ambush. Passively gain 5 speed.",
        Passive = new SkillConfig.StatBuff()
        {
            BoostType = SkillConfig.StatType.BaseSpeed,
            BoostValue = 5
        }
    };

    public static KohClass Psychic = new()
    {
        Id = 6,
        Name = "Psychic",
        Chance = 8,
        Rarity = ItemRarity.Rare,
        SkillKeys = new string[] { "SpoonThrow", "Befuddle", "Hypnotize", "PsionicBeam" },
        Description = "A psionic warrior that controls a 1v1 battle. Passively gain 1 attack.",
        Passive = new SkillConfig.StatBuff()
        {
            BoostType = SkillConfig.StatType.AttackPower,
            BoostValue = 1
        }
    };

    public static KohClass Telekinetic = new()
    {
        Id = 7,
        Name = "Telekinetic",
        Chance = 4,
        Rarity = ItemRarity.Epic,
        SkillKeys = new string[] { "Psybolt", "Regeneration", "WindPunch", "PsyThrow" },
        Description = "Grab, knock and manipulate your enemies with telekenetic powers. Passively gain 5 health.",
        Passive = new SkillConfig.StatBuff()
        {
            BoostType = SkillConfig.StatType.MaxHealth,
            BoostValue = 5
        }
    };

    public static KohClass StormLord = new()
    {
        Id = 8,
        Name = "Storm Lord",
        Chance = 2,
        Rarity = ItemRarity.Legendary,
        SkillKeys = new string[] { "IceFist", "IceStorm", "ChargingStation", "Thunderbolt" },
        Description = "The storm and thunder heed your calls. Passively gain 3 speed.",
        Passive = new SkillConfig.StatBuff()
        {
            BoostType = SkillConfig.StatType.BaseSpeed,
            BoostValue = 3
        }

    };

    public static KohClass Ranger = new()
    {
        Id = 9,
        Name = "Ranger",
        Chance = 8,
        Rarity = ItemRarity.Rare,
        SkillKeys = new string[] { "Psybolt", "Fireball", "LightFeet", "Regeneration" },
        Description = "An agile fighter that specializes in ranged attacks. Passively gain 3 speed.",
        Passive = new SkillConfig.StatBuff()
        {
            BoostType = SkillConfig.StatType.BaseSpeed,
            BoostValue = 3
        }
    };

    public static KohClass Blademaster = new()
    {
        Id = 10,
        Name = "Blademaster",
        Chance = 2,
        Rarity = ItemRarity.Legendary,
        SkillKeys = new string[] {"BladeStorm", "BladeFrenzy", "FlashOfSteel", "Shield"},
        Description = "A katana-wielding warrior who can outmaneuver opponents with ease. Passively gain 3 speed.",
        Passive = new SkillConfig.StatBuff()
        {
            BoostType = SkillConfig.StatType.BaseSpeed,
            BoostValue = 3
        }
    };

    public static KohClass Firelord = new()
    {
        Id = 11,
        Name = "Fire Lord",
        Chance = 2,
        Rarity = ItemRarity.Legendary,
        SkillKeys = new string[] { "Fireball", "FireTornado", "MeteorStrike", "SelfHeal" },
        Description = "Burn everything to ashes. Passively gain 1 attack.",
        Passive = new SkillConfig.StatBuff()
        {
            BoostType = SkillConfig.StatType.AttackPower,
            BoostValue = 1
        }
    };

    #endregion

    public static KohRandomSkill ShoulderCrash = new()
    {
        Id = 0,
        SkillKey = "ShoulderCrash",
        Chance = 12
    };
    
    public static KohRandomSkill GroundStomp = new()
    {
        Id = 1,
        SkillKey = "GroundStomp",
        Chance = 12
    };

    public static KohRandomSkill LeapSlam = new()
    {
        Id = 2,
        SkillKey = "LeapSlam",
        Chance = 3
    };

    public static KohRandomSkill Rage = new()
    {
        Id = 3,
        SkillKey = "Rage",
        Chance = 12
    };

    public static KohRandomSkill Battlecry = new()
    {
        Id = 4,
        SkillKey = "BattleCry",
        Chance = 3
    };

    public static KohRandomSkill DoublePunch = new()
    {
        Id = 5,
        SkillKey = "DoublePunch",
        Chance = 12
    };

    public static KohRandomSkill ClawSlash = new()
    {
        Id = 6,
        SkillKey = "ClawSlash",
        Chance = 6
    };

    public static KohRandomSkill SelfDestruct = new()
    {
        Id = 7,
        SkillKey = "SelfDestruct",
        Chance = 1
    };

    public static KohRandomSkill BladeStorm = new()
    {
        Id = 8,
        SkillKey = "BladeStorm",
        Chance = 1
    };

    public static KohRandomSkill Shield = new()
    {
        Id = 9,
        SkillKey = "Shield",
        Chance = 12
    };

    public static KohRandomSkill Rollout = new()
    {
        Id = 10,
        SkillKey = "RollOut",
        Chance = 6
    };

    public static KohRandomSkill GravityCrush = new()
    {
        Id = 11,
        SkillKey = "GravityCrush",
        Chance = 6
    };

    public static KohRandomSkill Parry = new()
    {
        Id = 12,
        SkillKey = "Parry",
        Chance = 1
    };

    public static KohRandomSkill IronAura = new()
    {
        Id = 13,
        SkillKey = "IronSkin",
        Chance = 12
    };

    public static KohRandomSkill SpikeGuard = new()
    {
        Id = 14,
        SkillKey = "SpikeShield",
        Chance = 6
    };

    public static KohRandomSkill Invisibility = new()
    {
        Id = 15,
        SkillKey = "Invisibility",
        Chance = 12
    };

    public static KohRandomSkill Shuriken = new()
    {
        Id = 16,
        SkillKey = "Shuriken",
        Chance = 12
    };

    public static KohRandomSkill LightFeet = new()
    {
        Id = 17,
        SkillKey = "LightFeet",
        Chance = 12
    };

    public static KohRandomSkill BearTrap = new()
    {
        Id = 18,
        SkillKey = "BearTrap",
        Chance = 3
    };

    public static KohRandomSkill ShadowStep = new()
    {
        Id = 19,
        SkillKey = "ShadowStep",
        Chance = 6
    };

    public static KohRandomSkill Backstab = new()
    {
        Id = 20,
        SkillKey = "Backstab",
        Chance = 3
    };

    public static KohRandomSkill TotalDarkness = new()
    {
        Id = 21,
        SkillKey = "TotalDarkness",
        Chance = 1
    };

    public static KohRandomSkill SpoonThrow = new()
    {
        Id = 22,
        SkillKey = "SpoonThrow",
        Chance = 12
    };

    public static KohRandomSkill Psybolt = new()
    {
        Id = 23,
        SkillKey = "Psybolt",
        Chance = 6
    };

    public static KohRandomSkill Befuddle = new()
    {
        Id = 24,
        SkillKey = "Befuddle",
        Chance = 6
    };

    public static KohRandomSkill SelfHeal = new()
    {
        Id = 25,
        SkillKey = "SelfHeal",
        Chance = 6
    };

    public static KohRandomSkill Hypnotize = new()
    {
        Id = 26,
        SkillKey = "Hypnotize",
        Chance = 3
    };

    public static KohRandomSkill PsionicBeam = new()
    {
        Id = 27,
        SkillKey = "PsionicBeam",
        Chance = 3
    };

    public static KohRandomSkill Regeneration = new()
    {
        Id = 28,
        SkillKey = "Regeneration",
        Chance = 3
    };

    public static KohRandomSkill PsyThrow = new()
    {
        Id = 29,
        SkillKey = "PsyThrow",
        Chance = 1
    };

    public static KohRandomSkill IceFist = new()
    {
        Id = 30,
        SkillKey = "IceFist",
        Chance = 12
    };

    public static KohRandomSkill WindPunch = new()
    {
        Id = 31,
        SkillKey = "WindPunch",
        Chance = 6
    };

    public static KohRandomSkill Fireball = new()
    {
        Id = 32,
        SkillKey = "Fireball",
        Chance = 6
    };

    public static KohRandomSkill Thunderbolt = new()
    {
        Id = 33,
        SkillKey = "Thunderbolt",
        Chance = 3
    };

    public static KohRandomSkill ChargingStation = new()
    {
        Id = 34,
        SkillKey = "ChargingStation",
        Chance = 3
    };

    public static KohRandomSkill IceStorm = new()
    {
        Id = 35,
        SkillKey = "IceStorm",
        Chance = 1
    };

    public static KohRandomSkill FireTornado = new()
    {
        Id = 36,
        SkillKey = "FireTornado", 
        Chance = 1
    };

    public static KohRandomSkill MeteorCrash = new()
    {
        Id = 37,
        SkillKey = "MeteorStrike", 
        Chance = 1
    };

    public static KohRandomSkill BladeFrenzy = new()
    {
        Id = 38,
        SkillKey = "BladeFrenzy",
        Chance = 1
    };
    
    public static KohRandomSkill FlashOfSteel = new()
    {
        Id = 39,
        SkillKey = "FlashOfSteel",
        Chance = 1
    };
    
    
    
    /// <summary>
    /// When a player joins, give them "None" class and draw two different Ids from these classes,
    /// Also generates a random class where 4 skills are drawn from the skill pool
    /// </summary>
    public static List<KohClass> Classes = new() { RandomClass, Brawler, Defender, Destroyer, DarkKnight, Ninja, Psychic, Telekinetic, StormLord, Ranger, Blademaster, Firelord};


    public static List<KohRandomSkill> RngSkills = new() 
    {
        ShoulderCrash,
        GroundStomp,
        LeapSlam,
        Rage,
        Battlecry,
        DoublePunch,
        ClawSlash,
        SelfDestruct,
        Shield,
        Rollout,
        GravityCrush,
        Parry,
        IronAura,
        SpikeGuard,
        Invisibility,
        Shuriken,
        LightFeet,
        BearTrap,
        ShadowStep,
        Backstab,
        TotalDarkness,
        SpoonThrow,
        Psybolt,
        Befuddle,
        SelfHeal,
        Hypnotize,
        PsionicBeam,
        Regeneration,
        PsyThrow,
        IceFist,
        WindPunch,
        Fireball,
        Thunderbolt,
        ChargingStation,
        IceStorm,
        BladeStorm,
        FlashOfSteel,
        BladeFrenzy,
        FireTornado,
        MeteorCrash
    };

    public static void KoHSanityCheck()
    {
        bool okay = true;
        // Run this to see if the skill keys are wrong (Don't trust GPT completely!)
        foreach (var kcl in Classes)
        {
            foreach (var key in kcl.SkillKeys)
            {
                if (!SkillConfig.ActiveSkills.Contains(key))
                {
                    Log.Error($"Key: {key} in class {kcl.Name} is not found in Active Skills!");
                    okay = false;
                }
            }
        }

        foreach (var krs in RngSkills)
        {
            if (!SkillConfig.ActiveSkills.Contains(krs.SkillKey))
            {
                Log.Error($"Key: {krs.SkillKey} in RNG config is not found!");
                okay = false;
            }
        }

        if (okay)
        {
            Log.Info("Sanity Check passed. Good to go");
        }
    }

    public static SkillPackage GenerateSkillPackage()
    {
        SkillPackage pkg = new SkillPackage();
        var c1 = Util.SampleWeightedList(Classes, kohc => kohc.Chance, Random.Shared);
        (KohClass, int) c2 = Util.SampleWeightedList(Classes, kohc => kohc.Chance, Random.Shared);
        while (c2.Item2 == c1.Item2)
        {
            c2 = Util.SampleWeightedList(Classes, kohc => kohc.Chance, Random.Shared);
        }

        pkg.ClassId0 = c1.Item1.Id;
        pkg.ClassId1 = c2.Item1.Id;
        
        HashSet<int> uniqueIndices = new HashSet<int>();
        while (uniqueIndices.Count < 4)
        {
            var index = Util.SampleWeightedList(RngSkills, kohs => kohs.Chance, Random.Shared);
            uniqueIndices.Add(index.Item1.Id);
        }

        pkg.Rng = uniqueIndices.ToArray();
        return pkg;
    }

    public static void DebugSkillPackage(FightPlayer fp)
    {
        Log.Warn($"{fp.Name}: Class = {fp.PlayerSkillPackage.ClassId0} & {fp.PlayerSkillPackage.ClassId1} \n " +
                 $"RNGs: {fp.PlayerSkillPackage.Rng[0]} / {fp.PlayerSkillPackage.Rng[1]} / {fp.PlayerSkillPackage.Rng[2]} / {fp.PlayerSkillPackage.Rng[3]}");
    }

    public static string GetClassName(int id)
    {
        if (id == -1)
        {
            return "None";
        }
        else
        {
            KohClass c = Classes.First(f => f.Id == id);
            return c.Name;
        }
    }

    public static KohClass GetClassPackage(int id)
    {
        if (id < 0)
        {
            throw new Exception("None class can't use this function!");
        }
        KohClass c = Classes.First(f => f.Id == id);
        return c;
    }

    public static string GetClassDescription(int id)
    {
        if (id == -1)
        {
            return "You are not using a class. You will automatically equip random skills when you enter combat.";

        }
        else
        {
            return GetClassPackage(id).Description;
        }
    }

    public static string GetRarityBackground(ItemRarity rarity)
    {
        switch (rarity)
        {
            case ItemRarity.Rare:
                return "UI/AbilityBook/AbilityInfo/info_window_defense.png";
            case ItemRarity.Epic:
                return "UI/AbilityBook/AbilityInfo/info_window_psionic.png";
            case ItemRarity.Legendary:
                return "UI/AbilityBook/AbilityInfo/info_window_elemental.png";
            default:
                return "UI/AbilityBook/AbilityInfo/info_window_basic.png";
        }
    }

    public static List<string> GetRandomSkillKeys(SkillPackage pkg)
    {
        return pkg.Rng.Select(id => RngSkills.First(skill => skill.Id == id).SkillKey).ToList();
    }
}