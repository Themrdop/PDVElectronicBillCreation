using System.Text;
using FirmaXadesNet;
using FirmaXadesNet.Signature.Parameters;
using System.Xml.Serialization;
using System.Security.Cryptography.X509Certificates;
using TiqueteElectronico;
using System.Collections.ObjectModel;
using System.Text.Json;
using Polly;
using Products.Interfaces;
using Products.Models;

namespace Domain.ElectronicBill;

public class ElectronicBill : IElectronicBill
{
  private const string COUNTRY_CODE = "506";
  public const string CODIGO_ACTIVIDAD = "552004";
  public const string CODIGO_PRODUCTO = "6332000000000";//Suministro de comida, servicio de restaurante sin mesero, con sitios para sentarse	13%
  private readonly IHttpClientFactory _httpClientFactory;
  private readonly IAsyncPolicy<HttpResponseMessage> _retryPolicy = Policy
      .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode && (r.StatusCode != System.Net.HttpStatusCode.BadRequest | r.StatusCode != System.Net.HttpStatusCode.Unauthorized))
      .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
  private readonly string _userName;
  private readonly string _password;
  public readonly string _JWT_Endpoint;
  public readonly string _endPoint;
  private readonly string _clientId;

  public ElectronicBill(IHttpClientFactory httpClientFactory)
  {
    _userName = Environment.GetEnvironmentVariable("HaciendaUserName") ?? throw new ArgumentException("HaciendaUserName not found");
    _password = Environment.GetEnvironmentVariable("HaciendaPassword") ?? throw new ArgumentException("HaciendaPassword not found");
    _endPoint = Environment.GetEnvironmentVariable("endpoint") ?? throw new ArgumentException("endpoint not found");
    _JWT_Endpoint = Environment.GetEnvironmentVariable("JWT_Endpoint") ?? throw new ArgumentException("JWT_Endpoint not found");
    _clientId = Environment.GetEnvironmentVariable("clientId") ?? throw new ArgumentException("clientId not found");
    _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
  }

  public async Task<Bill> SendElectronicBill(Bill bill, byte[] certificate, string ping, CancellationToken cancellationToken)
  {
    string haciendaKey = generateKey(bill);
    string xmlDocument = GetXMLDocument(haciendaKey, bill);
    string singedXML = singXML(xmlDocument, certificate, ping);
    string singedXMLInBase64 = Base64_Encode(singedXML);
    HaciendaMessage haciendaMessage = creatMessage(haciendaKey, bill.electronicBill.emitter, bill.electronicBill.recipient, singedXMLInBase64);
    HttpResponseMessage result = await sendToHacienda(haciendaMessage, cancellationToken);

    switch (result.StatusCode)
    {
      case System.Net.HttpStatusCode.Accepted:
        bill.electronicBill.location = result.Headers.Location;
        bill.electronicBill.electronicBill = singedXML;
        bill.electronicBill.clave = haciendaKey;

        break;

      case System.Net.HttpStatusCode.BadRequest:
        bill.electronicBill.errorMessages = result.ReasonPhrase;
        throw new Exception(result.ReasonPhrase);
      default:
        break;
    }

    return bill;

  }

  private string GetXMLDocument(string haciendaKey, Bill bill)
  {
    string xmlDocument = string.Empty;
    decimal phone = decimal.TryParse(bill.electronicBill.recipient.phone, out phone) ? phone : 0;

    var msj = new TiqueteElectronico.TiqueteElectronico
    {
      Clave = haciendaKey,
      CodigoActividad = CODIGO_ACTIVIDAD,
      FechaEmision = DateTime.Now,
      NumeroConsecutivo = GetFormatedConsecutive(bill.consecutive),
      Emisor = new EmisorType
      {
        Nombre = bill.electronicBill.emitter.name,
        Identificacion = new IdentificacionType
        {
          Tipo = bill.electronicBill.emitter,
          Numero = bill.electronicBill.emitter.numeroIdentificacion
        },
        NombreComercial = bill.electronicBill.Comers.name,
        Ubicacion = new UbicacionType
        {
          BarrioSpecified = true,
          Provincia = bill.electronicBill.Comers.Provincia,
          Canton = bill.electronicBill.Comers.Canton.ToString().PadLeft(2, '0'),
          Distrito = bill.electronicBill.Comers.Distrito.ToString().PadLeft(2, '0'),
          Barrio = bill.electronicBill.Comers.Barrio.ToString().PadLeft(2, '0'),
          OtrasSenas = bill.electronicBill.Comers.address
        },
        Telefono = new TelefonoType
        {
          CodigoPais = short.Parse(COUNTRY_CODE),
          NumTelefono = decimal.Parse(bill.electronicBill.Comers.phone)
        },
        CorreoElectronico = bill.electronicBill.Comers.email,
      },
      CondicionVenta = bill,
      MedioPago = (Collection<TiqueteElectronicoMedioPago>)bill,
      PlazoCredito = bill.PlazoCredito,
    };

    if (!string.IsNullOrEmpty(bill.electronicBill.recipient.numeroIdentificacion))
    {
      msj.Receptor = new ReceptorType
      {
        Nombre = bill.electronicBill.recipient.name,
        Identificacion = new IdentificacionType
        {
          Tipo = bill.electronicBill.recipient,
          Numero = bill.electronicBill.recipient.numeroIdentificacion
        },
        CorreoElectronico = bill.electronicBill.recipient.email,
      };
    }

    int linea = 1;
    decimal subtotal = 0;
    decimal descuento = 0;
    decimal total = 0;
    decimal impuesto = 0;
    decimal sumaSubtotal = 0;
    decimal sumaImpuesto = 0;
    decimal sumaTotal = 0;
    decimal sumaDescuento = 0;


    foreach (var pro in bill.products)
    {
      total = pro.price * pro.amount;
      descuento = total * bill.Descuento / 100;
      subtotal = total - descuento;
      impuesto = subtotal * 0.13M;

      msj.DetalleServicio.Add(new TiqueteElectronicoDetalleServicioLineaDetalle
      {
        NumeroLinea = linea.ToString(),
        Codigo = CODIGO_PRODUCTO,
        Cantidad = pro.amount,
        UnidadMedida = UnidadMedidaType.Unid,
        UnidadMedidaComercial = string.Empty,
        Detalle = pro.name,
        PrecioUnitario = pro.price,
        MontoTotal = total,
        SubTotal = subtotal,
        MontoTotalLinea = subtotal + impuesto,
        Impuesto = new Collection<ImpuestoType>{
            new ImpuestoType{
              Codigo = ImpuestoTypeCodigo.IVA,
              CodigoTarifa = ImpuestoTypeCodigoTarifa.Item08,
              CodigoTarifaSpecified = true,
              TarifaSpecified = true,
              Tarifa = 13,
              Monto = impuesto,

            }
          },
        ImpuestoNeto = impuesto
      });
      linea++;
      sumaSubtotal += subtotal;
      sumaImpuesto += impuesto;
      sumaTotal += total;
      sumaDescuento += descuento;
    }

    msj.ResumenFactura = new TiqueteElectronicoResumenFactura
    {
      CodigoTipoMoneda = new CodigoMonedaType
      {
        CodigoMoneda = CodigoMonedaTypeCodigoMoneda.CRC,
        TipoCambio = 1M
      },
      TotalMercanciasGravadasSpecified = true,
      TotalGravadoSpecified = true,
      TotalMercanciasGravadas = sumaTotal,
      TotalGravado = sumaTotal,
      TotalVenta = sumaTotal,
      TotalDescuentos = sumaDescuento,
      TotalVentaNeta = sumaTotal - sumaDescuento,
      TotalImpuesto = sumaImpuesto,
      TotalComprobante = sumaTotal
    };

    using var stringwriter = new Utf8StringWriter();
    var serializer = new XmlSerializer(typeof(TiqueteElectronico.TiqueteElectronico));
    serializer.Serialize(stringwriter, msj);
    xmlDocument = stringwriter.ToString();

    return xmlDocument;
  }

  private class Utf8StringWriter : StringWriter
  {
    private static readonly Encoding UTF8NoBOM = new UTF8Encoding(false);
    public override Encoding Encoding
    {
      get { return UTF8NoBOM; }
    }
  }

  private async Task<HttpResponseMessage> sendToHacienda(HaciendaMessage haciendaMessage, CancellationToken cancellationToken)
  {
    var httpClient = _httpClientFactory.CreateClient();

    string token = await getToken(cancellationToken);
    httpClient.DefaultRequestHeaders.Add("authorization", "bearer " + token);
    return await _retryPolicy.ExecuteAsync(() => System.Net.Http.Json.HttpClientJsonExtensions.PostAsJsonAsync(httpClient, _endPoint + "/recepcion", haciendaMessage, cancellationToken: cancellationToken));
  }

  private async Task<string> getToken(CancellationToken cancellationToken)
  {

    var httpClient = _httpClientFactory.CreateClient();

    Dictionary<string, string> values = new();
    values.Add("client_id", _clientId);//)"api-prod");
    values.Add("grant_type", "password");
    values.Add("username", _userName);
    values.Add("password", _password);

    var content = new FormUrlEncodedContent(values);
    var response = await _retryPolicy.ExecuteAsync(() => httpClient.PostAsync(_JWT_Endpoint, content));
    var responseString = await response.Content.ReadAsStringAsync();

    if ((response.StatusCode != System.Net.HttpStatusCode.OK))
    {
      throw new Exception(("Error: " + responseString));
    }

    var token = JsonSerializer.Deserialize<Token>(responseString);
    return token?.access_token ?? string.Empty;
  }

  private string singXML(string xmlDoc, byte[] certificado, string pin)
  {
    X509Certificate2? cert = null;

    cert = new X509Certificate2(certificado, pin);

    XadesService xadesService = new XadesService();
    SignatureParameters parametros = new SignatureParameters();

    parametros.SignaturePolicyInfo = new SignaturePolicyInfo();
    parametros.SignaturePolicyInfo.PolicyIdentifier = "https://tribunet.hacienda.go.cr/docs/esquemas/2016/v4.1/Resolucion_Comprobantes_Electronicos_DGT-R-48-2016.pdf";
    //La propiedad PolicyHash es la misma para todos, es un cálculo en base al archivo pdf indicado en PolicyIdentifier
    parametros.SignaturePolicyInfo.PolicyHash = "Ohixl6upD6av8N7pEvDABhEL6hM=";
    parametros.SignaturePackaging = SignaturePackaging.ENVELOPED;
    parametros.DataFormat = new DataFormat();
    parametros.Signer = new FirmaXadesNet.Crypto.Signer(cert);

    FirmaXadesNet.Signature.SignatureDocument docFirmado = xadesService.Sign(xmlDoc, parametros);
    return docFirmado.Document.OuterXml;
  }

  private X509Certificate2 GetCertificateByThumbprint(object thumbprintCertificado)
  {
    throw new NotImplementedException();
  }

  private HaciendaMessage creatMessage(string haciendaKey, Person emitter, Person recipient, string singedXMLInBase64)
  {
    return new HaciendaMessage
    {
      clave = haciendaKey,
      fecha = DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ssZ"),
      emisor = emitter,
      receptor = recipient,
      comprobanteXml = singedXMLInBase64
    };
  }

  private string generateKey(Bill bill)
  {
    var currentDate = DateTime.Now;
    var Consecutivo = GetFormatedConsecutive(bill.consecutive);
    return new StringBuilder().Append(COUNTRY_CODE)
                                .Append(currentDate.Day.ToString().PadLeft(2, '0'))
                                .Append(currentDate.Month.ToString().PadLeft(2, '0'))
                                .Append(currentDate.Year.ToString().Substring(2))
                                .Append(bill.electronicBill.emitter.numeroIdentificacion.PadLeft(12, '0'))
                                .Append(Consecutivo)
                                .Append('1')
                                .Append(CreaCodigoSeguridad(Consecutivo))
                                .ToString();
  }

  private string GetFormatedConsecutive(long consecutive)
  {
    return new StringBuilder().Append("001")
                              .Append("00001")
                              .Append("04")
                              .Append(consecutive.ToString().PadLeft(10, '0')).ToString();
  }

  public static string CreaCodigoSeguridad(string consecutivo)
  {
    try
    {
      var currentDate = DateTime.Now;

      string concatenado = new StringBuilder().Append("001")
                          .Append("01")
                          .Append("01")
                          .Append(currentDate.ToString("yyyyMMddHHmmss"))
                          .Append(consecutivo.PadLeft(10, '0'))
                          .ToString();

      int calculo = 0;
      calculo = (calculo
                  + (int.Parse(concatenado.Substring(0, 1)) * 3));
      calculo = (calculo
                  + (int.Parse(concatenado.Substring(1, 1)) * 2));
      calculo = (calculo
                  + (int.Parse(concatenado.Substring(2, 1)) * 9));
      calculo = (calculo
                  + (int.Parse(concatenado.Substring(3, 1)) * 8));
      calculo = (calculo
                  + (int.Parse(concatenado.Substring(4, 1)) * 7));
      calculo = (calculo
                  + (int.Parse(concatenado.Substring(5, 1)) * 6));
      calculo = (calculo
                  + (int.Parse(concatenado.Substring(6, 1)) * 5));
      calculo = (calculo
                  + (int.Parse(concatenado.Substring(7, 1)) * 4));
      calculo = (calculo
                  + (int.Parse(concatenado.Substring(8, 1)) * 3));
      calculo = (calculo
                  + (int.Parse(concatenado.Substring(9, 1)) * 2));
      calculo = (calculo
                  + (int.Parse(concatenado.Substring(10, 1)) * 9));
      calculo = (calculo
                  + (int.Parse(concatenado.Substring(11, 1)) * 8));
      calculo = (calculo
                  + (int.Parse(concatenado.Substring(12, 1)) * 7));
      calculo = (calculo
                  + (int.Parse(concatenado.Substring(13, 1)) * 6));
      calculo = (calculo
                  + (int.Parse(concatenado.Substring(14, 1)) * 5));
      calculo = (calculo
                  + (int.Parse(concatenado.Substring(15, 1)) * 4));
      calculo = (calculo
                  + (int.Parse(concatenado.Substring(16, 1)) * 3));
      calculo = (calculo
                  + (int.Parse(concatenado.Substring(17, 1)) * 2));
      calculo = (calculo
                  + (int.Parse(concatenado.Substring(18, 1)) * 9));
      calculo = (calculo
                  + (int.Parse(concatenado.Substring(19, 1)) * 8));
      calculo = (calculo
                  + (int.Parse(concatenado.Substring(20, 1)) * 7));
      calculo = (calculo
                  + (int.Parse(concatenado.Substring(21, 1)) * 6));
      calculo = (calculo
                  + (int.Parse(concatenado.Substring(22, 1)) * 5));
      calculo = (calculo
                  + (int.Parse(concatenado.Substring(23, 1)) * 4));
      calculo = (calculo
                  + (int.Parse(concatenado.Substring(24, 1)) * 3));
      calculo = (calculo
                  + (int.Parse(concatenado.Substring(25, 1)) * 2));
      calculo = (calculo
                  + (int.Parse(concatenado.Substring(26, 1)) * 9));
      calculo = (calculo
                  + (int.Parse(concatenado.Substring(27, 1)) * 8));
      calculo = (calculo
                  + (int.Parse(concatenado.Substring(28, 1)) * 7));
      calculo = (calculo
                  + (int.Parse(concatenado.Substring(29, 1)) * 6));
      calculo = (calculo
                  + (int.Parse(concatenado.Substring(30, 1)) * 5));
      calculo = (calculo
                  + (int.Parse(concatenado.Substring(31, 1)) * 4));
      calculo = (calculo
                  + (int.Parse(concatenado.Substring(32, 1)) * 3));
      calculo = (calculo
                  + (int.Parse(concatenado.Substring(33, 1)) * 2));
      int mDV = 0;
      int digitoMod = 0;
      digitoMod = (calculo % 11);
      if (((digitoMod == 0)
                  || (digitoMod == 1)))
      {
        mDV = 0;
      }
      else
      {
        mDV = (11 - digitoMod);
      }

      return (consecutivo.Substring(7, 2).PadLeft(2, '0')
                  + (calculo.ToString().PadLeft(5, '0') + mDV.ToString()));
    }
    catch (Exception ex)
    {
      throw ex;
    }

  }

  public static string Base64_Encode(string str)
  {
    byte[] encbuff = Encoding.UTF8.GetBytes(str);
    return Convert.ToBase64String(encbuff);
  }
}

internal class Token
{
  public string access_token { get; set; } = string.Empty;
  public string token_type { get; set; } = string.Empty;
  public int expires_in { get; set; }
  public string refresh_token { get; set; } = string.Empty;
}