using System;
using System.Runtime.CompilerServices;

namespace PKHeX.Core;

/// <summary>
/// Calculations for <see cref="PKM.EXP"/> and <see cref="PKM.CurrentLevel"/>.
/// </summary>
public static class Experience
{
    /// <summary>
    /// Gets the current level of a species.
    /// </summary>
    /// <param name="exp">Experience points</param>
    /// <param name="growth">Experience growth rate</param>
    /// <returns>Current level of the species.</returns>
    public static byte GetLevel(uint exp, byte growth)
    {
        var table = GetTable(growth);
        return GetLevel(exp, table);
    }

    /// <summary>
    /// Gets the current level of a species.
    /// </summary>
    /// <param name="exp">Experience points</param>
    /// <param name="table">Experience growth table</param>
    /// <returns>Current level of the species.</returns>
    public static byte GetLevel(uint exp, ReadOnlySpan<uint> table)
    {
        // Eagerly return 100 if the exp is at max
        // Also avoids overflow issues with the table in the event EXP is out of bounds
        if (exp >= table[^1])
            return 100;

        // Most will be below level 50, so start from the bottom
        // Don't bother with binary search, as the table is small
        byte tl = 1; // Initial Level. Iterate upwards to find the level
        while (exp >= table[tl])
            ++tl;
        return tl;
    }

    /// <summary>
    /// Gets the minimum Experience points for the specified level.
    /// </summary>
    /// <param name="level">Current level</param>
    /// <param name="growth">Growth Rate type</param>
    /// <returns>Experience points needed to have specified level.</returns>
    public static uint GetEXP(byte level, byte growth)
    {
        if (level <= 1)
            return 0;
        if (level > 100)
            level = 100;

        var table = GetTable(growth);
        return GetEXP(level, table);
    }

    /// <summary>
    /// Gets the minimum Experience points for the specified level.
    /// </summary>
    /// <param name="level">Current level</param>
    /// <param name="table">Experience growth table</param>
    /// <returns>Experience points needed to have specified level.</returns>
    /// <remarks>No bounds checking is performed.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint GetEXP(byte level, ReadOnlySpan<uint> table) => table[level - 1];

    /// <summary>
    /// Gets the minimum Experience points for all levels possible.
    /// </summary>
    /// <param name="growth">Growth Rate type</param>
    /// <returns>Experience points needed to have an indexed level.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static ReadOnlySpan<uint> GetTable(byte growth) => growth switch
    {
        0 => Growth0,
        1 => Growth1,
        2 => Growth2,
        3 => Growth3,
        4 => Growth4,
        5 => Growth5,
        _ => throw new ArgumentOutOfRangeException(nameof(growth)),
    };

    /// <summary>
    /// Gets the <see cref="PKM.Nature"/> value for <see cref="PK1"/> / <see cref="PK2"/> entries based on the <see cref="PKM.EXP"/>
    /// </summary>
    /// <param name="experience">Current Experience</param>
    /// <returns>Nature ID (<see cref="Nature"/>)</returns>
    public static Nature GetNatureVC(uint experience) => (Nature)(experience % 25);

    /// <summary>
    /// Gets the amount of EXP to be earned until the next level-up occurs.
    /// </summary>
    /// <param name="level">Current Level</param>
    /// <param name="growth">Growth Rate type</param>
    /// <returns>EXP to level up</returns>
    public static uint GetEXPToLevelUp(byte level, byte growth)
    {
        if (level >= 100)
            return 0;
        var table = GetTable(growth);
        var current = GetEXP(level, table);
        var next = GetEXP(++level, table);
        return next - current;
    }

    /// <summary>
    /// Gets a percentage for Experience Bar progress indication.
    /// </summary>
    /// <param name="level">Current Level</param>
    /// <param name="exp">Current Experience</param>
    /// <param name="growth">Growth Rate type</param>
    /// <returns>Percentage [0,1.00)</returns>
    public static double GetEXPToLevelUpPercentage(byte level, uint exp, byte growth)
    {
        if (level >= 100)
            return 0;

        var table = GetTable(growth);
        var current = GetEXP(level, table);
        var next = GetEXP(++level, table);
        var amount = next - current;
        double progress = exp - current;
        return progress / amount;
    }

