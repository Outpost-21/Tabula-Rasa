using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.AI;

using HarmonyLib;

using O21Toolbox.Utility;

namespace O21Toolbox.HarmonyPatches.Patches
{
    public class Harmony_Animals
    {
		[HarmonyPatch(typeof(PawnComponentsUtility), "CreateInitialComponents")]
		public static class PawnComponentsUtility_CreateInitialComponents_Patch
		{
			[HarmonyPostfix]
			public static void CreateInitialComponents_Post(Pawn pawn)
			{
				if (AnimalApparelUtility.IsColonyAnimal(pawn))
				{
					AnimalApparelUtility.InitAnimalApparelTrackers(pawn);
				}
			}
		}

		[HarmonyPatch(typeof(PawnComponentsUtility), "AddAndRemoveDynamicComponents")]
		public static class PawnComponentsUtility_AddAndRemoveDynamicComponents_Patch
		{
			[HarmonyPostfix]
			public static void AddAndRemoveDynamicComponents_Post(Pawn pawn, bool actAsIfSpawned)
			{
				if (AnimalApparelUtility.IsColonyAnimal(pawn))
				{
					AnimalApparelUtility.InitAnimalApparelTrackers(pawn);
				}
			}
		}

		[HarmonyPatch(typeof(ITab_Pawn_Gear))]
		[HarmonyPatch("IsVisible", MethodType.Getter)]
		public static class ITab_Pawn_Gear_IsVisible_Patch
		{
			[HarmonyPostfix]
			public static void IsVisible_Post(ITab_Pawn_Gear __instance, ref bool __result)
			{
				if (!__result)
				{
					Pawn pawn = SelPawnForGearPatch(__instance);
					if (pawn != null)
					{
						if (pawn != null && AnimalApparelUtility.IsColonyAnimal(pawn))
						{
							__result = true;
						}
					}
				}
			}

			public static Pawn SelPawnForGearPatch(ITab_Pawn_Gear __instance)
			{
				Thing singleSelectedThing = Find.Selector.SingleSelectedThing;
				Pawn result = null;
				if (singleSelectedThing != null)
				{
					Pawn pawn = (Pawn)singleSelectedThing;
					if (pawn != null)
					{
						result = pawn;
					}
					else
					{
						Corpse corpse = (singleSelectedThing as Corpse);
						if (corpse == null)
						{
							throw new InvalidOperationException("Gear tab on non-pawn non-corpse " + singleSelectedThing);
						}
                        else
						{
							result = corpse.InnerPawn;
						}
					}
				}
				return result;
			}
		}

		[HarmonyPatch(typeof(FloatMenuOption), "", MethodType.Constructor)]
		[HarmonyPatch(new Type[]
		{
			typeof(string),
			typeof(Action),
			typeof(MenuOptionPriority),
			typeof(Action),
			typeof(Thing),
			typeof(float),
			typeof(Func<Rect, bool>),
			typeof(WorldObject)
		})]
		public static class FloatMenuOption_FloatMenuOption_Patch
		{
			[HarmonyPostfix]
			[HarmonyPriority(400)]
			public static void FloatMenuOption_Post(FloatMenuOption __instance, string label, Action action, MenuOptionPriority priority, Action mouseoverGuiAction, Thing revalidateClickTarget, float extraPartWidth, Func<Rect, bool> extraPartOnGUI, WorldObject revalidateWorldClickTarget)
			{
				if (action != null && action.Target != null && Traverse.Create(action.Target).Fields().Contains("recipe"))
				{
					RecipeDef recipeDef = (RecipeDef)Traverse.Create(action.Target).Field("recipe").GetValue();
					if (recipeDef != null)
					{
						bool flag;
						if (recipeDef.products != null)
						{
							flag = ((from t in recipeDef.products
										where t != null && t.thingDef != null && t.thingDef.apparel != null && t.thingDef.apparel.tags != null && t.thingDef.apparel.tags.Contains("Animal")
										select t).FirstOrDefault<ThingDefCountClass>() != null);
						}
						else
						{
							flag = false;
						}
						if (flag)
						{
							if (!__instance.Label.Contains("[Animal]"))
							{
								__instance.Label += " [Animal]";
							}
							__instance.Priority = MenuOptionPriority.Low;
						}
					}
				}
			}
		}

		[HarmonyPatch(typeof(PawnGraphicSet), "ResolveApparelGraphics")]
		public static class PawnGraphicSet_ResolveApparelGraphics_Patch
		{
			[HarmonyPrefix]
			[HarmonyBefore(new string[]
			{
				"com.tammybee.apparentclothes",
				"apparentclothes"
			})]
			[HarmonyPriority(600)]
			public static bool ResolveApparelGraphics_Pre(PawnGraphicSet __instance)
			{
				if (AnimalApparelUtility.IsColonyAnimal(__instance.pawn))
				{
					AnimalApparelUtility.InitAnimalApparelTrackers(__instance.pawn);
					__instance.ClearCache();
					__instance.apparelGraphics.Clear();
					foreach (Apparel apparel in __instance.pawn.apparel.WornApparel)
					{
						ApparelGraphicRecord item;
						if (ApparelGraphicRecordGetterAnimal.TryGetGraphicApparelAnimal(apparel, __instance.pawn, __instance.pawn.kindDef, out item))
						{
							__instance.apparelGraphics.Add(item);
						}
					}
					return false;
				}
				return true;
			}

