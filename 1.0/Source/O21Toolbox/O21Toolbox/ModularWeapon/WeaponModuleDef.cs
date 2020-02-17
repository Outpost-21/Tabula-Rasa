using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.ModularWeapon
{
    public class WeaponModuleDef : Def
    {
        /// <summary>
        /// Slot the module will fill. Only one module per slot. 
        /// </summary>
        public WeaponSlot weaponSlot;

        /// <summary>
        /// Stats the module will change.
        /// </summary>
        public ModuleStats moduleStats = null;

        /// <summary>
        /// If not null, this will list modules required to insert this one.
        /// </summary>
        public List<WeaponModuleDef> requiredModules = null;

        /// <summary>
        /// If not null, this will whitelist compatible modules.
        /// </summary>
        public List<WeaponModuleDef> combinationWhitelist = null;

        /// <summary>
        /// If not null, this will blacklist incompatible modules.
        /// </summary>
        public List<WeaponModuleDef> combinationBlacklist = null;
    }
    public class ModuleStats
    {
        /// <summary>
        /// If true, this mass is added to the weapons base amount.
        /// </summary>
        public bool affectMass = false;
        /// <summary>
        /// If true, this mass will override the initial mass. Only works if the module is in the body slot.
        /// </summary>
        public bool overrideMass = false;
        public float mass = 0.0f;

        /// <summary>
        /// If true, the accuracy stats will be used in the calculation of an average.
        /// </summary>
        public bool affectAccuracy = false;
        public float accuracyTouch = -1f;
        public float accuracyShort = -1f;
        public float accuracyMedium = -1f;
        public float accuracyLong = -1f;

        /// <summary>
        /// If true, the cooldown stat will be used in the calculation of an average.
        /// </summary>
        public bool affectCooldown = false;
        public float rangedCooldown = -1f;

        /// <summary>
        /// If true, adds any listed verbs.
        /// </summary>
        public bool addVerbs = false;
        /// <summary>
        /// If true, overrides all original verbs with the listed ones.
        /// </summary>
        public bool overrideVerbs = false;
        public List<Verb> verbs = null;

        /// <summary>
        /// If true, adds any listed tools.
        /// </summary>
        public bool addTools = false;
        /// <summary>
        /// If true, overrides all original verbs with the listed ones.
        /// </summary>
        public bool overrideTools = false;
        public List<Tool> tools = null;
    }
}
