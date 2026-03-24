using System;
using System.Collections.Generic;

namespace cahrdipos_system.Models;

public partial class Factura
{
    public int Id { get; set; }

    public string NumeroFactura { get; set; } = null!;

    public DateTime Fecha { get; set; }

    public decimal Subtotal { get; set; }

    public decimal Total { get; set; }

    public virtual ICollection<FacturaDetalle> FacturaDetalles { get; set; } = new List<FacturaDetalle>();
}
