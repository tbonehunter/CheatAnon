using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CJB.Common;
using CJBCheatsMenu.Framework;
using CJBCheatsMenu.Framework.Cheats.Warps;
using CJBCheatsMenu.Framework.Components;
using CJBCheatsMenu.Framework.Models;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Buildings;

namespace CJBCheatsMenu;

/// <summary>The mod entry point.</summary>
internal class ModEntry : Mod
{
    /*********
    ** Fields
    *********/
    /// <summary>The relative file to the warps data file.</summary>
    private readonly string WarpsPath = "assets/warps.json";

    /// <summary>The mod settings.</summary>
    private ModConfig Config = null!; // set in Entry

    /// <summary>Manages building and loading the warp data assets.</summary>
    private WarpContentLoader WarpContentLoader = null!; // set in Entry

    /// <summary>Manages the cheat implementations.</summary>
    private PerScreen<CheatManager> Cheats = null!; // set in Entry

    /// <summary>The known in-game location.</summary>
    private readonly PerScreen<Lazy<GameLocation[]>> Locations = new(ModEntry.GetLocationsForCache);


    /*********
    ** Public methods
    *********/
    /// <inheritdoc />
    public override void Entry(IModHelper helper)
    {
        CommonHelper.RemoveObsoleteFiles(this, "CJBCheatsMenu.pdb");

        // load config
        this.Config = helper.ReadConfig<ModConfig>();
        this.Monitor.Log($"Started with menu key {this.Config.OpenMenuKey}.");

        // init translations
        I18n.Init(helper.Translation);

        // load custom button textures
        WarpOptionsButton.Initialize(helper);

        // init warp content loader
        this.WarpContentLoader = new WarpContentLoader(this.ModManifest.UniqueID, () => this.Config, this.LoadModData(), this.Monitor, this.Helper.ModRegistry);

        // load cheats
        this.ResetLocationCache();
        this.Cheats = new PerScreen<CheatManager>(() => new CheatManager(this.Config, this.Helper.GameContent, this.Helper.Reflection, this.WarpContentLoader, () => this.Locations.Value.Value));

        // hook events
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.Content.AssetRequested += this.OnAssetRequested;

        helper.Events.Display.Rendered += this.OnRendered;
        helper.Events.Display.MenuChanged += this.OnMenuChanged;

        helper.Events.GameLoop.ReturnedToTitle += this.OnReturnedToTitle;
        helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        helper.Events.GameLoop.Saving += this.OnSaving;
        helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;

        helper.Events.Input.ButtonsChanged += this.OnButtonsChanged;

        helper.Events.World.LocationListChanged += this.OnLocationListChanged;
    }


