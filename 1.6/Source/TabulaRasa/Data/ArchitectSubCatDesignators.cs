using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace TabulaRasa
{
    public class ArchitectSubCatDesignators
    {
        public string category;
        public List<Designator> designators;

        public bool Visible => category != SubcategoryUtil.orderCat && designators.Any(d => d.Visible);
    }
}
