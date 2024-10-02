namespace Products.Models
{
  public class Person
  {
    public const string TIPO_IDENTIFICACION_FISICA = "01";
    public const string TIPO_IDENTIFICACION_JURIDICA = "02";
    public const string DIMEX = "03";
    public const string NITE = "04";

    public string name { get; set; } = string.Empty;
    public string tipoIdentificacion { get; set; } = TIPO_IDENTIFICACION_FISICA;
    public string numeroIdentificacion { get; set; } = string.Empty;
    public string phone { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;

    static public implicit operator TiqueteElectronico.IdentificacionTypeTipo(Person from)
    {
      return from.tipoIdentificacion switch
      {
        TIPO_IDENTIFICACION_FISICA => TiqueteElectronico.IdentificacionTypeTipo.Cedula_Fisica,
        TIPO_IDENTIFICACION_JURIDICA => TiqueteElectronico.IdentificacionTypeTipo.Cedula_Juridica,
        DIMEX => TiqueteElectronico.IdentificacionTypeTipo.DIMEX,
        NITE => TiqueteElectronico.IdentificacionTypeTipo.NITE,
        _ => throw new Exception("Tipo de identificacion no soportado")
      };
    }
  }
}