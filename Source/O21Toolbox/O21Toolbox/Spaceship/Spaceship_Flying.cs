using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Spaceship
{
    [StaticConstructorOnStartup]
    public abstract class Spaceship_Flying : Thing
    {
        // The SpaceshipDef where all modder defined info is pulled from.
        public SpaceshipDef thisSpaceshipDef;
        public SpaceshipKind spaceshipKind;

        public Vector3 spaceshipExactPosition = Vector3.zero;
        public Vector3 spaceshipShadowExactPosition = Vector3.zero;
        public float spaceshipExactRotation = 0f;

        // Draw.
        public Material spaceshipTexture = null;
        public Material spaceshipShadowTexture = null;
        public Matrix4x4 spaceshipMatrix = default(Matrix4x4);
        public Matrix4x4 spaceshipShadowMatrix = default(Matrix4x4);
        public Vector3 baseSpaceshipScale = new Vector3(1f, 1f, 1f);
        public Vector3 spaceshipScale = new Vector3(11f, 1f, 20f);
        public Vector3 spaceshipShadowScale = new Vector3(11f, 1f, 20f);

        public override Vector3 DrawPos
        {
            get
            {
                return this.spaceshipExactPosition;
            }
        }

        public Vector3 ShadowDrawPos
        {
            get
            {
                return this.spaceshipShadowExactPosition;
            }
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (respawningAfterLoad)
            {
                ConfigureSpaceshipTextures(this.thisSpaceshipDef);
            }
        }

        public void ConfigureSpaceshipTextures(SpaceshipDef def)
        {
            this.spaceshipTexture = MaterialPool.MatFrom(def.graphicPaths.flyingTexPath);
            this.spaceshipShadowTexture = MaterialPool.MatFrom(def.graphicPaths.shadowTexPath, ShaderDatabase.Transparent);
            this.baseSpaceshipScale = def.graphicPaths.drawSize;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<Vector3>(ref this.spaceshipExactPosition, "spaceshipExactPosition");
            Scribe_Values.Look<Vector3>(ref this.spaceshipShadowExactPosition, "spaceshipShadowExactPosition");
            Scribe_Values.Look<float>(ref this.spaceshipExactRotation, "spaceshipExactRotation");
            Scribe_Values.Look<SpaceshipKind>(ref this.spaceshipKind, "spaceshipKind");
            Scribe_Values.Look<Vector3>(ref this.spaceshipScale, "spaceshipScale");
            Scribe_Values.Look<Vector3>(ref this.spaceshipShadowScale, "spaceshipShadowScale");
        }

        public override void Tick()
        {
            ComputeShipExactPosition();
            ComputeShipShadowExactPosition();
            ComputeShipExactRotation();
            ComputeShipScale();
            SetShipVisibleAboveFog();
        }

        public abstract void ComputeShipExactPosition();
        public abstract void ComputeShipShadowExactPosition();
        public abstract void ComputeShipExactRotation();
        public abstract void ComputeShipScale();
        public abstract void SetShipVisibleAboveFog();

        public bool IsInBoundsAndVisible()
        {
            bool isInBoundsAndVisible = this.DrawPos.ToIntVec3().InBounds(this.Map)
                && (this.Map.fogGrid.IsFogged(this.DrawPos.ToIntVec3()) == false)
                && this.DrawPos.ToIntVec3().x >= 10 && this.DrawPos.ToIntVec3().x < this.Map.Size.x - 10
                && this.DrawPos.ToIntVec3().z >= 10 && this.DrawPos.ToIntVec3().z < this.Map.Size.z - 10;
            return isInBoundsAndVisible;
        }

        // ===================== Draw =====================
        public override void Draw()
        {
            this.spaceshipMatrix.SetTRS(this.DrawPos + Altitudes.AltIncVect, this.spaceshipExactRotation.ToQuat(), this.spaceshipScale);
            Graphics.DrawMesh(MeshPool.plane10, this.spaceshipMatrix, this.spaceshipTexture, 0);
            this.spaceshipShadowMatrix.SetTRS(this.ShadowDrawPos + Altitudes.AltIncVect, this.spaceshipExactRotation.ToQuat(), this.spaceshipShadowScale);
            Graphics.DrawMesh(MeshPool.plane10, this.spaceshipShadowMatrix, FadedMaterialPool.FadedVersionOf(this.spaceshipShadowTexture, 0.4f * GenCelestial.CurShadowStrength(this.Map)), 0);
        }
    }
}
