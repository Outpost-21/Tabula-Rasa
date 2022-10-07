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
    public class ITab_Hologram : ITab
    {
        public static readonly Vector2 WinSize = new Vector2(420f, 300f);

        public Building SelHolo => (Building)base.SelThing;

        public Comp_HologramProjection HoloComp => SelHolo.TryGetComp<Comp_HologramProjection>();

        public ITab_Hologram()
        {
            this.size = WinSize;
            this.labelKey = "TabulaRasa.ITab_Hologram";
        }

        public override void FillTab()
        {
            Rect rect = new Rect(0f, 0f, WinSize.x, WinSize.y).ContractedBy(10f);
            Func<List<FloatMenuOption>> hologramOptionsMaker = delegate ()
            {
                List<FloatMenuOption> options = new List<FloatMenuOption>();
                foreach (HologramDef def in HoloComp.viableHolos)
                {
                    options.Add(new FloatMenuOption(def.LabelCap, delegate ()
                    {
                        HoloComp.holoDef = def;
                        HoloComp.ResetHoloColors();
                    }));
                }
                if (!options.Any<FloatMenuOption>())
                {
                    options.Add(new FloatMenuOption("NoneBrackets".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null));
                }
                return options;
            };
            DrawInformation(rect, hologramOptionsMaker);
        }

        public void DrawInformation(Rect rect, Func<List<FloatMenuOption>> hologramOptionsMaker)
        {
            GUI.BeginGroup(rect);
            Text.Font = GameFont.Small;
            Rect rect2 = new Rect(0f, 0f, 150f, 29f);
            if (Widgets.ButtonText(rect2, "TabulaRasa.ITab_HologramSelect".Translate(), true, false, true))
            {
                Find.WindowStack.Add(new FloatMenu(hologramOptionsMaker()));
            }
            HologramDef holoDef = HoloComp.holoDef;
            Rect rect3 = new Rect(0f, 45f, rect.width, 260f);
            GUI.BeginGroup(rect3);
            Rect rect4 = new Rect(4f, 4f, 128f, 128f);
            for (int i = 0; i < holoDef.hologramLayers.Count(); i++)
            {
                GUI.DrawTexture(rect4, ContentFinder<Texture2D>.Get(holoDef.hologramLayers[i].texPath, true), ScaleMode.ScaleToFit, true, 0f, HoloComp.hologramColors[i], 0f, 0f);
            }
            Rect rectInfo = new Rect(136f, 4f, rect3.width - 128f, 260f);
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(rectInfo);
            listing.Label($"Current Holo: {holoDef.LabelCap}");
            listing.GapLine();
            for (int i = 0; i < holoDef.hologramLayers.Count(); i++)
            {
                if (holoDef.hologramLayers[i].canChangeColor)
                {
                    listing.AddHoloColorPickerButton($"Layer {i}", HoloComp.hologramColors[i], HoloComp, i);
                }
            }
            listing.End();
            GUI.EndGroup();
            GUI.EndGroup();
        }
    }
}
