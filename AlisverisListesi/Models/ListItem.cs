using System;
using System.Collections.Generic;

namespace AlisverisListesi.Models;

public partial class ListItem
{
    public int ListItemId { get; set; }

    public int? ListId { get; set; }

    public int? ProductId { get; set; }

    public int? Quantity { get; set; }

    public virtual ShoppingList? List { get; set; }

    public virtual Product? Product { get; set; }
}