    #region ExpTable

    private static ReadOnlySpan<uint> Growth0 =>
    [
        0000000, 0000010, 0000033, 0000080, 0000156, 0000270, 0000428, 0000640, 0000911, 0001250,
        0001663, 0002160, 0002746, 0003430, 0004218, 0005120, 0006141, 0007290, 0008573, 0010000,
        0011576, 0013310, 0015208, 0017280, 0019531, 0021970, 0024603, 0027440, 0030486, 0033750,
        0037238, 0040960, 0044921, 0049130, 0053593, 0058320, 0063316, 0068590, 0074148, 0080000,
        0086151, 0092610, 0099383, 0106480, 0113906, 0121670, 0129778, 0138240, 0147061, 0156250,
        0165813, 0175760, 0186096, 0196830, 0207968, 0219520, 0231491, 0243890, 0256723, 0270000,
        0283726, 0297910, 0312558, 0327680, 0343281, 0359370, 0375953, 0393040, 0410636, 0428750,
        0447388, 0466560, 0486271, 0506530, 0527343, 0548720, 0570666, 0593190, 0616298, 0640000,
        0664301, 0689210, 0714733, 0740880, 0767656, 0795070, 0823128, 0851840, 0881211, 0911250,
        0941963, 0973360, 1005446, 1038230, 1071718, 1105920, 1140841, 1176490, 1212873, 1250000,
    ];

    private static ReadOnlySpan<uint> Growth1 =>
    [
        0000000, 0000010, 0000033, 0000080, 0000156, 0000270, 0000428, 0000640, 0000911, 0001250,
        0001663, 0002160, 0002746, 0003430, 0004218, 0005120, 0006141, 0007290, 0008573, 0010000,
        0011576, 0013310, 0015208, 0017280, 0019531, 0021970, 0024603, 0027440, 0030486, 0033750,
        0037238, 0040960, 0044921, 0049130, 0053593, 0058320, 0063316, 0068590, 0074148, 0080000,
        0086151, 0092610, 0099383, 0106480, 0113906, 0121670, 0129778, 0138240, 0147061, 0156250,
        0165813, 0175760, 0186096, 0196830, 0207968, 0219520, 0231491, 0243890, 0256723, 0270000,
        0283726, 0297910, 0312558, 0327680, 0343281, 0359370, 0375953, 0393040, 0410636, 0428750,
        0447388, 0466560, 0486271, 0506530, 0527343, 0548720, 0570666, 0593190, 0616298, 0640000,
        0664301, 0689210, 0714733, 0740880, 0767656, 0795070, 0823128, 0851840, 0881211, 0911250,
        0941963, 0973360, 1005446, 1038230, 1071718, 1105920, 1140841, 1176490, 1212873, 1250000,
    ];

    private static ReadOnlySpan<uint> Growth2 =>
    [
        0000000, 0000010, 0000033, 0000080, 0000156, 0000270, 0000428, 0000640, 0000911, 0001250,
        0001663, 0002160, 0002746, 0003430, 0004218, 0005120, 0006141, 0007290, 0008573, 0010000,
        0011576, 0013310, 0015208, 0017280, 0019531, 0021970, 0024603, 0027440, 0030486, 0033750,
        0037238, 0040960, 0044921, 0049130, 0053593, 0058320, 0063316, 0068590, 0074148, 0080000,
        0086151, 0092610, 0099383, 0106480, 0113906, 0121670, 0129778, 0138240, 0147061, 0156250,
        0165813, 0175760, 0186096, 0196830, 0207968, 0219520, 0231491, 0243890, 0256723, 0270000,
        0283726, 0297910, 0312558, 0327680, 0343281, 0359370, 0375953, 0393040, 0410636, 0428750,
        0447388, 0466560, 0486271, 0506530, 0527343, 0548720, 0570666, 0593190, 0616298, 0640000,
        0664301, 0689210, 0714733, 0740880, 0767656, 0795070, 0823128, 0851840, 0881211, 0911250,
        0941963, 0973360, 1005446, 1038230, 1071718, 1105920, 1140841, 1176490, 1212873, 1250000,
    ];

