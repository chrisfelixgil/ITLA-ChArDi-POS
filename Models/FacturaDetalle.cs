using System;
using System.Collections.Generic;

namespace cahrdipos_system.Models;

public partial class FacturaDetalle
{
    public int Id { get; set; }

    public int FacturaId { get; set; }

    public int ProductoId { get; set; }

    public int Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public decimal Subtotal { get; set; }

    public virtual Factura Factura { get; set; } = null!;

    public virtual Producto Producto { get; set; } = null!;
}
