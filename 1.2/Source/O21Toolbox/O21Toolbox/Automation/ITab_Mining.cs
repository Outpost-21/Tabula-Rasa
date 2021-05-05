using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

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
			Rect viewRect = new Rect(0f, 0f, rect2.width - 16f, ThingFilterUI.viewHeight);
			Listing_Standard listing = new Listing_Standard();
			listing.BeginScrollView(rect2, ref scrollPosition, ref viewRect);
			foreach(ThingDef t in MiningUtility.CachedMineableThings)
            {
				bool flag = settings?.filter?.Allows(t) ?? false;
				bool flag2 = flag;
				listing.CheckboxLabeled(t.label, ref flag);
				if(flag != flag2)
                {
					settings.filter.SetAllow(t, flag);
                }
			}
			listing.EndScrollView(ref viewRect);
			PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.StorageTab, KnowledgeAmount.FrameDisplayed);
			GUI.EndGroup();
		}
    }
}
