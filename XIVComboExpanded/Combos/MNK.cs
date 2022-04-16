using System;
using System.Linq;

using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal static class MNK
{
    public const byte ClassID = 2;
    public const byte JobID = 20;

    public const uint
        Bootshine = 53,
        TrueStrike = 54,
        SnapPunch = 56,
        TwinSnakes = 61,
        ArmOfTheDestroyer = 62,
        Demolish = 66,
        PerfectBalance = 69,
        Rockbreaker = 70,
        DragonKick = 74,
        Meditation = 3546,
        FormShift = 4262,
        RiddleOfFire = 7395,
        Brotherhood = 7396,
        TrueNorth = 7546,
        FourPointFury = 16473,
        Enlightenment = 16474,
        HowlingFist = 25763,
        MasterfulBlitz = 25764,
        RiddleOfWind = 25766,
        ShadowOfTheDestroyer = 25767;

    public static class Buffs
    {
        public const ushort
            OpoOpoForm = 107,
            RaptorForm = 108,
            CoeurlForm = 109,
            PerfectBalance = 110,
            TrueNorth = 1250,
            LeadenFist = 1861,
            FormlessFist = 2513,
            DisciplinedFist = 3001;
    }

    public static class Debuffs
    {
        public const ushort
            Demolish = 246;
    }

    public static class Levels
    {
        public const byte
            Bootshine = 1,
            TrueStrike = 4,
            SnapPunch = 6,
            Meditation = 15,
            TwinSnakes = 18,
            ArmOfTheDestroyer = 26,
            Rockbreaker = 30,
            Demolish = 30,
            FourPointFury = 45,
            HowlingFist = 40,
            TrueNorth = 50,
            DragonKick = 50,
            PerfectBalance = 50,
            FormShift = 52,
            EnhancedPerfectBalance = 60,
            MasterfulBlitz = 60,
            RiddleOfFire = 68,
            Brotherhood = 70,
            RiddleOfWind = 72,
            Enlightenment = 74,
            ShadowOfTheDestroyer = 82;
    }
}

internal class MonkXCombo : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MonkXCombo;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == MNK.DragonKick)
        {
            var gauge = GetJobGauge<MNKGauge>();

            if (!InCombat())
            {
                if (level >= MNK.Levels.Meditation && gauge.Chakra < 5)
                    return MNK.Meditation;

                if (level >= MNK.Levels.FormShift && !HasEffect(MNK.Buffs.FormlessFist) && !HasEffect(MNK.Buffs.PerfectBalance))
                    return MNK.FormShift;
            }

            if (HasEffect(MNK.Buffs.FormlessFist))
            {
                if (HasEffect(MNK.Buffs.LeadenFist))
                    return MNK.Bootshine;

                if (!(FindEffect(MNK.Buffs.DisciplinedFist)?.RemainingTime > 6.0))
                    return MNK.TwinSnakes;
            }

            if (level >= MNK.Levels.MasterfulBlitz && !gauge.BeastChakra.Contains(BeastChakra.NONE) && FindEffect(MNK.Buffs.DisciplinedFist)?.RemainingTime > 3.0)
                return OriginalHook(MNK.MasterfulBlitz);

            // off-gcd
            if (InCombat() && IsOnCooldown(MNK.Bootshine))
            {
                if (level >= MNK.Levels.RiddleOfFire && IsOffCooldown(MNK.RiddleOfFire))
                    return MNK.RiddleOfFire;

                if (level >= MNK.Levels.Brotherhood && IsOffCooldown(MNK.Brotherhood))
                    return MNK.Brotherhood;

                if (level >= MNK.Levels.Meditation && gauge.Chakra >= 5)
                    return OriginalHook(MNK.Meditation);

                if (HasCharges(MNK.PerfectBalance) && !HasEffect(MNK.Buffs.PerfectBalance))
                {
                    if (level >= MNK.Levels.EnhancedPerfectBalance)
                    {
                        return MNK.PerfectBalance;
                    }
                    else if (level >= MNK.Levels.PerfectBalance && FindEffect(MNK.Buffs.DisciplinedFist)?.RemainingTime > 6.0)
                    {
                        return MNK.PerfectBalance;
                    }
                }

                if (level >= MNK.Levels.RiddleOfWind && IsOffCooldown(MNK.RiddleOfWind))
                    return MNK.RiddleOfWind;
            }

            // solar first
            if (level >= MNK.Levels.EnhancedPerfectBalance && HasEffect(MNK.Buffs.PerfectBalance) && !gauge.Nadi.HasFlag(Nadi.SOLAR))
            {
                if (!gauge.BeastChakra.Contains(BeastChakra.COEURL) && FindEffect(MNK.Buffs.DisciplinedFist)?.RemainingTime > 3.0)
                    return MNK.Demolish;

                if (!gauge.BeastChakra.Contains(BeastChakra.RAPTOR))
                    return MNK.TwinSnakes;

                return MNK.DragonKick;
            }

            if (HasEffect(MNK.Buffs.OpoOpoForm) || HasEffect(MNK.Buffs.PerfectBalance))
            {
                if (HasEffect(MNK.Buffs.LeadenFist))
                    return MNK.Bootshine;

                return level >= MNK.Levels.DragonKick ? MNK.DragonKick : MNK.Bootshine;
            }

            if (HasEffect(MNK.Buffs.RaptorForm))
            {
                if (FindEffect(MNK.Buffs.DisciplinedFist)?.RemainingTime > 6.0)
                    return MNK.TrueStrike;

                return level >= MNK.Levels.TwinSnakes ? MNK.TwinSnakes : MNK.TrueStrike;
            }

            if (HasEffect(MNK.Buffs.CoeurlForm))
            {
                if (FindTargetEffect(MNK.Debuffs.Demolish)?.RemainingTime > 6.0)
                    return MNK.SnapPunch;

                if (level >= MNK.Levels.Demolish)
                {
                    if (HasCharges(MNK.TrueNorth) && !HasEffect(MNK.Buffs.TrueNorth))
                        return MNK.TrueNorth;

                    return MNK.Demolish;
                }

                return MNK.SnapPunch;
            }

            return level >= MNK.Levels.DragonKick ? MNK.DragonKick : MNK.Bootshine;
        }

        return actionID;
    }
}

