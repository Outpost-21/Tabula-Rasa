using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace O21Toolbox.Drones
{
    public class MainTabWindow_Drones : MainTabWindow_PawnTable
    {
        private const int SpaceBetweenPriorityArrowsAndWorkLabels = 40;

        protected override PawnTableDef PawnTableDef
        {
            get
            {
                return PawnTableDefOf.Work;
            }
        }
        protected override float ExtraTopSpace
        {
            get
            {
                return 40f;
            }
        }

        protected override IEnumerable<Pawn> Pawns
        {
            get
            {
                return from p in Find.CurrentMap.mapPawns.PawnsInFaction(Faction.OfPlayer)
                       where p.def.thingClass == typeof(Drone_Pawn)
                       select p;
            }
        }

        public MainTabWindow_Drones()
        {
        }

        public override void PostOpen()
        {
            base.PostOpen();
            Find.World.renderer.wantedMode = WorldRenderMode.None;
        }

        public override void DoWindowContents(Rect rect)
        {
            base.DoWindowContents(rect);
            if (Event.current.type == EventType.Layout)
            {
                return;
            }
            this.DoManualPrioritiesCheckbox();
            GUI.color = new Color(1f, 1f, 1f, 0.5f);
            Text.Anchor = TextAnchor.UpperCenter;
            Text.Font = GameFont.Tiny;
            Widgets.Label(new Rect(370f, rect.y + 5f, 160f, 30f), "<= " + "HigherPriority".Translate());
            Widgets.Label(new Rect(630f, rect.y + 5f, 160f, 30f), "LowerPriority".Translate() + " =>");
            GUI.color = Color.white;
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
        }
        private void DoManualPrioritiesCheckbox()
        {
            Text.Font = GameFont.Small;
            GUI.color = Color.white;
            Text.Anchor = TextAnchor.UpperLeft;
            Rect rect = new Rect(5f, 5f, 140f, 30f);
            bool useWorkPriorities = Current.Game.playSettings.useWorkPriorities;
            Widgets.CheckboxLabeled(rect, "ManualPriorities".Translate(), ref Current.Game.playSettings.useWorkPriorities, false, null, null, false);
            if (useWorkPriorities != Current.Game.playSettings.useWorkPriorities)
            {
                foreach (Pawn pawn in PawnsFinder.AllMapsWorldAndTemporary_Alive)
                {
                    if (pawn.Faction == Faction.OfPlayer && pawn.workSettings != null)
                    {
                        pawn.workSettings.Notify_UseWorkPrioritiesChanged();
                    }
                }
            }
            if (Current.Game.playSettings.useWorkPriorities)
            {
                GUI.color = new Color(1f, 1f, 1f, 0.5f);
                Text.Font = GameFont.Tiny;
                Widgets.Label(new Rect(rect.x, rect.y + rect.height + 4f, rect.width, 60f), "PriorityOneDoneFirst".Translate());
                Text.Font = GameFont.Small;
                GUI.color = Color.white;
            }
            if (!Current.Game.playSettings.useWorkPriorities)
            {
                UIHighlighter.HighlightOpportunity(rect, "ManualPriorities-Off");
            }
        }
    }
}
