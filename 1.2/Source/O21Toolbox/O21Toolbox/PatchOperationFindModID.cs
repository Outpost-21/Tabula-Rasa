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
    public class PatchOperationFindModID : PatchOperation
	{
#pragma warning disable 649
		private List<string> mods;

		private PatchOperation match;

		private PatchOperation nomatch;
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
