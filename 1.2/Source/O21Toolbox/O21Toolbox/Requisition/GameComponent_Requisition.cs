using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Requisition
{
    public class GameComponent_Requisition : GameComponent
    {
        public Dictionary<Faction, int> factionRequisition = new Dictionary<Faction, int>();

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref factionRequisition, "factionRequisition");
        }

        public override void StartedNewGame()
        {
            base.StartedNewGame();

            InitialiseFactionRequisition();
        }

        public void InitialiseFactionRequisition()
        {
            List<Faction> allFactions = Current.Game.World.factionManager.AllFactionsListForReading;

            if (allFactions.Count > 0)
            {
                for (int i = 0; i < allFactions.Count; i++)
                {
                    if (allFactions[i].def.HasModExtension<DefModExt_Requisition>())
                    {
                        factionRequisition.Add(allFactions[i], 0);
                    }
                }
            }
        }

        public static void CallForAid(Map map, Faction faction, IntRange range)
        {
            IncidentParms incidentParms = new IncidentParms
            {
                target = map,
                faction = faction,
                points = Rand.RangeInclusive(range.min, range.max)
            };
            //incidentParms.raidArrivalMode = PawnsArrivalModeDefOf.CenterDrop;
            faction.lastMilitaryAidRequestTick = Find.TickManager.TicksGame;
            IncidentDefOf.RaidFriendly.Worker.TryExecute(incidentParms);
            //SoundStarter.PlayOneShotOnCamera();
        }
    }
}
