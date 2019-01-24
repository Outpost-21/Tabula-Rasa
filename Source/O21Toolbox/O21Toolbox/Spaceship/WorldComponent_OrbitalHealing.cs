﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;     // Always needed
using RimWorld;        // RimWorld specific functions are found here
using RimWorld.Planet; // RimWorld planet specific functions are found here
using Verse;           // RimWorld universal objects are here
using Verse.AI;        // Needed when you do something with the AI
using Verse.AI.Group;  // Needed when you do something with the AI

namespace O21Toolbox.Spaceship
{
    public class WorldComponent_OrbitalHealing : WorldComponent
    {
        public const int healDurationPerHediffInTicks = 6 * GenDate.TicksPerHour;

        public List<HealingPawn> healingPawns = new List<HealingPawn>();
        private List<Pawn> pawns = new List<Pawn>();      // Only used to expose data.
        private List<Map> originMaps = new List<Map>();   // Only used to expose data.
        private List<int> healEndTicks = new List<int>(); // Only used to expose data.

        public const int healUpdatePeriodInTicks = GenTicks.TickRareInterval;
        public int nextHealUpdateTick = 0;

        // ===================== Setup work =====================
        public WorldComponent_OrbitalHealing(World world)
            : base(world)
        {
        }

        public override void ExposeData()
        {
            base.ExposeData();
            ExposeHealingPawns();
        }

