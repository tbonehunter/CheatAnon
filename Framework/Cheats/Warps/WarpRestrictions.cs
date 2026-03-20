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

    /// <summary>Check if the player has unlocked Skull Cavern (desert access + Skull Key).</summary>
    public static bool CanAccessSkullCavern()
    {
        return CanAccessDesert() && Game1.player.hasSkullKey;
    }

    /// <summary>Check if the player has unlocked Ginger Island (boat repair).</summary>
    public static bool CanAccessGingerIsland()
    {
        return Game1.player.hasOrWillReceiveMail("willyBoatFixed");
    }

    /// <summary>Check if the player has unlocked Island North (boat repair + Leo's hut parrot paid).</summary>
    public static bool CanAccessIslandNorth()
    {
        // North turtle removed after paying the parrot in Leo's hut (1 Golden Walnut)
        return CanAccessGingerIsland()
            && Game1.player.hasOrWillReceiveMail("Island_FirstParrot");
    }

    /// <summary>Check if the player has unlocked Island West / Island Farm (boat repair + west turtle woken).</summary>
    public static bool CanAccessIslandWest()
    {
        // West turtle woken after paying the beach parrot (10 Golden Walnuts)
        return CanAccessGingerIsland()
            && Game1.player.hasOrWillReceiveMail("Island_Turtle");
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

    /// <summary>Check if the player has unlocked Mutant Bug Lair (has Rusty Key + has retrieved Dark Talisman).</summary>
    public static bool CanAccessMutantBugLair()
    {
        // Requires sewer access (Rusty Key) AND having already retrieved the Dark Talisman
        // from the chest in the Bug Lair on foot, so the warp is only for return visits
        return Game1.player.hasRustyKey
            && Game1.player.hasDarkTalisman;
    }

    /// <summary>Check if the player has unlocked Witch's Swamp (railroad access + magic ink quest complete).</summary>
    public static bool CanAccessWitchSwamp()
    {
        // Requires both: railroad area accessible (Summer 3 Year 1+)
        // AND magic ink quest completed for the Wizard
        return CanAccessRailroadArea() && Game1.player.hasMagicInk;
    }

    /// <summary>Check if the player has unlocked Wizard Tower (entered Community Center).</summary>
    public static bool CanAccessWizardTower()
    {
        return Game1.player.eventsSeen.Contains("611439"); // entered Community Center cutscene
    }

    /// <summary>Check if the player has unlocked Railroad/Bathhouse/Witch's Swamp (rubble cleared on Summer 3 Year 1).</summary>
    public static bool CanAccessRailroadArea()
    {
        // Rubble is cleared the night of Summer 2, Year 1; accessible from Summer 3 onward
        if (Game1.year > 1)
            return true;
        if (Game1.year == 1)
        {
            // Summer day 3 or later means access
            if (Game1.currentSeason == "summer" && Game1.dayOfMonth >= 3)
                return true;
            // Any season after summer in Year 1 means access (fall, winter)
            if (Game1.currentSeason == "fall" || Game1.currentSeason == "winter")
                return true;
        }
        return false;
    }

    /// <summary>Check if the player has unlocked Mines/Adventurer's Guild (landslide cleared Spring 5 Year 1).</summary>
    public static bool CanAccessMines()
    {
        // Landslide is cleared on Spring 5, Year 1
        if (Game1.year > 1)
            return true;
        if (Game1.year == 1)
        {
            // Spring is the first season; day 5 or later means access
            if (Game1.currentSeason == "spring" && Game1.dayOfMonth >= 5)
                return true;
            // Any season after spring in Year 1 means access
            if (Game1.currentSeason != "spring")
                return true;
        }
        return false;
    }

    /// <summary>Check if the player has unlocked Tide Pools (beach bridge repaired).</summary>
    public static bool CanAccessTidePools()
    {
        // Beach bridge is a property on the Beach location object
        if (Game1.getLocationFromName("Beach") is Beach beach)
            return beach.bridgeFixed.Value;
        return false;
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
            // Mines (landslide cleared Spring 5 Year 1)
            "Mine" => CanAccessMines(),
            
            // Desert locations
            "Desert" or "SandyHouse" or "Club" => CanAccessDesert(),
            
            // Skull Cavern (requires desert access + Skull Key from reaching mine level 120)
            "SkullCave" => CanAccessSkullCavern(),
            
            // Ginger Island - East (accessible immediately after boat repair)
            "IslandEast" or "IslandHut" or "LeoTreeHouse" or
            "IslandSouth" or "IslandSouthEast" or "IslandShrine" or
            "IslandShrinePepper" or "IslandShrineRubies" or
            "IslandShrineDiamonds" or "IslandSecret" => CanAccessGingerIsland(),
            
            // Ginger Island - North (requires Leo's hut parrot paid to remove north turtle)
            "IslandNorth" or "IslandFieldOffice" or "Caldera" or
            "VolcanoDungeon0" or "VolcanoDungeon1" or "VolcanoDungeon2" or 
            "VolcanoDungeon3" or "VolcanoDungeon4" or "VolcanoDungeon5" or
            "VolcanoDungeon6" or "VolcanoDungeon7" or "VolcanoDungeon8" or
            "VolcanoDungeon9" => CanAccessIslandNorth(),
            
            // Ginger Island - West / Farm (requires 10 Golden Walnuts to wake west turtle)
            "IslandWest" or "IslandFarmHouse" or "IslandFarmCave" or
            "QiNutRoom" => CanAccessIslandWest(),
            
            // Secret Woods
            "Woods" => CanAccessSecretWoods(),
            
            // Sewer
            "Sewer" => CanAccessSewer(),
            
            // Mutant Bug Lair (case-insensitive check)
            "BugLand" or "Bugland" => CanAccessMutantBugLair(),
            
            // Wizard Tower
            "WizardHouse" or "WizardHouseBasement" => CanAccessWizardTower(),
            
            // Railroad area (includes Bathhouse)
            "Railroad" or "BathHouse_Entry" or "BathHouse_MensLocker" or
            "BathHouse_WomensLocker" or "BathHouse_Pool" => CanAccessRailroadArea(),
            
            // Witch's Swamp (requires railroad access + magic ink quest complete)
            "WitchSwamp" or "WitchHut" or "WitchWarpCave" => CanAccessWitchSwamp(),
            
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

    /// <summary>Check if a warp to Forest at specific coordinates is the Wizard Tower warp.</summary>
    /// <param name="locationName">The location name.</param>
    /// <param name="tileX">The tile X coordinate.</param>
    /// <param name="tileY">The tile Y coordinate.</param>
    public static bool IsWizardTowerWarp(string locationName, int tileX, int tileY)
    {
        // Wizard Tower warp is at Forest (5, 27) according to warps.json
        return locationName == "Forest" && tileX == 5 && tileY == 27;
    }

    /// <summary>Check if a warp to Mountain at specific coordinates is the Adventurer's Guild warp.</summary>
    /// <param name="locationName">The location name.</param>
    /// <param name="tileX">The tile X coordinate.</param>
    /// <param name="tileY">The tile Y coordinate.</param>
    public static bool IsAdventurersGuildWarp(string locationName, int tileX, int tileY)
    {
        // Adventurer's Guild warp is at Mountain (76, 9) according to warps.json
        return locationName == "Mountain" && tileX == 76 && tileY == 9;
    }

    /// <summary>Check if a warp to Beach at specific coordinates is the Tide Pools warp.</summary>
    /// <param name="locationName">The location name.</param>
    /// <param name="tileX">The tile X coordinate.</param>
    /// <param name="tileY">The tile Y coordinate.</param>
    public static bool IsTidePoolsWarp(string locationName, int tileX, int tileY)
    {
        // Tide Pools warp is at Beach (87, 26) according to warps.json
        return locationName == "Beach" && tileX == 87 && tileY == 26;
    }
}
