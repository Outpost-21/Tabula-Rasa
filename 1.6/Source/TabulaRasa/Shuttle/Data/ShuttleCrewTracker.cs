using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace TabulaRasa.Shuttle
{
    public class ShuttleCrewTracker : IThingHolder, IExposable
    {
        public Shuttle shuttle;

        public ThingOwner<Pawn> innerPawns;

        private List<Pawn> tmpSavedPawns = new List<Pawn>();

        public int capacity;

        public Dictionary<int, Pawn> crewId = new Dictionary<int, Pawn>();

        public IThingHolder ParentHolder => null;

        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
        }

        public ThingOwner GetDirectlyHeldThings()
        {
            return innerPawns;
        }

        public Pawn CrewByID(int id)
        {
            if (crewId.ContainsKey(id))
            {
                return crewId[id];
            }
            return null;
        }

        public void AssignCrewID(Pawn pawn)
        {

        }

        public void ExposeData()
        {
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                tmpSavedPawns.Clear();
                tmpSavedPawns.AddRange(innerPawns.InnerListForReading);
                innerPawns.RemoveAll((Pawn x) => true);
            }
            Scribe_Collections.Look(ref tmpSavedPawns, "tmpSavedPawns", LookMode.Reference);
            Scribe_Deep.Look(ref innerPawns, "innerPawns", this);
            Scribe_Collections.Look(ref crewId, "crewID", LookMode.Value, LookMode.Reference);
            if (Scribe.mode == LoadSaveMode.PostLoadInit || Scribe.mode == LoadSaveMode.Saving)
            {
                for (int i = 0; i < tmpSavedPawns.Count; i++)
                {
                    innerPawns.TryAdd(tmpSavedPawns[i]);
                }
                tmpSavedPawns.Clear();
            }
        }
    }
}
