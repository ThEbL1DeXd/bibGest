using System;
using System.Collections.Generic;

namespace bibGest.Models;

public partial class Reservation
{
    public int ReservationId { get; set; }

    public int LivreId { get; set; }

    public int UtilisateurId { get; set; }

    public DateTime DateReservation { get; set; }

    public string Statut { get; set; } = null!;

    public virtual Livre Livre { get; set; } = null!;

    public virtual Utilisateur Utilisateur { get; set; } = null!;
}
