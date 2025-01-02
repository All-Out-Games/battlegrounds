using AO;
using Assembly.scripts;

namespace Assembly.Koh;

public class KoHClassWindow : UniqueUIWindow
{
    [Serialized] public KohClassButton RandomClassBtn;
    [Serialized] public KohClassButton ClassOneBtn;
    [Serialized] public KohClassButton ClassTwoBtn;
    [Serialized] public UIText SelectedClassName; // Class the player is currently on
    public int SelectedId = -1;

    [Serialized] public UIButton ConfirmBtn;
    [Serialized] public Entity GlorySwitchText;
    [Serialized] public Entity NormalSwitchText;

    [Serialized] public UIText ClickedClassName; // Class that the player clicked
    [Serialized] public UIText ClickedClassDesc;
    
    
    public FightPlayer LocalPlayer => Network.LocalPlayer as FightPlayer;
    private KohClassButton[] AllButton => new[] { RandomClassBtn, ClassOneBtn, ClassTwoBtn};

    public bool CostlySwitch; // When a player has a class and the round has already started, switching classes will cost glory.

    public override void OnInstantiate()
    {
        base.OnInstantiate();
        foreach (var btn in AllButton)
        {
            btn.SelectButton.OnClicked += btn.Click;
        }

        ConfirmBtn.OnClicked += OnConfirm;
    }

    public override void OpenWindow()
    {
        base.OpenWindow();
        // Init 3 class items. 
        ref var lpkg = ref LocalPlayer.PlayerSkillPackage;
        RandomClassBtn.InitWithClass(0, LocalPlayer.PlayerClassId, ref lpkg, this);
        ClassOneBtn.InitWithClass(lpkg.ClassId0, LocalPlayer.PlayerClassId, ref lpkg, this);
        ClassTwoBtn.InitWithClass(lpkg.ClassId1, LocalPlayer.PlayerClassId, ref lpkg, this);

        SelectedClassName.Text = KohClassData.GetClassName(LocalPlayer.PlayerClassId);

        CostlySwitch = KohManager.Instance.State == GameState.Round && LocalPlayer.PlayerClassId != -1; // Round started and the player has selected a class
        NormalSwitchText.LocalEnabled = !CostlySwitch;
        GlorySwitchText.LocalEnabled = CostlySwitch;
        if (CostlySwitch)
        {
            ConfirmBtn.Interactable = LocalPlayer.Gem >= KohGlobalData.SwitchClassCost;
        }
        
        ClickedClassName.Text = KohClassData.GetClassName(LocalPlayer.PlayerClassId);
        ClickedClassDesc.Text = KohClassData.GetClassDescription(LocalPlayer.PlayerClassId);
    }
    

    public void OnClassSelected(int id)
    {
        SelectedId = id;
        foreach (var btn in AllButton)
        {
            if (id == btn.ButtonId)
            {
                btn.CheckmarkEntity.LocalEnabled = true;
                ClickedClassName.Text = KohClassData.GetClassName(id);
                ClickedClassDesc.Text = KohClassData.GetClassDescription(id);
            }
            else
            {
                btn.CheckmarkEntity.LocalEnabled = false;
            }
        }
        
    }

    public void OnConfirm()
    {
        // Server RPC to confirm the class
        LocalPlayer.CallServer_RequestEquipClass(SelectedId, CostlySwitch);
        CloseWindow();
    }
}

public class KohClassButton : Component
{
    // Display the class name and 4 skills.
    // If the class is the random class, show the contents when the player owns the game pass
    [Serialized] public Entity CheckmarkEntity;
    [Serialized] public UIButton SelectButton;
    [Serialized] public KohSkillItem[] Items;
    
    [Serialized] private UIText _className;
    [Serialized] private Entity _skillList;
    [Serialized] private Entity _questionMark;
    [Serialized] private UIImage _backPlate;
    private KoHClassWindow _parent;

    public int ButtonId = -1;
    public void InitWithClass(int id, int currentPlayerClassId, ref SkillPackage pkg, KoHClassWindow parent)
    {
        _parent = parent;
        List<string> skillKeys;
        if (id < 0)
        {
            Log.Error("None ID shouldn't be here!");
        }

        if (id == 0)
        {
            ButtonId = id;
            // Random, use the id in the pkg
            skillKeys = KohClassData.GetRandomSkillKeys(pkg);
            _className.Text = "Random";
            _backPlate.Sprite = Assets.GetAsset<Texture>(KohClassData.GetRarityBackground(ItemRarity.Common));
        }
        else
        {
            ButtonId = id;
            // Predefined class
            var classpkg = KohClassData.GetClassPackage(id);
            skillKeys = classpkg.SkillKeys.ToList();
            _className.Text = classpkg.Name;
            _backPlate.Sprite = Assets.GetAsset<Texture>(KohClassData.GetRarityBackground(classpkg.Rarity));
        }
        

        if (skillKeys != null && skillKeys.Count == 4)
        {
            if (id == 0 && !KohGlobalData.OwnKoHGamePass(parent.LocalPlayer))
            {
                // Hide random skills for players who don't own the game pass
                _questionMark.LocalEnabled = true;
                _skillList.LocalEnabled = false;
            }
            else
            {
                // Set skills
                _questionMark.LocalEnabled = false;
                _skillList.LocalEnabled = true;
                for (int i = 0; i < 4; i++)
                {
                    Items[i].SetSkill(skillKeys[i], parent.LocalPlayer.GetSkillTree().GetSkillLevel(skillKeys[i]));
                }
            }
        }
        else
        {
            throw new Exception($"Skill Package Error! Id = {id}");
        }

        CheckmarkEntity.LocalEnabled = currentPlayerClassId == ButtonId;
    }

    public void Click()
    {
        _parent.OnClassSelected(ButtonId);
    }
}

public class KohSkillItem : Component
{
    [Serialized] private Entity[] _stars;
    [Serialized] public UIButton ItemButton;
    [Serialized] public UIText SkillName;

    public void SetSkill(string skillKey, int level)
    {
        for (int i = 0; i < 4; i++)
        {
            _stars[i].LocalEnabled = i < level - 1;
        }
        FightClubUtils.SetButtonTexture(ItemButton, SkillConfig.GetIconPath(skillKey));
        SkillName.Text = SkillConfig.GetConfig(skillKey).GetDisplayName();
    }
}