using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using bibGest.Models;

namespace bibGest.Data;

public partial class BibliothequeContext : DbContext
{
    public BibliothequeContext()
    {
    }

    public BibliothequeContext(DbContextOptions<BibliothequeContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Emprunt> Emprunts { get; set; }

    public virtual DbSet<Livre> Livres { get; set; }

    public virtual DbSet<Penalite> Penalites { get; set; }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<Utilisateur> Utilisateurs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Connection string is configured in Program.cs via dependency injection
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategorieId).HasName("PK__Categori__F643AD86A974D7FD");

            entity.HasIndex(e => e.NomCategorie, "UQ__Categori__9A363C71F8BCC5AB").IsUnique();

            entity.Property(e => e.CategorieId).HasColumnName("CategorieID");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.NomCategorie).HasMaxLength(100);
        });

        modelBuilder.Entity<Emprunt>(entity =>
        {
            entity.HasKey(e => e.EmpruntId).HasName("PK__Emprunts__629ED2775F6341B1");

            entity.Property(e => e.EmpruntId).HasColumnName("EmpruntID");
            entity.Property(e => e.DateEmprunt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DateRetourPrevue).HasColumnType("datetime");
            entity.Property(e => e.DateRetourReelle).HasColumnType("datetime");
            entity.Property(e => e.LivreId).HasColumnName("LivreID");
            entity.Property(e => e.UtilisateurId).HasColumnName("UtilisateurID");

            entity.HasOne(d => d.Livre).WithMany(p => p.Emprunts)
                .HasForeignKey(d => d.LivreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Emprunts__LivreI__5AEE82B9");

            entity.HasOne(d => d.Utilisateur).WithMany(p => p.Emprunts)
                .HasForeignKey(d => d.UtilisateurId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Emprunts__Utilis__5BE2A6F2");
        });

        modelBuilder.Entity<Livre>(entity =>
        {
            entity.HasKey(e => e.LivreId).HasName("PK__Livres__562AE7E70D473B3B");

            entity.HasIndex(e => e.Isbn, "UQ__Livres__447D36EA3A0F4217").IsUnique();

            entity.Property(e => e.LivreId).HasColumnName("LivreID");
            entity.Property(e => e.Auteur).HasMaxLength(150);
            entity.Property(e => e.CategorieId).HasColumnName("CategorieID");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500)
                .HasColumnName("ImageURL");
            entity.Property(e => e.Isbn)
                .HasMaxLength(20)
                .HasColumnName("ISBN");
            entity.Property(e => e.QuantiteDisponible).HasDefaultValue(1);
            entity.Property(e => e.QuantiteTotale).HasDefaultValue(1);
            entity.Property(e => e.Titre).HasMaxLength(255);

            entity.HasOne(d => d.Categorie).WithMany(p => p.Livres)
                .HasForeignKey(d => d.CategorieId)
                .HasConstraintName("FK__Livres__Categori__5070F446");
        });

        modelBuilder.Entity<Penalite>(entity =>
        {
            entity.HasKey(e => e.PenaliteId).HasName("PK__Penalite__9A6E955FD45877FC");

            entity.Property(e => e.PenaliteId).HasColumnName("PenaliteID");
            entity.Property(e => e.DateCreation)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EmpruntId).HasColumnName("EmpruntID");
            entity.Property(e => e.Montant).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Emprunt).WithMany(p => p.Penalites)
                .HasForeignKey(d => d.EmpruntId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Penalites__Empru__66603565");
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.ReservationId).HasName("PK__Reservat__B7EE5F04FC70F455");

            entity.Property(e => e.ReservationId).HasColumnName("ReservationID");
            entity.Property(e => e.DateReservation)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LivreId).HasColumnName("LivreID");
            entity.Property(e => e.Statut)
                .HasMaxLength(20)
                .HasDefaultValue("EnAttente");
            entity.Property(e => e.UtilisateurId).HasColumnName("UtilisateurID");

            entity.HasOne(d => d.Livre).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.LivreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reservati__Livre__60A75C0F");

            entity.HasOne(d => d.Utilisateur).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.UtilisateurId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reservati__Utili__619B8048");
        });

        modelBuilder.Entity<Utilisateur>(entity =>
        {
            entity.HasKey(e => e.UtilisateurId).HasName("PK__Utilisat__6CB6AE1FA3E86205");

            entity.HasIndex(e => e.Email, "UQ__Utilisat__A9D105343BEF7851").IsUnique();

            entity.Property(e => e.UtilisateurId).HasColumnName("UtilisateurID");
            entity.Property(e => e.DateInscription)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.EstActif).HasDefaultValue(true);
            entity.Property(e => e.MotDePasseHash).HasMaxLength(255);
            entity.Property(e => e.Nom).HasMaxLength(100);
            entity.Property(e => e.Prenom).HasMaxLength(100);
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasDefaultValue("Membre");
            entity.Property(e => e.Telephone).HasMaxLength(20);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
