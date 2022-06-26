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
    public class Comp_CompositeWeapon : ThingComp
    {
        public CompProperties_CompositeWeapon Props => (CompProperties_CompositeWeapon)props;

        public Graphic graphicInt;

        public virtual Graphic Graphic
        {
            set => graphicInt = value;
            get
            {
                if (graphicInt == null)
                {
                    Graphic result;
                    if (Props.graphicData == null)
                    {
                        Log.ErrorOnce(parent.def + " has no SecondLayer graphicData but we are trying to access it.",
                            764532);
                        return BaseContent.BadGraphic;
                    }
                    var newColor1 = parent.DrawColor : overrideColor;
                    var newColor2 = parent.DrawColorTwo ?? overrideColor;
                    if (Props.whiteout)
                    {
                        newColor1 = Color.white;
                        newColor2 = Color.white;
                    }
                    Comp_SlottedBonus slottedBonus = parent.TryGetComp<Comp_SlotLoadable>()?.Slots.FirstOrDefault(x => (x.def as SlotLoadableDef).doesChangeGraphic == true)?.SlotOccupant?.TryGetComp<Comp_SlottedBonus>() ?? null;

                    GraphicData graphics = new GraphicData();
                    graphics.CopyFrom(slottedBonus?.Props?.graphicData ?? Props.graphicData);

                    graphics.texPath = (slottedBonus?.Props?.pathPrefix ?? (Props.defaultPathPrefix ?? "")) + graphics.texPath + (slottedBonus?.Props?.pathSuffix ?? (Props.defaultPathSuffix ?? ""));
                    result = graphics.Graphic.GetColoredVersion(graphics.shaderType.Shader, newColor1, newColor2);
                    graphicInt = PostGraphicEffects(result);
                }
                return graphicInt;
            }
        }

        public virtual Graphic PostGraphicEffects(Graphic graphic)
        {
            return graphic;
        }
    }
}
