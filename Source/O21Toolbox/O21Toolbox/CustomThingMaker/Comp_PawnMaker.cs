using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.CustomThingMaker
{
    public class Comp_PawnMaker : ThingComp
    {
        public CompProperties_PawnMaker Spawnprops
        {
            get
            {
                return this.props as CompProperties_PawnMaker;
            }
        }
        
        public override void CompTick()
        {
            this.CheckShouldSpawn();
        }
        
        private void CheckShouldSpawn()
        {
            this.SpawnDude();
            this.parent.Destroy(DestroyMode.Vanish);
        }
        
        public void SpawnDude()
        {
            PawnGenerationRequest request = new PawnGenerationRequest(this.Spawnprops.Pawnkind, Faction.OfPlayer, PawnGenerationContext.NonPlayer, -1, false, false, false, false, true, false, 1f, false, true, true, false, false, false, false, null, null, null, null, null, null, null, null);
            Pawn newThing = PawnGenerator.GeneratePawn(request);
            GenSpawn.Spawn(newThing, this.parent.Position, this.parent.Map, WipeMode.Vanish);
        }
    }
}
