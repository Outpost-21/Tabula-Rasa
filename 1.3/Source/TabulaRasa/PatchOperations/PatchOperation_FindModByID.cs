using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using System.Xml;

namespace TabulaRasa
{
    public class PatchOperation_FindModByID : PatchOperation
	{
		public List<string> mods;

		public PatchOperation match;

		public PatchOperation nomatch;

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
