﻿using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Items.Ecnchantments;
using Kingmaker.Blueprints.Items.Weapons;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Designers.EventConditionActionSystem.Actions;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.ElementsSystem;
using Kingmaker.Enums.Damage;
using Kingmaker.Items;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules.Damage;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.UnitLogic.Mechanics.Components;
using Kingmaker.Utility;
using System.Linq;
using static TabletopTweaks.Core.Main;
using TabletopTweaks.Core.NewActions;
using TabletopTweaks.Core.Utilities;

namespace TabletopTweaks.Core.Bugfixes.Features {
    class MythicAbilities {
        [HarmonyPatch(typeof(BlueprintsCache), "Init")]
        static class BlueprintsCache_Init_Patch {
            static bool Initialized;

            static void Postfix() {
                if (Initialized) return;
                Initialized = true;
                Main.LogHeader("Patching Mythic Abilities");
                PatchBloodlineAscendance();
                PatchSecondBloodline();
                PatchBloodragerSecondBloodline();
                PatchExposeVulnerability();
                PatchMythicCharge();
                PatchCloseToTheAbyss();
            }
            static void PatchCloseToTheAbyss() {
                if (Main.ModContext.Fixes.MythicAbilities.IsDisabled("CloseToTheAbyss")) { return; }
                var MythicDemonGore = Resources.GetBlueprint<BlueprintItemWeapon>("bd4417c15511afe42850fb4d3a6b4a32");
                var TwoHandedDamageMultiplierEnchantment = Resources.GetModBlueprint<BlueprintWeaponEnchantment>("TwoHandedDamageMultiplierEnchantment");

                MythicDemonGore.m_Enchantments = MythicDemonGore.m_Enchantments
                    .AppendToArray(TwoHandedDamageMultiplierEnchantment.ToReference<BlueprintWeaponEnchantmentReference>());
                Main.LogPatch("Patched", MythicDemonGore);
            }
            static void PatchBloodlineAscendance() {
                if (Main.ModContext.Fixes.MythicAbilities.IsDisabled("BloodlineAscendance")) { return; }
                var BloodlineAscendance = Resources.GetBlueprint<BlueprintFeatureSelection>("ce85aee1726900641ab53ede61ac5c19");
                var SeekerBloodlineSelection = Resources.GetBlueprint<BlueprintFeatureSelection>("7bda7cdb0ccda664c9eb8978cf512dbc");

                SeekerBloodlineSelection.m_Features.ForEach(bloodline => {
                    var capstone = ((BlueprintProgression)bloodline.Get()).LevelEntries.Where(entry => entry.Level == 20)
                        .SelectMany(entry => entry.Features.Select(f => f))
                        .Where(f => f.GetComponent<Prerequisite>())
                        .OfType<BlueprintFeature>()
                        .First();
                    capstone.GetComponents<Prerequisite>().ForEach(c => c.Group = Prerequisite.GroupType.Any);
                    if (!capstone.GetComponents<PrerequisiteFeature>().Any(c => c.m_Feature.Get() == bloodline.Get())) {
                        capstone.AddPrerequisiteFeature(bloodline, Prerequisite.GroupType.Any);
                    }
                });
                BloodlineAscendance.RemoveComponents<PrerequisiteFeature>();
                BloodlineAscendance.AddPrerequisites(Helpers.Create<PrerequisiteFeaturesFromList>(c => {
                    c.m_Features = new BlueprintFeatureReference[] {
                        Resources.GetBlueprint<BlueprintFeature>("24bef8d1bee12274686f6da6ccbc8914").ToReference<BlueprintFeatureReference>(),    // SorcererBloodlineSelection
                        Resources.GetBlueprint<BlueprintFeature>("7bda7cdb0ccda664c9eb8978cf512dbc").ToReference<BlueprintFeatureReference>(),    // SeekerBloodlineSelection
                        Resources.GetBlueprint<BlueprintFeature>("a46d4bd93601427409d034a997673ece").ToReference<BlueprintFeatureReference>(),    // SylvanBloodlineProgression
                        Resources.GetBlueprint<BlueprintFeature>("7d990675841a7354c957689a6707c6c2").ToReference<BlueprintFeatureReference>(),    // SageBloodlineProgression
                        Resources.GetBlueprint<BlueprintFeature>("8a95d80a3162d274896d50c2f18bb6b1").ToReference<BlueprintFeatureReference>(),    // EmpyrealBloodlineProgression
                        Resources.GetBlueprint<BlueprintFeature>("da48f9d7f697ae44ca891bfc50727988").ToReference<BlueprintFeatureReference>(),    // BloodOfDragonsSelection - Dragon Disciple
                        Resources.GetBlueprint<BlueprintFeature>("7c813fb495d74246918a690ba86f9c86").ToReference<BlueprintFeatureReference>(),    // NineTailedHeirBloodlineSelection
                        Resources.GetBlueprint<BlueprintFeature>("94c29f69cdc34594a6a4677441ed7375").ToReference<BlueprintFeatureReference>()     // EldritchScionBloodlineSelection
                    };
                    c.Amount = 1;
                }));
                Main.LogPatch("Patched", BloodlineAscendance);
            }
            static void PatchSecondBloodline() {
                if (Main.ModContext.Fixes.MythicAbilities.IsDisabled("SecondBloodline")) { return; }
                BlueprintFeatureSelection SecondBloodline = Resources.GetBlueprint<BlueprintFeatureSelection>("3cf2ab2c320b73347a7c21cf0d0995bd");

                SecondBloodline.RemoveComponents<PrerequisiteFeature>();
                SecondBloodline.AddPrerequisites(Helpers.Create<PrerequisiteFeaturesFromList>(c => {
                    c.m_Features = new BlueprintFeatureReference[] {
                        Resources.GetBlueprint<BlueprintFeature>("24bef8d1bee12274686f6da6ccbc8914").ToReference<BlueprintFeatureReference>(),    // SorcererBloodlineSelection
                        Resources.GetBlueprint<BlueprintFeature>("7bda7cdb0ccda664c9eb8978cf512dbc").ToReference<BlueprintFeatureReference>(),    // SeekerBloodlineSelection
                        Resources.GetBlueprint<BlueprintFeature>("a46d4bd93601427409d034a997673ece").ToReference<BlueprintFeatureReference>(),    // SylvanBloodlineProgression
                        Resources.GetBlueprint<BlueprintFeature>("7d990675841a7354c957689a6707c6c2").ToReference<BlueprintFeatureReference>(),    // SageBloodlineProgression
                        Resources.GetBlueprint<BlueprintFeature>("8a95d80a3162d274896d50c2f18bb6b1").ToReference<BlueprintFeatureReference>(),    // EmpyrealBloodlineProgression
                        Resources.GetBlueprint<BlueprintFeature>("da48f9d7f697ae44ca891bfc50727988").ToReference<BlueprintFeatureReference>(),    // BloodOfDragonsSelection - Dragon Disciple
                        Resources.GetBlueprint<BlueprintFeature>("7c813fb495d74246918a690ba86f9c86").ToReference<BlueprintFeatureReference>(),    // NineTailedHeirBloodlineSelection
                        Resources.GetBlueprint<BlueprintFeature>("94c29f69cdc34594a6a4677441ed7375").ToReference<BlueprintFeatureReference>()     // EldritchScionBloodlineSelection
                };
                    c.Amount = 1;
                }));
                Main.LogPatch("Patched", SecondBloodline);
            }
            static void PatchBloodragerSecondBloodline() {
                if (Main.ModContext.Fixes.MythicAbilities.IsDisabled("SecondBloodragerBloodline")) { return; }
                var ReformedFiendBloodlineSelection = Resources.GetBlueprint<BlueprintFeatureSelection>("dd62cb5011f64cd38b8b08abb19ba2cc");
                var BloodragerBloodlineSelection = Resources.GetBlueprint<BlueprintFeatureSelection>("62b33ac8ceb18dd47ad4c8f06849bc01");
                var SecondBloodragerBloodline = Resources.GetBlueprint<BlueprintFeatureSelection>("b7f62628915bdb14d8888c25da3fac56");
                var SecondBloodragerBloodlineReformedFiend = Resources.GetBlueprint<BlueprintFeatureSelection>("5e4089c46a9f47cdadac7b19d69d11e1");

                SecondBloodragerBloodline.RemoveComponents<PrerequisiteFeature>();
                SecondBloodragerBloodline.AddPrerequisite<PrerequisiteFeaturesFromList>(c => {
                    c.m_Features = new BlueprintFeatureReference[] {
                        ReformedFiendBloodlineSelection.ToReference<BlueprintFeatureReference>(),
                        BloodragerBloodlineSelection.ToReference<BlueprintFeatureReference>()
                    };
                    c.Amount = 1;
                });
                FeatTools.Selections.MythicAbilitySelection.RemoveFeatures(SecondBloodragerBloodlineReformedFiend);
            }
            static void PatchExposeVulnerability() {
                if (Main.ModContext.Fixes.MythicAbilities.IsDisabled("ExposeVulnerability")) { return; }

                var ExposeVulnerability = Resources.GetBlueprint<BlueprintFeature>("8ce3c4b3c1ad24f4dbb6cb4c72e1ec53");
                var ExposeVulnerabilityBuff = Resources.GetBlueprintReference<BlueprintBuffReference>("4edf0af9fd0ebb94ba5ef08b38768e06");

                ExposeVulnerability.FlattenAllActions()
                    .OfType<Conditional>()
                    .ForEach(c => {
                        c.IfTrue = Helpers.CreateActionList(
                            new ContextActionDealDamageTTT() {
                                DamageType = new DamageTypeDescription() {
                                    Type = DamageType.Energy,
                                    Energy = DamageEnergyType.Divine
                                },
                                Duration = new ContextDurationValue() {
                                    DiceCountValue = new ContextValue(),
                                    BonusValue = new ContextValue()
                                },
                                Value = new ContextDiceValue() {
                                    DiceType = DiceType.D6,
                                    DiceCountValue = new ContextValue() {
                                        ValueType = ContextValueType.Rank,
                                    },
                                    BonusValue = new ContextValue()
                                },
                                IgnoreWeapon = true,
                                IgnoreCritical = true
                            },
                            new ContextActionRemoveBuff() {
                                m_Buff = ExposeVulnerabilityBuff
                            },
                            new ContextActionRemoveBuff() {
                                m_Buff = ExposeVulnerabilityBuff
                            }
                        );
                    });
                Main.LogPatch(ExposeVulnerability);
            }
            static void PatchMythicCharge() {
                if (Main.ModContext.Fixes.MythicAbilities.IsDisabled("MythicCharge")) { return; }

                var MythicCharge = Resources.GetBlueprint<BlueprintFeature>("3d1a968428c9b3d4d9d4d27e656b65a8");
                MythicCharge.RemoveComponents<AddInitiatorAttackWithWeaponTrigger>();
                MythicCharge.AddComponent<AdditionalDiceOnAttack>(c => {
                    c.OnHit = true;
                    c.OnCharge = true;
                    c.InitiatorConditions = new ConditionsChecker();
                    c.TargetConditions = new ConditionsChecker();
                    c.Value = new ContextDiceValue() {
                        DiceType = DiceType.D6,
                        DiceCountValue = new ContextValue() {
                            ValueType = ContextValueType.Rank
                        },
                        BonusValue = 0
                    };
                    c.DamageType = new DamageTypeDescription() {
                        Type = DamageType.Energy,
                        Energy = DamageEnergyType.Divine
                    };
                });
                Main.LogPatch("Patched", MythicCharge);
            }
        }
        [HarmonyPatch(typeof(ItemEntity), "AddEnchantment")]
        static class ItemEntity_AddEnchantment_EnduringSpells_Patch {
            private static readonly BlueprintFeature EnduringSpells = Resources.GetBlueprint<BlueprintFeature>("2f206e6d292bdfb4d981e99dcf08153f");
            private static readonly BlueprintFeature EnduringSpellsGreater = Resources.GetBlueprint<BlueprintFeature>("13f9269b3b48ae94c896f0371ce5e23c");

