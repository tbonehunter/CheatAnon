using StardewValley;
using StardewValley.Locations;
using StardewValley.Tools;

namespace CJBCheatsMenu.Framework.Cheats.Warps;

/// <summary>Checks whether the player meets progression requirements for various warp locations.</summary>
internal static class WarpRestrictions
{
    /*********
    ** Public methods
    *********/
    /// <summary>Check if the player has unlocked Desert access (Vault bundles or Joja).</summary>
    public static bool CanAccessDesert()
    {
        return Game1.player.mailReceived.Contains("ccVault") // Community Center Vault complete
            || Game1.player.mailReceived.Contains("JojaMember"); // or Joja member (bypasses all bundle restrictions)
    }

    /// <summary>Check if the player has unlocked Ginger Island (boat repair).</summary>
    public static bool CanAccessGingerIsland()
    {
        return Game1.player.hasOrWillReceiveMail("willyBoatFixed");
    }

    /// <summary>Check if the player has unlocked Secret Woods (Steel Axe or better).</summary>
    public static bool CanAccessSecretWoods()
    {
        // Check if player has Steel Axe or better
        if (Game1.player.getToolFromName("Axe") is Axe axe)
            return axe.UpgradeLevel >= Tool.steel; // 2 = Steel, 3 = Gold, 4 = Iridium
        
        return false;
    }

    /// <summary>Check if the player has unlocked Sewer (Rusty Key).</summary>
    public static bool CanAccessSewer()
    {
        return Game1.player.hasRustyKey;
    }

    /// <summary>Check if the player has unlocked Mutant Bug Lair (Dark Talisman quest + Rusty Key).</summary>
    public static bool CanAccessMutantBugLair()
    {
        return Game1.player.hasRustyKey
            && Game1.player.mailReceived.Contains("talkedToGunther");
    }

    /// <summary>Check if the player has unlocked Wizard Tower (entered Community Center).</summary>
    public static bool CanAccessWizardTower()
    {
        return Game1.player.eventsSeen.Contains("611439"); // entered Community Center cutscene
    }

    /// <summary>Check if the player has unlocked Railroad/Bathhouse/Witch's Swamp (rubble cleared on Summer 3 Year 1).</summary>
    public static bool CanAccessRailroadArea()
    {
        // Check if it's Summer 3 Year 1 or later
        if (Game1.year < 1)
            return false;
        if (Game1.year == 1 && (Game1.currentSeason != "summer" || Game1.dayOfMonth < 3))
            return false;
        
        return true;
    }

    /// <summary>Check if the player has unlocked Quarry (Crafts Room bundles or Joja).</summary>
    public static bool CanAccessQuarry()
    {
        return Game1.player.mailReceived.Contains("ccCraftsRoom") // Crafts Room complete
            || Game1.player.mailReceived.Contains("JojaMember"); // or Joja member
    }

    /// <summary>Check if the player has unlocked Mastery Cave (reached Mastery level).</summary>
    public static bool CanAccessMasteryCave()
    {
        // Check if player has all skills maxed (prerequisite for mastery)
        // In Stardew Valley 1.6+, mastery cave requires all skills at level 10
        bool allSkillsMaxed = Game1.player.FarmingLevel >= 10
            && Game1.player.MiningLevel >= 10
            && Game1.player.ForagingLevel >= 10
            && Game1.player.FishingLevel >= 10
            && Game1.player.CombatLevel >= 10;
        
        return allSkillsMaxed;
    }

    /// <summary>Check if the player can access a specific warp location.</summary>
    /// <param name="locationName">The internal location name.</param>
    /// <param name="enforceRestrictions">Whether to enforce progression restrictions.</param>
    public static bool CanAccessLocation(string locationName, bool enforceRestrictions)
    {
        // If restrictions are disabled, allow all warps
        if (!enforceRestrictions)
            return true;

        // Check location-specific restrictions
        return locationName switch
        {
            // Desert locations
            "Desert" or "SkullCave" or "SandyHouse" => CanAccessDesert(),
            
            // Ginger Island locations (including volcano dungeon levels)
            "IslandSouth" or "IslandWest" or "IslandNorth" or "IslandEast" or
            "IslandFarmHouse" or "IslandFarmCave" or "IslandFieldOffice" or
            "IslandHut" or "IslandShrinePepper" or "IslandShrineRubies" or
            "IslandShrineDiamonds" or "IslandShrine" or "Caldera" or
            "LeoTreeHouse" or "QiNutRoom" or "IslandSecret" or
            "VolcanoDungeon0" or "VolcanoDungeon1" or "VolcanoDungeon2" or 
            "VolcanoDungeon3" or "VolcanoDungeon4" or "VolcanoDungeon5" or
            "VolcanoDungeon6" or "VolcanoDungeon7" or "VolcanoDungeon8" or "VolcanoDungeon9" => CanAccessGingerIsland(),
            
            // Secret Woods
            "Woods" => CanAccessSecretWoods(),
            
            // Sewer
            "Sewer" => CanAccessSewer(),
            
            // Mutant Bug Lair (case-insensitive check)
            "BugLand" or "Bugland" => CanAccessMutantBugLair(),
            
            // Wizard Tower
            "WizardHouse" or "WizardHouseBasement" => CanAccessWizardTower(),
            
            // Railroad area (includes Bathhouse and Witch's Swamp)
            "Railroad" or "BathHouse_Entry" or "BathHouse_MensLocker" or
            "BathHouse_WomensLocker" or "BathHouse_Pool" or
            "WitchSwamp" or "WitchHut" or "WitchWarpCave" => CanAccessRailroadArea(),
            
            // Quarry (removed - now handled in WarpCheat by checking tile coordinates)
            // "Mountain" when IsQuarryWarp(locationName) => CanAccessQuarry(),
            
            // Mastery Cave
            "MasteryCave" => CanAccessMasteryCave(),
            
            // All other locations are unrestricted
            _ => true
        };
    }

    /// <summary>Check if a warp to Mountain at specific coordinates is the Quarry warp.</summary>
    /// <param name="locationName">The location name.</param>
    /// <param name="tileX">The tile X coordinate.</param>
    /// <param name="tileY">The tile Y coordinate.</param>
    public static bool IsQuarryWarp(string locationName, int tileX, int tileY)
    {
        // Quarry warp is at Mountain (127, 12) according to warps.json
        return locationName == "Mountain" && tileX == 127 && tileY == 12;
    }
}
