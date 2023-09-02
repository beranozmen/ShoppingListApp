using System;
using System.Collections.Generic;

namespace AlisverisListesi.Models;

public partial class ShoppingList
{
    public int ListId { get; set; }

    public int? UserId { get; set; }

    public string? Name { get; set; }

    public bool? IsCompleted { get; set; }

    public virtual ICollection<ListItem> ListItems { get; set; } = new List<ListItem>();

    public virtual User? User { get; set; }
}