internal class MonkXAoECombo : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MonkXAoECombo;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == MNK.Rockbreaker)
        {
            var gauge = GetJobGauge<MNKGauge>();

            if (!InCombat())
            {
                if (level >= MNK.Levels.Meditation && gauge.Chakra < 5)
                    return MNK.Meditation;

                if (level >= MNK.Levels.FormShift && !HasEffect(MNK.Buffs.FormlessFist) && !HasEffect(MNK.Buffs.PerfectBalance))
                    return MNK.FormShift;
            }

            // make sure DisciplinedFist
            if (HasEffect(MNK.Buffs.FormlessFist) && !(FindEffect(MNK.Buffs.DisciplinedFist)?.RemainingTime > 3.0))
                return MNK.FourPointFury;

            if (level >= MNK.Levels.MasterfulBlitz && !gauge.BeastChakra.Contains(BeastChakra.NONE) && FindEffect(MNK.Buffs.DisciplinedFist)?.RemainingTime > 3.0)
                return OriginalHook(MNK.MasterfulBlitz);

            // off-gcd
            if (InCombat() && IsOnCooldown(MNK.Bootshine))
            {
                if (level >= MNK.Levels.RiddleOfFire && IsOffCooldown(MNK.RiddleOfFire))
                    return MNK.RiddleOfFire;

                if (level >= MNK.Levels.Brotherhood && IsOffCooldown(MNK.Brotherhood))
                    return MNK.Brotherhood;

                if (level >= MNK.Levels.Meditation && gauge.Chakra >= 5)
                    return level >= MNK.Levels.HowlingFist ? OriginalHook(MNK.HowlingFist) : OriginalHook(MNK.Meditation);

                if (HasCharges(MNK.PerfectBalance) && !HasEffect(MNK.Buffs.PerfectBalance))
                {
                    if (level >= MNK.Levels.EnhancedPerfectBalance)
                    {
                        return MNK.PerfectBalance;
                    }
                    else if (level >= MNK.Levels.PerfectBalance && FindEffect(MNK.Buffs.DisciplinedFist)?.RemainingTime > 6.0)
                    {
                        return MNK.PerfectBalance;
                    }
                }

                if (level >= MNK.Levels.RiddleOfWind && IsOffCooldown(MNK.RiddleOfWind))
                    return MNK.RiddleOfWind;
            }

            // solar first
            if (level >= MNK.Levels.EnhancedPerfectBalance && HasEffect(MNK.Buffs.PerfectBalance) && !gauge.Nadi.HasFlag(Nadi.SOLAR))
            {
                if (!gauge.BeastChakra.Contains(BeastChakra.COEURL) && FindEffect(MNK.Buffs.DisciplinedFist)?.RemainingTime > 3.0)
                    return MNK.Rockbreaker;

                if (!gauge.BeastChakra.Contains(BeastChakra.RAPTOR))
                    return MNK.FourPointFury;

                return OriginalHook(MNK.ArmOfTheDestroyer);
            }

            if (HasEffect(MNK.Buffs.PerfectBalance))
            {
                if (level >= MNK.Levels.ShadowOfTheDestroyer)
                    return OriginalHook(MNK.ArmOfTheDestroyer);

                return level >= MNK.Levels.Rockbreaker ? MNK.Rockbreaker : OriginalHook(MNK.ArmOfTheDestroyer);
            }

            if (HasEffect(MNK.Buffs.OpoOpoForm))
                return OriginalHook(MNK.ArmOfTheDestroyer);

            if (HasEffect(MNK.Buffs.RaptorForm))
                return level >= MNK.Levels.FourPointFury ? MNK.FourPointFury : (level >= MNK.Levels.TwinSnakes ? MNK.TwinSnakes : OriginalHook(MNK.ArmOfTheDestroyer));

            if (HasEffect(MNK.Buffs.CoeurlForm))
                return level >= MNK.Levels.Rockbreaker ? MNK.Rockbreaker : OriginalHook(MNK.ArmOfTheDestroyer);

            // Shadow of the Destroyer
            return OriginalHook(MNK.ArmOfTheDestroyer);
        }

        return actionID;
    }
}

