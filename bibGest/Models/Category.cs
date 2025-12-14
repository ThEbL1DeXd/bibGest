using System;
using System.Collections.Generic;

namespace bibGest.Models;

public partial class Category
{
    public int CategorieId { get; set; }

    public string NomCategorie { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Livre> Livres { get; set; } = new List<Livre>();
}
