using System;
using System.IO;
using System.Resources;
using System.Xml;

namespace Resources
{
   public class ResourceWriter : IResourceWriter
   {
      private string _fileName;
      private bool _isWriterInitialized;
      private XmlTextWriter _xmlTextWriter;
      private bool _hasBeenSaved;

      private XmlWriter Writer
      {
         get
         {
            if (!_isWriterInitialized)
            {
               InitializeWriter();
            }

            return _xmlTextWriter;
         }
      }

      public ResourceWriter(string fileName) => _fileName = fileName;

      #region Close and Dispose 
      ~ResourceWriter() => Dispose(false);

      private void Dispose(bool disposing)
      {
         if (disposing)
         {
            if (!_hasBeenSaved)
            {
               Generate();
            }

            if (_xmlTextWriter != null)
            {
               _xmlTextWriter.Close();
               _xmlTextWriter = null;
            }
         }
      }


      void IResourceWriter.Close() => (this as IDisposable).Dispose();

      void IDisposable.Dispose()
      {
         Dispose(true);
         GC.SuppressFinalize(this);
      }

      #endregion

      private void InitializeWriter()
      {
         if (_xmlTextWriter is null)
         {
            bool writeHeaderRequired = false;

            _xmlTextWriter = new XmlTextWriter(_fileName, System.Text.Encoding.UTF8);

            _xmlTextWriter.Formatting = Formatting.Indented;
            _xmlTextWriter.Indentation = 2;

            if (!writeHeaderRequired)
            {
               _xmlTextWriter.WriteStartDocument(); // writes <?xml version="1.0" encoding="utf-8"?>
            }
         }
         else
         {
            _xmlTextWriter.WriteStartDocument();
         }

         _xmlTextWriter.WriteStartElement("root");
         XmlTextReader reader = new XmlTextReader(new StringReader(ResourceConstants.ResourceSchema))
         {
            WhitespaceHandling = WhitespaceHandling.None
         };
         _xmlTextWriter.WriteNode(reader, true);

         _xmlTextWriter.WriteStartElement(ResourceConstants.ResHeaderStr);
         {
            _xmlTextWriter.WriteAttributeString(ResourceConstants.NameStr, ResourceConstants.ResMimeTypeStr);
            _xmlTextWriter.WriteStartElement(ResourceConstants.ValueStr);
            {
               _xmlTextWriter.WriteString(ResourceConstants.ResMimeType);
            }
            _xmlTextWriter.WriteEndElement();
         }
         _xmlTextWriter.WriteEndElement();

         _xmlTextWriter.WriteStartElement(ResourceConstants.ResHeaderStr);
         {
            _xmlTextWriter.WriteAttributeString(ResourceConstants.NameStr, ResourceConstants.VersionStr);
            _xmlTextWriter.WriteStartElement(ResourceConstants.ValueStr);
            {
               _xmlTextWriter.WriteString(ResourceConstants.Version);
            }
            _xmlTextWriter.WriteEndElement();
         }
         _xmlTextWriter.WriteEndElement();

         _xmlTextWriter.WriteStartElement(ResourceConstants.ResHeaderStr);
         {
            _xmlTextWriter.WriteAttributeString(ResourceConstants.NameStr, ResourceConstants.ReaderStr);
            _xmlTextWriter.WriteStartElement(ResourceConstants.ValueStr);
            {
               _xmlTextWriter.WriteString("System.Resources.ResXResourceReader, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
            }
            _xmlTextWriter.WriteEndElement();
         }
         _xmlTextWriter.WriteEndElement();

         _xmlTextWriter.WriteStartElement(ResourceConstants.ResHeaderStr);
         {
            _xmlTextWriter.WriteAttributeString(ResourceConstants.NameStr, ResourceConstants.WriterStr);
            _xmlTextWriter.WriteStartElement(ResourceConstants.ValueStr);
            {
               _xmlTextWriter.WriteString("System.Resources.ResXResourceWriter, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
            }
            _xmlTextWriter.WriteEndElement();
         }
         _xmlTextWriter.WriteEndElement();

         _isWriterInitialized = true;
      }

      public void AddResource(string key, string value, string comment)
      {
         Writer.WriteStartElement(ResourceConstants.DataStr);
         {
            Writer.WriteAttributeString(ResourceConstants.NameStr, key);
            Writer.WriteAttributeString("xml", "space", null, "preserve");

            Writer.WriteStartElement(ResourceConstants.ValueStr);
            {
               if (!string.IsNullOrEmpty(value))
               {
                  Writer.WriteString(value);
               }
            }
            Writer.WriteEndElement();

            if (!string.IsNullOrEmpty(comment))
            {
               Writer.WriteStartElement(ResourceConstants.CommentStr);
               {
                  Writer.WriteString(comment);
               }
               Writer.WriteEndElement();
            }
         }
         Writer.WriteEndElement();
      }

      #region IResourceWriter

      public void Generate()
      {
         if (_hasBeenSaved)
         {
            throw new InvalidOperationException();
         }

         Writer.WriteEndElement();
         Writer.Flush();

         _hasBeenSaved = true;
      }

      public void AddResource(string name, string value)
      {
         AddResource(name, value, null);
      }

      public void AddResource(string name, byte[] value)
      {
         throw new NotImplementedException();
      }

      public void AddResource(string name, object value)
      {
         throw new NotImplementedException();
      }

      #endregion
   }
}
