public partial class SkillConfig
{

    public struct SkillTreeTabConfig
    {
        public SkillTreeTabs Tab;
        public string TabName;
        
        // Tree page background
        public string SkillPageBg;
        public string InfoScreenBg;
        
        // Tree Icon
        public string TreeIcon;
        public string TreeIconPressed;
    }
    
    // This affects how many pages appear on the ability book and ability vendor. Each page will have a tag that classifies the items.
    public static readonly List<SkillTreeTabs> STConfigTabsList = new List<SkillTreeTabs>()
    {
        SkillTreeTabs.Basic,
        SkillTreeTabs.Brawler,
        SkillTreeTabs.Defensive,
        SkillTreeTabs.Stealth,
        SkillTreeTabs.Psionic,
        SkillTreeTabs.Elemental
    };

    public static readonly Dictionary<SkillTreeTabs, string> STTabsNameQueryDict =
        new Dictionary<SkillTreeTabs, string>()
        {
            {SkillTreeTabs.Basic, "Basic"},
            {SkillTreeTabs.Brawler, "Brawler"},
            {SkillTreeTabs.Defensive, "Defensive"},
            {SkillTreeTabs.Stealth, "Stealth"},
            {SkillTreeTabs.Psionic, "Psionic"},
            {SkillTreeTabs.Elemental, "Elemental"}
        };
    
    #region Tab Configs

    public static readonly SkillTreeTabConfig BasicTreeTabConfig = new SkillTreeTabConfig()
    {
        Tab = SkillTreeTabs.Basic,
        TabName = "Basic",
        SkillPageBg = "UI/SkillTree/skill_page_basic.png",
        InfoScreenBg = "UI/AbilityBook/AbilityInfo/info_window_basic.png",
        TreeIcon = "UI/SkillTree/small_skill_tree_icons/basic.png",
        TreeIconPressed = "UI/SkillTree/small_skill_tree_icons/basic.png"
    };
    
    public static readonly SkillTreeTabConfig BrawlerTreeTabConfig = new SkillTreeTabConfig()
    {
        Tab = SkillTreeTabs.Brawler,
        TabName = "Brawler",
        SkillPageBg = "UI/SkillTree/skill_page_brawler.png",
        InfoScreenBg = "UI/AbilityBook/AbilityInfo/info_window_brawler.png",
        TreeIcon = "UI/SkillTree/small_skill_tree_icons/brawler.png",
        TreeIconPressed = "UI/SkillTree/small_skill_tree_icons/brawler.png"
    };
    
    public static readonly SkillTreeTabConfig DefensiveTabConfig = new SkillTreeTabConfig()
    {
        Tab = SkillTreeTabs.Defensive,
        TabName = "Defensive",
        SkillPageBg = "UI/SkillTree/skill_page_defense.png",
        InfoScreenBg = "UI/AbilityBook/AbilityInfo/info_window_defense.png",
        TreeIcon = "UI/SkillTree/small_skill_tree_icons/defense.png",
        TreeIconPressed = "UI/SkillTree/small_skill_tree_icons/defense.png"
    };
    
    public static readonly SkillTreeTabConfig StealthTabConfig = new SkillTreeTabConfig()
    {
        Tab = SkillTreeTabs.Stealth,
        TabName = "Stealth",
        SkillPageBg = "UI/SkillTree/skill_page_stealth.png",
        InfoScreenBg = "UI/AbilityBook/AbilityInfo/info_window_stealth.png",
        TreeIcon = "UI/SkillTree/small_skill_tree_icons/stealth.png",
        TreeIconPressed = "UI/SkillTree/small_skill_tree_icons/stealth.png"
    };
    
    public static readonly SkillTreeTabConfig PsionicTabConfig = new SkillTreeTabConfig()
    {
        Tab = SkillTreeTabs.Psionic,
        TabName = "Psionic",
        SkillPageBg = "UI/SkillTree/skill_page_psionic.png",
        InfoScreenBg = "UI/AbilityBook/AbilityInfo/info_window_psionic.png",
        TreeIcon = "UI/SkillTree/small_skill_tree_icons/psionic.png",
        TreeIconPressed = "UI/SkillTree/small_skill_tree_icons/psionic.png"
    };
    
    public static readonly SkillTreeTabConfig ElementalTabConfig = new SkillTreeTabConfig()
    {
        Tab = SkillTreeTabs.Elemental,
        TabName = "Elemental",
        SkillPageBg = "UI/SkillTree/skill_page_elemental.png",
        InfoScreenBg = "UI/AbilityBook/AbilityInfo/info_window_elemental.png",
        TreeIcon = "UI/SkillTree/small_skill_tree_icons/element.png",
        TreeIconPressed = "UI/SkillTree/small_skill_tree_icons/element.png"
    };

    #endregion

    public static readonly Dictionary<SkillTreeTabs, SkillTreeTabConfig> STTabQueryDict =
        new Dictionary<SkillTreeTabs, SkillTreeTabConfig>()
        {
            {SkillTreeTabs.Basic, BasicTreeTabConfig},
            {SkillTreeTabs.Brawler, BrawlerTreeTabConfig},
            {SkillTreeTabs.Defensive, DefensiveTabConfig},
            {SkillTreeTabs.Stealth, StealthTabConfig},
            {SkillTreeTabs.Psionic, PsionicTabConfig},
            {SkillTreeTabs.Elemental, ElementalTabConfig}
        };



}