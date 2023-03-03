using RimWorld;
using System.Xml;
using Verse;

namespace HRF
{
    public class NeedAdjustRecord
    {
        public NeedDef need;

        public float adjustValue;

        public void LoadDataFromXmlCustom(XmlNode xmlRoot)
        {
            DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(this, "need", xmlRoot);
            adjustValue = ParseHelper.FromString<float>(xmlRoot.FirstChild.Value);
        }
    }
}