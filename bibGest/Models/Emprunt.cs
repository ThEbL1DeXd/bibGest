using System;
using System.Collections.Generic;

namespace bibGest.Models;

public partial class Emprunt
{
    public int EmpruntId { get; set; }

    public int LivreId { get; set; }

    public int UtilisateurId { get; set; }

    public DateTime DateEmprunt { get; set; }

    public DateTime DateRetourPrevue { get; set; }

    public DateTime? DateRetourReelle { get; set; }

    public int Statut { get; set; }

    public virtual Livre Livre { get; set; } = null!;

    public virtual ICollection<Penalite> Penalites { get; set; } = new List<Penalite>();

    public virtual Utilisateur Utilisateur { get; set; } = null!;
}
