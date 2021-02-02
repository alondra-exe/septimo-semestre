using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MVCUnidad2Actividad4.Models
{
    public partial class pixarContext : DbContext
    {
        public pixarContext()
        {
        }

        public pixarContext(DbContextOptions<pixarContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Apariciones> Apariciones { get; set; }
        public virtual DbSet<Categoria> Categoria { get; set; }
        public virtual DbSet<Cortometraje> Cortometraje { get; set; }
        public virtual DbSet<Pelicula> Pelicula { get; set; }
        public virtual DbSet<Personaje> Personaje { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySql("server=localhost;user=root;password=root;database=pixar", x => x.ServerVersion("5.7.18-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Apariciones>(entity =>
            {
                entity.ToTable("apariciones");

                entity.HasIndex(e => e.IdPelicula)
                    .HasName("fkPelicula_idx");

                entity.HasIndex(e => e.IdPersonaje)
                    .HasName("fkPersonaje_idx");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.IdPelicula).HasColumnType("int(11)");

                entity.Property(e => e.IdPersonaje).HasColumnType("int(11)");

                entity.HasOne(d => d.IdPeliculaNavigation)
                    .WithMany(p => p.Apariciones)
                    .HasForeignKey(d => d.IdPelicula)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fkPelicula");

                entity.HasOne(d => d.IdPersonajeNavigation)
                    .WithMany(p => p.Apariciones)
                    .HasForeignKey(d => d.IdPersonaje)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fkPersonaje");
            });

            modelBuilder.Entity<Categoria>(entity =>
            {
                entity.ToTable("categoria");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Nombre)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<Cortometraje>(entity =>
            {
                entity.ToTable("cortometraje");

                entity.HasIndex(e => e.IdCategoria)
                    .HasName("fkCategoria_idx");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Descripcion)
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.IdCategoria).HasColumnType("int(11)");

                entity.Property(e => e.Nombre)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.HasOne(d => d.IdCategoriaNavigation)
                    .WithMany(p => p.Cortometraje)
                    .HasForeignKey(d => d.IdCategoria)
                    .HasConstraintName("fkCategoria");
            });

            modelBuilder.Entity<Pelicula>(entity =>
            {
                entity.ToTable("pelicula");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Descripción)
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.FechaEstreno).HasColumnType("date");

                entity.Property(e => e.Nombre)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.NombreOriginal)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<Personaje>(entity =>
            {
                entity.ToTable("personaje");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Nombre)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