			public static class ApparelGraphicRecordGetterAnimal
			{
				public static bool TryGetGraphicApparelAnimal(Apparel apparel, Pawn pawn, PawnKindDef kindDef, out ApparelGraphicRecord rec)
				{
					try
					{
						string text = "";
						string text2 = "";
						string text3 = "";
						if (apparel.def.apparel.wornGraphicPath.NullOrEmpty())
						{
							rec = new ApparelGraphicRecord(null, null);
							return false;
						}
						Graphic graphic;
						if (apparel.def.apparel.tags == null || !apparel.def.apparel.tags.Contains("AnimalALL"))
						{
							if (kindDef != null)
							{
								text = text + "_" + kindDef.defName.ToString();
							}
							if (pawn.ageTracker != null)
							{
								PawnKindLifeStage curKindLifeStage = pawn.ageTracker.CurKindLifeStage;
								if (curKindLifeStage != null && curKindLifeStage.bodyGraphicData != null && !string.IsNullOrEmpty(curKindLifeStage.bodyGraphicData.texPath))
								{
									text3 = "_" + pawn.ageTracker.CurLifeStageIndex.ToString();
									text2 += text3;
								}
								if (pawn.gender == Gender.Female && curKindLifeStage != null && curKindLifeStage.femaleGraphicData != null && !string.IsNullOrEmpty(curKindLifeStage.femaleGraphicData.texPath))
								{
									text2 += "_female";
								}
								if (!string.IsNullOrEmpty(text2))
								{
									if (ContentFinder<Texture2D>.Get(apparel.def.apparel.wornGraphicPath + text + text2 + "_north", false) != null)
									{
										graphic = GraphicDatabase.Get<Graphic_Multi>(apparel.def.apparel.wornGraphicPath + text + text2,
											apparel.def.apparel.useWornGraphicMask ? ShaderDatabase.CutoutComplex : ShaderDatabase.Cutout, 
											apparel.def.graphicData.drawSize, 
											apparel.DrawColor);
										rec = new ApparelGraphicRecord(graphic, apparel);
										if (Prefs.DevMode)
										{
											Log.Message(string.Concat(new object[]
											{
												"O21Toolbox: Special Graphics loaded for Pawn: ",
												pawn.Name,
												" - looking for: '",
												apparel.def.apparel.wornGraphicPath,
												text,
												text2,
												"' Shader:",
												(apparel.def.graphicData.shaderType != null) ? apparel.def.graphicData.shaderType.ToString() : "null"
											}), false);
										}
										return true;
									}
									if (!string.IsNullOrEmpty(text3) && ContentFinder<Texture2D>.Get(apparel.def.apparel.wornGraphicPath + text + text3 + "_north", false) != null)
									{
										graphic = GraphicDatabase.Get<Graphic_Multi>(apparel.def.apparel.wornGraphicPath + text + text3, 
											apparel.def.apparel.useWornGraphicMask ? ShaderDatabase.CutoutComplex : ShaderDatabase.Cutout, 
											apparel.def.graphicData.drawSize, 
											apparel.DrawColor);
										rec = new ApparelGraphicRecord(graphic, apparel);
										if (Prefs.DevMode)
										{
											Log.Message(string.Concat(new object[]
											{
												"O21Toolbox: Special Graphics loaded for Pawn: ",
												pawn.Name,
												" - looking for: '",
												apparel.def.apparel.wornGraphicPath,
												text,
												text3,
												"' Shader:",
												(apparel.def.graphicData.shaderType != null) ? apparel.def.graphicData.shaderType.ToString() : "null"
											}), false);
										}
										return true;
									}
								}
							}
						}
						else
						{
							graphic = GraphicDatabase.Get<Graphic_Multi>(apparel.def.apparel.wornGraphicPath + text,
								apparel.def.apparel.useWornGraphicMask ? ShaderDatabase.CutoutComplex : ShaderDatabase.Cutout,
								apparel.def.graphicData.drawSize,
								apparel.DrawColor);
							rec = new ApparelGraphicRecord(graphic, apparel);
							return true;
						}
					}
					catch (Exception ex)
					{
						bool devMode3 = Prefs.DevMode;
						if (devMode3)
						{
							Log.Error("ApparelGraphicRecordGetterAnimal: TryGetGraphicApparelAnimal: error: " + ex.Message, false);
						}
					}
					rec = new ApparelGraphicRecord(null, null);
					return false;
				}
			}
		}

