using System;
using System.Collections.Generic;

namespace bibGest.Models;

public partial class Penalite
{
    public int PenaliteId { get; set; }

    public int EmpruntId { get; set; }

    public decimal Montant { get; set; }

    public DateTime DateCreation { get; set; }

    public bool EstPayee { get; set; }

    public virtual Emprunt Emprunt { get; set; } = null!;
}
