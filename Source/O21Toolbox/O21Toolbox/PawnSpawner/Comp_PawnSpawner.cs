using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox
{
    public class Comp_PawnSpawner : ThingComp
    {
        public CompProperties_PawnSpawner Spawnprops => this.props as CompProperties_PawnSpawner;
        
        public override void CompTick()
        {
            this.CheckShouldSpawn();
        }
        
        private void CheckShouldSpawn()
        {
            this.SpawnPawn();
            this.parent.Destroy(DestroyMode.Vanish);
        }
        
        public void SpawnPawn()
        {
            PawnGenerationRequest request = new PawnGenerationRequest(this.Spawnprops.pawnKind, Faction.OfPlayer, PawnGenerationContext.NonPlayer, -1, false, false, false, false, true, false, 1f, false, true, true, false, false, false, false, null, null, null, null, null, null, null, null);
            Pawn newThing = PawnGenerator.GeneratePawn(request);
            GenSpawn.Spawn(newThing, this.parent.Position, this.parent.Map, WipeMode.Vanish);
        }
    }
}
