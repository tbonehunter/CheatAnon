// WarpOptionsButton.cs
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using CJBCheatsMenu.Framework.Models;

namespace CJBCheatsMenu.Framework.Components;

/// <summary>A button which shows a warp location label with a Go button and a Set Hotkey button.</summary>
internal class WarpOptionsButton : BaseOptionsElement
{
    /*********
    ** Fields
    *********/
    /// <summary>The action to perform when the Go button is clicked.</summary>
    private readonly Action GoAction;

    /// <summary>The unique warp ID used as the hotkey dictionary key (format: "Location:X,Y").</summary>
    private readonly string WarpId;

    /// <summary>The mod configuration for reading/writing warp hotkeys.</summary>
    private readonly ModConfig Config;

    /// <summary>The source rectangle for the vanilla 'Set' button sprite, used as fallback and for the Set Hotkey button.</summary>
    private static readonly Rectangle VanillaButtonSprite = new(294, 428, 21, 11);

    /// <summary>The custom texture for the Go button, loaded once at startup. Falls back to the vanilla sprite if null.</summary>
    internal static Texture2D? GoButtonTexture;

    /// <summary>The Go button area in screen pixels (relative to slot origin).</summary>
    private readonly Rectangle GoButtonBounds;

    /// <summary>The Set Hotkey button area in screen pixels (relative to slot origin).</summary>
    private readonly Rectangle HotkeyButtonBounds;

    /// <summary>Buttons that cannot be bound as hotkeys.</summary>
    private static readonly HashSet<SButton> InvalidButtons =
    [
        SButton.None,
        SButton.Escape,
        SButton.ControllerB,
        SButton.MouseLeft,
        SButton.MouseRight,
        SButton.LeftThumbstickDown,
        SButton.LeftThumbstickLeft,
        SButton.LeftThumbstickRight,
        SButton.LeftThumbstickUp,
        SButton.RightThumbstickDown,
        SButton.RightThumbstickLeft,
        SButton.RightThumbstickRight,
        SButton.LeftShoulder,
        SButton.RightShoulder
    ];


    /*********
    ** Accessors
    *********/
    /// <summary>Whether the control is currently listening for a key press to bind.</summary>
    public bool IsListening { get; private set; }


    /*********
    ** Static methods
    *********/
    /// <summary>Load the custom button textures. Call once from ModEntry.Entry.</summary>
    /// <param name="helper">The mod helper used to load content.</param>
    public static void Initialize(IModHelper helper)
    {
        WarpOptionsButton.GoButtonTexture = helper.ModContent.Load<Texture2D>("assets/button-go.png");
    }


    /*********
    ** Public methods
    *********/
    /// <summary>Construct an instance.</summary>
    /// <param name="label">The warp location label.</param>
    /// <param name="slotWidth">The field width.</param>
    /// <param name="warpId">The unique warp ID for the hotkey dictionary (format: "Location:X,Y").</param>
    /// <param name="config">The mod configuration.</param>
    /// <param name="goAction">The action to perform when the Go button is clicked.</param>
    public WarpOptionsButton(string label, int slotWidth, string warpId, ModConfig config, Action goAction)
        : base(label, -1, -1, slotWidth + 1, 11 * Game1.pixelZoom)
    {
        this.WarpId = warpId;
        this.Config = config;
        this.GoAction = goAction;

        // Go button on the far right — same position as the existing "Set" button
        this.GoButtonBounds = new Rectangle(slotWidth - 28 * Game1.pixelZoom, -1 + Game1.pixelZoom * 3, 21 * Game1.pixelZoom, 11 * Game1.pixelZoom);

        // Set Hotkey button immediately to the left of the Go button, with a 4-pixel gap
        this.HotkeyButtonBounds = new Rectangle(slotWidth - (28 + 21 + 4) * Game1.pixelZoom, -1 + Game1.pixelZoom * 3, 21 * Game1.pixelZoom, 11 * Game1.pixelZoom);
    }

