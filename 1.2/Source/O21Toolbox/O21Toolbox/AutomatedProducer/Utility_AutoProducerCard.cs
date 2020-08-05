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
    public class Utility_AutoProducerCard
	{
		public static void DrawAutoBillCard(Rect rect, Building_AutomatedProducer building)
        {

        }

        public static void DrawRepeatModeConfig(Comp_AutomatedProducer comp)
		{
			List<FloatMenuOption> modeList = new List<FloatMenuOption>();
			modeList.Add(new FloatMenuOption("Don't Repeat", delegate ()
			{
				comp.repeatMode = RepeatMode.none;
			}));
			if (comp.currentRecipe != null && comp.currentRecipe.canCountProducts)
			{
				FloatMenuOption item = new FloatMenuOption("Repeat Until X", delegate()
				{
					comp.repeatMode = RepeatMode.until;
					comp.repeatTarget = 0;
				});
				modeList.Add(item);
			}
			modeList.Add(new FloatMenuOption("Repeat X Times", delegate ()
			{
				comp.repeatMode = RepeatMode.times;
			}));
			modeList.Add(new FloatMenuOption("Repeat Forever", delegate ()
			{
				comp.repeatMode = RepeatMode.forever;
			}));
			Find.WindowStack.Add(new FloatMenu(modeList));
		}

		public static string RepeatInfoString(Comp_AutomatedProducer comp)
		{
			string info = "";
			if(comp.repeatMode == RepeatMode.none)
			{
				info = "N/A";
			}
			else if (comp.repeatMode == RepeatMode.until)
			{
				if(comp.currentRecipe != null)
				{
					info = comp.repeatTarget + " / " + comp.CheckRepeatCountProducts(comp.currentRecipe.products.FirstOrDefault().thingDef);
				}
				else
				{
					info = "No Recipe";
				}
			}
			else if (comp.repeatMode == RepeatMode.times)
			{
				info = comp.repeatCount + " times";
			}
			return info;
		}

		public static void DoConfigInterface(Rect baseRect, Color baseColor, Comp_AutomatedProducer comp)
		{
			//Rect rect = new Rect(28f, 32f, 100f, 30f);
			//GUI.color = new Color(1f, 1f, 1f, 0.65f);
			//Widgets.Label(rect, comp.RepeatString());
			GUI.color = baseColor;
			WidgetRow widgetRow = new WidgetRow(baseRect.xMax, baseRect.y + 29f, UIDirection.LeftThenDown, 99999f, 4f);
			if (widgetRow.ButtonText(comp.RepeatString(), null, true, false))
			{
				DrawRepeatModeConfig(comp);
			}
			if (widgetRow.ButtonIcon(TexButtons.Plus, null, null))
			{
				if (comp.repeatMode == RepeatMode.forever)
				{
					comp.repeatMode = RepeatMode.times;
					comp.repeatCount = 1;
				}
				else if (comp.repeatMode == RepeatMode.until)
				{
					int num = comp.currentRecipe.products.FirstOrDefault().thingDef.stackLimit / 5;
					if (comp.currentRecipe.products.FirstOrDefault().thingDef.HasComp(typeof(Comp_PawnSpawner)))
					{
						num = 1;
					}
					comp.repeatTarget += num;
				}
				else if (comp.repeatMode == RepeatMode.times)
				{
					comp.repeatCount++;
				}
				SoundDefOf.Click.PlayOneShotOnCamera(null);
			}
			if (widgetRow.ButtonIcon(TexButtons.Minus, null, null))
			{
				if (comp.repeatMode == RepeatMode.forever)
				{
					comp.repeatMode = RepeatMode.times;
					comp.repeatCount = 1;
				}
				else if (comp.repeatMode == RepeatMode.until)
				{
					if(comp.repeatTarget > 0)
					{
						int num = comp.currentRecipe.products.FirstOrDefault().thingDef.stackLimit / 5;
						if (comp.currentRecipe.products.FirstOrDefault().thingDef.HasComp(typeof(Comp_PawnSpawner)))
						{
							num = 1;
						}
						comp.repeatTarget -= num;
					}
				}
				else if (comp.repeatMode == RepeatMode.times)
				{
					if(comp.repeatCount > 0)
					{
						comp.repeatCount--;
					}
				}
				SoundDefOf.Click.PlayOneShotOnCamera(null);
			}
		}
    }
}
