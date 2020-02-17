using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

using Harmony;

using O21Toolbox.Alliances;

namespace O21Toolbox.Harmony
{
    public class Harmony_Alliances
    {
        static Harmony_Alliances()
        {

        }

        public static void Harmony_Patch(HarmonyInstance O21ToolboxHarmony, Type patchType)
        {
            O21ToolboxHarmony.Patch(AccessTools.Method(typeof(Faction), "TryMakeInitialRelationsWith", null, null), null, new HarmonyMethod(patchType, "TryMakeInitialRelationsWithPostfix", null), null);
        }

        public static void TryMakeInitialRelationsWithPostfix(Faction __instance, Faction other)
        {
            IEnumerable<AllianceDef> enumerable = from def in DefDatabase<AllianceDef>.AllDefs
                                                  where def.memberFactions != null
                                                  select def;
            foreach (AllianceDef current in enumerable)
            {
                if (current.memberFactions.Contains(__instance.def.defName))
                {
                    foreach (string faction in current.memberFactions)
                    {
                        if (faction != __instance.def.defName && other.def.defName.Contains(faction))
                        {
                            FactionRelation factionRelation = other.RelationWith(__instance, false);
                            factionRelation.goodwill = 100;
                            factionRelation.kind = FactionRelationKind.Ally;
                            FactionRelation factionRelation2 = __instance.RelationWith(other, false);
                            factionRelation2.goodwill = 100;
                            factionRelation2.kind = FactionRelationKind.Ally;
                        }
                    }

                    current.factionRelations.ForEach(delegate (RelationFaction rf)
                    {
                        if (other.def.defName.Contains(rf.faction))
                        {
                            int relation = rf.relation;
                            FactionRelationKind kind = (relation > 75) ? FactionRelationKind.Ally : ((relation <= -10) ? FactionRelationKind.Hostile : FactionRelationKind.Neutral);
                            FactionRelation factionRelation = other.RelationWith(__instance, false);
                            factionRelation.goodwill = relation;
                            factionRelation.kind = kind;
                            FactionRelation factionRelation2 = __instance.RelationWith(other, false);
                            factionRelation2.goodwill = relation;
                            factionRelation2.kind = kind;
                        }
                    });

                    current.allianceRelations.ForEach(delegate (RelationAlliance ra)
                    {
                        if (ra.alliance.memberFactions.Contains(other.def.defName))
                        {
                            int relation = ra.relation;
                            FactionRelationKind kind = (relation > 75) ? FactionRelationKind.Ally : ((relation <= -10) ? FactionRelationKind.Hostile : FactionRelationKind.Neutral);
                            FactionRelation factionRelation = other.RelationWith(__instance, false);
                            factionRelation.goodwill = relation;
                            factionRelation.kind = kind;
                            FactionRelation factionRelation2 = __instance.RelationWith(other, false);
                            factionRelation2.goodwill = relation;
                            factionRelation2.kind = kind;
                        }
                    });
                }

                current.playerRelations.ForEach(delegate (RelationPlayer rp)
                {
                    PawnKindDef basicMemberKind = __instance.def.basicMemberKind;
                    if (basicMemberKind != null && rp.factionBasicMemberKind.Contains(basicMemberKind.defName) && current.memberFactions.Contains(other.def.defName))
                    {
                        int relation = rp.relation;
                        FactionRelationKind kind = (relation > 75) ? FactionRelationKind.Ally : ((relation <= -10) ? FactionRelationKind.Hostile : FactionRelationKind.Neutral);
                        FactionRelation factionRelation = other.RelationWith(__instance, false);
                        factionRelation.goodwill = relation;
                        factionRelation.kind = kind;
                        FactionRelation factionRelation2 = __instance.RelationWith(other, false);
                        factionRelation2.goodwill = relation;
                        factionRelation2.kind = kind;
                    }
                });
            }
        }
    }
}
