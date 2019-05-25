using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.ModularWeapon
{
    public class DefModExtension_ModularWeapon : DefModExtension
    {
        /// <summary>
        /// Overlays textures instead of overriding. 
        /// </summary>
        public bool compositeTexture = false;

        public List<WeaponTextureOption> weaponTextureOptions = null;

        public Graphic GetCurrentTexture(Thing eq)
        {
            Graphic result = null;
            WeaponTextureOption match = null;
            if(GetMatchingTextureOption(eq) != null)
            {

            }
            this.result = match.graphicData.GraphicColoredFor(this);
            return result;
        }

        private object GetMatchingTextureOption(Thing eq)
        {
            IEnumerable<WeaponTextureOption> enumerable = from wto in this.weaponTextureOptions
                                                          where wto.weaponModules
                                                          select wto;
            bool flag = enumerable != null;
            if (flag)
            {
                foreach (Hediff hediff in enumerable)
                {
                    HediffWithComps hediffWithComps = hediff as HediffWithComps;
                    bool flag2 = hediffWithComps != null;
                    if (flag2)
                    {
                        HediffComp_TendDuration hediffComp_TendDuration = HediffUtility.TryGetComp<HediffComp_TendDuration>(hediffWithComps);
                        hediffComp_TendDuration.tendQuality = 2f;
                        hediffComp_TendDuration.tendTicksLeft = Find.TickManager.TicksGame;
                        this.pawn.health.Notify_HediffChanged(hediff);
                    }
                }
            }
        }

        /// <summary>
        /// Restricts the weapon to only using whitelisted modules.
        /// </summary>
        public List<WeaponModuleDef> moduleWhitelist;
    }

    public class WeaponTextureOption
    {
        /// <summary>
        /// Texture to swap to when modules match.
        /// </summary>
        public string texPath = null;

        /// <summary>
        /// Only used if compositeTexture = true.
        /// </summary>
        public Vector3 drawSize = new Vector3(1, 1);
        public Vector3 offset = new Vector3(1, 1);

        /// <summary>
        /// Required modules.
        /// </summary>
        public List<WeaponModuleDef> weaponModules = null;

        /// <summary>
        /// If true, the texture will not swap when modules are added that are NOT in the weaponModules list.
        /// This means even if the current modules match, an additional module with no alternative will default
        /// to the original texture.
        /// </summary>
        public bool ignoreUnusedSlots = false;
    }
}
