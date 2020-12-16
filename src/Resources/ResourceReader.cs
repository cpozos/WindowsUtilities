using System;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.Serialization;
using System.Xml;

namespace Resources
{
   public class ResourceReader : IResourceReader
   {
      private string _fileName;
      private bool _isReaderDirty;
      private ListDictionary _resData;
      private FileStream _stream;
      private string _resHeaderVersion;
      private string _resHeaderMimeType;
      private string _resHeaderReaderType;
      private string _resHeaderWriterType;

      public ResourceReader(string fileName) => _fileName = fileName;

      #region Dispose and Close

      public void Close() => Dispose();

      public void Dispose()
      {
         Dispose(true);
         GC.SuppressFinalize(this);
      }

      private void Dispose(bool disposing)
      {
      }

      #endregion

      private void EnsureResData()
      {
         if (_resData is null)
         {
            _resData = new ListDictionary();

            using (_stream = new FileStream(_fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
               using (XmlTextReader contentReader = new XmlTextReader(_stream))
               {
                  SetupNameTable(contentReader);
                  contentReader.WhitespaceHandling = WhitespaceHandling.None;
                  ParseXml(contentReader);
               }
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
         bool success = false;
         try
         {
            try
            {
               while (reader.Read())
               {
                  if (reader.NodeType == XmlNodeType.Element)
                  {
                     string s = reader.LocalName;

                     if (reader.LocalName.Equals(ResourceConstants.AssemblyStr))
                     {
                        ParseAssemblyNode(reader);
                     }
                     else if (reader.LocalName.Equals(ResourceConstants.DataStr))
                     {
                        ParseDataNode(reader, false);
                     }
                     else if (reader.LocalName.Equals(ResourceConstants.ResHeaderStr))
                     {
                        ParseResHeaderNode(reader);
                     }
                     else if (reader.LocalName.Equals(ResourceConstants.MetadataStr))
                     {
                        ParseDataNode(reader, true);
                     }
                  }
               }

               success = true;
            }
            catch (SerializationException se)
            {
               Point pt = GetPosition(reader);
               string newMessage = string.Format(SR.SerializationException, reader[ResourceConstants.TypeStr], pt.Y, pt.X, se.Message);
               XmlException xml = new XmlException(newMessage, se, pt.Y, pt.X);
               SerializationException newSe = new SerializationException(newMessage, xml);

               throw newSe;
            }
            catch (TargetInvocationException tie)
            {
               Point pt = GetPosition(reader);
               string newMessage = string.Format(SR.InvocationException, reader[ResourceConstants.TypeStr], pt.Y, pt.X, tie.InnerException.Message);
               XmlException xml = new XmlException(newMessage, tie.InnerException, pt.Y, pt.X);
               TargetInvocationException newTie = new TargetInvocationException(newMessage, xml);

               throw newTie;
            }
            catch (XmlException e)
            {
               throw new ArgumentException(string.Format(SR.InvalidResXFile, e.Message), e);
            }
            catch (Exception e)
            {
               Point pt = GetPosition(reader);
               XmlException xmlEx = new XmlException(e.Message, e, pt.Y, pt.X);
               throw new ArgumentException(string.Format(SR.InvalidResXFile, xmlEx.Message), xmlEx);
            }
         }
         finally
         {
            if (!success)
            {
               _resData = null;
            }
         }

         bool validFile = false;

         //if (_resHeaderMimeType == ResourceConstants.ResMimeType)
         //{
         //   Type readerType = typeof(ResXResourceReader);
         //   Type writerType = typeof(ResXResourceWriter);

         //   string readerTypeName = _resHeaderReaderType;
         //   string writerTypeName = _resHeaderWriterType;
         //   if (readerTypeName !=null && readerTypeName.IndexOf(',') != -1)
         //   {
         //      readerTypeName = readerTypeName.Split(',')[0].Trim();
         //   }
         //   if (writerTypeName !=null && writerTypeName.IndexOf(',') != -1)
         //   {
         //      writerTypeName = writerTypeName.Split(',')[0].Trim();
         //   }

         //   if (readerTypeName !=null &&
         //       writerTypeName !=null &&
         //       readerTypeName.Equals(readerType.FullName) &&
         //       writerTypeName.Equals(writerType.FullName))
         //   {
         //      validFile = true;
         //   }
         //}

         if (!validFile)
         {
            _resData = null;
            throw new ArgumentException();
         }
      }

      private void ParseResHeaderNode(XmlReader reader)
      {
         string name = reader[ResourceConstants.NameStr];
         if (name != null)
         {
            reader.ReadStartElement();

            // The "1.1" schema requires the correct casing of the strings
            // in the resheader, however the "1.0" schema had a different
            // casing. By checking the Equals first, we should
            // see significant performance improvements.

            if (name == ResourceConstants.VersionStr)
            {
               if (reader.NodeType == XmlNodeType.Element)
               {
                  _resHeaderVersion = reader.ReadElementString();
               }
               else
               {
                  _resHeaderVersion = reader.Value.Trim();
               }
            }
            else if (name == ResourceConstants.ResMimeTypeStr)
            {
               if (reader.NodeType == XmlNodeType.Element)
               {
                  _resHeaderMimeType = reader.ReadElementString();
               }
               else
               {
                  _resHeaderMimeType = reader.Value.Trim();
               }
            }
            else if (name == ResourceConstants.ReaderStr)
            {
               if (reader.NodeType == XmlNodeType.Element)
               {
                  _resHeaderReaderType = reader.ReadElementString();
               }
               else
               {
                  _resHeaderReaderType = reader.Value.Trim();
               }
            }
            else if (name == ResourceConstants.WriterStr)
            {
               if (reader.NodeType == XmlNodeType.Element)
               {
                  _resHeaderWriterType = reader.ReadElementString();
               }
               else
               {
                  _resHeaderWriterType = reader.Value.Trim();
               }
            }
            else
            {
               switch (name.ToLower(CultureInfo.InvariantCulture))
               {
                  case ResourceConstants.VersionStr:
                     if (reader.NodeType == XmlNodeType.Element)
                     {
                        _resHeaderVersion = reader.ReadElementString();
                     }
                     else
                     {
                        _resHeaderVersion = reader.Value.Trim();
                     }
                     break;
                  case ResourceConstants.ResMimeTypeStr:
                     if (reader.NodeType == XmlNodeType.Element)
                     {
                        _resHeaderMimeType = reader.ReadElementString();
                     }
                     else
                     {
                        _resHeaderMimeType = reader.Value.Trim();
                     }
                     break;
                  case ResourceConstants.ReaderStr:
                     if (reader.NodeType == XmlNodeType.Element)
                     {
                        _resHeaderReaderType = reader.ReadElementString();
                     }
                     else
                     {
                        _resHeaderReaderType = reader.Value.Trim();
                     }
                     break;
                  case ResourceConstants.WriterStr:
                     if (reader.NodeType == XmlNodeType.Element)
                     {
                        _resHeaderWriterType = reader.ReadElementString();
                     }
                     else
                     {
                        _resHeaderWriterType = reader.Value.Trim();
                     }
                     break;
               }
            }
         }
      }

      private void ParseAssemblyNode(XmlReader reader)
      {
         string alias = reader[ResourceConstants.AliasStr];
         string typeName = reader[ResourceConstants.NameStr];

         AssemblyName assemblyName = new AssemblyName(typeName);

         if (string.IsNullOrEmpty(alias))
         {
            alias = assemblyName.Name;
         }

         _aliasResolver.PushAlias(alias, assemblyName);
      }

      private void ParseDataNode(XmlTextReader reader, bool isMetaData)
      {
         DataNodeInfo nodeInfo = new DataNodeInfo
         {
            Name = reader[ResourceConstants.NameStr]
         };

         string typeName = reader[ResourceConstants.TypeStr];

         string alias = null;
         AssemblyName assemblyName = null;

         if (!string.IsNullOrEmpty(typeName))
         {
            alias = GetAliasFromTypeName(typeName);
         }

         if (!string.IsNullOrEmpty(alias))
         {
            assemblyName = _aliasResolver.ResolveAlias(alias);
         }

         if (assemblyName != null)
         {
            nodeInfo.TypeName = GetTypeFromTypeName(typeName) + ", " + assemblyName.FullName;
         }
         else
         {
            nodeInfo.TypeName = reader[ResourceConstants.TypeStr];
         }

         nodeInfo.MimeType = reader[ResourceConstants.MimeTypeStr];

         bool finishedReadingDataNode = false;
         nodeInfo.ReaderPosition = GetPosition(reader);
         while (!finishedReadingDataNode && reader.Read())
         {
            if (reader.NodeType == XmlNodeType.EndElement && (reader.LocalName.Equals(ResourceConstants.DataStr) || reader.LocalName.Equals(ResourceConstants.MetadataStr)))
            {
               // we just found </data>, quit or </metadata>
               finishedReadingDataNode = true;
            }
            else
            {
               // could be a <value> or a <comment>
               if (reader.NodeType == XmlNodeType.Element)
               {
                  if (reader.Name.Equals(ResourceConstants.ValueStr))
                  {
                     WhitespaceHandling oldValue = reader.WhitespaceHandling;
                     try
                     {
                        // based on the documentation at https://docs.microsoft.com/dotnet/api/system.xml.xmltextreader.whitespacehandling
                        // this is ok because:
                        // "Because the XmlTextReader does not have DTD information available to it,
                        // SignificantWhitepsace nodes are only returned within the an xml:space='preserve' scope."
                        // the xml:space would not be present for anything else than string and char (see ResXResourceWriter)
                        // so this would not cause any breaking change while reading data from Everett (we never outputed
                        // xml:space then) or from whidbey that is not specifically either a string or a char.
                        // However please note that manually editing a resx file in Everett and in Whidbey because of the addition
                        // of xml:space=preserve might have different consequences...
                        reader.WhitespaceHandling = WhitespaceHandling.Significant;
                        nodeInfo.ValueData = reader.ReadString();
                     }
                     finally
                     {
                        reader.WhitespaceHandling = oldValue;
                     }
                  }
                  else if (reader.Name.Equals(ResourceConstants.CommentStr))
                  {
                     nodeInfo.Comment = reader.ReadString();
                  }
               }
               else
               {
                  // weird, no <xxxx> tag, just the inside of <data> as text
                  nodeInfo.ValueData = reader.Value.Trim();
               }
            }
         }

         if (nodeInfo.Name is null)
         {
            throw new ArgumentException(string.Format(SR.InvalidResXResourceNoName, nodeInfo.ValueData));
         }

         ResXDataNode dataNode = new ResXDataNode(nodeInfo, BasePath);

         if (UseResXDataNodes)
         {
            _resData[nodeInfo.Name] = dataNode;
         }
         else
         {
            IDictionary data = _resData;
            if (_assemblyNames is null)
            {
               data[nodeInfo.Name] = dataNode.GetValue(_typeResolver);
            }
            else
            {
               data[nodeInfo.Name] = dataNode.GetValue(_assemblyNames);
            }
         }
      }


      public IDictionaryEnumerator GetEnumerator()
      {
         _isReaderDirty = true;
         EnsureResData();
         return _resData.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
   }
}
