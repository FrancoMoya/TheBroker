using System;
using System.Collections.Generic;

namespace TheBroker.Models;

public partial class OrdersHistory
{
    public int Id { get; set; }

    public int TxNumber { get; set; }

    public DateTime? ChangedDate { get; set; }

    public string Action { get; set; } = null!;

    public string StatusBefore { get; set; } = null!;

    public string? StatusAfter { get; set; }

    public int IdSymbol { get; set; }

    public string Description { get; set; } = null!;
}
