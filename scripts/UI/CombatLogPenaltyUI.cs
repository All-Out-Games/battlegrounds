using AO;


public static class CombatLogPenaltyUI
{
    static Texture ButtonGreen = Assets.KeepLoaded<Texture>("$AO/new/modal/buttons_2/button_2.png", synchronous: false);

    public static void Draw(FightPlayer player)
    {
        if (Network.IsServer) return;
        if (!player.Alive() || !player.IsLocal) return;
        if (!player.CombatLogDialogOpen) return;

        // Darken background + block input behind the dialog
        UI.Image(UI.ScreenRect, null, new Vector4(0, 0, 0, 0.8f));

        var windowRect = UI.SafeRect.CenterRect();
        windowRect = windowRect.Grow(300, 500, 300, 500);

        UI.Blocker(windowRect, "combat_log_penalty");
        UI.Image(windowRect, FightClubGameManager.References.FrameWhite, Vector4.White, SceneReferenceHolder.WhiteFrameSlice);

        // Layout
        var contentRect = windowRect.Inset(65, 65, 65, 65);
        var footerRect = contentRect.CutBottom(175);
        var headerRect = contentRect.CutTop(140);

        // Header
        {
            var titleRect = headerRect.InsetLeft(160);
            var titleTs = new UI.TextSettings()
            {
                Font = UI.Fonts.BarlowBold,
                Size = 56,
                Color = Vector4.White,
                DropShadowColor = new Vector4(0f, 0f, 0.02f, 0.5f),
                DropShadowOffset = new Vector2(0f, -3f),
                HorizontalAlignment = UI.HorizontalAlignment.Left,
                VerticalAlignment = UI.VerticalAlignment.Center,
                WordWrap = true,
                Outline = true,
                OutlineThickness = 3,
            };
            UI.TextAsync(titleRect, player.CombatLogDialogTitle ?? "Combat Logging", titleTs);
        }

        // Body
        {
            var bodyTs = new UI.TextSettings()
            {
                Font = UI.Fonts.BarlowBold,
                Size = 34,
                Color = Vector4.White,
                DropShadowColor = new Vector4(0f, 0f, 0.02f, 0.5f),
                DropShadowOffset = new Vector2(0f, -3f),
                HorizontalAlignment = UI.HorizontalAlignment.Left,
                VerticalAlignment = UI.VerticalAlignment.Top,
                WordWrap = true,
                WordWrapOffset = 10,
                Outline = true,
                OutlineThickness = 3,
            };
            UI.TextAsync(contentRect, player.CombatLogDialogMessage ?? "", bodyTs);
        }

        // Footer / OK button
        {
            var okRect = footerRect.CenterRect().Grow(60, 240, 60, 240);
            using var _ = UI.PUSH_ID("combat_log_ok");
            var bs = new UI.ButtonSettings()
            {
                Sprite = ButtonGreen,
                PressScaling = 0.25f,
            };
            var ts = new UI.TextSettings()
            {
                Font = UI.Fonts.BarlowBold,
                Size = 46,
                Color = Vector4.White,
                DropShadowColor = new Vector4(0f, 0f, 0.02f, 0.5f),
                DropShadowOffset = new Vector2(0f, -3f),
                HorizontalAlignment = UI.HorizontalAlignment.Center,
                VerticalAlignment = UI.VerticalAlignment.Center,
                WordWrap = false,
                Outline = true,
                OutlineThickness = 3,
            };

            if (UI.Button(okRect, "OK", bs, ts).Clicked)
            {
                player.CombatLogDialogOpen = false;
            }
        }
    }
}