		[HarmonyPatch(typeof(PawnGraphicSet), "ResolveAllGraphics")]
		public static class PawnGraphicSet_ResolveAllGraphics_Patch
		{
			[HarmonyPostfix]
			public static void PawnGraphicSet_ResolveAllGraphics_Done(PawnGraphicSet __instance)
			{
				if (AnimalApparelUtility.IsColonyAnimal(__instance.pawn))
				{
					__instance.ResolveApparelGraphics();
				}
			}
		}

		[HarmonyPatch(typeof(Pawn_ApparelTracker), "CanWearWithoutDroppingAnything")]
		public static class Pawn_ApparelTracker_CanWearWithoutDroppingAnything_Patch
		{
			[HarmonyPostfix]
			public static void Pawn_ApparelTracker_CanWearWithoutDroppingAnything_Done(Pawn_ApparelTracker __instance, ThingDef apDef, ref bool __result)
			{
				try
				{
					if (__result && AnimalApparelUtility.IsColonyAnimal(__instance.pawn))
					{
						ThingOwner<Apparel> thingOwner = null;
						try
						{
							thingOwner = Traverse.Create(__instance).Field("wornApparel").GetValue<ThingOwner<Apparel>>();
						}
						catch
						{
						}
						if (thingOwner != null)
						{
							for (int i = 0; i < thingOwner.Count; i++)
							{
								if (!CanWearTogetherAnimal(apDef, thingOwner[i].def, __instance.pawn.RaceProps.body))
								{
									__result = false;
								}
							}
						}
					}
				}
				catch
				{
				}
			}

			public static bool CanWearTogetherAnimal(ThingDef A, ThingDef B, BodyDef body)
			{
				bool result;
				if (A.defName == B.defName)
				{
					result = false;
				}
				else
				{
					bool flag = false;
					for (int i = 0; i < A.apparel.layers.Count; i++)
					{
						for (int j = 0; j < B.apparel.layers.Count; j++)
						{
							if (A.apparel.layers[i] == B.apparel.layers[j])
							{
								flag = true;
							}
							if (flag)
							{
								break;
							}
						}
						if (flag)
						{
							break;
						}
					}
					if (!flag)
					{
						result = true;
					}
					else
					{
						BodyPartGroupDef[] interferingBodyPartGroups = A.apparel.GetInterferingBodyPartGroups(body);
						BodyPartGroupDef[] interferingBodyPartGroups2 = B.apparel.GetInterferingBodyPartGroups(body);
						for (int k = 0; k < interferingBodyPartGroups.Length; k++)
						{
							if (interferingBodyPartGroups2.Contains(interferingBodyPartGroups[k]))
							{
								return false;
							}
						}
						result = true;
					}
				}
				return result;
			}
		}

		[HarmonyPatch(typeof(Pawn_ApparelTracker), "Wear")]
		public static class Pawn_ApparelTracker_Wear_Patch
		{
			[HarmonyPrefix]
			public static bool Pawn_ApparelTracker_Wear_Pre(Pawn_ApparelTracker __instance, Apparel newApparel, bool dropReplacedApparel = true, bool locked = false)
			{
				try
				{
					if (AnimalApparelUtility.IsColonyAnimal(__instance.pawn))
					{
						ThingOwner<Apparel> thingOwner = null;
						try
						{
							thingOwner = Traverse.Create(__instance).Field("wornApparel").GetValue<ThingOwner<Apparel>>();
						}
						catch
						{
						}
						if (thingOwner != null)
						{
							if (newApparel.Spawned)
							{
								newApparel.DeSpawn(DestroyMode.Vanish);
							}
							if (!ApparelUtility.HasPartsToWear(__instance.pawn, newApparel.def))
							{
								Log.Warning(string.Concat(new object[]
								{
									__instance.pawn.ToString(),
									" tried to wear ",
									newApparel,
									" but has no body parts required to wear it."
								}), false);
							}
							else
							{
								for (int i = thingOwner.Count - 1; i >= 0; i--)
								{
									Apparel apparel = thingOwner[i];
									if (!Pawn_ApparelTracker_CanWearWithoutDroppingAnything_Patch.CanWearTogetherAnimal(newApparel.def, apparel.def, __instance.pawn.RaceProps.body))
									{
										if (dropReplacedApparel)
										{
											Apparel apparelOut;
											if (!__instance.TryDrop(apparel, out apparelOut, __instance.pawn.Position, __instance.pawn.Faction.HostileTo(Faction.OfPlayer)))
											{
												Log.Error(__instance.pawn.ToString() + " could not drop " + apparel, false);
												return false;
											}
										}
										else
										{
											__instance.Remove(apparel);
										}
									}
								}
								if (newApparel.Wearer != null)
								{
									Log.Warning(string.Concat(new object[]
									{
										__instance.pawn.ToString(),
										" is trying to wear ",
										newApparel,
										" but this apparel already has a wearer (",
										newApparel.Wearer,
										"). This may or may not cause bugs."
									}), false);
								}
								thingOwner.TryAdd(newApparel, false);
								if (!locked)
								{
									return false;
								}
								__instance.Lock(newApparel);
							}
						}
						return false;
					}
				}
				catch
				{
				}
				return true;
			}
		}

