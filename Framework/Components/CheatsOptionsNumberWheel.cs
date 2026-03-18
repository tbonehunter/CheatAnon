using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CJBCheatsMenu.Framework.Models;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace CJBCheatsMenu.Framework.Components;

/// <summary>A button with a label and number selector which invokes a callback when clicked.</summary>
internal class CheatsOptionsNumberWheel : CheatsOptionsButton<CheatsOptionsNumberWheel>
{
    /*********
    ** Fields
    *********/
    /// <summary>The minimum value that can be selected using the field.</summary>
    private readonly int MinValue;

    /// <summary>The maximum value that can be selected using the field.</summary>
    private readonly int MaxValue;

    /// <summary>Format the value for display.</summary>
    private readonly Func<int, string> FormatValue;

    /// <summary>The current value's area in screen pixels.</summary>
    private Rectangle CurrentValueBounds;

    /// <summary>The minus button area in screen pixels.</summary>
    private Rectangle MinusButtonBounds;

    /// <summary>The plus button area in screen pixels.</summary>
    private Rectangle PlusButtonBounds;

    /// <summary>The source rectangle for the 'minus' button sprite.</summary>
    private readonly Rectangle MinusButtonSource = new(177, 345, 7, 8);

    /// <summary>The source rectangle for the 'plus' button sprite.</summary>
    private readonly Rectangle PlusButtonSource = new(184, 345, 7, 8);

    /// <summary>The source rectangle for the hotkey 'Set' button sprite.</summary>
    private static readonly Rectangle SetButtonSprite = new(294, 428, 21, 11);

    /// <summary>The hotkey button area in screen pixels (only set when warpId is provided).</summary>
    private Rectangle HotkeyButtonBounds;

    /// <summary>The unique warp ID for the hotkey dictionary, or null if hotkeys are not supported.</summary>
    private readonly string? WarpId;

    /// <summary>The mod configuration for reading/writing warp hotkeys.</summary>
    private readonly ModConfig? Config;

    /// <summary>Buttons that cannot be bound as hotkeys.</summary>
    private static readonly HashSet<SButton> InvalidButtons =
    [
        SButton.None, SButton.Escape, SButton.ControllerB,
        SButton.MouseLeft, SButton.MouseRight,
        SButton.LeftThumbstickDown, SButton.LeftThumbstickLeft, SButton.LeftThumbstickRight, SButton.LeftThumbstickUp,
        SButton.RightThumbstickDown, SButton.RightThumbstickLeft, SButton.RightThumbstickRight,
        SButton.LeftShoulder, SButton.RightShoulder
    ];


    /*********
    ** Accessors
    *********/
    /// <summary>The current value.</summary>
    public int Value { get; private set; }

    /// <summary>Whether the control is currently listening for a key press to bind.</summary>
    public bool IsListening { get; private set; }


    /*********
    ** Public methods
    *********/
    /// <summary>Construct an instance.</summary>
    /// <param name="label">The field label.</param>
    /// <param name="slotWidth">The field width.</param>
    /// <param name="action">The action to perform when the set button is clicked.</param>
    /// <param name="initialValue">The starting value of the number.</param>
    /// <param name="maxValue">The maximum value of the number.</param>
    /// <param name="minValue">The minimum value of the number.</param>
    /// <param name="formatValue">Format the value for display.</param>
    /// <param name="tooltip">An optional tooltip shown when the cursor hovers over this element.</param>
    /// <param name="warpId">The unique warp ID for the hotkey dictionary, or null to disable hotkey support.</param>
    /// <param name="config">The mod configuration for reading/writing warp hotkeys.</param>
    public CheatsOptionsNumberWheel(string label, int slotWidth, Action<CheatsOptionsNumberWheel> action, int initialValue, int minValue, int maxValue, Func<int, string>? formatValue = null, string? tooltip = null, string? warpId = null, ModConfig? config = null)
        : base(label, slotWidth, action)
    {
        this.Value = Math.Clamp(initialValue, minValue, maxValue);
        this.MaxValue = maxValue;
        this.MinValue = minValue;
        this.FormatValue = formatValue ?? (value => value.ToString());
        this.Tooltip = tooltip;
        this.WarpId = warpId;
        this.Config = config;

        if (warpId != null)
            this.HotkeyButtonBounds = new Rectangle(slotWidth - (28 + 21 + 4) * Game1.pixelZoom, -1 + Game1.pixelZoom * 3, 21 * Game1.pixelZoom, 11 * Game1.pixelZoom);

        this.UpdateLayout();
    }

