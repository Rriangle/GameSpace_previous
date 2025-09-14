using System;
using System.Collections.Generic;

namespace GameSpace.Models;

public partial class EVoucherType
{
    public int EvoucherTypeId { get; set; }

    public string Name { get; set; } = null!;

    public decimal ValueAmount { get; set; }

    public DateTime ValidFrom { get; set; }

    public DateTime ValidTo { get; set; }

    public int PointsCost { get; set; }

    public int TotalAvailable { get; set; }

    public string? Description { get; set; }
}
