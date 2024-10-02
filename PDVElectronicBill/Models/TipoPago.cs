public class Pago
{
    public TipoPago tipoPago { get; set; }
    public decimal monto { get; set; }
}

public enum TipoPago
{
    Efectivo = 3,
    Tarjeta = 1,
    Transferencia_DepositoBancario = 0
}