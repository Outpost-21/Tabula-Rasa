using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace TabulaRasa
{
    public class WorldDrawLayer_UngeneratedPlanetPartsAsDefaultBiome : WorldDrawLayer
    {
        public override IEnumerable Regenerate()
        {
            foreach (object obj in base.Regenerate())
            {
                yield return obj;
            }
            Vector3 viewCenter = planetLayer.ViewCenter;
            float viewAngle = planetLayer.ViewAngle;
            if (viewAngle < 180f)
            {
                List<Vector3> collection;
                List<int> collection2;
                SphereGenerator.Generate(4, planetLayer.Radius + -0.16f, -viewCenter, 180f - Mathf.Min(viewAngle, 180f) + 10f, out collection, out collection2);
                LayerSubMesh subMesh = GetSubMesh(planetLayer.Def.DefaultBiome.DrawMaterial);
                subMesh.verts.AddRange(collection);
                subMesh.tris.AddRange(collection2);
            }
            FinalizeMesh(MeshParts.All);
        }
    }
}