            static bool Prefix(MechanicsContext parentContext, ref Rounds? duration, BlueprintItemEnchantment blueprint) {
                if (Main.ModContext.Fixes.MythicAbilities.IsDisabled("EnduringSpells")) { return true; }
                if (parentContext != null && parentContext.MaybeOwner != null && duration != null) {
                    var abilityData = parentContext.SourceAbilityContext?.Ability;
                    if (abilityData == null || abilityData.Spellbook == null || abilityData.SourceItem != null) { return true; }
                    var owner = parentContext.MaybeOwner;
                    if (owner.Descriptor.HasFact(EnduringSpells)) {
                        if (owner.Descriptor.HasFact(EnduringSpellsGreater) && duration >= (DurationRate.Minutes.ToRounds() * 5)) {
                            duration = DurationRate.Days.ToRounds();
                        } else if (duration >= DurationRate.Hours.ToRounds()) {
                            duration = DurationRate.Days.ToRounds();
                        }
                    }
                }
                return true;
            }
        }
        [HarmonyPatch(typeof(AscendantElement), "OnEventAboutToTrigger")]
        static class AscendantElement_OnEventAboutToTrigger_Patch {

            static bool Prefix(AscendantElement __instance, RuleCalculateDamage evt) {
                if (Main.ModContext.Fixes.MythicAbilities.IsDisabled("AscendantElement")) { return true; }
                foreach (BaseDamage baseDamage in evt.DamageBundle) {
                    EnergyDamage energyDamage;
                    if ((energyDamage = (baseDamage as EnergyDamage)) != null && energyDamage.EnergyType == __instance.Element) {
                        baseDamage.AddDecline(new DamageDecline(DamageDeclineType.None, __instance));
                        energyDamage.IgnoreReduction = true;
                        energyDamage.IgnoreImmunities = true;
                    }
                }
                return false;
            }
        }
    }
}