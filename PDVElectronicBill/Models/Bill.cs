using System.Collections.ObjectModel;
using TiqueteElectronico;

namespace Products.Models
{
  public class Bill
  {
    private const decimal IVA = 0.13M;

    public const string CondicionVentaContado = "01";
    public const string CondicionVentaCredito = "02";
    public const string CondicionVentaConsignacion = "03";
    public const string CondicionVentaApartado = "04";
    public const string CondicionVentaArrendamientoConOpcionDeCompra = "05";
    public const string CondicionVentaArrendamientoEnFunciónFinanciera = "06";
    public const string CondicionVentaCobroAFavorDeUnTercero = "07";
    public const string CondicionVentaServiciosPrestadosAlEstadoACredito = "08";
    public const string CondicionVentaPagoDelServiciosPrestadoAlEstado = "09";
    public const string CondicionVentaOtros = "99";

    private decimal _total = 0.0M;
    public long consecutive { get; set; } = 0L;
    public DateTime date {get; set; } = DateTime.UtcNow;

    public IEnumerable<Product> products { get; set; } = new List<Product>();
    public decimal total
    {
      get
      {
        return _total;
      }
      set
      {
        tax = value * IVA;
        _total = value;
      }
    }
    public decimal tax { get; private set; } = 0.0M;
    public ElectronicBill electronicBill { get; set; } = new();
    public string CondicionVenta { get; set; } = CondicionVentaContado;
    public string PlazoCredito { get; set; } = "0";
    public decimal Descuento { get; set; } = 0.0M;

    public IEnumerable<TipoPago> MedioPago { get; set; } = new List<TipoPago>();
    public int NumeroVoucher { get; set; }
    public decimal Efectivo { get; set; }

    public override string ToString()
    {
      return $"Factura No. {consecutive} del {date.ToString("dd/MM/yyyy")} con un total de {total.ToString("C2")}";
    }

    public static implicit operator TiqueteElectronicoCondicionVenta(Bill from)
    {
      return from.CondicionVenta switch
      {
        CondicionVentaContado => TiqueteElectronicoCondicionVenta.Contado,
        CondicionVentaCredito => TiqueteElectronicoCondicionVenta.Credito,
        CondicionVentaConsignacion => TiqueteElectronicoCondicionVenta.Consignacion,
        CondicionVentaApartado => TiqueteElectronicoCondicionVenta.Apartado,
        CondicionVentaArrendamientoConOpcionDeCompra => TiqueteElectronicoCondicionVenta.ArrendamientoConOpcionDeCompra,
        CondicionVentaArrendamientoEnFunciónFinanciera => TiqueteElectronicoCondicionVenta.ArrendamientoEnFuncionFinanciera,
        CondicionVentaCobroAFavorDeUnTercero => TiqueteElectronicoCondicionVenta.CobroAFavorDeUnTercero,
        CondicionVentaServiciosPrestadosAlEstadoACredito => TiqueteElectronicoCondicionVenta.ServiciosPrestadosAlEstadoACredito,
        CondicionVentaPagoDelServiciosPrestadoAlEstado => TiqueteElectronicoCondicionVenta.PagoDelServiciosPrestadoAlEstado,
        CondicionVentaOtros => TiqueteElectronicoCondicionVenta.Otros,
        _ => throw new Exception("Condicion de venta no soportada")
      };
    }

    public static implicit operator Collection<TiqueteElectronicoMedioPago>(Bill from)
    {
      var lstMedioPago = new Collection<TiqueteElectronicoMedioPago>();
      foreach (var medioPago in from.MedioPago)
      {
        lstMedioPago.Add(medioPago switch
        {
          TipoPago.Efectivo => TiqueteElectronicoMedioPago.Efectivo,
          TipoPago.Tarjeta => TiqueteElectronicoMedioPago.Tarjeta,
          TipoPago.Transferencia_DepositoBancario => TiqueteElectronicoMedioPago.Transferencia_DepositoBancario,
          _ => throw new Exception("Medio de pago no soportado")
        });
      }

      return lstMedioPago;
    }
  }
}