using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using UnityEngine;
using RimWorld;
using Verse;

using O21Toolbox.Utility;

namespace O21Toolbox
{
    public class PatchOperation_FindModID : PatchOperation
	{
#pragma warning disable 649
		public List<string> mods;

        public PatchOperation match;

        public PatchOperation nomatch;
#pragma warning restore 649

		public override bool ApplyWorker(XmlDocument xml)
		{
			bool flag = false;
            if (ModLister.AnyFromListActive(mods))
            {
				flag = true;
            }

			if (flag)
			{
				if (match != null)
				{
					return match.Apply(xml);
				}
			}
			else if (nomatch != null)
			{
				return nomatch.Apply(xml);
			}
			return true;
		}
	}
}
