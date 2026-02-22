using System;
using System.Collections.Generic;
using CJBCheatsMenu.Framework.Models;
using StardewModdingAPI;
using StardewValley;

namespace CJBCheatsMenu.Framework;

/// <summary>Context metadata available to cheat implementations.</summary>
internal class CheatContext
{
    /*********
    ** Fields
    *********/
    /// <summary>Get a cached list of all in-game locations.</summary>
    private readonly Func<IEnumerable<GameLocation>> GetAllLocationsImpl;


    /*********
    ** Accessors
    *********/
    /// <summary>The mod configuration.</summary>
    public ModConfig Config { get; }

    /// <summary>Simplifies access to private code.</summary>
    public IReflectionHelper Reflection { get; }

    /// <summary>The display width of an option slot during the last cheats menu render.</summary>
    public int SlotWidth { get; set; }


    /*********
    ** Public methods
    *********/
    /// <summary>Construct an instance.</summary>
    /// <param name="config">The mod configuration.</param>
    /// <param name="reflection">Simplifies access to private code.</param>
    /// <param name="getAllLocations">Get a cached list of all in-game locations.</param>
    public CheatContext(ModConfig config, IReflectionHelper reflection, Func<IEnumerable<GameLocation>> getAllLocations)
    {
        this.Config = config;
        this.Reflection = reflection;
        this.GetAllLocationsImpl = getAllLocations;
    }

    /// <summary>Get all in-game locations.</summary>
    public IEnumerable<GameLocation> GetAllLocations()
    {
        return this.GetAllLocationsImpl();
    }

    /// <summary>Check if the Player & Tools cheat category is enabled.</summary>
    public bool IsPlayerAndToolsEnabled() => this.Config.EnablePlayerAndTools;

    /// <summary>Check if the Farm & Fishing cheat category is enabled.</summary>
    public bool IsFarmAndFishingEnabled() => this.Config.EnableFarmAndFishing;

    /// <summary>Check if the Relationships cheat category is enabled.</summary>
    public bool IsRelationshipsEnabled() => this.Config.EnableRelationships;

    /// <summary>Check if the Skills cheat category is enabled.</summary>
    public bool IsSkillsEnabled() => this.Config.EnableSkills;

    /// <summary>Check if the Time cheat category is enabled.</summary>
    public bool IsTimeEnabled() => this.Config.EnableTime;

    /// <summary>Check if the Warps cheat category is enabled.</summary>
    public bool IsWarpsEnabled() => this.Config.EnableWarps;

    /// <summary>Check if the Weather cheat category is enabled.</summary>
    public bool IsWeatherEnabled() => this.Config.EnableWeather;

    /// <summary>Check if the Advanced cheat category is enabled.</summary>
    public bool IsAdvancedEnabled() => this.Config.EnableAdvanced;
}
