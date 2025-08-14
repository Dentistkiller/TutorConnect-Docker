using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TutorConnectDocker.Models;

public partial class TutorContext : DbContext
{
    public TutorContext()
    {
    }

    public TutorContext(DbContextOptions<TutorContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Sessions> Sessions { get; set; }

    public virtual DbSet<Students> Students { get; set; }

    public virtual DbSet<Tutors> Tutors { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost,1433;Database=TutorDb;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Sessions>(entity =>
        {
            entity.HasKey(e => e.SessionId).HasName("PK__Sessions__C9F49290479ADA55");

            entity.HasOne(d => d.Student).WithMany(p => p.Sessions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Sessions__Studen__3D5E1FD2");

            entity.HasOne(d => d.Tutor).WithMany(p => p.Sessions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Sessions__TutorI__3E52440B");
        });

        modelBuilder.Entity<Students>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__Students__32C52B991466C7B9");
        });

        modelBuilder.Entity<Tutors>(entity =>
        {
            entity.HasKey(e => e.TutorId).HasName("PK__Tutors__77C70FE2ADA2A29A");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
