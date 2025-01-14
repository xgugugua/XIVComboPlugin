using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal static class BLM
{
    public const byte ClassID = 7;
    public const byte JobID = 25;

    public const uint
        Fire = 141,
        Blizzard = 142,
        Thunder = 144,
        Fire2 = 147,
        Transpose = 149,
        Fire3 = 152,
        Thunder3 = 153,
        Blizzard3 = 154,
        Scathe = 156,
        Freeze = 159,
        Flare = 162,
        LeyLines = 3573,
        Sharpcast = 3574,
        Blizzard4 = 3576,
        Fire4 = 3577,
        BetweenTheLines = 7419,
        Triplecast = 7421,
        Foul = 7422,
        Thunder2 = 7447,
        Swiftcast = 7561,
        Despair = 16505,
        UmbralSoul = 16506,
        Xenoglossy = 16507,
        Blizzard2 = 25793,
        HighFire2 = 25794,
        HighBlizzard2 = 25795,
        Paradox = 25797;

    public static class Buffs
    {
        public const ushort
            Thundercloud = 164,
            Firestarter = 165,
            Swiftcast = 167,
            LeyLines = 737,
            Sharpcast = 867,
            Triplecast = 1211,
            EnhancedFlare = 2960;
    }

    public static class Debuffs
    {
        public const ushort
            Thunder = 161,
            Thunder2 = 162,
            Thunder3 = 163,
            Thunder4 = 1210;
    }

    public static class Levels
    {
        public const byte
            Blizzard2 = 12,
            Fire2 = 18,
            Swiftcast = 18,
            Thunder2 = 26,
            Fire3 = 35,
            Blizzard3 = 35,
            Freeze = 58,
            Thunder3 = 45,
            Flare = 50,
            Sharpcast = 54,
            Blizzard4 = 58,
            Fire4 = 60,
            BetweenTheLines = 62,
            Triplecast = 66,
            Foul = 70,
            Despair = 72,
            UmbralSoul = 76,
            Xenoglossy = 80,
            HighFire2 = 82,
            HighBlizzard2 = 82,
            Paradox = 90;
    }
}

