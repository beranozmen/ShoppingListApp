using System;
using System.Collections.Generic;

namespace AlisverisListesi.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string? Name { get; set; }

    public int? CategoryId { get; set; }

    public string? ImageUrl { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<ListItem> ListItems { get; set; } = new List<ListItem>();
}
