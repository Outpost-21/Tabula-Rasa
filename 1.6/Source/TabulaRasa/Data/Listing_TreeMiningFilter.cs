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
    public class Listing_TreeMiningFilter : Listing_Tree
	{
		public MiningFilter filter;

		public MiningFilter parentFilter;

		public Listing_TreeMiningFilter(MiningFilter filter, MiningFilter parentFilter)
		{
			this.filter = filter;
			this.parentFilter = parentFilter;
		}

		public void DoCategoryChildren(int indentLevel, Map map)
		{
			foreach (ThingDef thingDef in from n in MiningUtility.CachedMineableThings
										  orderby n.label
										  select n)
			{
				this.DoThingDef(thingDef, indentLevel, map);
			}
		}

		private void DoThingDef(ThingDef tDef, int nestLevel, Map map)
		{
			string text = tDef.DescriptionDetailed;
			float num = -4f;
			num -= 19f;
			if (map != null)
			{
				int count = map.resourceCounter.GetCount(tDef);
				if (count > 0)
				{
					string text2 = count.ToStringCached();
					Rect rect2 = new Rect(0f, this.curY, this.LabelWidth + num, 40f);
					Text.Font = GameFont.Tiny;
					Text.Anchor = TextAnchor.UpperRight;
					GUI.color = new Color(0.5f, 0.5f, 0.1f);
					Widgets.Label(rect2, text2);
					num -= Text.CalcSize(text2).x;
					GenUI.ResetLabelAlign();
					Text.Font = GameFont.Small;
					GUI.color = Color.white;
				}
			}
			base.LabelLeft(tDef.LabelCap, text, nestLevel, num);
			bool flag = this.filter.Allows(tDef);
			bool flag2 = flag;
			Widgets.Checkbox(new Vector2(this.LabelWidth, this.curY), ref flag, this.lineHeight, false, true, null, null);
			if (flag != flag2)
			{
				this.filter.SetAllow(tDef, flag);
			}
			base.EndLine();
		}
	}
}