    /// <inheritdoc />
    public override void receiveLeftClick(int x, int y)
    {
        if (this.IsListening)
            return;

        // check hotkey button first (to the left of the Go button)
        if (this.WarpId != null && this.HotkeyButtonBounds.Contains(x, y) && Constants.TargetPlatform != GamePlatform.Android)
        {
            this.IsListening = true;
            Game1.playSound("breathin");
            GameMenu.forcePreventClose = true;
            return;
        }

        base.receiveLeftClick(x, y);

        if (this.greyedOut)
            return;

        // holding run (shift) will make the number go in steps of 10
        // holding control will do stops of 50, both will do steps of 500
        // todo: nicer way to do this on android?
        int multiplier = Game1.isOneOfTheseKeysDown(Game1.GetKeyboardState(), Game1.options.runButton) ? 10 : 1;
        multiplier *= Game1.GetKeyboardState().IsKeyDown(Keys.LeftControl) ? 50 : 1;

        // stupid hack to get the bounds to line up properly
        if (this.MinusButtonBounds.Contains(x - 8 * Game1.pixelZoom, y - 7 * Game1.pixelZoom))
            this.ChangeValue(-1 * multiplier);

        if (this.PlusButtonBounds.Contains(x - 8 * Game1.pixelZoom, y - 7 * Game1.pixelZoom))
            this.ChangeValue(1 * multiplier);
    }


    /*********
    ** Protected methods
    *********/
    /// <inheritdoc />
    public override void ReceiveButtonPress(SButton button)
    {
        if (!this.IsListening)
            return;

        if (CheatsOptionsNumberWheel.InvalidButtons.Contains(button))
            this.Config!.WarpHotkeys.Remove(this.WarpId!);
        else
            this.Config!.WarpHotkeys[this.WarpId!] = button;

        Game1.playSound(CheatsOptionsNumberWheel.InvalidButtons.Contains(button) ? "bigDeSelect" : "coin");
        this.IsListening = false;
        GameMenu.forcePreventClose = false;
    }

    /// <inheritdoc />
    public override string? GetHoverText(int slotX, int slotY)
    {
        if (this.WarpId != null)
        {
            if (this.SetButtonBounds.Contains(slotX, slotY))
                return "Go to this mine level.";
            if (this.HotkeyButtonBounds.Contains(slotX, slotY))
                return "Set a hotkey to warp to the entrance without opening the menu.\nPress Escape or any invalid key to clear.";
        }
        return base.GetHoverText(slotX, slotY);
    }

    /// <inheritdoc />
    protected override void DrawElement(SpriteBatch spriteBatch, int slotX, int slotY, IClickableMenu? context = null)
    {
        int x = this.bounds.X + slotX;
        int y = this.bounds.Y + slotY;

        // draw label
        Utility.drawTextWithShadow(spriteBatch, this.label, Game1.dialogueFont, new Vector2(x, y), this.greyedOut ? Game1.textColor * 0.33f : Game1.textColor);

        // draw minus button
        Utility.drawWithShadow(spriteBatch, Game1.mouseCursors, new Vector2(x + this.MinusButtonBounds.X, y + this.MinusButtonBounds.Y), this.MinusButtonSource, Color.White, 0f, Vector2.Zero, Game1.pixelZoom);

        // draw value
        Utility.drawTextWithShadow(spriteBatch, this.FormatValue(this.Value), Game1.dialogueFont, new Vector2(x + this.CurrentValueBounds.X, y + this.CurrentValueBounds.Y), Game1.textColor);

        // draw plus button
        Utility.drawWithShadow(spriteBatch, Game1.mouseCursors, new Vector2(x + this.PlusButtonBounds.X, y + this.PlusButtonBounds.Y), this.PlusButtonSource, Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom);
    }

