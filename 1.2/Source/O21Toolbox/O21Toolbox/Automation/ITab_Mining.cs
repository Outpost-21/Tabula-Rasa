using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.Sound;

namespace O21Toolbox.Automation
{
    public class ITab_Mining : ITab
    {
        public Vector2 scrollPosition;
        public static readonly Vector2 WinSize = new Vector2(300f, 480f);

		public Thing SelectedMiner => base.SelObject as Thing;

        public ITab_Mining()
        {
            size = ITab_Mining.WinSize;
            labelKey = "TabMining";
            tutorTag = "QuarryMining";
        }

        public override void FillTab()
		{
			MiningSettings settings = SelectedMiner.TryGetComp<Comp_Quarry>()?.mineableThings;
			Rect position = new Rect(0f, 0f, ITab_Storage.WinSize.x, ITab_Storage.WinSize.y).ContractedBy(10f);
			GUI.BeginGroup(position);
			MiningFilter parentFilter = null;
			if (settings != null)
			{
				parentFilter = settings.filter;
			}
			Rect rect2 = new Rect(0f, 20f, position.width, position.height - 20f);
			//Rect viewRect = new Rect(0f, 0f, rect2.width - 16f, ThingFilterUI.viewHeight * MiningUtility.CachedMineableThings.Count());
			//Listing_Standard listing = new Listing_Standard();
			//listing.BeginScrollView(rect2, ref scrollPosition, ref viewRect);
			//foreach(ThingDef t in MiningUtility.CachedMineableThings)
			//         {
			//	bool flag = settings?.filter?.Allows(t) ?? false;
			//	bool flag2 = flag;
			//	listing.CheckboxLabeled(t.label, ref flag);
			//	if(flag != flag2)
			//             {
			//		settings.filter.SetAllow(t, flag);
			//             }
			//}
			//listing.EndScrollView(ref viewRect);
			DoThingFilterConfigWindow(rect2, ref scrollPosition, settings.filter, parentFilter, 8, null, null, false, null, null);
			PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.StorageTab, KnowledgeAmount.FrameDisplayed);
			GUI.EndGroup();
		}

		public static void DoThingFilterConfigWindow(Rect rect, ref Vector2 scrollPosition, MiningFilter filter, MiningFilter parentFilter = null, int openMask = 1, IEnumerable<ThingDef> forceHiddenDefs = null, IEnumerable<SpecialThingFilterDef> forceHiddenFilters = null, bool forceHideHitPointsConfig = false, List<ThingDef> suppressSmallVolumeTags = null, Map map = null)
		{
			Widgets.DrawMenuSection(rect);
			Text.Font = GameFont.Tiny;
			float num = rect.width - 2f;
			Rect rect2 = new Rect(rect.x + 1f, rect.y + 1f, num / 2f, 24f);
			if (Widgets.ButtonText(rect2, "ClearAll".Translate(), true, true, true))
			{
				filter.SetDisallowAll();
				SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera(null);
			}
			if (Widgets.ButtonText(new Rect(rect2.xMax + 1f, rect2.y, rect.xMax - 1f - (rect2.xMax + 1f), 24f), "AllowAll".Translate(), true, true, true))
			{
				filter.SetAllowAll();
				SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera(null);
			}
			Text.Font = GameFont.Small;
			rect.yMin = rect2.yMax;
			Rect viewRect = new Rect(0f, 0f, rect.width - 16f, ThingFilterUI.viewHeight);
			Widgets.BeginScrollView(rect, ref scrollPosition, viewRect, true);
			float num2 = 2f;
			float num3 = num2;
			Rect rect3 = new Rect(0f, num2, viewRect.width, 9999f);
			Listing_TreeMiningFilter listing_treeMiningFilter = new Listing_TreeMiningFilter(filter, parentFilter);
			listing_treeMiningFilter.Begin(rect3);
			listing_treeMiningFilter.DoCategoryChildren(0, map);
			listing_treeMiningFilter.End();
			if (Event.current.type == EventType.Layout)
			{
				ThingFilterUI.viewHeight = num3 + listing_treeMiningFilter.CurHeight + 90f;
			}
			Widgets.EndScrollView();
		}
	}
}
