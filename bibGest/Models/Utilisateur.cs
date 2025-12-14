using System;
using System.Collections.Generic;

namespace bibGest.Models;

public partial class Utilisateur
{
    public int UtilisateurId { get; set; }

    public string Nom { get; set; } = null!;

    public string Prenom { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string MotDePasseHash { get; set; } = null!;

    public string Role { get; set; } = null!;

    public string? Telephone { get; set; }

    public DateTime DateInscription { get; set; }

    public bool EstActif { get; set; }

    public virtual ICollection<Emprunt> Emprunts { get; set; } = new List<Emprunt>();

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
