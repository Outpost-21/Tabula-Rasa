using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Networks
{
    public class Comp_CustomNetwork_Storage : Comp_CustomNetwork
    {
        public new CompProperties_CustomNetwork_Storage Props => (CompProperties_CustomNetwork_Storage)this.props;

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