    /*********
    ** Private methods
    *********/
    /// <inheritdoc cref="IGameLoopEvents.GameLaunched" />
    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        var configMenu = new GenericModConfigMenuIntegration(
            manifest: this.ModManifest,
            modRegistry: this.Helper.ModRegistry,
            config: this.Config,
            save: () => this.Helper.WriteConfig(this.Config)
        );
        configMenu.Register();
    }

    /// <inheritdoc cref="IGameLoopEvents.SaveLoaded" />
    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        this.ResetLocationCache();
        this.Cheats.Value.OnSaveLoaded();
    }

    /// <inheritdoc cref="IGameLoopEvents.ReturnedToTitle" />
    private void OnReturnedToTitle(object? sender, ReturnedToTitleEventArgs e)
    {
        this.ResetLocationCache();
    }

    /// <inheritdoc cref="IWorldEvents.LocationListChanged" />
    private void OnLocationListChanged(object? sender, LocationListChangedEventArgs e)
    {
        this.ResetLocationCache();
    }

    /// <inheritdoc cref="IInputEvents.ButtonsChanged" />
    private void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e)
    {
        // reload config
        if (this.Config.ReloadConfigKey.JustPressed())
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            this.Monitor.Log($"Reloaded config; menu key is {this.Config.OpenMenuKey}.");
        }

        // open menu
        if (this.Config.OpenMenuKey.JustPressed())
        {
            if (Game1.activeClickableMenu is CheatsMenu menu)
                menu.ExitIfValid();

            else if (!Context.IsPlayerFree || Game1.currentMinigame != null)
            {
                // Players often ask for help due to the menu not opening when expected. To
                // simplify troubleshooting, log when the key is ignored.
                if (Game1.currentMinigame != null)
                    this.Monitor.Log($"Received menu open key, but a '{Game1.currentMinigame.GetType().Name}' minigame is active.");
                else if (Game1.eventUp)
                    this.Monitor.Log("Received menu open key, but an event is active.");
                else if (Game1.activeClickableMenu != null)
                    this.Monitor.Log($"Received menu open key, but a '{Game1.activeClickableMenu.GetType().Name}' menu is already open.");
                else
                    this.Monitor.Log("Received menu open key, but the player isn't free.");
            }

            else
            {
                this.Monitor.Log("Received menu open key.");
                CommonHelper.WarnOnGameMenuKeyConflict(this.Helper.Input, this.Monitor, this.Config.OpenMenuKey, "cheats menu");
                this.OpenCheatsMenu();
            }
        }

        // try warp hotkeys when player is completely free (no menu open)
        if (Context.IsPlayerFree && Game1.currentMinigame == null && Game1.activeClickableMenu == null)
            this.TryWarpHotkey(e);

        // handle button if applicable
        if (Game1.keyboardDispatcher?.Subscriber is null)
            this.Cheats.Value.OnButtonsChanged(e);
    }

    /// <inheritdoc cref="IContentEvents.AssetRequested"/>
    private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
    {
        // apply cheats
        if (this.Cheats.IsActiveForScreen())
            this.Cheats.Value.HarvestWithScythe.OnAssetRequested(this.Cheats.Value.Context, e);

        // load warp sections
        this.WarpContentLoader.OnAssetRequested(e);
    }

    /// <inheritdoc cref="IDisplayEvents.Rendered"/>
    private void OnRendered(object? sender, RenderedEventArgs e)
    {
        if (!Context.IsWorldReady)
            return;

        this.Cheats.Value.OnRendered();
    }

    /// <inheritdoc cref="IGameLoopEvents.UpdateTicked"/>
    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (!Context.IsWorldReady)
            return;

        this.Cheats.Value.OnUpdateTicked(e);
    }

    /// <inheritdoc cref="IGameLoopEvents.Saving"/>
    private void OnSaving(object? sender, SavingEventArgs e)
    {
        this.Cheats.Value.OnSaving();
    }

    /// <inheritdoc cref="IDisplayEvents.MenuChanged"/>
    private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
    {
        // save config
        if (e.OldMenu is CheatsMenu)
        {
            this.Helper.WriteConfig(this.Config);
            this.Cheats.Value.OnOptionsChanged();
        }
    }

    /// <summary>Check if any pressed button matches a warp hotkey and execute the warp.</summary>
    /// <param name="e">The button event data.</param>
    private void TryWarpHotkey(ButtonsChangedEventArgs e)
    {
        ModConfig config = this.Config;

        foreach (SButton button in e.Pressed)
        {
            // find a warp hotkey matching this button
            string? warpId = null;
            foreach ((string key, SButton bound) in config.WarpHotkeys)
            {
                if (bound == button) { warpId = key; break; }
            }
            if (warpId == null) continue;

            // parse "Location:X,Y" key format
            int colon = warpId.IndexOf(':');
            if (colon < 0) continue;
            string location = warpId[..colon];
            string[] coords = warpId[(colon + 1)..].Split(',');
            if (coords.Length != 2) continue;
            if (!int.TryParse(coords[0].Trim(), out int tileX) || !int.TryParse(coords[1].Trim(), out int tileY)) continue;

            // enforce progression restrictions if set
            if (!WarpRestrictions.CanAccessLocation(location, config.EnforceWarpProgressionRestrictions))
            {
                this.Monitor.Log($"Hotkey warp to '{location}' blocked by progression restrictions.", LogLevel.Debug);
                continue;
            }

            // reset swimming state then warp
            Game1.player.swimming.Value = false;
            Game1.player.changeOutOfSwimSuit();

            if (location == "Farm" && tileX == 0 && tileY == 0)
            {
                // mirror WarpCheat.WarpToFarm(): drop farmhands in front of their cabin
                string cabinName = Game1.player.homeLocation.Value;
                if (!Context.IsMainPlayer && cabinName != null)
                {
                    bool warped = false;
                    foreach (GameLocation loc in Game1.locations)
                    {
                        foreach (Building building in loc.buildings)
                        {
                            if (building.indoors.Value?.uniqueName.Value == cabinName)
                            {
                                int cx = building.tileX.Value + building.humanDoor.X;
                                int cy = building.tileY.Value + building.humanDoor.Y + 1;
                                Game1.warpFarmer(loc.Name, cx, cy, false);
                                warped = true;
                                break;
                            }
                        }
                        if (warped) break;
                    }
                    if (warped) return;
                }

                // main player (or cabin not found) — go to farmhouse entrance
                var farmhousePos = Game1.getFarm().GetMainFarmHouseEntry();
                Game1.warpFarmer("Farm", farmhousePos.X, farmhousePos.Y, false);
            }
            else
            {
                Game1.warpFarmer(location, tileX, tileY, false);
            }

            return; // execute at most one hotkey warp per tick
        }
    }

    /// <summary>Load the default warps from the data file, if it's valid.</summary>
    [SuppressMessage("ReSharper", "ConstantNullCoalescingCondition", Justification = "The warps field is initialized in this method.")]
    private ModData LoadModData()
    {
        try
        {
            ModData? modData = this.Helper.Data.ReadJsonFile<ModData>(this.WarpsPath);
            if (modData != null)
                return modData;

            this.Monitor.Log($"Some of the mod files are missing ({this.WarpsPath}); try reinstalling this mod.", LogLevel.Error);
        }
        catch (Exception ex)
        {
            this.Monitor.Log($"Some of the mod files are broken or corrupted ({this.WarpsPath}); try reinstalling this mod.\n{ex}", LogLevel.Error);
        }

        return new ModData(null, null);
    }

    /// <summary>Open the cheats menu.</summary>
    private void OpenCheatsMenu()
    {
        // This method simplifies reflection when another mod wants to open the menu.

        this.OpenCheatsMenu(this.Config.DefaultTab, isNewMenu: true);
    }

    /// <summary>Open the cheats menu.</summary>
    /// <param name="tab">The tab to preselect.</param>
    /// <param name="isNewMenu">Whether to play the open-menu sound.</param>
    private void OpenCheatsMenu(MenuTab tab, bool isNewMenu)
    {
        Game1.activeClickableMenu = new CheatsMenu(tab, this.Cheats.Value, this.Monitor, isNewMenu, ReopenForTab);

        void ReopenForTab(MenuTab newTab) => this.OpenCheatsMenu(newTab, isNewMenu: false);
    }

    /// <summary>Reset the cached location list.</summary>
    private void ResetLocationCache()
    {
        if (this.Locations.Value.IsValueCreated)
            this.Locations.Value = ModEntry.GetLocationsForCache();
    }

    /// <summary>Get a cached lookup of available locations.</summary>
    private static Lazy<GameLocation[]> GetLocationsForCache()
    {
        return new(() => Context.IsWorldReady
            ? CommonHelper.GetAllLocations().ToArray()
            : []
        );
    }
}