    /// <inheritdoc />
    public override void draw(SpriteBatch spriteBatch, int slotX, int slotY, IClickableMenu? context = null)
    {
        if (this.WarpId != null && Constants.TargetPlatform != GamePlatform.Android)
        {
            // Draw label + level selector only (no vanilla Set sprite)
            this.DrawElement(spriteBatch, slotX, slotY, context);

            // Go button — custom texture if loaded, otherwise vanilla
            if (WarpOptionsButton.GoButtonTexture != null)
                Utility.drawWithShadow(spriteBatch, WarpOptionsButton.GoButtonTexture,
                    new Vector2(this.SetButtonBounds.X + slotX, this.SetButtonBounds.Y + slotY),
                    new Rectangle(0, 0, 21, 11), Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom, false, 0.15f);
            else
                Utility.drawWithShadow(spriteBatch, Game1.mouseCursors,
                    new Vector2(this.SetButtonBounds.X + slotX, this.SetButtonBounds.Y + slotY),
                    CheatsOptionsNumberWheel.SetButtonSprite, Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom, false, 0.15f);

            // Set Hotkey button — vanilla sprite to the left
            Utility.drawWithShadow(spriteBatch, Game1.mouseCursors,
                new Vector2(this.HotkeyButtonBounds.X + slotX, this.HotkeyButtonBounds.Y + slotY),
                CheatsOptionsNumberWheel.SetButtonSprite, Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom, false, 0.15f);
        }
        else
        {
            base.draw(spriteBatch, slotX, slotY, context);
        }

        // key-listening overlay
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

    /// <summary>Calculate the element positions and dimensions.</summary>
    private void UpdateLayout()
    {
        // calculate the correct height for the button to be in the middle of the line
        int buttonY = (int)((double)this.bounds.Height / Game1.pixelZoom / 2d + this.MinusButtonSource.Height / 2d);

        // leave space for the label at the front
        int labelWidth = (int)Game1.dialogueFont.MeasureString(this.label).X / Game1.pixelZoom;
        int xOffset = labelWidth + 4;

        this.MinusButtonBounds = new Rectangle(xOffset * Game1.pixelZoom, buttonY, 7 * Game1.pixelZoom, 8 * Game1.pixelZoom);
        xOffset += this.MinusButtonSource.Width + 2;

        // get the maximum width the value label can be
        Vector2 valueSize = Game1.dialogueFont.MeasureString(new string('0', this.FormatValue(this.Value).Length));
        int maxValueWidth = (int)(valueSize.X / Game1.pixelZoom) + 1;
        int maxValueHeight = (int)valueSize.Y;

        // leave enough space to draw the value without overlapping the plus button
        this.CurrentValueBounds = new Rectangle(xOffset * Game1.pixelZoom, Game1.pixelZoom, maxValueWidth * Game1.pixelZoom, maxValueHeight);
        xOffset += maxValueWidth + 2;
        this.PlusButtonBounds = new Rectangle(xOffset * Game1.pixelZoom, buttonY, 7 * Game1.pixelZoom, 8 * Game1.pixelZoom);
    }

    /// <summary>Change the selected number, and play a sound to signal success.</summary>
    /// <param name="delta">The amount to add to the current value.</param>
    private void ChangeValue(int delta)
    {
        int oldValue = this.Value;
        this.Value = Math.Clamp(this.Value + delta, this.MinValue, this.MaxValue);

        if (this.Value != oldValue)
        {
            Game1.playSound("drumkit6");

            if (this.FormatValue(this.Value).Length != this.FormatValue(oldValue).Length)
                this.UpdateLayout();
        }
    }
}
