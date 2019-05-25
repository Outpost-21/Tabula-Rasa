using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RimWorld;
using UnityEngine;
using Verse;

namespace O21Toolbox.ModularWeapon
{
    public class ITab_Pawn_ModularWeapon : ITab
    {
        public ITab_Pawn_ModularWeapon()
        {
            this.size = new Vector2(460f, 450f);
            this.labelKey = "TabWeapon";
            this.tutorTag = "Weapon";
        }

        public override bool IsVisible
        {
            get
            {
                Pawn selPawnForWeapon = this.SelPawnForWeapon;
                return this.ShouldShowWeaponModules(selPawnForWeapon);
            }
        }

        private bool CanControl
        {
            get
            {
                Pawn selPawnForWeapon = this.SelPawnForWeapon;
                return !selPawnForWeapon.Downed && !selPawnForWeapon.InMentalState && (selPawnForWeapon.Faction == Faction.OfPlayer || selPawnForWeapon.IsPrisonerOfColony) && (!selPawnForWeapon.IsPrisonerOfColony || !selPawnForWeapon.Spawned || selPawnForWeapon.Map.mapPawns.AnyFreeColonistSpawned) && (!selPawnForWeapon.IsPrisonerOfColony || (!PrisonBreakUtility.IsPrisonBreaking(selPawnForWeapon) && (selPawnForWeapon.CurJob == null || !selPawnForWeapon.CurJob.exitMapOnArrival)));
            }
        }

        private bool CanControlColonist
        {
            get
            {
                return this.CanControl && this.SelPawnForWeapon.IsColonistPlayerControlled;
            }
        }

        private Pawn SelPawnForWeapon
        {
            get
            {
                if (base.SelPawn != null)
                {
                    return base.SelPawn;
                }
                Corpse corpse = base.SelThing as Corpse;
                if (corpse != null)
                {
                    return corpse.InnerPawn;
                }
                throw new InvalidOperationException("Gear tab on non-pawn non-corpse " + base.SelThing);
            }
        }

        protected override void FillTab()
        {
            throw new NotImplementedException();
        }
    }
}
