using System.Collections.Generic;
using CJBCheatsMenu.Framework.Components;
using CJBCheatsMenu.Framework.Models;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;

namespace CJBCheatsMenu.Framework.Cheats.Skills;

/// <summary>A cheat which increases or decreases skill levels.</summary>
internal class SkillsCheat : BaseCheat
{
    /*********
    ** Fields
    *********/
    /// <summary>Cumulative XP required to reach each level (index = level).</summary>
    internal static readonly int[] CumulativeXpAtLevel =
        [0, 100, 380, 770, 1300, 2150, 3300, 4800, 6900, 10000, 15000];


    /*********
    ** Public methods
    *********/
    /// <inheritdoc />
    public override IEnumerable<OptionsElement> GetFields(CheatContext context)
    {
        int[] skillIds = [Farmer.farmingSkill, Farmer.miningSkill, Farmer.foragingSkill, Farmer.fishingSkill, Farmer.combatSkill];
        foreach (int id in skillIds)
        {
            int currentLevel = Game1.player.GetSkillLevel(id);
            yield return this.GetSkillIncreaseButton(context, id, currentLevel);
            yield return this.GetSkillDecreaseButton(context, id, currentLevel);
        }
    }


    /*********
    ** Private methods
    *********/
    /// <summary>Get the button to increase a skill level.</summary>
    /// <param name="context">The cheat context.</param>
    /// <param name="id">The game's skill ID.</param>
    /// <param name="currentLevel">The current skill level.</param>
    private CheatsOptionsButton GetSkillIncreaseButton(CheatContext context, int id, int currentLevel)
    {
        string skillName = Farmer.getSkillDisplayNameFromIndex(id);

        return new CheatsOptionsButton(
            label: I18n.Skills_IncreaseSkill(skillName: skillName, currentLevel: currentLevel),
            slotWidth: context.SlotWidth,
            toggle: () => this.IncreaseSkill(id),
            disabled: currentLevel >= 10
        );
    }

    /// <summary>Get the button to decrease a skill level.</summary>
    /// <param name="context">The cheat context.</param>
    /// <param name="id">The game's skill ID.</param>
    /// <param name="currentLevel">The current skill level.</param>
    private CheatsOptionsButton GetSkillDecreaseButton(CheatContext context, int id, int currentLevel)
    {
        string skillName = Farmer.getSkillDisplayNameFromIndex(id);

        return new CheatsOptionsButton(
            label: I18n.Skills_DecreaseSkill(skillName: skillName, currentLevel: currentLevel),
            slotWidth: context.SlotWidth,
            toggle: () => this.DecreaseSkill(id, context.Config),
            disabled: currentLevel <= 0 || !context.Config.AllowSkillRegression
        );
    }

    /// <summary>Get the experience points needed to level up once to the given level.</summary>
    /// <param name="level">The next skill level.</param>
    private int GetExperiencePoints(int level)
    {
        if (level is < 0 or > 9)
            return 0;

        int[] exp = [100, 280, 390, 530, 850, 1150, 1500, 2100, 3100, 5000];

        return exp[level];
    }

    /// <summary>Increase a skill level by one.</summary>
    /// <param name="skillId">The skill ID.</param>
    private void IncreaseSkill(int skillId)
    {
        int expToNext = this.GetExperiencePoints(Game1.player.GetSkillLevel(skillId));
        IList<Point> newLevels = Game1.player.newLevels;

        int wasNewLevels = newLevels.Count;
        Game1.player.gainExperience(skillId, expToNext);
        if (newLevels.Count > wasNewLevels)
            newLevels.RemoveAt(newLevels.Count - 1);

        Game1.exitActiveMenu();
        Game1.activeClickableMenu = new LevelUpMenu(skillId, Game1.player.GetSkillLevel(skillId));
    }

    /// <summary>Decrease a skill level by one, adjusting XP and optionally stripping invalidated professions.</summary>
    /// <param name="skillId">The skill ID.</param>
    /// <param name="config">The mod configuration.</param>
    private void DecreaseSkill(int skillId, ModConfig config)
    {
        int currentLevel = Game1.player.GetSkillLevel(skillId);
        if (currentLevel <= 0 || !config.AllowSkillRegression)
            return;

        int newLevel = currentLevel - 1;
        Game1.player.experiencePoints[skillId] = CumulativeXpAtLevel[newLevel];
        SetSkillLevel(Game1.player, skillId, newLevel);

        if (config.StripInvalidProfessions)
            StripProfessionsAboveLevel(skillId, newLevel);

        Game1.exitActiveMenu();
    }

    /// <summary>Compute the skill level implied by a cumulative XP value.</summary>
    /// <param name="xp">The cumulative XP total.</param>
    internal static int LevelFromXp(int xp)
    {
        for (int i = CumulativeXpAtLevel.Length - 1; i >= 0; i--)
        {
            if (xp >= CumulativeXpAtLevel[i])
                return i;
        }
        return 0;
    }

    /// <summary>Directly set a skill's level field on the farmer.</summary>
    /// <param name="player">The player.</param>
    /// <param name="skillId">The skill ID.</param>
    /// <param name="level">The target level.</param>
    internal static void SetSkillLevel(Farmer player, int skillId, int level)
    {
        switch (skillId)
        {
            case 0: player.farmingLevel.Value  = level; break;
            case 1: player.fishingLevel.Value  = level; break;
            case 2: player.foragingLevel.Value = level; break;
            case 3: player.miningLevel.Value   = level; break;
            case 4: player.combatLevel.Value   = level; break;
        }
    }

    /// <summary>Remove professions that the player no longer qualifies for after regression.</summary>
    /// <param name="skillId">The skill ID.</param>
    /// <param name="newLevel">The new (lowered) skill level.</param>
    private static void StripProfessionsAboveLevel(int skillId, int newLevel)
    {
        int baseId = skillId * 6;
        Farmer player = Game1.player;
        bool changed = false;

        // remove level-10 professions (baseId+2 through baseId+5) when below level 10
        if (newLevel < 10)
        {
            for (int p = baseId + 2; p <= baseId + 5; p++)
            {
                if (player.professions.Contains(p))
                {
                    // Defender (27) grants +25 max health; subtract the bonus on removal
                    int healthBonus = p switch { 27 => 25, _ => 0 };
                    player.health    -= healthBonus;
                    player.maxHealth -= healthBonus;
                    player.professions.Remove(p);
                    changed = true;
                }
            }
        }

        // remove level-5 professions (baseId and baseId+1) when below level 5
        if (newLevel < 5)
        {
            for (int p = baseId; p <= baseId + 1; p++)
            {
                if (player.professions.Contains(p))
                {
                    // Fighter (24) grants +15 max health; subtract the bonus on removal
                    int healthBonus = p switch { 24 => 15, _ => 0 };
                    player.health    -= healthBonus;
                    player.maxHealth -= healthBonus;
                    player.professions.Remove(p);
                    changed = true;
                }
            }
        }

        if (changed)
            LevelUpMenu.RevalidateHealth(player);
    }
}