		[HarmonyPatch(typeof(JobGiver_OptimizeApparel), "ApparelScoreGain")]
		public static class JobGiver_OptimizeApparel_ApparelScoreGain_Patch
		{
			[HarmonyPrefix]
			public static bool JobGiver_OptimizeApparel_ApparelScoreGain_Pre(Pawn pawn, Apparel ap, ref float __result)
			{
				try
				{
					if (AnimalApparelUtility.IsColonyAnimal(pawn))
					{
						if (ap.def == ThingDefOf.Apparel_ShieldBelt && pawn.equipment != null && pawn.equipment.Primary != null && pawn.equipment.Primary.def.IsRangedWeapon)
						{
							__result = -1000f;
							return false;
						}
						float num = ApparelScoreRawAnimal(pawn, ap);
						List<Apparel> wornApparel = pawn.apparel.WornApparel;
						bool flag = false;
						for (int i = 0; i < wornApparel.Count; i++)
						{
							if (!Pawn_ApparelTracker_CanWearWithoutDroppingAnything_Patch.CanWearTogetherAnimal(wornApparel[i].def, ap.def, pawn.RaceProps.body))
							{
								if (!pawn.outfits.forcedHandler.AllowedToAutomaticallyDrop(wornApparel[i]) || pawn.apparel.IsLocked(wornApparel[i]))
								{
									__result = -1000f;
									return false;
								}
								num -= ApparelScoreRawAnimal(pawn, wornApparel[i]);
								flag = true;
							}
						}
						if (!flag)
						{
							num *= 10f;
						}
						__result = num;
						return false;
					}
				}
				catch
				{
				}
				return true;
			}

			public static float ApparelScoreRawAnimal(Pawn pawn, Apparel ap)
			{
				float num = 0.1f + (ap.GetStatValue(StatDefOf.ArmorRating_Sharp, true) + ap.GetStatValue(StatDefOf.ArmorRating_Blunt, true));
				if (ap.def.useHitPoints)
				{
					float x = (float)ap.HitPoints / (float)ap.MaxHitPoints;
					num *= Traverse.Create<JobGiver_OptimizeApparel>().Field("HitPointsPercentScoreFactorCurve").GetValue<SimpleCurve>().Evaluate(x);
				}
				float num2 = num + ap.GetSpecialApparelScoreOffset();
				float num3 = 1f;
				if (Traverse.Create<JobGiver_OptimizeApparel>().Field("neededWarmth").GetValue<NeededWarmth>() == NeededWarmth.Warm)
				{
					float statValue = ap.GetStatValue(StatDefOf.Insulation_Cold, true);
					num3 *= Traverse.Create<JobGiver_OptimizeApparel>().Field("InsulationColdScoreFactorCurve_NeedWarm").GetValue<SimpleCurve>().Evaluate(statValue);
				}
				float num4 = num2 * num3;
				if (!ap.def.apparel.CorrectGenderForWearing(pawn.gender))
				{
					num4 *= 0.01f;
				}
				return num4;
			}
		}

		[HarmonyPatch(typeof(ApparelUtility), "HasPartsToWear")]
		public static class ApparelUtility_HasPartsToWear_Patch
		{
			[HarmonyPostfix]
			public static void ApparelUtility_HasPartsToWear_Post(Pawn p, ThingDef apparel, ref bool __result)
			{
				try
				{
					if (__result)
					{
						if (apparel != null && apparel.IsApparel && apparel.apparel.tags != null && apparel.apparel.tags.Count > 0 && apparel.apparel.tags.Contains("Animal"))
						{
							if (AnimalApparelUtility.IsAnimal(p))
							{
								if (p.kindDef != null && apparel.apparel.tags.Contains(p.kindDef.defName.ToString()))
								{
									__result = true;
								}
								else
								{
									if (apparel.apparel.tags.Contains("AnimalALL"))
									{
										__result = true;
									}
									else
									{
										__result = false;
									}
								}
							}
							else
							{
								__result = false;
							}
						}
						else
						{
							if (apparel != null && apparel.IsApparel && apparel.apparel.tags != null && apparel.apparel.tags.Count > 0 && apparel.apparel.tags.Contains("AnimalCompatible"))
							{
								if (AnimalApparelUtility.IsAnimal(p))
								{
									if (p.kindDef != null && apparel.apparel.tags.Contains(p.kindDef.defName.ToString()))
									{
										__result = true;
									}
									else
									{
										if (apparel.apparel.tags.Contains("AnimalALL"))
										{
											__result = true;
										}
										else
										{
											__result = false;
										}
									}
								}
							}
							else
							{
								if (AnimalApparelUtility.IsAnimal(p))
								{
									__result = false;
								}
							}
						}
					}
				}
				catch
				{
				}
			}
		}

