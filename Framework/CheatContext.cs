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

    // ──────────────────────────────────────────────────────────────
    // Subcategory helpers — each checks category AND subcategory
    // ──────────────────────────────────────────────────────────────

    /****
    ** Player & Tools subcategories
    ****/
    /// <summary>Check if the Player stats subcategory is enabled (health, stamina, speed, etc.).</summary>
    public bool IsPlayerStatsEnabled() => this.Config.EnablePlayerAndTools && this.Config.EnablePlayerStats;

    /// <summary>Check if the Tools subcategory is enabled (one-hit break, infinite water, scythe).</summary>
    public bool IsToolsEnabled() => this.Config.EnablePlayerAndTools && this.Config.EnableTools;

    /// <summary>Check if the Tool Enchantments subcategory is enabled.</summary>
    public bool IsToolEnchantmentsEnabled() => this.Config.EnablePlayerAndTools && this.Config.EnableToolEnchantments;

    /// <summary>Check if the Add Money subcategory is enabled.</summary>
    public bool IsAddMoneyEnabled() => this.Config.EnablePlayerAndTools && this.Config.EnableAddMoney;

    /// <summary>Check if the Add Casino Coins subcategory is enabled.</summary>
    public bool IsAddCasinoCoinsEnabled() => this.Config.EnablePlayerAndTools && this.Config.EnableAddCasinoCoins;

    /// <summary>Check if the Add Golden Walnuts subcategory is enabled.</summary>
    public bool IsAddGoldenWalnutsEnabled() => this.Config.EnablePlayerAndTools && this.Config.EnableAddGoldenWalnuts;

    /// <summary>Check if the Add Qi Gems subcategory is enabled.</summary>
    public bool IsAddQiGemsEnabled() => this.Config.EnablePlayerAndTools && this.Config.EnableAddQiGems;

    /****
    ** Farm & Fishing subcategories
    ****/
    /// <summary>Check if the Farm subcategory is enabled (auto-water, fences, building, animals, hay).</summary>
    public bool IsFarmEnabled() => this.Config.EnableFarmAndFishing && this.Config.EnableFarm;

    /// <summary>Check if the Fishing subcategory is enabled.</summary>
    public bool IsFishingEnabled() => this.Config.EnableFarmAndFishing && this.Config.EnableFishing;

    /// <summary>Check if the Fast Machine Processing subcategory is enabled.</summary>
    public bool IsFastMachinesEnabled() => this.Config.EnableFarmAndFishing && this.Config.EnableFastMachines;

    /****
    ** Skills subcategories
    ****/
    /// <summary>Check if the Skill Levels subcategory is enabled.</summary>
    public bool IsSkillLevelsEnabled() => this.Config.EnableSkills && this.Config.EnableSkillLevels;

    /// <summary>Check if the Professions subcategory is enabled.</summary>
    public bool IsProfessionsEnabled() => this.Config.EnableSkills && this.Config.EnableProfessions;

    /****
    ** Relationships subcategories
    ****/
    /// <summary>Check if the Give Gifts Anytime subcategory is enabled.</summary>
    public bool IsGiveGiftsAnytimeEnabled() => this.Config.EnableRelationships && this.Config.EnableGiveGiftsAnytime;

    /// <summary>Check if the No Friendship Decay subcategory is enabled.</summary>
    public bool IsNoFriendshipDecayEnabled() => this.Config.EnableRelationships && this.Config.EnableNoFriendshipDecay;

    /// <summary>Check if the Adjust Friendship Levels subcategory is enabled.</summary>
    public bool IsAdjustFriendshipLevelsEnabled() => this.Config.EnableRelationships && this.Config.EnableAdjustFriendshipLevels;

    /****
    ** Advanced subcategories
    ****/
    /// <summary>Check if the Complete Quests subcategory is enabled.</summary>
    public bool IsCompleteQuestsEnabled() => this.Config.EnableAdvanced && this.Config.EnableCompleteQuests;

    /// <summary>Check if the Wallet Items subcategory is enabled.</summary>
    public bool IsWalletItemsEnabled() => this.Config.EnableAdvanced && this.Config.EnableWalletItems;

    /// <summary>Check if the Unlocked Areas subcategory is enabled.</summary>
    public bool IsUnlockedAreasEnabled() => this.Config.EnableAdvanced && this.Config.EnableUnlockedAreas;

    /// <summary>Check if the Unlocked Content subcategory is enabled.</summary>
    public bool IsUnlockedContentEnabled() => this.Config.EnableAdvanced && this.Config.EnableUnlockedContent;

    /// <summary>Check if the Community Center subcategory is enabled.</summary>
    public bool IsCommunityCenterEnabled() => this.Config.EnableAdvanced && this.Config.EnableCommunityCenter;

    /****
    ** Warp section helper
    ****/
    /// <summary>Check if a specific warp section is enabled.</summary>
    /// <param name="sectionId">The section ID from warps.json (e.g. "warp-section.main").</param>
    public bool IsWarpSectionEnabled(string sectionId)
    {
        if (!this.Config.EnableWarps)
            return false;

        return sectionId switch
        {
            "warp-section.main"     => this.Config.EnableWarpSectionMain,
            "warp-section.town"     => this.Config.EnableWarpSectionTown,
            "warp-section.forest"   => this.Config.EnableWarpSectionForest,
            "warp-section.mountain" => this.Config.EnableWarpSectionMountain,
            "warp-section.beach"    => this.Config.EnableWarpSectionBeach,
            "warp-section.desert"   => this.Config.EnableWarpSectionDesert,
            "warp-section.island"   => this.Config.EnableWarpSectionIsland,
            _                       => true // unknown sections default to visible
        };
    }

    /****
    ** "Any subcategory enabled" helpers — used for tab visibility
    ****/
    /// <summary>Check if at least one Player &amp; Tools subcategory is enabled.</summary>
    public bool HasAnyPlayerAndToolsSubEnabled() =>
        this.Config.EnablePlayerStats || this.Config.EnableTools || this.Config.EnableToolEnchantments ||
        this.Config.EnableAddMoney || this.Config.EnableAddCasinoCoins ||
        this.Config.EnableAddGoldenWalnuts || this.Config.EnableAddQiGems;

    /// <summary>Check if at least one Farm &amp; Fishing subcategory is enabled.</summary>
    public bool HasAnyFarmAndFishingSubEnabled() =>
        this.Config.EnableFarm || this.Config.EnableFishing || this.Config.EnableFastMachines;

    /// <summary>Check if at least one Skills subcategory is enabled.</summary>
    public bool HasAnySkillsSubEnabled() =>
        this.Config.EnableSkillLevels || this.Config.EnableProfessions;

    /// <summary>Check if at least one Relationships subcategory is enabled.</summary>
    public bool HasAnyRelationshipsSubEnabled() =>
        this.Config.EnableGiveGiftsAnytime || this.Config.EnableNoFriendshipDecay || this.Config.EnableAdjustFriendshipLevels;

    /// <summary>Check if at least one Warp section subcategory is enabled.</summary>
    public bool HasAnyWarpsSubEnabled() =>
        this.Config.EnableWarpSectionMain || this.Config.EnableWarpSectionTown ||
        this.Config.EnableWarpSectionForest || this.Config.EnableWarpSectionMountain ||
        this.Config.EnableWarpSectionBeach || this.Config.EnableWarpSectionDesert ||
        this.Config.EnableWarpSectionIsland;

    /// <summary>Check if at least one Advanced subcategory is enabled.</summary>
    public bool HasAnyAdvancedSubEnabled() =>
        this.Config.EnableCompleteQuests || this.Config.EnableWalletItems ||
        this.Config.EnableUnlockedAreas || this.Config.EnableUnlockedContent ||
        this.Config.EnableCommunityCenter;
}
