using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using CinemaCore.Models;
using Microsoft.EntityFrameworkCore;

namespace WebCinema;

public partial class CinemaContext : DbContext
{
    public CinemaContext()
    {
    }

    public CinemaContext(DbContextOptions<CinemaContext> options)
    : base(options)
    {
    }

    public virtual DbSet<Actors> Actors { get; set; }

    public virtual DbSet<ActorCasts> ActorCasts { get; set; }

    public virtual DbSet<CinemaHalls> CinemaHalls { get; set; }

    public virtual DbSet<CountryProductions> CountryProductions { get; set; }

    public virtual DbSet<Films> Films { get; set; }

    public virtual DbSet<FilmProductions> FilmProductions { get; set; }

    public virtual DbSet<Genres> Genres { get; set; }

    public virtual DbSet<ListEvents> ListEvents { get; set; }

    public virtual DbSet<Places> Places { get; set; }

    public virtual DbSet<Staffs> Staffs { get; set; }

    public virtual DbSet<StaffCasts> StaffCasts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(ConnectionStringProvider.GetConnectionString("DefaultConnection"));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Actors>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Actors__3214EC074E0FE264");

            entity.Property(e => e.MiddleName).HasMaxLength(60);
            entity.Property(e => e.Name).HasMaxLength(60);
            entity.Property(e => e.Surname).HasMaxLength(60);
        });

        modelBuilder.Entity<ActorCasts>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ActorCas__3214EC077655889E");

            entity.HasOne(d => d.Actor).WithMany(p => p.ActorCasts)
                .HasForeignKey(d => d.ActorId)
                .HasConstraintName("FK__ActorCast__Actor__30F848ED");

            entity.HasOne(d => d.Film).WithMany(p => p.ActorCasts)
                .HasForeignKey(d => d.FilmId)
                .HasConstraintName("FK__ActorCast__FilmI__31EC6D26");
        });

        modelBuilder.Entity<CinemaHalls>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CinemaHa__3214EC07E49505F4");
        });

        modelBuilder.Entity<CountryProductions>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CountryP__3214EC0794D456C8");

            entity.Property(e => e.Name).HasMaxLength(150);
        });

        modelBuilder.Entity<Films>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Films__3214EC0719B62E2F");

            entity.Property(e => e.Description).HasMaxLength(350);
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.CountryProduction).WithMany(p => p.Films)
                .HasForeignKey(d => d.CountryProductionId)
                .HasConstraintName("FK__Films__Descripti__2C3393D0");

            entity.HasOne(d => d.FilmProduction).WithMany(p => p.Films)
                .HasForeignKey(d => d.FilmProductionId)
                .HasConstraintName("FK__Films__FilmProdu__2D27B809");

            entity.HasOne(d => d.Genre).WithMany(p => p.Films)
                .HasForeignKey(d => d.GenreId)
                .HasConstraintName("FK__Films__GenreId__2E1BDC42");
        });

        modelBuilder.Entity<FilmProductions>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__FilmProd__3214EC07B0DFD8B3");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Genres>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Genres__3214EC07448D23DC");

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<ListEvents>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ListEven__3214EC073D3C01CD");

            entity.Property(e => e.Date).HasColumnType("date");
            //entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.TicketPrice).HasColumnType("money");

            entity.HasOne(d => d.Film).WithMany(p => p.ListEvents)
                .HasForeignKey(d => d.FilmId)
                .HasConstraintName("FK__ListEvent__FilmI__36B12243");
        });

        modelBuilder.Entity<Places>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Places__3214EC075730CEBE");

            entity.HasOne(d => d.CinemaHall).WithMany(p => p.Places)
                .HasForeignKey(d => d.CinemaHallId)
                .HasConstraintName("FK__Places__CinemaHa__3F466844");

            entity.HasOne(d => d.ListEvent).WithMany(p => p.Places)
                .HasForeignKey(d => d.ListEventId)
                .HasConstraintName("FK__Places__ListEven__403A8C7D");
        });

        modelBuilder.Entity<Staffs>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Staffs__3214EC071C662BFD");

            entity.Property(e => e.MiddleName).HasMaxLength(60);
            entity.Property(e => e.Name).HasMaxLength(60);
            entity.Property(e => e.Post).HasMaxLength(150);
            entity.Property(e => e.Surname).HasMaxLength(60);
        });

        modelBuilder.Entity<StaffCasts>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__StaffCas__3214EC07BCCB72C7");

            entity.HasOne(d => d.ListEvent).WithMany(p => p.StaffCasts)
                .HasForeignKey(d => d.ListEventId)
                .HasConstraintName("FK__StaffCast__ListE__3A81B327");

            entity.HasOne(d => d.Staff).WithMany(p => p.StaffCasts)
                .HasForeignKey(d => d.StaffId)
                .HasConstraintName("FK__StaffCast__Staff__398D8EEE");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