		[HarmonyPatch(typeof(PawnRenderer), "RenderPawnInternal")]
		[HarmonyPatch(new Type[]
		{
			typeof(Vector3),
			typeof(float),
			typeof(bool),
			typeof(Rot4),
			typeof(Rot4),
			typeof(RotDrawMode),
			typeof(bool),
			typeof(bool),
			typeof(bool)
		})]
		public static class PawnRenderer_RenderPawnInternal_Patch
		{
			[HarmonyPrefix]
			public static bool PawnRenderer_RenderPawnInternal_Pre(PawnRenderer __instance, Vector3 rootLoc, float angle, bool renderBody, Rot4 bodyFacing, Rot4 headFacing, RotDrawMode bodyDrawType, bool portrait, bool headStump, bool invisible)
			{
				try
				{
					Pawn pawn = null;
					try
					{
						pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
					}
					catch
					{
					}
					if (pawn != null)
					{
						if (AnimalApparelUtility.IsColonyAnimal(pawn))
						{
							PawnWoundDrawer pawnWoundDrawer = null;
							try
							{
								pawnWoundDrawer = Traverse.Create(__instance).Field("woundOverlays").GetValue<PawnWoundDrawer>();
							}
							catch
							{
							}
							if (pawnWoundDrawer != null)
							{
								PawnHeadOverlays pawnHeadOverlays = null;
								try
								{
									pawnHeadOverlays = Traverse.Create(__instance).Field("statusOverlays").GetValue<PawnHeadOverlays>();
								}
								catch
								{
								}
								if (pawnHeadOverlays != null)
								{
									RenderPawnInternalAnimal(__instance, pawn, pawnWoundDrawer, pawnHeadOverlays, rootLoc, angle, renderBody, bodyFacing, headFacing, bodyDrawType, portrait, headStump, invisible);
									return false;
								}
							}
						}
					}
				}
				catch
				{
				}
				return true;
			}

