using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace TabulaRasa
{
    public class ScenPart_ReinforcementJoins : ScenPart
	{
		private const float IntervalMidpoint = 30f;
		private const float IntervalDeviation = 15f;
		public float intervalDays;
		public bool repeat;
		public string intervalDaysBuffer;
		public float occurTick;
		public bool isFinished;
		public int maxPawns;
		public string maxPawnsBuffer;
		public PlayerPawnsArriveMethod arrivalMode = PlayerPawnsArriveMethod.DropPods;
		public PawnKindDef pawnKind;
		public FactionDef faction;

		public float IntervalTicks
		{
			get
			{
				return 60000f * this.intervalDays;
			}
		}

		public bool MaxPawnsReached => maxPawns <= 0 || maxPawns <= Find.AnyPlayerHomeMap.PlayerPawnsForStoryteller.Count();

		public override void Tick()
		{
			base.Tick();
			if (Find.AnyPlayerHomeMap == null)
			{
				return;
			}
            if (MaxPawnsReached)
            {
				return;
            }
			if (isFinished)
			{
				return;
			}
			if (pawnKind == null)
			{
				Log.Error("Trying to tick ScenPart_SpecificPawnKindJoins but the pawnKind is null");
				isFinished = true;
				return;
			}
			if (Find.TickManager.TicksGame >= occurTick)
			{
				if (!SendPawn())
				{
					isFinished = true;
					return;
				}
				if (repeat && intervalDays > 0f)
				{
					occurTick += IntervalTicks;
					return;
				}
				isFinished = true;
			}
		}

		public bool SendPawn()
		{
			Map map = Find.AnyPlayerHomeMap;
			if (arrivalMode == PlayerPawnsArriveMethod.Standing)
			{
				if (!CanSpawnJoiner(map))
				{
					return false;
				}
			}
			Pawn pawn = GeneratePawn();
			if (arrivalMode == PlayerPawnsArriveMethod.Standing)
			{
				SpawnJoiner(map, pawn);
			}
			else
			{
				SpawnDropPodJoiner(map, pawn);
			}

			TaggedString baseLetterLabel = "TabulaRasa_LetterLabel_PawnKindJoins".Translate().Formatted(pawn.Named("PAWN")).AdjustedFor(pawn, "PAWN", true);
			TaggedString baseLetterText = "TabulaRasa_LetterText_PawnKindJoins".Translate().Formatted(pawn.Named("PAWN")).AdjustedFor(pawn, "PAWN", true);
			PawnRelationUtility.TryAppendRelationsWithColonistsInfo(ref baseLetterText, ref baseLetterLabel, pawn);
			SendLetter(baseLetterLabel, baseLetterText, LetterDefOf.PositiveEvent, pawn);
			return true;
		}
		public void SendLetter(TaggedString label, TaggedString text, LetterDef letterDef, LookTargets lookTargets)
		{
			if (label.NullOrEmpty() || text.NullOrEmpty())
			{
				Log.Error("Sending standard incident letter with no label or text.");
			}
			ChoiceLetter choiceLetter = LetterMaker.MakeLetter(label, text, letterDef, lookTargets);
			Find.LetterStack.ReceiveLetter(choiceLetter, null);
		}

		public void SpawnDropPodJoiner(Map map, Pawn pawn)
		{
			DropPodUtility.MakeDropPodAt(DropCellFinder.TradeDropSpot(map), map, MakeDropPodInfo(pawn));
		}

		public ActiveDropPodInfo MakeDropPodInfo(Pawn pawn)
		{
			ActiveDropPodInfo activeDropPodInfo = new ActiveDropPodInfo();
			activeDropPodInfo.innerContainer.TryAdd(pawn);
			return activeDropPodInfo;
		}

		public bool CanSpawnJoiner(Map map)
		{
			if (faction != null)
			{
				Faction intFaction = Find.FactionManager.FirstFactionOfDef(faction);
				if (intFaction != null && intFaction.AllyOrNeutralTo(Faction.OfPlayer))
				{
					return false;
				}
			}
			IntVec3 intVec;
			return TryFindEntryCell(map, out intVec);
		}

		public void SpawnJoiner(Map map, Pawn pawn)
		{
			IntVec3 loc;
			TryFindEntryCell(map, out loc);
			GenSpawn.Spawn(pawn, loc, map, WipeMode.Vanish);
		}

		public bool TryFindEntryCell(Map map, out IntVec3 cell)
		{
			return CellFinder.TryFindRandomEdgeCellWith((IntVec3 c) => map.reachability.CanReachColony(c) && !c.Fogged(map), map, CellFinder.EdgeRoadChance_Neutral, out cell);
		}

		public Pawn GeneratePawn()
		{
			return PawnGenerator.GeneratePawn(new PawnGenerationRequest(this.pawnKind, Faction.OfPlayer, PawnGenerationContext.NonPlayer));
		}

		public override void PostGameStart()
		{
			base.PostGameStart();
			this.occurTick = (float)Find.TickManager.TicksGame + this.IntervalTicks;
		}

		public override void Randomize()
		{
			base.Randomize();
			this.intervalDays = 15f * Rand.Gaussian(0f, 1f) + 30f;
			if (this.intervalDays < 0f)
			{
				this.intervalDays = 0f;
			}
			this.maxPawns = (Rand.Range(0, 14));
			this.repeat = (Rand.Range(0, 100) < 50);
			this.pawnKind = PawnKindDefOf.Colonist;
			this.arrivalMode = ((Rand.Value < 0.5f) ? PlayerPawnsArriveMethod.DropPods : PlayerPawnsArriveMethod.Standing);
		}

		public override void DoEditInterface(Listing_ScenEdit listing)
		{
			float rowCount = 5f;
			Rect scenPartRect = listing.GetScenPartRect(this, RowHeight * rowCount);

			Rect rect = new Rect(scenPartRect.x, scenPartRect.y, scenPartRect.width, scenPartRect.height / rowCount);
			if (Widgets.ButtonText(rect, this.pawnKind.LabelCap, true, true, true))
			{
				List<FloatMenuOption> list = new List<FloatMenuOption>();
				list.AddRange(from s in DefDatabase<PawnKindDef>.AllDefsListForReading
							  where s.RaceProps.Humanlike
							  select s into pkd

							  select new FloatMenuOption(string.Format("{0} | {1}", pkd.label.CapitalizeFirst(), pkd.race.LabelCap), delegate ()
							  {
								  this.pawnKind = pkd;
							  }, MenuOptionPriority.Default, null, null, 0f, null, null, true, 0));
				Find.WindowStack.Add(new FloatMenu(list));
			}

			Rect rect3 = new Rect(scenPartRect.x, scenPartRect.y + RowHeight, scenPartRect.width, RowHeight);
			Widgets.TextFieldNumericLabeled<float>(rect3, "intervalDays".Translate(), ref this.intervalDays, ref this.intervalDaysBuffer, 0f, 1E+09f);

			Rect rect5 = new Rect(scenPartRect.x, scenPartRect.y + (RowHeight * 2), scenPartRect.width, RowHeight);
			Widgets.TextFieldNumericLabeled<int>(rect5, "TabulaRasa.MaxPawns".Translate(), ref this.maxPawns, ref this.maxPawnsBuffer, 0f, 1E+09f);

			Rect rect4 = new Rect(scenPartRect.x, scenPartRect.y + (RowHeight * 3), scenPartRect.width, RowHeight);
			Widgets.CheckboxLabeled(rect4, "repeat".Translate(), ref this.repeat, false, null, null, false);

			Rect rect2 = new Rect(scenPartRect.x, scenPartRect.y + (RowHeight * 4), scenPartRect.width, RowHeight);
			string labelFormatted;
			if (arrivalMode == PlayerPawnsArriveMethod.Standing)
			{
				labelFormatted = "PlayerPawnsArriveMethod_Standing".Formatted();

			}
			else
			{
				labelFormatted = "PlayerPawnsArriveMethod_DropPods".Formatted();
			}
			if (Widgets.ButtonText(rect2, this.arrivalMode.ToStringHuman(), true, true, true))
			{
				List<FloatMenuOption> list = new List<FloatMenuOption>();
				list.Add(new FloatMenuOption("PlayerPawnsArriveMethod_Standing".Formatted(), delegate ()
				{
					arrivalMode = PlayerPawnsArriveMethod.Standing;
				}));
				list.Add(new FloatMenuOption("PlayerPawnsArriveMethod_DropPods".Formatted(), delegate ()
				{
					arrivalMode = PlayerPawnsArriveMethod.DropPods;
				}));

				list.AddRange(from s in DefDatabase<PawnKindDef>.AllDefsListForReading
							  where s.RaceProps.Humanlike
							  select s into pkd

							  select new FloatMenuOption(string.Format("{0} | {1}", pkd.label.CapitalizeFirst(), pkd.race.LabelCap), delegate ()
							  {
								  this.pawnKind = pkd;
							  }, MenuOptionPriority.Default, null, null, 0f, null, null, true, 0));
				Find.WindowStack.Add(new FloatMenu(list));
			}
		}

		public override string Summary(RimWorld.Scenario scen)
		{
			string summary = "\nA " + pawnKind.label.CapitalizeFirst() + " will join the colony ";
			if (repeat)
			{
				summary += "every ";
			}
			else
			{
				summary += "after ";
			}
			summary += intervalDays.ToString() + " days.";
			if (arrivalMode == PlayerPawnsArriveMethod.Standing)
			{
				summary += " They will arrive at the edge of the colony map.";
			}
			else
			{
				summary += " They will arrive by drop pod.";
			}
			if (maxPawns > 0)
			{
				summary += "\nThese pawns will stop arriving if the colony has " + maxPawns + " or more colonists already.";
			}
			if (faction != null)
			{
				summary += "\nThese pawns will stop arriving if relations with " + faction.fixedName + " become hostile.";
			}
			return summary;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<float>(ref this.intervalDays, "intervalDays", 0f, false);
			Scribe_Values.Look<bool>(ref this.repeat, "repeat", false, false);
			Scribe_Values.Look<float>(ref this.occurTick, "occurTick", 0f, false);
			Scribe_Values.Look<bool>(ref this.isFinished, "isFinished", false, false);
			Scribe_Values.Look<PlayerPawnsArriveMethod>(ref this.arrivalMode, "arrivalMode", PlayerPawnsArriveMethod.Standing);
			Scribe_Defs.Look<PawnKindDef>(ref this.pawnKind, "pawnKind");
			if (Scribe.mode == LoadSaveMode.PostLoadInit && this.pawnKind == null)
			{
				this.pawnKind = PawnKindDefOf.Colonist;
				Log.Error("ScenPart has null pawnKind reference after loading. Changing to " + this.pawnKind.ToStringSafe());
			}
			Scribe_Defs.Look<FactionDef>(ref faction, "faction");
		}
	}
}
