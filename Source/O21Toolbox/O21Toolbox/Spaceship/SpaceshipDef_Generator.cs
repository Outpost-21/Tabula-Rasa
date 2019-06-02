using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Spaceship
{
    public class SpaceshipDef_Generator
    {
        public static IEnumerable<ThingDef> ImpliedSpaceshipDefs()
        {
            foreach(SpaceshipDef ship in DefDatabase<SpaceshipDef>.AllDefs)
            {
                ThingDef landed = new ThingDef();
                landed.defName = ship.defName + "_Landed";
                landed.label = ship.label;
                landed.description = ship.description;
                landed.graphicData.texPath = ship.graphicPaths.landedTexPath;
                landed.graphicData.drawSize = ship.graphicPaths.drawSize;
                landed.useHitPoints = true;
                landed.tickerType = TickerType.Normal;
                landed.category = ThingCategory.Ethereal;
                landed.size = new IntVec2((int)ship.size.x, (int)ship.size.y);
                landed.selectable = true;
                landed.altitudeLayer = AltitudeLayer.MetaOverlays;
                landed.thingClass = typeof(Spaceship_Building);
                Log.Message("Created implied def: " + landed.defName);
                yield return landed;

                ThingDef takingOff = new ThingDef();
                takingOff.defName = ship.defName + "_TakingOff";
                takingOff.label = ship.label;
                takingOff.description = ship.description;
                takingOff.graphicData.texPath = ship.graphicPaths.flyingTexPath;
                takingOff.graphicData.drawSize = ship.graphicPaths.drawSize;
                takingOff.useHitPoints = true;
                takingOff.tickerType = TickerType.Normal;
                takingOff.category = ThingCategory.Ethereal;
                takingOff.size = new IntVec2((int)ship.size.x, (int)ship.size.y);
                takingOff.selectable = true;
                takingOff.altitudeLayer = AltitudeLayer.MetaOverlays;
                takingOff.thingClass = typeof(Spaceship_TakingOff);
                Log.Message("Created implied def: " + takingOff.defName);
                yield return takingOff;

                ThingDef landing = new ThingDef();
                landing.defName = ship.defName + "_Landing";
                landing.label = ship.label;
                landing.description = ship.description;
                landing.graphicData.texPath = ship.graphicPaths.flyingTexPath;
                landing.graphicData.drawSize = ship.graphicPaths.drawSize;
                landing.useHitPoints = true;
                landing.tickerType = TickerType.Normal;
                landing.category = ThingCategory.Ethereal;
                landing.size = new IntVec2((int)ship.size.x, (int)ship.size.y);
                landing.selectable = true;
                landing.altitudeLayer = AltitudeLayer.MetaOverlays;
                landing.thingClass = typeof(Spaceship_Landing);
                Log.Message("Created implied def: " + landing.defName);
                yield return landing;

                ThingDef airstrike = new ThingDef();
                airstrike.defName = ship.defName + "_Airstrike";
                airstrike.label = ship.label;
                airstrike.description = ship.description;
                airstrike.graphicData.texPath = ship.graphicPaths.flyingTexPath;
                airstrike.graphicData.drawSize = ship.graphicPaths.drawSize;
                airstrike.useHitPoints = true;
                airstrike.tickerType = TickerType.Normal;
                airstrike.category = ThingCategory.Ethereal;
                airstrike.size = new IntVec2((int)ship.size.x, (int)ship.size.y);
                airstrike.selectable = true;
                airstrike.altitudeLayer = AltitudeLayer.MetaOverlays;
                airstrike.thingClass = typeof(Spaceship_Airstrike);
                Log.Message("Created implied def: " + airstrike.defName);
                yield return airstrike;
            }
            yield break;
        }
    }
}
