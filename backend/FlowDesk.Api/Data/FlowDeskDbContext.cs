using FlowDesk.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FlowDesk.Api.Data;

public class FlowDeskDbContext(DbContextOptions<FlowDeskDbContext> options) : DbContext(options)
{
    public DbSet<Anfrage> Anfragen => Set<Anfrage>();
    public DbSet<Abteilung> Abteilungen => Set<Abteilung>();
    public DbSet<Standort> Standorte => Set<Standort>();
    public DbSet<KiVorschlag> KiVorschlaege => Set<KiVorschlag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Anfrage>()
            .HasOne(a => a.Standort)
            .WithMany()
            .HasForeignKey(a => a.StandortId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Anfrage>()
            .HasMany(a => a.Abteilungen)
            .WithMany()
            .UsingEntity(j => j.ToTable("AnfrageAbteilungen"));

        modelBuilder.Entity<KiVorschlag>()
            .HasOne(k => k.Anfrage)
            .WithOne(a => a.KiVorschlag)
            .HasForeignKey<KiVorschlag>(k => k.AnfrageId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<KiVorschlag>()
            .HasMany(k => k.VorgeschlageneAbteilungen)
            .WithMany()
            .UsingEntity(j => j.ToTable("KiVorschlagAbteilungen"));
    }
}
