namespace Resources
{
   internal static class ResourceConstants
   {

      internal static readonly string ResMimeType = "text/microsoft-resx";
      internal static readonly string Version = "2.0";
      internal const string TypeStr = "type";
      internal const string NameStr = "name";
      internal const string DataStr = "data";
      internal const string MetadataStr = "metadata";
      internal const string MimeTypeStr = "mimetype";
      internal const string ValueStr = "value";
      internal const string ResHeaderStr = "resheader";
      internal const string VersionStr = "version";
      internal const string ResMimeTypeStr = "resmimetype";
      internal const string ReaderStr = "reader";
      internal const string WriterStr = "writer";
      internal const string CommentStr = "comment";
      internal const string AssemblyStr = "assembly";
      internal const string AliasStr = "alias";

      internal static readonly string BinSerializedObjectMimeType = "application/x-microsoft.net.object.binary.base64";
      internal static readonly string SoapSerializedObjectMimeType = "application/x-microsoft.net.object.soap.base64";



      internal static readonly string ResourceSchema = @"
   <xsd:schema id=""root"" xmlns="""" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:msdata=""urn:schemas-microsoft-com:xml-msdata"">
      <xsd:import namespace=""http://www.w3.org/XML/1998/namespace""/>
      <xsd:element name=""root"" msdata:IsDataSet=""true"">
         <xsd:complexType>
               <xsd:choice maxOccurs=""unbounded"">
                  <xsd:element name=""metadata"">
                     <xsd:complexType>
                           <xsd:sequence>
                           <xsd:element name=""value"" type=""xsd:string"" minOccurs=""0""/>
                           </xsd:sequence>
                           <xsd:attribute name=""name"" use=""required"" type=""xsd:string""/>
                           <xsd:attribute name=""type"" type=""xsd:string""/>
                           <xsd:attribute name=""mimetype"" type=""xsd:string""/>
                           <xsd:attribute ref=""xml:space""/>
                     </xsd:complexType>
                  </xsd:element>
                  <xsd:element name=""assembly"">
                     <xsd:complexType>
                     <xsd:attribute name=""alias"" type=""xsd:string""/>
                     <xsd:attribute name=""name"" type=""xsd:string""/>
                     </xsd:complexType>
                  </xsd:element>
                  <xsd:element name=""data"">
                     <xsd:complexType>
                           <xsd:sequence>
                              <xsd:element name=""value"" type=""xsd:string"" minOccurs=""0"" msdata:Ordinal=""1"" />
                              <xsd:element name=""comment"" type=""xsd:string"" minOccurs=""0"" msdata:Ordinal=""2"" />
                           </xsd:sequence>
                           <xsd:attribute name=""name"" type=""xsd:string"" use=""required"" msdata:Ordinal=""1"" />
                           <xsd:attribute name=""type"" type=""xsd:string"" msdata:Ordinal=""3"" />
                           <xsd:attribute name=""mimetype"" type=""xsd:string"" msdata:Ordinal=""4"" />
                           <xsd:attribute ref=""xml:space""/>
                     </xsd:complexType>
                  </xsd:element>
                  <xsd:element name=""resheader"">
                     <xsd:complexType>
                           <xsd:sequence>
                              <xsd:element name=""value"" type=""xsd:string"" minOccurs=""0"" msdata:Ordinal=""1"" />
                           </xsd:sequence>
                           <xsd:attribute name=""name"" type=""xsd:string"" use=""required"" />
                     </xsd:complexType>
                  </xsd:element>
               </xsd:choice>
         </xsd:complexType>
      </xsd:element>
      </xsd:schema>
      ";

   }
}
