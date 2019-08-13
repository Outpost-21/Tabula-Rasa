using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Networks
{
    public class Comp_NetworkStorage : Comp_Network
    {
        public new CompProperties_NetworkStorage Props => (CompProperties_NetworkStorage)this.props;

        /** public float AmountCanAccept
        {
            get
            {
                if (this.parent.IsBrokenDown())
                {
                    return 0f;
                }
                CompProperties_NetworkStorage props = this.Props;
                return (props.storedMax - this.storedCurrent);
            }
        } **/
    }
}