    /// <inheritdoc />
    public override string? GetHoverText(int slotX, int slotY)
    {
        if (this.GoButtonBounds.Contains(slotX, slotY))
            return "Go to this location.";
        if (this.HotkeyButtonBounds.Contains(slotX, slotY))
            return "Set a hotkey to warp here without opening the menu.\nPress Escape or any invalid key to clear.";
        return null;
    }

    /// <inheritdoc />
    public override void receiveLeftClick(int x, int y)
    {
        if (this.greyedOut || this.IsListening)
            return;

        if (this.GoButtonBounds.Contains(x, y))
        {
            this.GoAction();
        }
        else if (this.HotkeyButtonBounds.Contains(x, y) && Constants.TargetPlatform != GamePlatform.Android)
        {
            this.IsListening = true;
            Game1.playSound("breathin");
            GameMenu.forcePreventClose = true;
        }
    }

    /// <inheritdoc />
    public override void ReceiveButtonPress(SButton button)
    {
        if (!this.IsListening)
            return;

        if (WarpOptionsButton.InvalidButtons.Contains(button))
        {
            // invalid or escape press clears the binding
            this.Config.WarpHotkeys.Remove(this.WarpId);
            Game1.playSound("bigDeSelect");
        }
        else
        {
            this.Config.WarpHotkeys[this.WarpId] = button;
            Game1.playSound("coin");
        }

        this.IsListening = false;
        GameMenu.forcePreventClose = false;
    }

    /// <inheritdoc />
    public override void draw(SpriteBatch spriteBatch, int slotX, int slotY, IClickableMenu? context = null)
    {
        // append current hotkey to label if one is bound
        string hotkeyText = this.Config.WarpHotkeys.TryGetValue(this.WarpId, out SButton hotkey) && hotkey != SButton.None
            ? $" [{hotkey}]"
            : "";
        string displayLabel = $"{this.label}{hotkeyText}";

        // draw label
        Utility.drawTextWithShadow(spriteBatch, displayLabel, Game1.dialogueFont,
            new Vector2(this.bounds.X + slotX, this.bounds.Y + slotY),
            this.greyedOut ? Game1.textColor * 0.33f : Game1.textColor, 1f, 0.15f);

        if (Constants.TargetPlatform != GamePlatform.Android)
        {
            // Go button — use custom texture if loaded, otherwise fall back to vanilla sprite
            if (WarpOptionsButton.GoButtonTexture != null)
                Utility.drawWithShadow(spriteBatch, WarpOptionsButton.GoButtonTexture,
                    new Vector2(this.GoButtonBounds.X + slotX, this.GoButtonBounds.Y + slotY),
                    new Rectangle(0, 0, 21, 11),
                    Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom, false, 0.15f);
            else
                Utility.drawWithShadow(spriteBatch, Game1.mouseCursors,
                    new Vector2(this.GoButtonBounds.X + slotX, this.GoButtonBounds.Y + slotY),
                    WarpOptionsButton.VanillaButtonSprite, Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom, false, 0.15f);

            // Set Hotkey button — vanilla 'Set' sprite (same as CheatsOptionsButton)
            Utility.drawWithShadow(spriteBatch, Game1.mouseCursors,
                new Vector2(this.HotkeyButtonBounds.X + slotX, this.HotkeyButtonBounds.Y + slotY),
                WarpOptionsButton.VanillaButtonSprite, Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom, false, 0.15f);
        }

        // key-listening overlay — darkens the screen and prompts for a key press
        if (this.IsListening)
        {
            spriteBatch.Draw(Game1.staminaRect,
                new Rectangle(0, 0, Game1.graphics.GraphicsDevice.Viewport.Width, Game1.graphics.GraphicsDevice.Viewport.Height),
                new Rectangle(0, 0, 1, 1), Color.Black * 0.75f, 0.0f, Vector2.Zero, SpriteEffects.None, 0.999f);
            spriteBatch.DrawString(Game1.dialogueFont, I18n.Controls_PressNewKey(),
                Utility.getTopLeftPositionForCenteringOnScreen(Game1.tileSize * 3, Game1.tileSize),
                Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.9999f);
        }
    }
}
