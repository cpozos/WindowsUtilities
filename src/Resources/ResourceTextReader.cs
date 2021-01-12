
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Resources;
using System.Xml;
using System.Xml.Linq;

namespace Resources
{
   public class ResourceTextReader : IResourceReader
   {
      private FileStream _stream;
      private string _fileName;
      private Dictionary<string, string> _resData;

      public ResourceTextReader(string fileName)
      {
         _fileName = fileName;
         _resData = new Dictionary<string, string>();
      }

      #region Dispose and Close
      ~ResourceTextReader() => Dispose(false);

      public void Close() => Dispose();

      public void Dispose()
      {
         Dispose(true);
         GC.SuppressFinalize(this);
      }

      private void Dispose(bool disposing)
      {
         if (disposing)
         {
            if (_stream != null)
            {
               _stream.Dispose();
               _stream = null;
            }
         }

         _resData = null;
         _fileName = null;
      }

      #endregion

      private void EnsureData()
      {
         using (_stream = new FileStream(_fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
         {
            using (var contentReader = new XmlTextReader(_stream))
            {
               SetupNameTable(contentReader);
               contentReader.WhitespaceHandling = WhitespaceHandling.None;
               ParseXml(contentReader);
            }
         }
      }
      private void SetupNameTable(XmlReader reader)
      {
         reader.NameTable.Add(ResourceConstants.TypeStr);
         reader.NameTable.Add(ResourceConstants.NameStr);
         reader.NameTable.Add(ResourceConstants.DataStr);
         reader.NameTable.Add(ResourceConstants.MetadataStr);
         reader.NameTable.Add(ResourceConstants.MimeTypeStr);
         reader.NameTable.Add(ResourceConstants.ValueStr);
         reader.NameTable.Add(ResourceConstants.ResHeaderStr);
         reader.NameTable.Add(ResourceConstants.VersionStr);
         reader.NameTable.Add(ResourceConstants.ResMimeTypeStr);
         reader.NameTable.Add(ResourceConstants.ReaderStr);
         reader.NameTable.Add(ResourceConstants.WriterStr);
         reader.NameTable.Add(ResourceConstants.BinSerializedObjectMimeType);
         reader.NameTable.Add(ResourceConstants.SoapSerializedObjectMimeType);
         reader.NameTable.Add(ResourceConstants.AssemblyStr);
         reader.NameTable.Add(ResourceConstants.AliasStr);
      }
      private void ParseXml(XmlTextReader reader)
      {
         XDocument doc = XDocument.Load(reader);

         var data = doc.Descendants("root")
            .Descendants("data")
            .Where(e =>
            {
               var attXmlSpace = e.Attribute(XNamespace.Xml.GetName("space"));
               var attName = e.Attribute("name");
               return
                  e.Attributes().Count() == 2
                  && attXmlSpace != null && attName != null
                  && attXmlSpace.Value == "preserve"
                  && !string.IsNullOrWhiteSpace(attName.Value);
            })
            .Select(e => new KeyValuePair<string, string>(
               e.Attribute("name").Value,
               e.Value
            ));


         if (data?.Count() > 0)
         {
            foreach (var item in data)
            {
               if(_resData.TryGetValue(item.Key, out string value))
               {
                  throw new Exception($"The resource {item.Key} is repeated");
               }

               _resData.Add(item.Key, item.Value);
            }
         }
      }

      public IDictionaryEnumerator GetEnumerator()
      {
         EnsureData();
         return _resData.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
   }
}
