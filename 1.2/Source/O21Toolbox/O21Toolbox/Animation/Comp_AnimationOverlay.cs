using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Animation
{
    public class Comp_AnimationOverlay : ThingComp
    {
        public CompProperties_AnimationOverlay Props => (CompProperties_AnimationOverlay)props;

        private int timer = 0;
        private Graphic mainTex;

        public CompPowerTrader powerComp;
        public CompRefuelable fuelComp;

        public float HealthPercent => (float)(parent.HitPoints / parent.MaxHitPoints);

        public bool IsPowered
        {
            get
            {
                if (Props.idleWithFuel)
                {
                    if(fuelComp == null || fuelComp.HasFuel)
                    {
                        return true;
                    }
                    return false;
                }
                else
                {
                    if(powerComp == null || powerComp.PowerOn)
                    {
                        return true;
                    }
                    return false;
                }
            }
        }

        public Graphic[] CurGraphics
        {
            get
            {
                if(CurSettings != null)
                {
                    return CurSettings.graphics;
                }
                return null;
            }
        }

        public AnimationOverlaySetting CurSettings => Props.settings.Where(s => s.type == CurStatus).FirstOrDefault();

        public AnimationStatus CurStatus
        {
            get
            {
                if (HealthPercent < Props.damageThreshold && Props.settings.Where(s => s.type == AnimationStatus.damaged) != null)
                {
                    return AnimationStatus.damaged;
                }
                else if (PawnIsUsing && Props.settings.Where(s => s.type == AnimationStatus.inUse) != null)
                {
                    return AnimationStatus.inUse;
                }
                else if (IsPowered && Props.settings.Where(s => s.type == AnimationStatus.idle) != null)
                {
                    return AnimationStatus.idle;
                }
                else
                {
                    return AnimationStatus.inactive;
                }
            }
        }

        public bool PawnIsUsing
        {
            get
            {
                Pawn pawn = parent.Map.thingGrid.ThingAt<Pawn>(parent.InteractionCell);
                if(pawn != null)
                {
                    if(pawn.CurJob.targetA != null && pawn.CurJob.targetA == parent)
                    {
                        return true;
                    }
                    else if (pawn.CurJob.targetB != null && pawn.CurJob.targetB == parent)
                    {
                        return true;
                    }
                    else if (pawn.CurJob.targetC != null && pawn.CurJob.targetC == parent)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            LongEventHandler.ExecuteWhenFinished(GraphicLoader);
            powerComp = parent.TryGetComp<CompPowerTrader>();
            fuelComp = parent.TryGetComp<CompRefuelable>();
        }

        public void GraphicLoader()
        {
            for (int i = 0; i < Props.settings.Count; i++)
            {
                Props.settings[i].graphics = new Graphic_Single[Props.settings[i].frameCount];
                for (int j = 0; j < Props.settings[i].frameCount; j++)
                {
                    try
                    {
                        Props.settings[i].graphics[j] = GraphicDatabase.Get<Graphic_Single>(Props.settings[i].graphicPath + "_" + j, ShaderDatabase.Transparent);
                        Props.settings[i].graphics[j].drawSize = parent.def.graphicData.drawSize;
                    }
                    catch
                    {
                        Log.Error("Could not load texture: " + Props.settings[i].graphicPath + "_" + j);
                    }
                }
            }
        }

        public override void CompTick()
        {
            base.CompTick();

            HandleAnimation();
            if (CurGraphics != null && timer >= CurGraphics.Count() * CurSettings.fps)
            {
                timer = 0;
            }
        }

        private void HandleAnimation()
        {
            if(CurGraphics != null && timer < (CurGraphics.Count() * CurSettings.fps))
            {
                int i = timer / CurSettings.fps;

                mainTex = CurGraphics[i];
            }
            else
            {
                mainTex = null;
            }
            timer += 1;
        }

        public override void PostDraw()
        {
            base.PostDraw();

            if(mainTex != null)
            {
                MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
                propertyBlock.SetColor(ShaderPropertyIDs.Color, parent.def.graphicData.color);
                propertyBlock.SetColor(ShaderPropertyIDs.ColorTwo, parent.def.graphicData.colorTwo);
                Matrix4x4 matrix = default(Matrix4x4);
                matrix.SetTRS(parent.DrawPos + new Vector3(0, 1, 0), Quaternion.AngleAxis(0, Vector3.up), new Vector3(parent.def.graphicData.drawSize.x, 1, parent.def.graphicData.drawSize.y));
                Graphics.DrawMesh(MeshPool.plane10, matrix, mainTex.MatSingle, 0, null, 0, propertyBlock);
            }
        }
    }
}
