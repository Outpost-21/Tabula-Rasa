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
    public class PatchOperation_ReplaceThingCount : PatchOperationPathed
	{
        public string replacement;

        public override bool ApplyWorker(XmlDocument xml)
        {
            XmlNode[] array = xml.SelectNodes(xpath).Cast<XmlNode>().ToArray();
            foreach (XmlNode xmlNode in array)
            {
                // Create new node for new cost and set to the proper value.
                XmlNode materialXmlNode = xmlNode.OwnerDocument.CreateElement(replacement);
                materialXmlNode.InnerXml = xmlNode.InnerXml;
                materialXmlNode.InnerText = xmlNode.InnerText;
                xmlNode.ParentNode.InsertBefore(materialXmlNode, xmlNode);

                // Remove original node
                xmlNode.ParentNode.RemoveChild(xmlNode);
            }

            return true;
        }
    }
}