internal class BlackMageXCombo : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BlackMageXCombo;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        // only work with Xenoglossy
        if (actionID != BLM.Xenoglossy) return actionID;

        var gauge = GetJobGauge<BLMGauge>();

        if (level >= BLM.Levels.Sharpcast && HasCharges(BLM.Sharpcast) && !HasEffect(BLM.Buffs.Sharpcast))
            return BLM.Sharpcast;

        if (!gauge.InAstralFire && !gauge.InUmbralIce)
        {
            if (lastComboMove != OriginalHook(BLM.Thunder) && !HasEffect(BLM.Buffs.Swiftcast))
                return OriginalHook(BLM.Thunder);

            if (gauge.PolyglotStacks > 0)
            {
                if (level >= BLM.Levels.Xenoglossy)
                    return BLM.Xenoglossy;

                if (level >= BLM.Levels.Foul)
                    return BLM.Foul;
            }

            if (level >= BLM.Levels.Swiftcast && IsOffCooldown(BLM.Swiftcast) && !HasEffect(BLM.Buffs.Swiftcast))
                return BLM.Swiftcast;

            if (level < 35)
            {
                if (LocalPlayer?.CurrentMp >= 5000) return BLM.Fire;

                return BLM.Blizzard;
            }

            return level >= BLM.Levels.Fire3 ? BLM.Fire3 : BLM.Fire;
        }

        if (gauge.InUmbralIce)
        {
            if (gauge.PolyglotStacks > 0)
            {
                if (level >= BLM.Levels.Xenoglossy)
                    return BLM.Xenoglossy;

                if (level >= BLM.Levels.Foul)
                    return BLM.Foul;
            }

            //if (level >= BLM.Levels.Blizzard4 && gauge.UmbralHearts == 0)
            //    return BLM.Blizzard4;

            if (lastComboMove != OriginalHook(BLM.Thunder))
            {
                if (!(FindTargetEffect(BLM.Debuffs.Thunder)?.RemainingTime >= 12.0 || FindTargetEffect(BLM.Debuffs.Thunder3)?.RemainingTime >= 12.0))
                    return OriginalHook(BLM.Thunder);
            }

            return level >= BLM.Levels.Fire3 ? BLM.Fire3 : BLM.Fire;
        }

        if (gauge.InAstralFire)
        {
            if (gauge.ElementTimeRemaining >= 9000 && LocalPlayer?.CurrentMp >= 6000 && !HasEffect(BLM.Buffs.Triplecast) && !HasEffect(BLM.Buffs.Swiftcast))
            {
                if (level >= BLM.Levels.Triplecast && HasCharges(BLM.Triplecast))
                    return BLM.Triplecast;

                if (level >= BLM.Levels.Swiftcast && IsOffCooldown(BLM.Swiftcast))
                    return BLM.Swiftcast;
            }

            if (level >= BLM.Levels.Despair &&
                LocalPlayer?.CurrentMp >= 800 &&
                (LocalPlayer?.CurrentMp < 2500 || gauge.ElementTimeRemaining < 6000))
            {
                return BLM.Despair;
            }

            if (LocalPlayer?.CurrentMp < 2500 || gauge.ElementTimeRemaining < 3500)
            {
                return level >= BLM.Levels.Blizzard3 ? BLM.Blizzard3 : BLM.Blizzard;
            }

            if (level >= BLM.Levels.Fire4)
                return BLM.Fire4;

            if (level >= BLM.Levels.Fire3 && HasEffect(BLM.Buffs.Firestarter))
                return BLM.Fire3;

            return BLM.Fire;
        }

        if (level >= BLM.Levels.Fire3 && HasEffect(BLM.Buffs.Firestarter))
            return BLM.Fire3;

        return BLM.Fire;
    }
}

internal class BlackMageAoEXCombo : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BlackMageAoEXCombo;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        // only work with Foul
        if (actionID != BLM.Foul) return actionID;

        var gauge = GetJobGauge<BLMGauge>();

        if (level >= BLM.Levels.Sharpcast && HasCharges(BLM.Sharpcast) && !HasEffect(BLM.Buffs.Sharpcast))
            return BLM.Sharpcast;

        if (level >= BLM.Levels.Foul && gauge.PolyglotStacks > 0)
            return BLM.Foul;

        if (!gauge.InAstralFire && !gauge.InUmbralIce)
        {
            if (lastComboMove != OriginalHook(BLM.Thunder2) && level >= BLM.Levels.Thunder2)
            {
                if (HasEffect(BLM.Buffs.Thundercloud))
                    return OriginalHook(BLM.Thunder2);

                if (level >= BLM.Levels.Swiftcast && IsOffCooldown(BLM.Swiftcast) && !HasEffect(BLM.Buffs.Swiftcast))
                    return BLM.Swiftcast;

                return OriginalHook(BLM.Thunder2);
            }

            if (level < 35)
            {
                if (LocalPlayer?.CurrentMp >= 5000) return BLM.Fire2;

                return BLM.Blizzard2;
            }

            return OriginalHook(BLM.Blizzard2);
        }

        if (gauge.InUmbralIce)
        {
            if (level >= BLM.Levels.Freeze && gauge.UmbralHearts == 0)
                return BLM.Freeze;

            if (lastComboMove != OriginalHook(BLM.Thunder2) && level >= BLM.Levels.Thunder2)
            {
                if (!(FindTargetEffect(BLM.Debuffs.Thunder2)?.RemainingTime >= 7.0 || FindTargetEffect(BLM.Debuffs.Thunder4)?.RemainingTime >= 7.0))
                    return OriginalHook(BLM.Thunder2);
            }

            return level >= BLM.Levels.Fire2 ? OriginalHook(BLM.Fire2) : OriginalHook(BLM.Blizzard2);
        }

        if (gauge.InAstralFire)
        {
            if (LocalPlayer?.CurrentMp >= 6000 && !HasEffect(BLM.Buffs.Triplecast) && !HasEffect(BLM.Buffs.Swiftcast))
            {
                if (level >= BLM.Levels.Triplecast && HasCharges(BLM.Triplecast))
                    return BLM.Triplecast;

                if (level >= BLM.Levels.Swiftcast && IsOffCooldown(BLM.Swiftcast))
                    return BLM.Swiftcast;
            }

            if (LocalPlayer?.CurrentMp >= 1000 && level >= BLM.Levels.Flare && HasEffect(BLM.Buffs.EnhancedFlare))
                return BLM.Flare;

            if (LocalPlayer?.CurrentMp < 3000)
            {
                if (!InCombat() && !HasTarget()) return BLM.Transpose;

                return OriginalHook(BLM.Blizzard2);
            }

            return level >= BLM.Levels.Fire2 ? OriginalHook(BLM.Fire2) : OriginalHook(BLM.Blizzard2);
        }

        return OriginalHook(BLM.Blizzard2);
    }
}