internal class MonkAoECombo : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MonkAoECombo;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == MNK.MasterfulBlitz)
        {
            var gauge = GetJobGauge<MNKGauge>();

            // Blitz
            if (level >= MNK.Levels.MasterfulBlitz && !gauge.BeastChakra.Contains(BeastChakra.NONE))
                return OriginalHook(MNK.MasterfulBlitz);

            if (level >= MNK.Levels.PerfectBalance && HasEffect(MNK.Buffs.PerfectBalance))
            {
                // Solar
                if (level >= MNK.Levels.EnhancedPerfectBalance && !gauge.Nadi.HasFlag(Nadi.SOLAR))
                {
                    if (level >= MNK.Levels.FourPointFury && !gauge.BeastChakra.Contains(BeastChakra.RAPTOR))
                        return MNK.FourPointFury;

                    if (level >= MNK.Levels.Rockbreaker && !gauge.BeastChakra.Contains(BeastChakra.COEURL))
                        return MNK.Rockbreaker;

                    if (level >= MNK.Levels.ArmOfTheDestroyer && !gauge.BeastChakra.Contains(BeastChakra.OPOOPO))
                        // Shadow of the Destroyer
                        return OriginalHook(MNK.ArmOfTheDestroyer);

                    return level >= MNK.Levels.ShadowOfTheDestroyer
                        ? MNK.ShadowOfTheDestroyer
                        : MNK.Rockbreaker;
                }

                // Lunar.  Also used if we have both Nadi as Tornado Kick/Phantom Rush isn't picky, or under 60.
                return level >= MNK.Levels.ShadowOfTheDestroyer
                    ? MNK.ShadowOfTheDestroyer
                    : MNK.Rockbreaker;
            }

            // FPF with FormShift
            if (level >= MNK.Levels.FormShift && HasEffect(MNK.Buffs.FormlessFist))
            {
                if (level >= MNK.Levels.FourPointFury)
                    return MNK.FourPointFury;
            }

            // 1-2-3 combo
            if (level >= MNK.Levels.FourPointFury && HasEffect(MNK.Buffs.RaptorForm))
                return MNK.FourPointFury;

            if (level >= MNK.Levels.ArmOfTheDestroyer && HasEffect(MNK.Buffs.OpoOpoForm))
                // Shadow of the Destroyer
                return OriginalHook(MNK.ArmOfTheDestroyer);

            if (level >= MNK.Levels.Rockbreaker && HasEffect(MNK.Buffs.CoeurlForm))
                return MNK.Rockbreaker;

            // Shadow of the Destroyer
            return OriginalHook(MNK.ArmOfTheDestroyer);
        }

        return actionID;
    }
}

