using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CatalogoBotanico.Models
{
    public partial class sistem14_botanicaContext : DbContext
    {
        public sistem14_botanicaContext()
        {
        }

        public sistem14_botanicaContext(DbContextOptions<sistem14_botanicaContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Admindata> Admindata { get; set; }
        public virtual DbSet<Plantdata> Plantdata { get; set; }
        public virtual DbSet<Userdata> Userdata { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql("server=204.93.167.23;user=sistem14_admin;password=admin2020;database=sistem14_botanica", x => x.ServerVersion("5.6.46-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admindata>(entity =>
            {
                entity.ToTable("admindata");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(15)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnType("tinytext")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<Plantdata>(entity =>
            {
                entity.ToTable("plantdata");

                entity.HasIndex(e => e.IdUser)
                    .HasName("fk_IdUser");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.CommonName)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Division)
                    .IsRequired()
                    .HasColumnType("tinytext")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Family)
                    .IsRequired()
                    .HasColumnType("tinytext")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Gender)
                    .IsRequired()
                    .HasColumnType("tinytext")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.IdUser).HasColumnType("int(11)");

                entity.Property(e => e.Info)
                    .IsRequired()
                    .HasColumnType("varchar(150)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ScientificName)
                    .IsRequired()
                    .HasColumnType("tinytext")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Subfamily)
                    .IsRequired()
                    .HasColumnType("tinytext")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.HasOne(d => d.IdUserNavigation)
                    .WithMany(p => p.Plantdata)
                    .HasForeignKey(d => d.IdUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_IdUser");
            });

            modelBuilder.Entity<Userdata>(entity =>
            {
                entity.ToTable("userdata");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Active)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.Alias)
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("'Usuario'")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Bio)
                    .HasColumnType("varchar(150)")
                    .HasDefaultValueSql("'No hay información.'")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Code).HasColumnType("int(5)");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnType("varchar(320)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(15)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnType("tinytext")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
