using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace O21Toolbox.Scenario
{
    public class ScenPart_SpecificPawnKindJoins : ScenPart
	{
		private const float IntervalMidpoint = 30f;
		private const float IntervalDeviation = 15f;
		private float intervalDays;
		private bool repeat;
		private string intervalDaysBuffer;
		private float occurTick;
		private bool isFinished;
		public PlayerPawnsArriveMethod arrivalMode = PlayerPawnsArriveMethod.DropPods;
		public PawnKindDef pawnKind;

		public float IntervalTicks
		{
			get
			{
				return 60000f * this.intervalDays;
			}
		}

		public override void Tick()
		{
			base.Tick();
			if (Find.AnyPlayerHomeMap == null)
			{
				return;
			}
			if (this.isFinished)
			{
				return;
			}
			if (this.pawnKind == null)
			{
				Log.Error("Trying to tick ScenPart_SpecificPawnKindJoins but the pawnKind is null");
				this.isFinished = true;
				return;
			}
			if ((float)Find.TickManager.TicksGame >= this.occurTick)
			{
				if (!SendPawn())
                {
					this.isFinished = true;
					return;
                }
				if (this.repeat && this.intervalDays > 0f)
				{
					this.occurTick += this.IntervalTicks;
					return;
				}
				this.isFinished = true;
			}
		}

		public bool SendPawn()
		{
			Map map = Find.AnyPlayerHomeMap;
			if (arrivalMode == PlayerPawnsArriveMethod.Standing)
			{
				if (!this.CanSpawnJoiner(map))
				{
					return false;
				}
			}
			Pawn pawn = this.GeneratePawn();
			if (arrivalMode == PlayerPawnsArriveMethod.Standing)
			{
				this.SpawnJoiner(map, pawn);
			}
            else
            {
				this.SpawnDropPodJoiner(map, pawn);
            }

			TaggedString baseLetterLabel = "O21_LetterLabel_PawnKindJoins".Translate().Formatted(pawn.Named("PAWN")).AdjustedFor(pawn, "PAWN", true);
			TaggedString baseLetterText = "O21_LetterText_PawnKindJoins".Translate().Formatted(pawn.Named("PAWN")).AdjustedFor(pawn, "PAWN", true);
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
            if (!map.listerBuildings.AllBuildingsColonistOfDef(ThingDefOf.OrbitalTradeBeacon).EnumerableNullOrEmpty())
			{
				DropPodUtility.MakeDropPodAt(map.listerBuildings.AllBuildingsColonistOfDef(ThingDefOf.OrbitalTradeBeacon).FirstOrDefault().Position, map, MakeDropPodInfo(pawn));
            }
            else
            {
				DropPodUtility.MakeDropPodAt(DropCellFinder.TradeDropSpot(map), map, MakeDropPodInfo(pawn));
            }
        }

		public ActiveDropPodInfo MakeDropPodInfo(Pawn pawn)
		{
			ActiveDropPodInfo activeDropPodInfo = new ActiveDropPodInfo();
			activeDropPodInfo.innerContainer.TryAdd(pawn);
			return activeDropPodInfo;
		}

		public bool CanSpawnJoiner(Map map)
		{
			IntVec3 intVec;
			return TryFindEntryCell(map, out intVec);
		}

		public void SpawnJoiner(Map map, Pawn pawn)
		{
			IntVec3 loc;
			this.TryFindEntryCell(map, out loc);
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
			this.repeat = (Rand.Range(0, 100) < 50);
			this.pawnKind = PawnKindDefOf.Colonist;
			this.arrivalMode = ((Rand.Value < 0.5f) ? PlayerPawnsArriveMethod.DropPods : PlayerPawnsArriveMethod.Standing);
		}

		public override void DoEditInterface(Listing_ScenEdit listing)
		{
			Rect scenPartRect = listing.GetScenPartRect(this, ScenPart.RowHeight * 4f);

			Rect rect = new Rect(scenPartRect.x, scenPartRect.y, scenPartRect.width, scenPartRect.height / 4f);
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

			Rect rect3 = new Rect(scenPartRect.x, scenPartRect.y + scenPartRect.height / 4f, scenPartRect.width, scenPartRect.height / 4f);
			Widgets.TextFieldNumericLabeled<float>(rect3, "intervalDays".Translate(), ref this.intervalDays, ref this.intervalDaysBuffer, 0f, 1E+09f);

			Rect rect4 = new Rect(scenPartRect.x, scenPartRect.y + scenPartRect.height * 2f / 4f, scenPartRect.width, scenPartRect.height / 4f);
			Widgets.CheckboxLabeled(rect4, "repeat".Translate(), ref this.repeat, false, null, null, false);

			Rect rect2 = new Rect(scenPartRect.x, scenPartRect.y + scenPartRect.height * 3f / 4f, scenPartRect.width, scenPartRect.height / 4f);
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
			if(arrivalMode == PlayerPawnsArriveMethod.Standing)
            {
				summary += " They will arrive at the edge of the colony map.";
            }
            else
            {
				summary += " They will arrive by drop pod.";
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
				Log.Error("ScenPart has null incident after loading. Changing to " + this.pawnKind.ToStringSafe());
			}
		}
	}
}
