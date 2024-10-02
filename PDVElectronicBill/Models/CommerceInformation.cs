namespace Products.Models
{
  public class CommerceInformation
  {
    public string name { get; set; } = string.Empty;
    public string cedula { get; set; } = string.Empty;
    public string address { get; set; } = string.Empty;
    public string phone { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;
    public string? logo { get; set; } = string.Empty;
    public byte Provincia { get; set; }
    public byte Canton { get; set; }
    public byte Distrito { get; set; }
    public byte Barrio { get; set; }
    public string pinCertificado { get; set; } = string.Empty;

    public override string ToString()
    {
      return $"{name} con cedula {cedula} en la direccion {address} con el telefono {phone} y el correo {email}";
    }
  }
}