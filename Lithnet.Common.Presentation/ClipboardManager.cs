using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Lithnet.Common.ObjectModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.IO;
using System.Windows;
using System.Xml;

namespace Lithnet.Common.Presentation
{
    public static class ClipboardManager
    {
        public static void CopyToClipboard(object model, string identifier)
        {
            DataContractSerializer s = new DataContractSerializer(model.GetType());
            StringBuilder output = new StringBuilder();
            XmlWriterSettings writerSettings = new XmlWriterSettings();
            writerSettings.Indent = true;
            writerSettings.IndentChars = "  ";
            writerSettings.NewLineChars = Environment.NewLine;
            writerSettings.NamespaceHandling = NamespaceHandling.OmitDuplicates;

            XmlWriter ms = XmlWriter.Create(output, writerSettings);

            s.WriteObject(ms, model);
            ms.Close();

            IDataObject ido = new DataObject();
            ido.SetData(model.GetType().FullName, output.ToString());

            if (identifier != null)
            {
                ido.SetData("Lithnet.Identifier", identifier);
            }
            Clipboard.SetDataObject(ido, true);
        }

        public static string GetObjectIdentifierFromClipboard()
        {
            IDataObject ido = Clipboard.GetDataObject();

            if (ido.GetDataPresent("Lithnet.Identifier"))
            {
                return ido.GetData("Lithnet.Identifier") as string;
            }
            else
            {
                return null;
            }
        }

        public static object GetObjectFromClipBoard(IList<Type> allowedTypes)
        {
            IDataObject ido = Clipboard.GetDataObject();
            string[] formats = ido.GetFormats();

            Type t = allowedTypes.First(u => formats.Contains(u.FullName));

            if (ido.GetDataPresent(t.FullName))
            {
                DataContractSerializer s = new DataContractSerializer(t);
                string xml = (string)ido.GetData(t.FullName);
                TextReader r = new StringReader(xml);
                XmlReader sr = XmlReader.Create(r);
                return s.ReadObject(sr);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        //private bool IsModelSerializable()
        //{
        //   // Type t = this.model.GetType();
        //   // return Attribute.IsDefined(t, typeof(DataContractAttribute)) || (this.model is IXmlSerializable);
        //}
    }
}