			private static void RenderPawnInternalAnimal(PawnRenderer __instance, Pawn pawn, PawnWoundDrawer woundOverlays, PawnHeadOverlays statusOverlays, Vector3 rootLoc, float angle, bool renderBody, Rot4 bodyFacing, Rot4 headFacing, RotDrawMode bodyDrawType, bool portrait, bool headStump, bool invisible)
			{
				if (!__instance.graphics.AllResolved)
				{
					__instance.graphics.ResolveAllGraphics();
				}
				Quaternion quaternion = Quaternion.AngleAxis(angle, Vector3.up);
				Mesh mesh = null;
				Vector3 vector = rootLoc;
				int num;
				int num2;
				bool flag;
				GetPawnsStandingAtOrAboutToStandAtAnimal(pawn.Position, pawn.Map, out num, out num2, out flag, pawn, true);
				if (flag)
				{
					vector.y += 0.007575758f * (float)num2;
					vector.y += 0.02272727f * (float)num2;
				}
				if (renderBody)
				{
					Vector3 loc = vector;
					loc.y += 0.007575758f;
					if (bodyDrawType == RotDrawMode.Dessicated && !pawn.RaceProps.Humanlike && __instance.graphics.dessicatedGraphic != null && !portrait)
					{
						__instance.graphics.dessicatedGraphic.Draw(loc, bodyFacing, pawn, angle);
					}
					else
					{
						mesh = ((!pawn.RaceProps.Humanlike) ? __instance.graphics.nakedGraphic.MeshAt(bodyFacing) : MeshPool.humanlikeBodySet.MeshAt(bodyFacing));
						List<Material> list = __instance.graphics.MatsBodyBaseAt(bodyFacing, bodyDrawType);
						for (int i = 0; i < list.Count; i++)
						{
							Material mat = OverrideMaterialIfNeeded(__instance, list[i], pawn);
							GenDraw.DrawMeshNowOrLater(mesh, loc, quaternion, mat, portrait);
							loc.y += 0.003787879f;
						}
						if (bodyDrawType == RotDrawMode.Fresh)
						{
							Vector3 drawLoc = vector;
							drawLoc.y += 0.01893939f;
							woundOverlays.RenderOverBody(drawLoc, mesh, quaternion, portrait);
						}
						for (int j = 0; j < __instance.graphics.apparelGraphics.Count; j++)
						{
							ApparelGraphicRecord apparelGraphicRecord = __instance.graphics.apparelGraphics[j];
							if (apparelGraphicRecord.sourceApparel.def.apparel.LastLayer == ApparelLayerDefOf.Shell && (apparelGraphicRecord.sourceApparel.def.apparel.tags == null || !apparelGraphicRecord.sourceApparel.def.apparel.tags.Contains("AnimalShellOuter")))
							{
								Material mat2 = OverrideMaterialIfNeeded(__instance, apparelGraphicRecord.graphic.MatAt(bodyFacing, null), pawn);
								GenDraw.DrawMeshNowOrLater(mesh, loc, quaternion, mat2, portrait);
								loc.y += 0.003787879f;
							}
						}
					}
				}
				Vector3 vector2 = vector;
				Vector3 a = vector;
				if (bodyFacing != Rot4.North)
				{
					a.y += 0.02651515f;
					vector2.y += 0.02272727f;
				}
				else
				{
					a.y += 0.02272727f;
					vector2.y += 0.02651515f;
				}
				if (__instance.graphics.headGraphic != null)
				{
					Vector3 b = quaternion * __instance.BaseHeadOffsetAt(headFacing);
					Material material = __instance.graphics.HeadMatAt(headFacing, bodyDrawType, headStump);
					if (material != null)
					{
						GenDraw.DrawMeshNowOrLater(MeshPool.humanlikeHeadSet.MeshAt(headFacing), a + b, quaternion, material, portrait);
					}
					Vector3 vector3 = vector + b;
					vector3.y += 0.03030303f;
					bool flag2 = false;
					if (!portrait || !Prefs.HatsOnlyOnMap)
					{
						Mesh mesh2 = __instance.graphics.HairMeshSet.MeshAt(headFacing);
						List<ApparelGraphicRecord> apparelGraphics = __instance.graphics.apparelGraphics;
						for (int k = 0; k < apparelGraphics.Count; k++)
						{
							if (apparelGraphics[k].sourceApparel.def.apparel.LastLayer == ApparelLayerDefOf.Overhead)
							{
								if (!apparelGraphics[k].sourceApparel.def.apparel.hatRenderedFrontOfFace)
								{
									flag2 = true;
									GenDraw.DrawMeshNowOrLater(mesh2, vector3, quaternion, OverrideMaterialIfNeeded(__instance, apparelGraphics[k].graphic.MatAt(bodyFacing, null), pawn), portrait);
								}
								else
								{
									Vector3 loc2 = vector + b;
									loc2.y += ((bodyFacing == Rot4.North) ? 0.003787879f : 0.03409091f);
									GenDraw.DrawMeshNowOrLater(mesh2, loc2, quaternion, OverrideMaterialIfNeeded(__instance, apparelGraphics[k].graphic.MatAt(bodyFacing, null), pawn), portrait);
								}
							}
						}
					}
					if (!flag2 && bodyDrawType != RotDrawMode.Dessicated && !headStump)
					{
						GenDraw.DrawMeshNowOrLater(__instance.graphics.HairMeshSet.MeshAt(headFacing), vector3, quaternion, __instance.graphics.HairMatAt(headFacing), (portrait ? 1 : 0) != 0);
					}
				}
				if (renderBody)
				{
					for (int l = 0; l < __instance.graphics.apparelGraphics.Count; l++)
					{
						ApparelGraphicRecord apparelGraphicRecord2 = __instance.graphics.apparelGraphics[l];
						if (apparelGraphicRecord2.sourceApparel.def.apparel.LastLayer == ApparelLayerDefOf.Shell && apparelGraphicRecord2.sourceApparel.def.apparel.tags != null && apparelGraphicRecord2.sourceApparel.def.apparel.tags.Contains("AnimalShellOuter"))
						{
							GenDraw.DrawMeshNowOrLater(mesh, vector2, quaternion, OverrideMaterialIfNeeded(__instance, apparelGraphicRecord2.graphic.MatAt(bodyFacing, null), pawn), portrait);
						}
					}
				}
				if (!portrait && pawn.RaceProps.Animal && pawn.inventory != null && pawn.inventory.innerContainer.Count > 0 && __instance.graphics.packGraphic != null)
				{
					Graphics.DrawMesh(mesh, vector2, quaternion, __instance.graphics.packGraphic.MatAt(bodyFacing, null), 0);
				}
				if (!portrait)
				{
					Traverse.Create(__instance).Method("DrawEquipment", new object[]
					{
						vector
					}).GetValue();
					if (pawn.apparel != null)
					{
						List<Apparel> wornApparel = pawn.apparel.WornApparel;
						for (int m = 0; m < wornApparel.Count; m++)
						{
							wornApparel[m].DrawWornExtras();
						}
					}
					Vector3 bodyLoc = vector;
					bodyLoc.y += 0.04166667f;
					statusOverlays.RenderStatusOverlays(bodyLoc, quaternion, MeshPool.humanlikeHeadSet.MeshAt(headFacing));
				}
			}

