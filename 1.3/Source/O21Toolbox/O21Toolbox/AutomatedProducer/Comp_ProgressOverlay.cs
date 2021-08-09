using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.AutomatedProducer
{
    public class Comp_ProgressOverlay : ThingComp
    {
		public CompProperties_ProgressOverlay Props => (CompProperties_ProgressOverlay)props;

        public Comp_AutomatedProducer prodComp;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

			prodComp = parent.TryGetComp<Comp_AutomatedProducer>();
        }

        public override void PostDraw()
        {
            base.PostDraw();

			if(prodComp != null)
			{
				if (prodComp.currentRecipe != null)
				{
					string recipeTexPath = GetRecipeTexture(prodComp.currentRecipe);
					if (!recipeTexPath.NullOrEmpty())
					{
						Rot4 rotation = parent.Rotation;
						if(parent.def.graphicData.graphicClass == typeof(Graphic_Multi))
                        {
							rotation = Rot4.North;
                        }
						Vector3 s = new Vector3(parent.def.graphicData.drawSize.x, 1f, parent.def.graphicData.drawSize.y);
						Vector3 drawPos = this.parent.DrawPos;
						drawPos.y += 0.085f;
						Matrix4x4 matrix = default(Matrix4x4);
						matrix.SetTRS(drawPos, rotation.AsQuat, s);
						Graphics.DrawMesh(MeshPool.plane10, matrix, MaterialPool.MatFrom(recipeTexPath, parent.def.graphicData.shaderType.Shader), 0);
					}
				}
			}
		}

		public string GetRecipeTexture(RecipeDef_Automated recipe)
        {
			string result = "";
			RecipeState state = Props.recipeStates.Find(rs => rs.recipeDef == recipe.defName);
			if(state != null)
			{
				IEnumerable<ProgressState> progStates = from s in state.states
												 where s.progress < prodComp.WorkProgress
												 select s;

                if (!progStates.EnumerableNullOrEmpty()) 
				{
					result = progStates.Last().texPath;

					if (parent.def.graphicData.graphicClass == typeof(Graphic_Multi))
					{
						result += ("_" + parent.Rotation.ToStringWord().ToLower());
					}
				}
			}
			return result;
        }
    }
}
