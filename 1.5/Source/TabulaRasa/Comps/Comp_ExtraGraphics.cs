using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace TabulaRasa
{
    public class Comp_ExtraGraphics : ThingComp
	{
		public CompProperties_ExtraGraphics Props => (CompProperties_ExtraGraphics)props;

		public Thing thingToGrab;

		public Graphic_Multi newGraphic;

		public Graphic_Single newGraphicSingle;

		public string newGraphicPath = "";

		public string newGraphicSinglePath = "";

		public override void PostExposeData()
		{
			Scribe_Values.Look(ref newGraphicPath, "newGraphicPath");
			Scribe_Values.Look(ref newGraphicSinglePath, "newGraphicSinglePath");
		}

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			thingToGrab = this.parent;
			LongEventHandler.ExecuteWhenFinished(new Action(delegate () { ChangeGraphic(respawningAfterLoad); }));
		}

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			if (parent.Faction != null && parent.Faction.IsPlayer && parent.StyleDef == null)
			{
				yield return new Command_Action
				{
					defaultLabel = "O21_ChangeGraphic".Translate(),
					defaultDesc = "O21_ChangeGraphicDesc".Translate(),
					icon = ContentFinder<Texture2D>.Get("Toolbox/UI/Cycle", true),
					action = delegate { SelectableGraphicListing(); }
				};
			}
			yield break;
		}

		public void SelectableGraphicListing()
		{
			List<FloatMenuOption> floatList = new List<FloatMenuOption>();
			int floatIndex = 0;
			foreach (ExtraGraphicDetails egd in Props.extraGraphics)
			{
				string text;
				if (egd.label.NullOrEmpty())
				{
					text = "Option (" + floatIndex.ToString() + ")";
					floatIndex++;
				}
				else
				{
					text = egd.label;
				}
				floatList.Add(new FloatMenuOption(text, delegate ()
				{
					SetGraphic(egd);
					parent.Map.mapDrawer.MapMeshDirty(parent.Position, (ulong)MapMeshFlagDefOf.Things | (ulong)MapMeshFlagDefOf.Buildings);
				}, MenuOptionPriority.Default, null, null, 29f, null, null));
			}
			Find.WindowStack.Add(new FloatMenu(floatList));
		}

		public void SetGraphic(ExtraGraphicDetails egd)
		{
			try
			{
				Vector2 drawSize = parent.Graphic.drawSize;
				Color color = parent.Graphic.color;
				ShaderTypeDef shaderType = parent.def.graphicData.shaderType;
				if (parent.def.graphicData.graphicClass == typeof(Graphic_Multi))
				{
					newGraphicPath = egd.path;
					newGraphic = (Graphic_Multi)GraphicDatabase.Get<Graphic_Multi>(newGraphicPath, shaderType.Shader, drawSize, color);

					Type typeFromHandle = typeof(Thing);
					FieldInfo field = typeFromHandle.GetField("graphicInt", BindingFlags.Instance | BindingFlags.NonPublic);
					field.SetValue(thingToGrab, newGraphic);
				}
				else
				{
					newGraphicSinglePath = egd.path;
					newGraphicSingle = (Graphic_Single)GraphicDatabase.Get<Graphic_Single>(newGraphicSinglePath, shaderType.Shader, drawSize, color);

					Type typeFromHandle2 = typeof(Thing);
					FieldInfo field2 = typeFromHandle2.GetField("graphicInt", BindingFlags.Instance | BindingFlags.NonPublic);
					field2.SetValue(thingToGrab, newGraphicSingle);
				}
			}
			catch
			{
				Log.Warning("Caught an exeption changing graphic. Most likely a misconfigured def.");
			}
		}

		public void ChangeGraphic(bool reloading = false)
		{
			try
			{
				Vector2 drawSize = parent.Graphic.drawSize;
				Color color = parent.Graphic.color;
				ShaderTypeDef shaderType = parent.def.graphicData.shaderType;
				if (parent.Faction != null && parent.Faction.IsPlayer)
				{
					if (parent.def.graphicData.graphicClass == typeof(Graphic_Multi))
					{
						if (!reloading)
						{
							int num = Props.extraGraphics.FindIndex(egi => egi.path == newGraphicPath);
							if (num + 1 > Props.extraGraphics.Count - 1)
							{
								num = 0;
							}
							else
							{
								num++;
							}
							newGraphicPath = Props.extraGraphics[num].path;
							newGraphic = (Graphic_Multi)GraphicDatabase.Get<Graphic_Multi>(newGraphicPath, shaderType.Shader, drawSize, color);
						}
						else
						{
							if (newGraphicPath == "")
							{
								newGraphicPath = Props.extraGraphics[0].path;
								newGraphic = (Graphic_Multi)GraphicDatabase.Get<Graphic_Multi>(newGraphicPath, shaderType.Shader, drawSize, color);
							}
							else
							{
								newGraphic = (Graphic_Multi)GraphicDatabase.Get<Graphic_Multi>(newGraphicPath, shaderType.Shader, drawSize, color);
							}
							reloading = false;
						}
						Type typeFromHandle = typeof(Thing);
						FieldInfo field = typeFromHandle.GetField("graphicInt", BindingFlags.Instance | BindingFlags.NonPublic);
						field.SetValue(thingToGrab, newGraphic);
					}
					else
					{
						if (parent.def.graphicData.graphicClass == typeof(Graphic_Single))
						{
							if (!reloading)
							{
								int num2 = Props.extraGraphics.FindIndex(egi => egi.path == newGraphicPath);
								if (num2 + 1 > Props.extraGraphics.Count - 1)
								{
									num2 = 0;
								}
								else
								{
									num2++;
								}
								newGraphicSinglePath = Props.extraGraphics[num2].path;
								newGraphicSingle = (Graphic_Single)GraphicDatabase.Get<Graphic_Single>(newGraphicSinglePath, shaderType.Shader, drawSize, color);
							}
							else
							{
								if (newGraphicSinglePath == "")
								{
									newGraphicSinglePath = Props.extraGraphics[0].path;
									newGraphicSingle = (Graphic_Single)GraphicDatabase.Get<Graphic_Single>(newGraphicSinglePath, shaderType.Shader, drawSize, color);
								}
								else
								{
									newGraphicSingle = (Graphic_Single)GraphicDatabase.Get<Graphic_Single>(newGraphicSinglePath, shaderType.Shader, drawSize, color);
								}
								reloading = false;
							}
							Type typeFromHandle2 = typeof(Thing);
							FieldInfo field2 = typeFromHandle2.GetField("graphicInt", BindingFlags.Instance | BindingFlags.NonPublic);
							field2.SetValue(thingToGrab, newGraphicSingle);
						}
					}
				}
			}
			catch
			{
				LogUtil.Message("Probably added mid-save. Ignoring load error.");
			}
		}
	}
}