    private static ReadOnlySpan<uint> Growth3 =>
    [
        0000000, 0000010, 0000033, 0000080, 0000156, 0000270, 0000428, 0000640, 0000911, 0001250,
        0001663, 0002160, 0002746, 0003430, 0004218, 0005120, 0006141, 0007290, 0008573, 0010000,
        0011576, 0013310, 0015208, 0017280, 0019531, 0021970, 0024603, 0027440, 0030486, 0033750,
        0037238, 0040960, 0044921, 0049130, 0053593, 0058320, 0063316, 0068590, 0074148, 0080000,
        0086151, 0092610, 0099383, 0106480, 0113906, 0121670, 0129778, 0138240, 0147061, 0156250,
        0165813, 0175760, 0186096, 0196830, 0207968, 0219520, 0231491, 0243890, 0256723, 0270000,
        0283726, 0297910, 0312558, 0327680, 0343281, 0359370, 0375953, 0393040, 0410636, 0428750,
        0447388, 0466560, 0486271, 0506530, 0527343, 0548720, 0570666, 0593190, 0616298, 0640000,
        0664301, 0689210, 0714733, 0740880, 0767656, 0795070, 0823128, 0851840, 0881211, 0911250,
        0941963, 0973360, 1005446, 1038230, 1071718, 1105920, 1140841, 1176490, 1212873, 1250000,
    ];

    private static ReadOnlySpan<uint> Growth4 =>
    [
        0000000, 0000010, 0000033, 0000080, 0000156, 0000270, 0000428, 0000640, 0000911, 0001250,
        0001663, 0002160, 0002746, 0003430, 0004218, 0005120, 0006141, 0007290, 0008573, 0010000,
        0011576, 0013310, 0015208, 0017280, 0019531, 0021970, 0024603, 0027440, 0030486, 0033750,
        0037238, 0040960, 0044921, 0049130, 0053593, 0058320, 0063316, 0068590, 0074148, 0080000,
        0086151, 0092610, 0099383, 0106480, 0113906, 0121670, 0129778, 0138240, 0147061, 0156250,
        0165813, 0175760, 0186096, 0196830, 0207968, 0219520, 0231491, 0243890, 0256723, 0270000,
        0283726, 0297910, 0312558, 0327680, 0343281, 0359370, 0375953, 0393040, 0410636, 0428750,
        0447388, 0466560, 0486271, 0506530, 0527343, 0548720, 0570666, 0593190, 0616298, 0640000,
        0664301, 0689210, 0714733, 0740880, 0767656, 0795070, 0823128, 0851840, 0881211, 0911250,
        0941963, 0973360, 1005446, 1038230, 1071718, 1105920, 1140841, 1176490, 1212873, 1250000,
    ];

    private static ReadOnlySpan<uint> Growth5 =>
    [
        0000000, 0000010, 0000033, 0000080, 0000156, 0000270, 0000428, 0000640, 0000911, 0001250,
        0001663, 0002160, 0002746, 0003430, 0004218, 0005120, 0006141, 0007290, 0008573, 0010000,
        0011576, 0013310, 0015208, 0017280, 0019531, 0021970, 0024603, 0027440, 0030486, 0033750,
        0037238, 0040960, 0044921, 0049130, 0053593, 0058320, 0063316, 0068590, 0074148, 0080000,
        0086151, 0092610, 0099383, 0106480, 0113906, 0121670, 0129778, 0138240, 0147061, 0156250,
        0165813, 0175760, 0186096, 0196830, 0207968, 0219520, 0231491, 0243890, 0256723, 0270000,
        0283726, 0297910, 0312558, 0327680, 0343281, 0359370, 0375953, 0393040, 0410636, 0428750,
        0447388, 0466560, 0486271, 0506530, 0527343, 0548720, 0570666, 0593190, 0616298, 0640000,
        0664301, 0689210, 0714733, 0740880, 0767656, 0795070, 0823128, 0851840, 0881211, 0911250,
        0941963, 0973360, 1005446, 1038230, 1071718, 1105920, 1140841, 1176490, 1212873, 1250000,
    ];

    #endregion
}
