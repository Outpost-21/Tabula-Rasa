using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace HRF
{
    public class Verb_Carve : Verb_AreaEffect
    {
        protected override void AffectCell(IntVec3 cell)
        {
            if (Props.removeRoofs)
            {
                var roof = cell.GetRoof(caster.Map);
                if (roof != null && roof.isThickRoof)
                {
                    var canRemove = false;
                    foreach (var roof2 in GenRadial.RadialCellsAround(cell, 1, false)
                        .Select(pos => pos.GetRoof(caster.Map)))
                        if (roof2 == null || !roof2.isThickRoof)
                            canRemove = true;
                    if (canRemove) caster.Map.roofGrid.SetRoof(cell, null);
                }
            }

            var rocks = cell.GetThingList(caster.Map).Where(t => t?.def?.building?.mineableThing != null).ToList();
            foreach (var rock in rocks)
            {
                rock.Destroy();
                if (!Props.alwaysGetChunks && Rand.Value < rock.def.building.mineableDropChance) continue;

                var num = Mathf.Max(1, rock.def.building.EffectiveMineableYield);

                var thing = ThingMaker.MakeThing(rock.def.building.mineableThing);
                thing.stackCount = num;
                if (caster.Faction != Faction.OfPlayer) thing.SetForbidden(true);
                GenPlace.TryPlaceThing(thing, rock.Position, caster.Map, ThingPlaceMode.Near);
            }
        }
    }
}