internal class BlackFireBlizzard4 : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BlmAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == BLM.Blizzard4)
        {
            var gauge = GetJobGauge<BLMGauge>();

            if (IsEnabled(CustomComboPreset.BlackUmbralSoulFeature))
            {
                if (level >= BLM.Levels.UmbralSoul && gauge.InUmbralIce && !HasTarget())
                    return BLM.UmbralSoul;
            }
        }

        if (actionID == BLM.Fire4 || actionID == BLM.Blizzard4)
        {
            var gauge = GetJobGauge<BLMGauge>();

            if (IsEnabled(CustomComboPreset.BlackEnochianFeature))
            {
                if (gauge.InAstralFire)
                {
                    if (IsEnabled(CustomComboPreset.BlackEnochianDespairFeature))
                    {
                        if (level >= BLM.Levels.Despair && LocalPlayer?.CurrentMp < 2400)
                            return BLM.Despair;
                    }

                    if (IsEnabled(CustomComboPreset.BlackEnochianNoSyncFeature) || level >= BLM.Levels.Fire4)
                        return BLM.Fire4;

                    return BLM.Fire;
                }

                if (gauge.InUmbralIce)
                {
                    if (IsEnabled(CustomComboPreset.BlackEnochianNoSyncFeature) || level >= BLM.Levels.Blizzard4)
                        return BLM.Blizzard4;

                    return BLM.Blizzard;
                }
            }
        }

        return actionID;
    }
}

internal class BlackTranspose : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BlackManaFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == BLM.Transpose)
        {
            var gauge = GetJobGauge<BLMGauge>();

            if (level >= BLM.Levels.UmbralSoul && gauge.IsEnochianActive && gauge.InUmbralIce)
                return BLM.UmbralSoul;
        }

        return actionID;
    }
}

internal class BlackLeyLines : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BlackLeyLinesFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == BLM.LeyLines)
        {
            if (level >= BLM.Levels.BetweenTheLines && HasEffect(BLM.Buffs.LeyLines))
                return BLM.BetweenTheLines;
        }

        return actionID;
    }
}

internal class BlackFire : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BlackFireFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == BLM.Fire)
        {
            var gauge = GetJobGauge<BLMGauge>();

            if (level >= BLM.Levels.Paradox && gauge.IsParadoxActive && gauge.InUmbralIce)
                return BLM.Paradox;

            if (level >= BLM.Levels.Fire3)
            {
                if (IsEnabled(CustomComboPreset.BlackFireOption))
                {
                    if (gauge.AstralFireStacks < 3)
                        return BLM.Fire3;
                }

                if (IsNotEnabled(CustomComboPreset.BlackFireOption2))
                {
                    if (!gauge.InAstralFire)
                        return BLM.Fire3;
                }

                if (HasEffect(BLM.Buffs.Firestarter))
                    return BLM.Fire3;
            }
        }

        return actionID;
    }
}

