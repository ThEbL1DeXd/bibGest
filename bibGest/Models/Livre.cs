using System;
using System.Collections.Generic;

namespace bibGest.Models;

public partial class Livre
{
    public int LivreId { get; set; }

    public string Titre { get; set; } = null!;

    public string Auteur { get; set; } = null!;

    public string Isbn { get; set; } = null!;

    public int? AnneePublication { get; set; }

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public int QuantiteTotale { get; set; }

    public int QuantiteDisponible { get; set; }

    public int? CategorieId { get; set; }

    public virtual Category? Categorie { get; set; }

    public virtual ICollection<Emprunt> Emprunts { get; set; } = new List<Emprunt>();

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
