using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox
{
    [StaticConstructorOnStartup]
    public static class EnhancedSettings
    {
        public static void CheckboxEnhanced(this Listing_Standard listing, string name, string explanation, ref bool value, string tooltip = null)
		{
			float curHeight = listing.CurHeight;
			Text.Font = GameFont.Small;
			GUI.color = Color.white;
			listing.CheckboxLabeled(name, ref value, null);
			Text.Font = GameFont.Tiny;
			listing.ColumnWidth -= 34f;
			GUI.color = Color.gray;
			listing.Label(explanation, -1f, null);
			listing.ColumnWidth += 34f;
			Text.Font = GameFont.Small;
			Rect rect = listing.GetRect(0f);
			rect.height = listing.CurHeight - curHeight;
			rect.y -= rect.height;
			if (Mouse.IsOver(rect))
			{
				Widgets.DrawHighlight(rect);
				if (!tooltip.NullOrEmpty())
				{
					TooltipHandler.TipRegion(rect, tooltip);
				}
			}
			GUI.color = Color.white;
			listing.Gap(6f);
		}

		public static void Note(this Listing_Standard listing, string name, GameFont font = GameFont.Small)
		{
			Text.Font = font;
			listing.ColumnWidth -= 34f;
			GUI.color = Color.white;
			listing.Label(name, -1f, null);
			listing.ColumnWidth += 34f;
			Text.Font = GameFont.Small;
		}

		public static void ValueLabeled<T>(this Listing_Standard listing, string name, string explanation, ref T value, string tooltip = null)
		{
			float curHeight = listing.CurHeight;
			Rect rect = listing.GetRect(Text.LineHeight + listing.verticalSpacing);
			Text.Font = GameFont.Small;
			GUI.color = Color.white;
			TextAnchor anchor = Text.Anchor;
			Text.Anchor = TextAnchor.MiddleLeft;
			Widgets.Label(rect, (name));
			Text.Anchor = TextAnchor.MiddleRight;
			if (typeof(T).IsEnum)
			{
				Widgets.Label(rect, (value.ToString().Replace("_", " ")));
			}
			else
			{
				Widgets.Label(rect, value.ToString());
			}
			Text.Anchor = anchor;

			Text.Font = GameFont.Tiny;
			listing.ColumnWidth -= 34f;
			GUI.color = Color.gray;
			listing.Label(explanation, -1f, null);
			listing.ColumnWidth += 34f;
			Text.Font = GameFont.Small;

			rect = listing.GetRect(0f);
			rect.height = listing.CurHeight - curHeight;
			rect.y -= rect.height;
			if (Mouse.IsOver(rect))
			{
				Widgets.DrawHighlight(rect);
				if (!tooltip.NullOrEmpty())
				{
					TooltipHandler.TipRegion(rect, tooltip);
				}
				if (Event.current.isMouse && Event.current.button == 0 && Event.current.type == EventType.MouseDown)
				{
					T[] array = Enum.GetValues(typeof(T)).Cast<T>().ToArray<T>();
					for (int i = 0; i < array.Length; i++)
					{
						T t = array[(i + 1) % array.Length];
						if (array[i].ToString() == value.ToString())
						{
							value = t;
							break;
						}
					}
					Event.current.Use();
				}
			}
			GUI.color = Color.white;
			listing.Gap(6f);
		}

		public static void DropdownLabeled<T>(this Listing_Standard listing, string name, string explanation, ref T value, float width, string tooltip = null)
		{
			float curHeight = listing.CurHeight;
			Rect rect = listing.GetRect(Text.LineHeight + listing.verticalSpacing);
			Text.Font = GameFont.Small;
			GUI.color = Color.white;
			TextAnchor anchor = Text.Anchor;
			Text.Anchor = TextAnchor.MiddleLeft;
			Widgets.Label(rect, (name));
			Text.Anchor = TextAnchor.MiddleRight;
			T tempValue = value;
			Func<List<FloatMenuOption>> pageOptionsMaker = delegate ()
			{
				List<FloatMenuOption> list = new List<FloatMenuOption>();
				List<T> enumValues = Enum.GetValues(typeof(T)).Cast<T>().ToList<T>();
				foreach (T enumValue in enumValues)
				{
					list.Add(new FloatMenuOption(enumValue.ToString().Replace("_", " "), delegate ()
					{
						tempValue = enumValue;
					}));
				}
				if (!list.Any<FloatMenuOption>())
				{
					list.Add(new FloatMenuOption("NoneBrackets".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null));
				}
				return list;
			};

			Rect rect2 = new Rect(width - 150f, 0f, 150f, 29f);
			if (Widgets.ButtonText(rect2, value.ToString().Replace("_", " "), true, true, true))
			{
				Find.WindowStack.Add(new FloatMenu(pageOptionsMaker()));
			}
			//if (typeof(T).IsEnum)
			//{
			//	Widgets.Label(rect, (value.ToString().Replace("_", " ")));
			//}

			Text.Anchor = anchor;

			Text.Font = GameFont.Tiny;
			listing.ColumnWidth -= 34f;
			GUI.color = Color.gray;
			listing.Label(explanation, -1f, null);
			listing.ColumnWidth += 34f;
			Text.Font = GameFont.Small;

			rect = listing.GetRect(0f);
			rect.height = listing.CurHeight - curHeight;
			rect.y -= rect.height;
			GUI.color = Color.white;
			listing.Gap(6f);
		}
	}
}