internal class BlackBlizzard : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BlmAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == BLM.Blizzard || actionID == BLM.Blizzard3)
        {
            var gauge = GetJobGauge<BLMGauge>();

            if (IsEnabled(CustomComboPreset.BlackUmbralSoulFeature))
            {
                if (level >= BLM.Levels.UmbralSoul && gauge.InUmbralIce && !HasTarget())
                    return BLM.UmbralSoul;
            }

            if (IsEnabled(CustomComboPreset.BlackBlizzardFeature))
            {
                if (level >= BLM.Levels.Paradox && gauge.IsParadoxActive && (gauge.InUmbralIce || LocalPlayer?.CurrentMp >= 1600))
                    return BLM.Paradox;

                if (level >= BLM.Levels.Blizzard3)
                    return BLM.Blizzard3;

                return BLM.Blizzard;
            }
        }

        return actionID;
    }
}

internal class BlackFreezeFlare : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BlmAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == BLM.Freeze)
        {
            var gauge = GetJobGauge<BLMGauge>();

            if (IsEnabled(CustomComboPreset.BlackUmbralSoulFeature))
            {
                if (level >= BLM.Levels.UmbralSoul && gauge.InUmbralIce && !HasTarget())
                    return BLM.UmbralSoul;
            }
        }

        if (actionID == BLM.Freeze || actionID == BLM.Flare)
        {
            var gauge = GetJobGauge<BLMGauge>();

            if (IsEnabled(CustomComboPreset.BlackFreezeFlareFeature))
            {
                if (level >= BLM.Levels.Freeze && gauge.InUmbralIce)
                    return BLM.Freeze;

                if (level >= BLM.Levels.Flare && gauge.InAstralFire)
                    return BLM.Flare;
            }
        }

        return actionID;
    }
}

internal class BlackFire2 : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BlackFire2Feature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == BLM.Fire2 || actionID == BLM.HighFire2)
        {
            var gauge = GetJobGauge<BLMGauge>();

            if (IsEnabled(CustomComboPreset.BlackFireBlizzard2Option))
            {
                if (gauge.AstralFireStacks < 3)
                    return actionID;
            }

            if (level >= BLM.Levels.Flare && gauge.InAstralFire && (gauge.UmbralHearts == 1 || LocalPlayer?.CurrentMp < 3800 || HasEffect(BLM.Buffs.EnhancedFlare)))
                return BLM.Flare;
        }

        return actionID;
    }
}

internal class BlackBlizzard2 : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BlmAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == BLM.Blizzard2 || actionID == BLM.HighBlizzard2)
        {
            var gauge = GetJobGauge<BLMGauge>();

            if (IsEnabled(CustomComboPreset.BlackUmbralSoulFeature))
            {
                if (level >= BLM.Levels.UmbralSoul && gauge.InUmbralIce && !HasTarget())
                    return BLM.UmbralSoul;
            }

            if (IsEnabled(CustomComboPreset.BlackBlizzard2Feature))
            {
                if (IsEnabled(CustomComboPreset.BlackFireBlizzard2Option))
                {
                    if (gauge.UmbralIceStacks < 3)
                        return actionID;
                }

                if (level >= BLM.Levels.Freeze && gauge.InUmbralIce)
                    return BLM.Freeze;
            }
        }

        return actionID;
    }
}

internal class BlackScathe : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BlackScatheFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == BLM.Scathe)
        {
            var gauge = GetJobGauge<BLMGauge>();

            if (level >= BLM.Levels.Xenoglossy && gauge.PolyglotStacks > 0)
                return BLM.Xenoglossy;
        }

        return actionID;
    }
}
