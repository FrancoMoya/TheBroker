using System;
using System.Collections.Generic;

namespace TheBroker.Models;

public partial class StockMarketShare
{
    public int Id { get; set; }

    public string Symbol { get; set; } = null!;

    public decimal UnitPrice { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
