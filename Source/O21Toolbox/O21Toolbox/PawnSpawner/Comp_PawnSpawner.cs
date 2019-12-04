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
        public CompProperties_PawnSpawner Props => props as CompProperties_PawnSpawner;
        
        public override void CompTick()
        {
            SpawnPawn();
            parent.Destroy(DestroyMode.Vanish);
        }
        
        public void SpawnPawn()
        {
            bool newbornFlag = Props.newborn;
            PawnGenerationRequest request = new PawnGenerationRequest(kind:Props.pawnKind, faction:Faction.OfPlayer, newborn:newbornFlag, forceGenerateNewPawn:true);
            Pawn newThing = PawnGenerator.GeneratePawn(request);
            GenSpawn.Spawn(newThing, parent.Position, parent.Map, WipeMode.Vanish);
        }
    }
}
