using System.Collections.Generic;
using System.Xml;

namespace UtilityLibrary.IO
{
    public static class XmlMethods
    {
        //Possible TODO - Make sure this can take List attributes too
        public static void WriteStringList(XmlWriter aXmlWriter, string aElementListTag, string aElementsTag, List<string> aElements, List<(string AttributeTag, string AttributeContent)[]> aAttributes = null)
        {
            //write our animation lists
            aXmlWriter.WriteStartElement(aElementListTag);
            for(int _index = 0; _index < aElements.Count; _index++)
            {
                aXmlWriter.WriteStartElement(aElementsTag);

                //Write each attribute corresponding to the element
                if(aAttributes != null)
                {
                    WriteAttributes(aXmlWriter, aAttributes[_index]);
                }

                aXmlWriter.WriteString(aElements[_index]);
                aXmlWriter.WriteEndElement();
            }
            aXmlWriter.WriteEndElement();
        }

        public static void WriteStringElement(XmlWriter aXmlWriter, string aElementTag, string aElement, params (string AttributeTag, string AttributeContent)[] aAttributes)
        {
            //Write our name and tag
            aXmlWriter.WriteStartElement(aElementTag);

            //Write all the passed along attributes
            WriteAttributes(aXmlWriter, aAttributes);

            aXmlWriter.WriteString(aElement);
            aXmlWriter.WriteEndElement();
        }


        public static void WriteDocumentEnd(XmlWriter aXmlWriter)
        {
            //End the root node
            aXmlWriter.WriteEndElement();

            //End the document itself
            aXmlWriter.WriteEndDocument();
        }

        public static void WriteAttributes(XmlWriter aXmlWriter, (string AttributeTag, string AttributeContent)[] aAttributeArray)
        {
            foreach(var _attribute in aAttributeArray)
            {
                //We can't have an empty tag but we can have empty optional content
                if(!string.IsNullOrWhiteSpace(_attribute.AttributeTag) && _attribute.AttributeContent != null)
                {
                    aXmlWriter.WriteAttributeString(_attribute.AttributeTag, _attribute.AttributeContent);
                }
            }
        }
    }
}
