using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using VFECore.Abilities;
using AbilityDef = VFECore.Abilities.AbilityDef;

namespace HRF
{
    [HotSwappableAttribute]
    [StaticConstructorOnStartup]
    public class ITab_Pawn_Class : ITab
    {
        public ITab_Pawn_Class()
        {
            this.size = new Vector2(500f, 470f);
            this.labelKey = "ART.Class";
        }
        public override void FillTab()
        {
            try
            {
                var pawn = PawnToShowInfoAbout;
                var comp = pawn.GetComp<CompPawnClass>();
                var pawnClass = comp.ClassTraitDef;
                var trait = comp.ClassTrait;

                var rect = new Rect(0f, 0f, this.size.x, this.size.y);
                GUI.BeginGroup(rect);
                Text.Font = GameFont.Small;

                Rect innerMenu = new Rect(15f, 15f, rect.width - 30f, rect.height - 15f);
                Vector2 pos = new Vector2(innerMenu.x, innerMenu.y);
                var classIconRect = new Rect(pos.x, pos.y, 90, 90);
                GUI.DrawTexture(classIconRect, pawnClass.uiIcon);

                pos.x += 75 + 35;
                Widgets.Label(new Rect(pos.x, pos.y, 180, 24), "ART.ClassName".Translate(trait.LabelCap));
                pos.y += 24;
                Widgets.Label(new Rect(pos.x, pos.y, 180, 24), "ART.Level".Translate(comp.level));
                pos.y += 24;
                Widgets.Label(new Rect(pos.x, pos.y, 180, 24), "ART.AbilityPointsAvailable".Translate(comp.abilityPoints));
                pos.y += 24;
                if (pawnClass.maxLevel > comp.level)
                {
                    var nextLevelProgress = new Rect(pos.x, pos.y, 150, 24);
                    Widgets.Label(nextLevelProgress, "ART.Experience".Translate());
                    var nextLevelProgressBar = new Rect(nextLevelProgress.xMax, nextLevelProgress.y, 120, 24);
                    Widgets.FillableBar(nextLevelProgressBar, comp.GainedXPSinceLastLevel / comp.RequiredXPtoGain);
                    TooltipHandler.TipRegion(nextLevelProgressBar, "ART.CurrentXP".Translate(comp.GainedXPSinceLastLevel, comp.RequiredXPtoGain));
                }
                pos.y += 75;

                var abilities = pawn.GetAbilities();
                var list = new List<List<AbilityLearnState>>();
                var rowList = new List<AbilityLearnState>();
                int j = 0;
                foreach (var abilityEntry in abilities)
                {
                    if (j > 0 && j % 2 == 0)
                    {
                        list.Add(rowList.ListFullCopy());
                        rowList = new List<AbilityLearnState> { abilityEntry };
                    }
                    else
                    {
                        rowList.Add(abilityEntry);
                    }
                    j++;
                }
                if (rowList.Any())
                {
                    list.Add(rowList.ListFullCopy());
                }

                var lowerRect = new Rect(innerMenu.x, pos.y, innerMenu.width, innerMenu.height - 70);
                var totalRect = new Rect(lowerRect.x, lowerRect.y, lowerRect.width - 16, (list.Count * 42f));
                Widgets.BeginScrollView(lowerRect, ref scrollPosition, totalRect);

                foreach (var row in list)
                {
                    pos.x = 75;
                    for (var i = 0; i < row.Count; i++)
                    {
                        var entry = row[i];
                        var abilityData = comp.GetAbilityDataFrom(entry.abilityDef);
                        var nextAbility = entry.GetLearnableAbility(abilityData);
                        var ability = entry.learned ? comp.GetLearnedAbility(entry.abilityDef) : null;
                        bool learnedAbility = ability != null;
                        var iconBox = new Rect(pos.x, pos.y, 100, 100);

                        GUI.color = learnedAbility ? Color.white : Color.gray;
                        GUI.DrawTexture(iconBox, entry.abilityDef.icon);
                        GUI.color = Color.white;

                        Text.Anchor = TextAnchor.MiddleCenter;
                        var abilityNameRect = new Rect(pos.x - 40, iconBox.yMax + 5, 100 + 80, 32);
                        Widgets.Label(abilityNameRect, entry.abilityDef.LabelCap);

                        Text.Anchor = TextAnchor.UpperLeft;
                        TooltipHandler.TipRegion(iconBox, ability != null ? ability.GetDescriptionForPawn() : entry.abilityDef.description);
                        bool fullyUnlocked = nextAbility is null && ability != null;
                        var canLearnNextTier = fullyUnlocked ? false : comp.CanUnlockAbility(nextAbility);
                        if ((pawn.HostFaction == Faction.OfPlayer || pawn.Faction == Faction.OfPlayer) && !fullyUnlocked)
                        {
                            var iconSize = 50;
                            var plusRect = new Rect(iconBox.xMax + (iconSize / 3f), iconBox.y + ((iconBox.height / 2f) - (iconSize / 2f)), iconSize, iconSize);
                            var plusSignTexture = ContentFinder<Texture2D>.Get("UI/Buttons/Plus");
                            if (canLearnNextTier && Widgets.ButtonImage(plusRect, plusSignTexture))
                            {
                                comp.LearnAbility(nextAbility);
                            }
                        }
                        pos.x += 200;
                        GUI.color = Color.white;
                    }
                    pos.y += 150;
                }

                Widgets.EndScrollView();
                GUI.EndGroup();
            }
            catch (Exception ex)
            {
                Log.Error("Exception: " + ex);
            }
        }

        private Vector2 scrollPosition;
        public override bool IsVisible => this.PawnToShowInfoAbout is Pawn pawn && pawn.HasPawnClassComp(out var comp) 
            && comp.HasClass(out _);
        private Pawn PawnToShowInfoAbout
        {
            get
            {
                Pawn pawn = null;
                bool flag = base.SelPawn != null;
                if (flag)
                {
                    pawn = base.SelPawn;
                }
                else
                {
                    Corpse corpse = base.SelThing as Corpse;
                    bool flag2 = corpse != null;
                    if (flag2)
                    {
                        pawn = corpse.InnerPawn;
                    }
                }
                bool flag3 = pawn == null;
                Pawn result;
                if (flag3)
                {
                    result = null;
                }
                else
                {
                    result = pawn;
                }
                return result;
            }
        }
    }
}