        public void ExposeHealingPawns()
        {
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                // Store healing pawns data in separate lists.
                this.pawns.Clear();
                this.originMaps.Clear();
                this.healEndTicks.Clear();
                foreach (HealingPawn healingPawn in this.healingPawns)
                {
                    this.pawns.Add(healingPawn.pawn);
                    this.originMaps.Add(healingPawn.originMap);
                    this.healEndTicks.Add(healingPawn.healEndTick);
                }
            }
            Scribe_Collections.Look<Pawn>(ref this.pawns, "pawnList", LookMode.Deep);
            Scribe_Collections.Look<Map>(ref this.originMaps, "originMapList", LookMode.Reference);
            Scribe_Collections.Look<int>(ref this.healEndTicks, "healEndTickList");
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                // Restore healing pawns data from separate lists.
                this.healingPawns.Clear();
                if (this.pawns != null)
                {
                    for (int pawnIndex = 0; pawnIndex < pawns.Count; pawnIndex++)
                    {
                        HealingPawn healingPawn = new HealingPawn(this.pawns[pawnIndex], this.originMaps[pawnIndex], this.healEndTicks[pawnIndex]);
                        this.healingPawns.Add(healingPawn);
                    }
                }
            }
        }

        // ===================== Main function =====================
        public override void WorldComponentTick()
        {
            base.WorldComponentTick();
            if ((Find.TickManager.TicksGame >= this.nextHealUpdateTick)
                && (Util_Faction.MiningCoFaction.HostileTo(Faction.OfPlayer) == false))
            {
                this.nextHealUpdateTick = Find.TickManager.TicksGame + healUpdatePeriodInTicks;
                List<HealingPawn> leftHealingPawns = new List<HealingPawn>();
                foreach (HealingPawn healingPawn in this.healingPawns)
                {
                    bool pawnIsSentToSurface = false;
                    if (Find.TickManager.TicksGame >= healingPawn.healEndTick)
                    {
                        pawnIsSentToSurface = TrySendPawnBackToSurface(healingPawn);
                    }
                    if (pawnIsSentToSurface == false)
                    {
                        leftHealingPawns.Add(healingPawn);
                    }
                }
                this.healingPawns = leftHealingPawns.ListFullCopy();
            }
        }

        // ===================== Other functions =====================
        public void Notify_BecameHostileToColony()
        {
            if (this.healingPawns.Count > 0)
            {
                string letterText = "";
                StringBuilder pawnKeptAsGuestString = new StringBuilder();
                if (this.healingPawns.Count == 1)
                {
                    string hisHerIts = GenderUtility.GetPossessive(this.healingPawns.First().pawn.gender);
                    string himHerIt = GenderUtility.GetObjective(this.healingPawns.First().pawn.gender);
                    letterText = "-- Comlink with MiningCo. --\n\n"
                    + "\"A friend of yours is being healed in our medibay. I think we will keep " + himHerIt + " aboard as... \"guest\" for " + hisHerIts + " own safety.\n"
                    + "It is really dangerous down there, you know, and there are lot of menial - ehr, I mean \"interresting\" - tasks to do aboard an orbital station to keep " + himHerIt + " occupied.\n\n"
                    + "MiningCo. medibay officer out.\"\n\n"
                    + "-- End of transmission --\n\n"
                    + "The following colonist is kept as \"guest\" aboard the orbital station until you pay a compensation: " + this.healingPawns.First().pawn.Name.ToStringShort + ".";
                }
                else
                {
                    letterText = "-- Comlink with MiningCo. --\n\n"
                    + "\"Some friends of yours are being healed in our medibay. I think we will keep them aboard as... \"guests\" for their own safety.\n"
                    + "It is really dangerous down there, you know, and there are lot of menial - ehr, I mean \"interresting\" - tasks to do aboard an orbital station to keep them occupied.\n\n"
                    + "MiningCo. medibay officer out.\"\n\n"
                    + "-- End of transmission --\n\n"
                    + "The following colonists are kept as \"guests\" aboard the orbital station until you pay a compensation: ";
                    for (int pawnIndex = 0; pawnIndex < this.healingPawns.Count; pawnIndex++)
                    {
                        pawnKeptAsGuestString.AppendWithComma(this.healingPawns[pawnIndex].pawn.Name.ToStringShort);
                    }
                    pawnKeptAsGuestString.Append(".");
                    letterText += pawnKeptAsGuestString.ToString();
                }
                Find.LetterStack.ReceiveLetter("Orbital \"guests\"", letterText, LetterDefOf.NeutralEvent);
            }
        }

        public bool HasAnyTreatableHediff(Pawn pawn)
        {
            foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
            {
                if (IsTreatableHediff(hediff))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsTreatableHediff(Hediff hediff)
        {
            if (hediff.Visible
                && (hediff.IsPermanent() == false)
                && (hediff.def.tendable
                || hediff.def.makesSickThought
                || hediff.def == HediffDefOf.Hypothermia
                || hediff.def == HediffDefOf.Heatstroke
                || hediff.def == HediffDefOf.ToxicBuildup
                || hediff.def == HediffDefOf.BloodLoss
                || hediff.def == HediffDefOf.Malnutrition))
            {
                return true;
            }
            return false;
        }

        public void Notify_PawnStartingOrbitalHealing(Pawn pawn, Map originMap)
        {
            int removedHediffsCount = HealTreatableHediffs(pawn);
            int healDurationInTicks = removedHediffsCount * healDurationPerHediffInTicks;
            healDurationInTicks = Math.Min(healDurationInTicks, 5 * GenDate.TicksPerDay);
            HealingPawn attendedPawn = new HealingPawn(pawn, originMap, Find.TickManager.TicksGame + healDurationInTicks);
            this.healingPawns.Add(attendedPawn);
        }

        public int HealTreatableHediffs(Pawn pawn)
        {
            int healedHediffsCount = 0;
            List<Hediff> hediffToHealList = new List<Hediff>();
            foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
            {
                if (IsTreatableHediff(hediff))
                {
                    hediffToHealList.Add(hediff);
                    if ((hediff.def == HediffDefOf.Carcinoma)
                        || (hediff.def == HediffDefOf.Plague))
                    {
                        healedHediffsCount += 10;
                    }
                }
            }
            healedHediffsCount = hediffToHealList.Count;
            foreach (Hediff hediff in hediffToHealList)
            {
                hediff.Heal(hediff.Severity);
            }
            return healedHediffsCount;
        }

        public bool TrySendPawnBackToSurface(HealingPawn healedPawn)
        {
            bool pawnIsSent = false;

            // First try to send pawn back to its original map.
            Map originMap = healedPawn.originMap;
            if (Find.Maps.Contains(originMap)
                && originMap.IsPlayerHome)
            {
                pawnIsSent = SendPawnBackToMap(healedPawn.pawn, originMap);
                if (pawnIsSent)
                {
                    return true;
                }
            }
            // Else try with any player home map.
            foreach (Map randomMap in Find.Maps.InRandomOrder())
            {
                if (randomMap.IsPlayerHome)
                {
                    pawnIsSent = SendPawnBackToMap(healedPawn.pawn, randomMap);
                    if (pawnIsSent)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        
        public bool SendPawnBackToMap(Pawn pawn, Map map)
        {
            IntVec3 dropSpot = IntVec3.Invalid;
            bool dropSpotIsValid = false;

            // Check orbital relay is powered on.
            Building_OrbitalRelay orbitalRelay = Util_OrbitalRelay.GetOrbitalRelay(map);
            if (orbitalRelay == null)
            {
                return false;
            }
            if (orbitalRelay.powerComp.PowerOn == false)
            {
                return false;
            }
            // Look for an available landing pad.
            Building_LandingPad landingPad = Util_LandingPad.GetBestAvailableLandingPad(map);
            if (landingPad == null)
            {
                return false;
            }
            // Get a nearby drop spot.
            dropSpotIsValid = DropCellFinder.TryFindDropSpotNear(landingPad.Position, map, out dropSpot, false, false);

            if (dropSpot.IsValid)
            {
                string hisHerIts = GenderUtility.GetPossessive(pawn.gender);
                string heSheIt = GenderUtility.GetPronoun(pawn.gender);
                string himHerIt = GenderUtility.GetObjective(pawn.gender);

                // Restore needs level.
                pawn.needs.SetInitialLevels();
                pawn.needs.roomsize.CurLevel = 0f; // Droppod is quite cramped.

                ActiveDropPodInfo dropPodInfo = new ActiveDropPodInfo();
                bool healingSuccessful = (Rand.Value < 0.98f);
                if (healingSuccessful)
                {
                    string orbitalHealingSuccessfulText = "-- Comlink with MiningCo. --\n\n"
                    + "\"Healing of " + pawn.Name.ToStringShort + " is now finished. Everything went fine during the treatment.\n"
                    + "We just launched " + hisHerIts + " drop pod toward your colony.\n\n"
                    + "I hope you are satisfied of our services.\n\n"
                    + "MiningCo. medibay officer out.\"\n\n"
                    + "-- End of transmission --";
                    Find.LetterStack.ReceiveLetter("Orbital healing finished", orbitalHealingSuccessfulText, LetterDefOf.PositiveEvent, new TargetInfo(dropSpot, map));
                }
                else
                {
                    // Dying pawn with heart attack.
                    string orbitalHealingFailedText = "-- Comlink with MiningCo. --\n\n"
                    + "\"Though we did our best to heal " + pawn.Name.ToStringShort + ", it seems " + hisHerIts + " metabolism was disturbed by the last injection.\n\n"
                    + "I am affraid that we need to immediately send " + himHerIt + " back to you as our rules strictly forbid civilian bodies storage.\n\n"
                    + "Please accept those silvers as a compensation.\n\n"
                    + "MiningCo. medibay officer out.\"\n\n"
                    + "-- End of transmission --";
                    Find.LetterStack.ReceiveLetter("Orbital healing interrupted", orbitalHealingFailedText, LetterDefOf.NegativeEvent, new TargetInfo(dropSpot, map));
                    pawn.health.AddHediff(HediffDef.Named("HeartAttack"));
                    pawn.health.AddHediff(HediffDefOf.Anesthetic);
                    Thing compensation = ThingMaker.MakeThing(ThingDefOf.Silver);
                    compensation.stackCount = Mathf.RoundToInt(0.5f * Util_Spaceship.orbitalHealingCost);
                    dropPodInfo.innerContainer.TryAdd(compensation);
                }
                dropPodInfo.innerContainer.TryAdd(pawn);
                dropPodInfo.leaveSlag = true;
                DropPodUtility.MakeDropPodAt(dropSpot, map, dropPodInfo);
                return true;
            }
            return false;
        }
    }
}
