namespace Products.Models
{
    /// <summary>
    /// <para>Mensaje de uso exclusivo por parte de la Direccion General de Tributación</para>
    /// </summary>
    [System.ComponentModel.DescriptionAttribute("Mensaje de uso exclusivo por parte de la Direccion General de Tributación")]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.805.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("MensajeHacienda", Namespace="https://cdn.comprobanteselectronicos.go.cr/xml-schemas/v4.3/mensajeHacienda", AnonymousType=true)]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("MensajeHacienda", Namespace="https://cdn.comprobanteselectronicos.go.cr/xml-schemas/v4.3/mensajeHacienda")]
    public partial class MensajeHacienda
    {
        
        /// <summary>
        /// <para>Clave numérica del comprobante</para>
        /// <para xml:lang="en">Pattern: \d{50,50}.</para>
        /// </summary>
        [System.ComponentModel.DescriptionAttribute("Clave numérica del comprobante")]
        [System.ComponentModel.DataAnnotations.RegularExpressionAttribute("\\d{50,50}")]
        [System.ComponentModel.DataAnnotations.RequiredAttribute()]
        [System.Xml.Serialization.XmlElementAttribute("Clave")]
        public string Clave { get; set; }
        
        /// <summary>
        /// <para>Nombre o razón social del emisor</para>
        /// <para xml:lang="en">Maximum length: 100.</para>
        /// </summary>
        [System.ComponentModel.DescriptionAttribute("Nombre o razón social del emisor")]
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(100)]
        [System.ComponentModel.DataAnnotations.RequiredAttribute()]
        [System.Xml.Serialization.XmlElementAttribute("NombreEmisor")]
        public string NombreEmisor { get; set; }
        
        /// <summary>
        /// <para>Tipo de identificacion: 01 Cedula Fisica, 02 Cedula Juridica, 03 DIMEX, 04 NITE</para>
        /// </summary>
        [System.ComponentModel.DescriptionAttribute("Tipo de identificacion: 01 Cedula Fisica, 02 Cedula Juridica, 03 DIMEX, 04 NITE")]
        [System.ComponentModel.DataAnnotations.RequiredAttribute()]
        [System.Xml.Serialization.XmlElementAttribute("TipoIdentificacionEmisor")]
        public MensajeHaciendaTipoIdentificacionEmisor TipoIdentificacionEmisor { get; set; }
        
        /// <summary>
        /// <para>Número de cédula fisica/jurídica/NITE/DIMEX del emisor</para>
        /// <para xml:lang="en">Maximum length: 12.</para>
        /// <para xml:lang="en">Pattern: \d{9,12}.</para>
        /// </summary>
        [System.ComponentModel.DescriptionAttribute("Número de cédula fisica/jurídica/NITE/DIMEX del emisor")]
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(12)]
        [System.ComponentModel.DataAnnotations.RegularExpressionAttribute("\\d{9,12}")]
        [System.ComponentModel.DataAnnotations.RequiredAttribute()]
        [System.Xml.Serialization.XmlElementAttribute("NumeroCedulaEmisor")]
        public string NumeroCedulaEmisor { get; set; }
        
        /// <summary>
        /// <para>Nombre o razon social del receptor</para>
        /// <para xml:lang="en">Maximum length: 100.</para>
        /// </summary>
        [System.ComponentModel.DescriptionAttribute("Nombre o razon social del receptor")]
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(100)]
        [System.Xml.Serialization.XmlElementAttribute("NombreReceptor")]
        public string NombreReceptor { get; set; }
        
        /// <summary>
        /// <para>Tipo de identificacion: 01 Cedula Fisica, 02 Cedula Juridica, 03 DIMEX, 04 NITE, 05 Otros</para>
        /// </summary>
        [System.ComponentModel.DescriptionAttribute("Tipo de identificacion: 01 Cedula Fisica, 02 Cedula Juridica, 03 DIMEX, 04 NITE, " +
            "05 Otros")]
        [System.Xml.Serialization.XmlElementAttribute("TipoIdentificacionReceptor", IsNullable=true)]
        public System.Nullable<MensajeHaciendaTipoIdentificacionReceptor> TipoIdentificacionReceptor { get; set; }
        
        /// <summary>
        /// <para xml:lang="en">Gets or sets a value indicating whether the TipoIdentificacionReceptor property is specified.</para>
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool TipoIdentificacionReceptorSpecified { get; set; }
        
        /// <summary>
        /// <para>Número de cédula fisica/jurídica/NITE/DIMEX del receptor</para>
        /// <para xml:lang="en">Maximum length: 12.</para>
        /// <para xml:lang="en">Pattern: \d{9,12}.</para>
        /// </summary>
        [System.ComponentModel.DescriptionAttribute("Número de cédula fisica/jurídica/NITE/DIMEX del receptor")]
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(12)]
        [System.ComponentModel.DataAnnotations.RegularExpressionAttribute("\\d{9,12}")]
        [System.Xml.Serialization.XmlElementAttribute("NumeroCedulaReceptor", IsNullable=true)]
        public string NumeroCedulaReceptor { get; set; }
        
        /// <summary>
        /// <para>Codigo del mensaje de respuesta. 1 aceptado, 3 rechazado</para>
        /// </summary>
        [System.ComponentModel.DescriptionAttribute("Codigo del mensaje de respuesta. 1 aceptado, 3 rechazado")]
        [System.ComponentModel.DataAnnotations.RequiredAttribute()]
        [System.Xml.Serialization.XmlElementAttribute("Mensaje")]
        public MensajeHaciendaMensaje Mensaje { get; set; }
        
        /// <summary>
        /// <para>Detalle del mensaje</para>
        /// </summary>
        [System.ComponentModel.DescriptionAttribute("Detalle del mensaje")]
        [System.ComponentModel.DataAnnotations.RequiredAttribute()]
        [System.Xml.Serialization.XmlElementAttribute("DetalleMensaje")]
        public string DetalleMensaje { get; set; }
        
        /// <summary>
        /// <para>Monto total del impuesto, que es obligatorio si el comprobante tenga impuesto.</para>
        /// <para xml:lang="en">Total number of digits: 18.</para>
        /// <para xml:lang="en">Total number of digits in fraction: 5.</para>
        /// </summary>
        [System.ComponentModel.DescriptionAttribute("Monto total del impuesto, que es obligatorio si el comprobante tenga impuesto.")]
        [System.Xml.Serialization.XmlElementAttribute("MontoTotalImpuesto")]
        public decimal MontoTotalImpuesto { get; set; }
        
        /// <summary>
        /// <para xml:lang="en">Gets or sets a value indicating whether the MontoTotalImpuesto property is specified.</para>
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool MontoTotalImpuestoSpecified { get; set; }
        
        /// <summary>
        /// <para>Monto total de la factura</para>
        /// <para xml:lang="en">Total number of digits: 18.</para>
        /// <para xml:lang="en">Total number of digits in fraction: 5.</para>
        /// </summary>
        [System.ComponentModel.DescriptionAttribute("Monto total de la factura")]
        [System.ComponentModel.DataAnnotations.RequiredAttribute()]
        [System.Xml.Serialization.XmlElementAttribute("TotalFactura")]
        public decimal TotalFactura { get; set; }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.805.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("MensajeHaciendaTipoIdentificacionEmisor", Namespace="https://cdn.comprobanteselectronicos.go.cr/xml-schemas/v4.3/mensajeHacienda", AnonymousType=true)]
    public enum MensajeHaciendaTipoIdentificacionEmisor
    {
        
        /// <summary>
        /// <para>Cedula Fisica</para>
        /// </summary>
        [System.ComponentModel.DescriptionAttribute("Cedula Fisica")]
        [System.Xml.Serialization.XmlEnumAttribute("01")]
        Item01,
        
        /// <summary>
        /// <para>Cedula Juridica</para>
        /// </summary>
        [System.ComponentModel.DescriptionAttribute("Cedula Juridica")]
        [System.Xml.Serialization.XmlEnumAttribute("02")]
        Item02,
        
        /// <summary>
        /// <para>DIMEX</para>
        /// </summary>
        [System.ComponentModel.DescriptionAttribute("DIMEX")]
        [System.Xml.Serialization.XmlEnumAttribute("03")]
        Item03,
        
        /// <summary>
        /// <para>NITE</para>
        /// </summary>
        [System.ComponentModel.DescriptionAttribute("NITE")]
        [System.Xml.Serialization.XmlEnumAttribute("04")]
        Item04,
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.805.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("MensajeHaciendaTipoIdentificacionReceptor", Namespace="https://cdn.comprobanteselectronicos.go.cr/xml-schemas/v4.3/mensajeHacienda", AnonymousType=true)]
    public enum MensajeHaciendaTipoIdentificacionReceptor
    {
        
        /// <summary>
        /// <para>Cedula Fisica</para>
        /// </summary>
        [System.ComponentModel.DescriptionAttribute("Cedula Fisica")]
        [System.Xml.Serialization.XmlEnumAttribute("01")]
        Item01,
        
        /// <summary>
        /// <para>Cedula Juridica</para>
        /// </summary>
        [System.ComponentModel.DescriptionAttribute("Cedula Juridica")]
        [System.Xml.Serialization.XmlEnumAttribute("02")]
        Item02,
        
        /// <summary>
        /// <para>DIMEX</para>
        /// </summary>
        [System.ComponentModel.DescriptionAttribute("DIMEX")]
        [System.Xml.Serialization.XmlEnumAttribute("03")]
        Item03,
        
        /// <summary>
        /// <para>NITE</para>
        /// </summary>
        [System.ComponentModel.DescriptionAttribute("NITE")]
        [System.Xml.Serialization.XmlEnumAttribute("04")]
        Item04,
        
        /// <summary>
        /// <para>Otros</para>
        /// </summary>
        [System.ComponentModel.DescriptionAttribute("Otros")]
        [System.Xml.Serialization.XmlEnumAttribute("05")]
        Item05,
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.805.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("MensajeHaciendaMensaje", Namespace="https://cdn.comprobanteselectronicos.go.cr/xml-schemas/v4.3/mensajeHacienda", AnonymousType=true)]
    public enum MensajeHaciendaMensaje
    {
        
        /// <summary>
        /// <para>Aceptado</para>
        /// </summary>
        [System.ComponentModel.DescriptionAttribute("Aceptado")]
        [System.Xml.Serialization.XmlEnumAttribute("1")]
        Item1,
        
        /// <summary>
        /// <para>Rechazado</para>
        /// </summary>
        [System.ComponentModel.DescriptionAttribute("Rechazado")]
        [System.Xml.Serialization.XmlEnumAttribute("3")]
        Item3,
    }
}