internal class MonkHowlingFistEnlightenment : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MonkHowlingFistMeditationFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == MNK.HowlingFist || actionID == MNK.Enlightenment)
        {
            var gauge = GetJobGauge<MNKGauge>();

            if (level >= MNK.Levels.Meditation && gauge.Chakra < 5)
                return MNK.Meditation;
        }

        return actionID;
    }
}

internal class MonkDragonKick : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MnkAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == MNK.DragonKick)
        {
            var gauge = GetJobGauge<MNKGauge>();

            if (IsEnabled(CustomComboPreset.MonkDragonKickMeditationFeature))
            {
                if (level >= MNK.Levels.Meditation && gauge.Chakra < 5 && !InCombat())
                    return MNK.Meditation;
            }

            if (IsEnabled(CustomComboPreset.MonkDragonKickBalanceFeature))
            {
                if (level >= MNK.Levels.MasterfulBlitz && !gauge.BeastChakra.Contains(BeastChakra.NONE))
                    return OriginalHook(MNK.MasterfulBlitz);
            }

            if (IsEnabled(CustomComboPreset.MonkBootshineFeature))
            {
                if (level < MNK.Levels.DragonKick || HasEffect(MNK.Buffs.LeadenFist))
                    return MNK.Bootshine;
            }
        }

        return actionID;
    }
}

internal class MonkTwinSnakes : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MnkAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == MNK.TwinSnakes)
        {
            if (IsEnabled(CustomComboPreset.MonkTwinSnakesFeature))
            {
                if (level < MNK.Levels.TwinSnakes)
                    return MNK.TrueStrike;

                if (IsEnabled(CustomComboPreset.MonkFormlessSnakesOption))
                {
                    if (level >= MNK.Levels.FormShift && HasEffect(MNK.Buffs.FormlessFist))
                        return MNK.TwinSnakes;
                }

                if (FindEffect(MNK.Buffs.DisciplinedFist)?.RemainingTime > 6.0)
                    return MNK.TrueStrike;
            }
        }

        return actionID;
    }
}

internal class MonkDemolish : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MnkAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == MNK.Demolish)
        {
            if (IsEnabled(CustomComboPreset.MonkDemolishFeature))
            {
                if (level < MNK.Levels.Demolish || FindTargetEffect(MNK.Debuffs.Demolish)?.RemainingTime > 6.0)
                    return MNK.SnapPunch;
            }
        }

        return actionID;
    }
}

internal class MonkPerfectBalance : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MonkPerfectBalanceFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == MNK.PerfectBalance)
        {
            var gauge = GetJobGauge<MNKGauge>();

            if (!gauge.BeastChakra.Contains(BeastChakra.NONE) && level >= MNK.Levels.MasterfulBlitz)
                // Chakra actions
                return OriginalHook(MNK.MasterfulBlitz);
        }

        return actionID;
    }
}

internal class MonkRiddleOfFire : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MnkAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == MNK.RiddleOfFire)
        {
            if (IsEnabled(CustomComboPreset.MonkRiddleOfFireBrotherhood))
            {
                if (level >= MNK.Levels.Brotherhood && IsOffCooldown(MNK.Brotherhood) && IsOnCooldown(MNK.RiddleOfFire))
                    return MNK.Brotherhood;
            }

            if (IsEnabled(CustomComboPreset.MonkRiddleOfFireWind))
            {
                if (level >= MNK.Levels.RiddleOfWind && IsOffCooldown(MNK.RiddleOfWind) && IsOnCooldown(MNK.RiddleOfFire))
                    return MNK.RiddleOfWind;
            }
        }

        return actionID;
    }
}