			private static Material OverrideMaterialIfNeeded(PawnRenderer PRinstance, Material original, Pawn pawn)
			{
				return PRinstance.graphics.flasher.GetDamagedMat(pawn.IsInvisible() ? InvisibilityMatPool.GetInvisibleMat(original) : original);
			}

			private static void GetPawnsStandingAtOrAboutToStandAtAnimal(IntVec3 at, Map map, out int pawnsCount, out int pawnsWithLowerIdCount, out bool forPawnFound, Pawn forPawn)
			{
				GetPawnsStandingAtOrAboutToStandAtAnimal(at, map, out pawnsCount, out pawnsWithLowerIdCount, out forPawnFound, forPawn, false);
			}

			private static void GetPawnsStandingAtOrAboutToStandAtAnimal(IntVec3 at, Map map, out int pawnsCount, out int pawnsWithLowerIdCount, out bool forPawnFound, Pawn forPawn, bool dontCountColonists)
			{
				pawnsCount = 0;
				pawnsWithLowerIdCount = 0;
				forPawnFound = false;
				bool flag2 = false;
				bool flag3 = false;
				if (AnimalApparelUtility.IsAnimal(forPawn))
				{
					Job curJob = forPawn.CurJob;
					if (curJob != null && curJob.def.defName == "Mate")
					{
						flag2 = true;
						if (forPawn.gender == Gender.Male)
						{
							flag3 = true;
						}
						else
						{
							if (!(forPawn.gender == Gender.Female))
							{
								if (curJob.targetA == forPawn)
								{
									flag3 = true;
								}
							}
						}
					}
				}
				foreach (IntVec3 intVec in CellRect.SingleCell(at).ExpandedBy(1))
				{
					if (intVec.InBounds(map))
					{
						List<Thing> thingList = intVec.GetThingList(map);
						int i = 0;
						while (i < thingList.Count)
						{
							Pawn pawn = thingList[i] as Pawn;
							if (pawn != null && pawn.GetPosture() == PawnPosture.Standing)
							{
								bool flag12 = false;
								if (flag2 && AnimalApparelUtility.IsAnimal(forPawn))
								{
									if (flag3)
									{
										flag12 = true;
									}
								}
								if (intVec != at)
								{
									if ((!AnimalApparelUtility.IsAnimal(forPawn) || (flag2 && flag12)) && (!pawn.pather.MovingNow || pawn.pather.nextCell != pawn.pather.Destination.Cell || pawn.pather.Destination.Cell != at))
									{
										goto IL_Jump;
									}
								}
								else
								{
									if ((!AnimalApparelUtility.IsAnimal(forPawn) || !AnimalApparelUtility.IsAnimal(forPawn) || (flag2 && flag12)) && pawn.pather.MovingNow)
									{
										goto IL_Jump;
									}
								}
								if (pawn == forPawn)
								{
									forPawnFound = true;
								}
								if (!dontCountColonists || AnimalApparelUtility.IsAnimal(pawn))
								{
									pawnsCount++;
									if (pawn.thingIDNumber < forPawn.thingIDNumber || (flag2 && flag12))
									{
										pawnsWithLowerIdCount++;
									}
								}
							}
							IL_Jump:
							i++;
							continue;
						}
					}
				}
			}
		}

		[HarmonyPatch(typeof(Pawn_DrawTracker))]
		[HarmonyPatch("DrawPos", MethodType.Getter)]
		public static class Pawn_DrawTracker_GiddyUp_Patch
		{
			[HarmonyPostfix]
			public static void DrawPos_Done(Pawn_DrawTracker __instance, ref Vector3 __result)
			{
				try
				{
					bool flag = __instance != null;
					if (flag)
					{
						Pawn pawn = null;
						try
						{
							pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
						}
						catch
						{
						}
						if (pawn != null && AnimalApparelUtility.IsAnimal(pawn))
						{
							string text = "";
							string text2 = "";
							try
							{
								MethodBase method = new StackFrame(2).GetMethod();
								text = method.Name;
								text2 = method.Module.Assembly.FullName;
							}
							catch
							{
							}
							bool flag3 = text.ToLower() == "prefix" && text2.ToLower().StartsWith("giddyupcore");
							if (flag3)
							{
								__result.y += 0.046875f;
							}
						}
					}
				}
				catch (Exception ex)
				{
					bool devMode = Prefs.DevMode;
					if (devMode)
					{
						Log.Error("DrawPos: error: " + ex.Message, false);
					}
				}
			}
		}

		[HarmonyPatch(typeof(JobGiver_OptimizeApparel), "TryGiveJob")]
		public static class JobGiver_OptimizeApparel_TryGiveJob_Patch
		{
			[HarmonyPrefix]
			[HarmonyBefore(new string[]
			{
				"com.outfitter.rimworld.mod",
				"outfitter"
			})]
			[HarmonyPriority(600)]
			public static bool TryGiveJob_Pre(JobGiver_OptimizeApparel __instance, Pawn pawn, ref Job __result)
			{
				try
				{
					bool flag = pawn != null && AnimalApparelUtility.IsAnimal(pawn);
					if (flag)
					{
						__result = TryGiveJobPREVENTOutfitterMod(__instance, pawn);
						return false;
					}
				}
				catch (Exception ex)
				{
					bool devMode = Prefs.DevMode;
					if (devMode)
					{
						Log.Error("JobGiver_OptimizeApparel_TryGiveJob_Patch: error: " + ex.Message, false);
					}
				}
				return true;
			}

