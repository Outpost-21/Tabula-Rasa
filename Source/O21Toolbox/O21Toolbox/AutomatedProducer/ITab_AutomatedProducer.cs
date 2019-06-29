using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.Sound;

namespace O21Toolbox.AutomatedProducer
{
    public class ITab_AutomatedProducer : ITab
    {
        private static readonly Vector2 WinSize = new Vector2(420f, 300f);
        
        [TweakValue("Interface", 0f, 128f)]
        private static float PasteX = 48f;
        [TweakValue("Interface", 0f, 128f)]
        private static float PasteY = 3f;
        [TweakValue("Interface", 0f, 32f)]
        private static float PasteSize = 24f;

        public ITab_AutomatedProducer()
        {
            this.size = ITab_AutomatedProducer.WinSize;
            this.labelKey = "TabAutoProducer";
        }

        protected Building_AutomatedProducer SelTable
        {
            get
            {
                return (Building_AutomatedProducer)base.SelThing;
            }
        }

        protected override void FillTab()
        {
            Rect rect = new Rect(ITab_AutomatedProducer.WinSize.x - ITab_AutomatedProducer.PasteX, ITab_AutomatedProducer.PasteY, ITab_AutomatedProducer.PasteSize, ITab_AutomatedProducer.PasteSize);
            Rect rect2 = new Rect(0f, 0f, ITab_AutomatedProducer.WinSize.x, ITab_AutomatedProducer.WinSize.y).ContractedBy(10f);
            Func<List<FloatMenuOption>> recipeOptionsMaker = delegate ()
            {
                List<FloatMenuOption> list = new List<FloatMenuOption>();
                foreach(RecipeDef_Automated recipe in this.SelTable.def.GetCompProperties<CompProperties_AutomatedProducer>().recipes)
                {
                    if (recipe.requiredResearch == null || recipe.requiredResearch.IsFinished)
                    {
                        list.Add(new FloatMenuOption(recipe.LabelCap, delegate ()
                        {
                            if (this.SelTable.GetComp<Comp_AutomatedProducer>().currentRecipe != recipe)
                            {
                                this.SelTable.GetComp<Comp_AutomatedProducer>().currentRecipe = recipe;
                                this.SelTable.GetComp<Comp_AutomatedProducer>().ResetWorkTick();
                            }
                        }, MenuOptionPriority.Default, null, null, 29f, (Rect rect3) => Widgets.InfoCardButton(rect.x + 5f, rect.y + (rect.height - 24f) / 2f, recipe), null));
                    }
                }
                if (!list.Any<FloatMenuOption>())
                {
                    list.Add(new FloatMenuOption("NoneBrackets".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null));
                }
                return list;
            };
            DrawRecipeCard(rect2, recipeOptionsMaker);
        }

        public void DrawRecipeCard(Rect rect, Func<List<FloatMenuOption>> recipeOptionsMaker)
        {
            GUI.BeginGroup(rect);
            Text.Font = GameFont.Small;
            if (GetRecipeCount() < 15)
            {
                Rect rect2 = new Rect(0f, 0f, 150f, 29f);
                if (Widgets.ButtonText(rect2, "SetAutoBill".Translate(), true, false, true))
                {
                    Find.WindowStack.Add(new FloatMenu(recipeOptionsMaker()));
                }
                UIHighlighter.HighlightOpportunity(rect2, "SetAutoBill");
            }
            Text.Anchor = TextAnchor.UpperLeft;
            GUI.color = Color.white;
            RecipeDef_Automated recipe = this.SelTable.GetComp<Comp_AutomatedProducer>().currentRecipe;
            string producingItem = "";
            string producingDescription = "";
            string producedItems = "";
            string inputListing = "Input: ";
            string itemInfo = "No Item Being Produced";
            Texture itemTexture = ContentFinder<Texture2D>.Get("UI/Toolbox/NoRecipe", true);
            // Button - Repeat Recipe
            Rect rect6 = new Rect(2f, 28f, 24f, 24f);
            Texture2D RepeatIcon = ContentFinder<Texture2D>.Get("UI/Toolbox/RepeatIcon", true);
            if (Widgets.ButtonImage(rect6, RepeatIcon, Color.white, Color.white * GenUI.SubtleMouseoverColor))
            {
                this.SelTable.GetComp<Comp_AutomatedProducer>().repeatCurrentRecipe = !this.SelTable.GetComp<Comp_AutomatedProducer>().repeatCurrentRecipe;
                SoundDefOf.Click.PlayOneShotOnCamera(null);
            }
            TooltipHandler.TipRegion(rect6, "RepeatAutoBillTip".Translate());

            // Button - Remove Recipe
            Rect rect5 = new Rect(rect.width - 26f, 28f, 24f, 24f);
            Texture2D DeleteX = ContentFinder<Texture2D>.Get("UI/Buttons/Delete", true);
            if (Widgets.ButtonImage(rect5, DeleteX, Color.white, Color.white * GenUI.SubtleMouseoverColor))
            {
                this.SelTable.GetComp<Comp_AutomatedProducer>().currentRecipe = null;
                SoundDefOf.Click.PlayOneShotOnCamera(null);
            }
            TooltipHandler.TipRegion(rect5, "DeleteAutoBillTip".Translate());

            // Info - Recipe Info
            Rect rectRepeat = new Rect(28f, 28f, 160f, 24f);
            Widgets.Label(rectRepeat, this.SelTable.GetComp<Comp_AutomatedProducer>().RepeatString());
            if (recipe != null)
            {
                foreach(ThingDefCount thing in recipe.products)
                {
                    producedItems = producedItems + "\n" + thing.Count.ToString() + " " + thing.ThingDef.LabelCap;
                }
                producingItem = recipe.recipeInfo.productionString + producedItems;
                if(recipe.costs != null)
                {
                    foreach (ThingDefCount thing in recipe.costs)
                    {
                        inputListing = inputListing + "\n" + thing.Count.ToString() + " " + thing.ThingDef.LabelCap;
                    }
                }
                producingDescription = recipe.description;
                itemTexture = ContentFinder<Texture2D>.Get("UI/Toolbox/UnknownItem", true);
                if (recipe.recipeInfo.recipeIcon != null)
                {
                    itemTexture = ContentFinder<Texture2D>.Get(recipe.recipeInfo.recipeIcon.ToString(), true);
                }
                itemInfo = producingDescription + "\n\n" + producingItem + "\n\n" + this.SelTable.GetComp<Comp_AutomatedProducer>().CurrentStatusLabel();
            }
                Rect rect3 = new Rect(0f, 45f, rect.width, 260f);
                GUI.BeginGroup(rect3);
                    Rect rectInfo = new Rect(136f, 4f, rect3.width - 128f, 260f);
                    Widgets.Label(rectInfo, itemInfo);
                    Rect rect4 = new Rect(4f, 4f, 128f, 128f);
                    GUI.DrawTexture(rect4, itemTexture);
                GUI.EndGroup();                
            GUI.EndGroup();
        }

        public int GetRecipeCount()
        {
            int result = 0;
            foreach (RecipeDef_Automated recipe in this.SelTable.def.GetCompProperties<CompProperties_AutomatedProducer>().recipes)
            {
                if (recipe.requiredResearch == null || recipe.requiredResearch.IsFinished)
                {
                    result++;
                }
            }
            return result;
        }
    }
}
