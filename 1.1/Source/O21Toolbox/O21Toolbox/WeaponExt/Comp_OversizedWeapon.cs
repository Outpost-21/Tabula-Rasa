using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.WeaponExt
{
    public class Comp_OversizedWeapon : ThingComp
    {
        public CompProperties_OversizedWeapon Props => props as CompProperties_OversizedWeapon;

        public Comp_OversizedWeapon()
        {
            if (!(props is CompProperties_OversizedWeapon))
                props = new CompProperties_OversizedWeapon();
        }


        public CompEquippable GetEquippable => parent?.GetComp<CompEquippable>();

        public Pawn GetPawn => GetEquippable?.verbTracker?.PrimaryVerb?.CasterPawn;

        private bool isEquipped = false;
        public bool IsEquipped
        {
            get
            {
                if (Find.TickManager.TicksGame % 60 != 0) return isEquipped;
                isEquipped = GetPawn != null;
                return isEquipped;
            }
        }

        private bool firstAttack = false;
        public bool FirstAttack
        {
            get => firstAttack;
            set => firstAttack = value;
        }
    }
}