			private static Job TryGiveJobPREVENTOutfitterMod(JobGiver_OptimizeApparel __instance, Pawn pawn)
			{
				StringBuilder stringBuilder = new StringBuilder();
				bool flag = pawn.outfits == null;
				Job result;
				if (flag)
				{
					Log.ErrorOnce(pawn.ToString() + " tried to run JobGiver_OptimizeApparel without an OutfitTracker", 5643897, false);
					result = null;
				}
				else
				{
					bool flag2 = pawn.Faction != Faction.OfPlayer;
					if (flag2)
					{
						Log.ErrorOnce("Non-colonist " + pawn + " tried to optimize apparel.", 764323, false);
						result = null;
					}
					else
					{
						bool flag3 = pawn.IsQuestLodger();
						if (flag3)
						{
							result = null;
						}
						else
						{
							bool flag4 = !DebugViewSettings.debugApparelOptimize;
							if (flag4)
							{
								bool flag5 = Find.TickManager.TicksGame < pawn.mindState.nextApparelOptimizeTick;
								if (flag5)
								{
									return null;
								}
							}
							else
							{
								stringBuilder = new StringBuilder();
								stringBuilder.AppendLine(string.Concat(new object[]
								{
									"Scanning for ",
									pawn,
									" at ",
									pawn.Position
								}));
							}
							Outfit currentOutfit = pawn.outfits.CurrentOutfit;
							List<Apparel> wornApparel = pawn.apparel.WornApparel;
							for (int i = wornApparel.Count - 1; i >= 0; i--)
							{
								bool flag6 = !currentOutfit.filter.Allows(wornApparel[i]) && pawn.outfits.forcedHandler.AllowedToAutomaticallyDrop(wornApparel[i]) && !pawn.apparel.IsLocked(wornApparel[i]);
								if (flag6)
								{
									Job job = JobMaker.MakeJob(JobDefOf.RemoveApparel, wornApparel[i]);
									job.haulDroppedApparel = true;
									return job;
								}
							}
							Thing thing = null;
							float num = 0f;
							List<Thing> list = pawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.Apparel);
							bool flag7 = list.Count == 0;
							if (flag7)
							{
								pawn.mindState.nextApparelOptimizeTick = Find.TickManager.TicksGame + UnityEngine.Random.Range(3000, 6000);
								result = null;
							}
							else
							{
								NeededWarmth neededWarmth = PawnApparelGenerator.CalculateNeededWarmth(pawn, pawn.Map.Tile, GenLocalDate.Twelfth(pawn));
								for (int j = 0; j < list.Count; j++)
								{
									Apparel apparel = (Apparel)list[j];
									bool flag8 = currentOutfit.filter.Allows(apparel) && apparel.IsInAnyStorage() && !apparel.IsForbidden(pawn) && !apparel.IsBurning() && (apparel.def.apparel.gender == Gender.None || apparel.def.apparel.gender == pawn.gender) && !apparel.def.apparel.tags.Contains("Royal");
									if (flag8)
									{
										float num2 = JobGiver_OptimizeApparel.ApparelScoreGain(pawn, apparel);
										bool debugApparelOptimize = DebugViewSettings.debugApparelOptimize;
										if (debugApparelOptimize)
										{
											stringBuilder.AppendLine(apparel.LabelCap + ": " + num2.ToString("F2"));
										}
										bool flag9 = (double)num2 >= 0.0500000007450581 && (double)num2 >= (double)num && ApparelUtility.HasPartsToWear(pawn, apparel.def) && pawn.CanReserveAndReach(apparel, PathEndMode.OnCell, pawn.NormalMaxDanger(), 1, -1, null, false);
										if (flag9)
										{
											thing = apparel;
											num = num2;
										}
									}
								}
								bool debugApparelOptimize2 = DebugViewSettings.debugApparelOptimize;
								if (debugApparelOptimize2)
								{
									stringBuilder.AppendLine("BEST: " + thing);
									Log.Message(stringBuilder.ToString(), false);
								}
								bool flag10 = thing != null;
								if (flag10)
								{
									result = JobMaker.MakeJob(JobDefOf.Wear, thing);
								}
								else
								{
									pawn.mindState.nextApparelOptimizeTick = Find.TickManager.TicksGame + UnityEngine.Random.Range(2500, 5000);
									result = null;
								}
							}
						}
					}
				}
				return result;
			}
		}
	}
}
