using System;
using System.Collections.Generic;

namespace TheBroker.Models;

public partial class Order
{
    public int TxNumber { get; set; }

    public string Action { get; set; } = null!;

    public string Status { get; set; } = null!;

    public int IdSymbol { get; set; }

    public int Quantity { get; set; }

    public DateTime? OrderDate { get; set; }

    public virtual StockMarketShare IdSymbolNavigation { get; set; } = null!;
}
