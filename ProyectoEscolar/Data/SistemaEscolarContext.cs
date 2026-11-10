using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using ProyectoEscolar.Models;

namespace ProyectoEscolar.Data;

public partial class SistemaEscolarContext : DbContext
{
    public SistemaEscolarContext()
    {
    }

    public SistemaEscolarContext(DbContextOptions<SistemaEscolarContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Alumno> Alumnos { get; set; }

    public virtual DbSet<Curso> Cursos { get; set; }

    public virtual DbSet<Docente> Docentes { get; set; }

    public virtual DbSet<Documentacione> Documentaciones { get; set; }

    public virtual DbSet<InscripcionDocumentacione> InscripcionDocumentaciones { get; set; }

    public virtual DbSet<Inscripcione> Inscripciones { get; set; }

    public virtual DbSet<TiposDocumento> TiposDocumentos { get; set; }

    public virtual DbSet<Tutore> Tutores { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=127.0.0.1;port=3307;database=SistemaEscolar;user=root;password=Admin123!", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.32-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Alumno>(entity =>
        {
            entity.HasKey(e => e.IdAlumno).HasName("PRIMARY");

            entity.ToTable("ALUMNOS");

            entity.HasIndex(e => e.IdTutor, "FK_alumnos_tutores");

            entity.HasIndex(e => e.Dni, "dni").IsUnique();

            entity.Property(e => e.IdAlumno).HasColumnName("idAlumno");
            entity.Property(e => e.Activo)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("activo");
            entity.Property(e => e.Apellido)
                .HasMaxLength(100)
                .HasColumnName("apellido");
            entity.Property(e => e.Direccion)
                .HasMaxLength(100)
                .HasColumnName("direccion");
            entity.Property(e => e.Dni)
                .HasMaxLength(50)
                .HasColumnName("dni");
            entity.Property(e => e.FechaNacimiento).HasColumnName("fechaNacimiento");
            entity.Property(e => e.IdTutor).HasColumnName("idTutor");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .HasColumnName("telefono");

            entity.HasOne(d => d.IdTutorNavigation).WithMany(p => p.Alumnos)
                .HasForeignKey(d => d.IdTutor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_alumnos_tutores");
        });

        modelBuilder.Entity<Curso>(entity =>
        {
            entity.HasKey(e => e.IdCurso).HasName("PRIMARY");

            entity.ToTable("CURSOS");

            entity.HasIndex(e => e.IdDocente, "FK_Cursos_Docentes");

            entity.Property(e => e.IdCurso).HasColumnName("idCurso");
            entity.Property(e => e.Activo)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("activo");
            entity.Property(e => e.AnioCurso).HasColumnName("anioCurso");
            entity.Property(e => e.CupoMaximo).HasColumnName("cupoMaximo");
            entity.Property(e => e.Division)
                .HasMaxLength(10)
                .HasColumnName("division");
            entity.Property(e => e.IdDocente).HasColumnName("idDocente");
            entity.Property(e => e.Nivel)
                .HasColumnType("enum('Inicial','Primaria','Secundaria')")
                .HasColumnName("nivel");
            entity.Property(e => e.Turno)
                .HasColumnType("enum('Mañana','Tarde')")
                .HasColumnName("turno");

            entity.HasOne(d => d.IdDocenteNavigation).WithMany(p => p.Cursos)
                .HasForeignKey(d => d.IdDocente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cursos_Docentes");
        });

        modelBuilder.Entity<Docente>(entity =>
        {
            entity.HasKey(e => e.IdDocente).HasName("PRIMARY");

            entity.ToTable("DOCENTES");

            entity.HasIndex(e => e.Dni, "dni").IsUnique();

            entity.HasIndex(e => e.Legajo, "legajo").IsUnique();

            entity.Property(e => e.IdDocente).HasColumnName("idDocente");
            entity.Property(e => e.Activo)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("activo");
            entity.Property(e => e.Apellido)
                .HasMaxLength(100)
                .HasColumnName("apellido");
            entity.Property(e => e.Dni)
                .HasMaxLength(20)
                .HasColumnName("dni");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Legajo)
                .HasMaxLength(50)
                .HasColumnName("legajo");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .HasColumnName("telefono");
        });

        modelBuilder.Entity<Documentacione>(entity =>
        {
            entity.HasKey(e => e.IdDocumentacion).HasName("PRIMARY");

            entity.ToTable("DOCUMENTACIONES");

            entity.HasIndex(e => e.IdTipoDocumento, "FK_Documentaciones_Tipos");

            entity.Property(e => e.IdDocumentacion).HasColumnName("idDocumentacion");
            entity.Property(e => e.FechaEmision).HasColumnName("fechaEmision");
            entity.Property(e => e.IdTipoDocumento).HasColumnName("idTipoDocumento");

            entity.HasOne(d => d.IdTipoDocumentoNavigation).WithMany(p => p.Documentaciones)
                .HasForeignKey(d => d.IdTipoDocumento)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Documentaciones_Tipos");
        });

        modelBuilder.Entity<InscripcionDocumentacione>(entity =>
        {
            entity.HasKey(e => new { e.IdInscripcion, e.IdDocumentacion })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("INSCRIPCION_DOCUMENTACIONES");

            entity.HasIndex(e => e.IdDocumentacion, "FK_InscripDoc_Documentacion");

            entity.Property(e => e.IdInscripcion).HasColumnName("idInscripcion");
            entity.Property(e => e.IdDocumentacion).HasColumnName("idDocumentacion");
            entity.Property(e => e.FechaPresentacion)
                .HasColumnType("datetime")
                .HasColumnName("fechaPresentacion");
            entity.Property(e => e.FechaValidacion)
                .HasColumnType("datetime")
                .HasColumnName("fechaValidacion");
            entity.Property(e => e.Observaciones)
                .HasMaxLength(500)
                .HasColumnName("observaciones");
            entity.Property(e => e.Presentado).HasColumnName("presentado");
            entity.Property(e => e.Validado).HasColumnName("validado");

            entity.HasOne(d => d.IdDocumentacionNavigation).WithMany(p => p.InscripcionDocumentaciones)
                .HasForeignKey(d => d.IdDocumentacion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InscripDoc_Documentacion");

            entity.HasOne(d => d.IdInscripcionNavigation).WithMany(p => p.InscripcionDocumentaciones)
                .HasForeignKey(d => d.IdInscripcion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InscripDoc_Inscripcion");
        });

        modelBuilder.Entity<Inscripcione>(entity =>
        {
            entity.HasKey(e => e.IdInscripcion).HasName("PRIMARY");

            entity.ToTable("INSCRIPCIONES");

            entity.HasIndex(e => e.IdCurso, "FK_Inscriciones_Cursos");

            entity.HasIndex(e => e.IdAlumno, "FK_Inscripciones_Alumnos");

            entity.Property(e => e.IdInscripcion).HasColumnName("idInscripcion");
            entity.Property(e => e.Activo)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("activo");
            entity.Property(e => e.CicloLectivo).HasColumnName("cicloLectivo");
            entity.Property(e => e.EstadoInscripcion)
                .HasDefaultValueSql("'Pendiente'")
                .HasColumnType("enum('Pendiente','Inscripto','En Lista de Espera','Baja/Rechazado')")
                .HasColumnName("estadoInscripcion");
            entity.Property(e => e.FechaResolucion)
                .HasColumnType("datetime")
                .HasColumnName("fechaResolucion");
            entity.Property(e => e.FechaSolicitud)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("fechaSolicitud");
            entity.Property(e => e.IdAlumno).HasColumnName("idAlumno");
            entity.Property(e => e.IdCurso).HasColumnName("idCurso");
            entity.Property(e => e.Observacion)
                .HasMaxLength(500)
                .HasColumnName("observacion");

            entity.HasOne(d => d.IdAlumnoNavigation).WithMany(p => p.Inscripciones)
                .HasForeignKey(d => d.IdAlumno)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Inscripciones_Alumnos");

            entity.HasOne(d => d.IdCursoNavigation).WithMany(p => p.Inscripciones)
                .HasForeignKey(d => d.IdCurso)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Inscriciones_Cursos");
        });

        modelBuilder.Entity<TiposDocumento>(entity =>
        {
            entity.HasKey(e => e.IdTipoDocumento).HasName("PRIMARY");

            entity.ToTable("TIPOS_DOCUMENTOS");

            entity.Property(e => e.IdTipoDocumento).HasColumnName("idTipoDocumento");
            entity.Property(e => e.Activo)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("activo");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.Obligatorio)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("obligatorio");
        });

        modelBuilder.Entity<Tutore>(entity =>
        {
            entity.HasKey(e => e.IdTutor).HasName("PRIMARY");

            entity.ToTable("TUTORES");

            entity.HasIndex(e => e.Dni, "dni").IsUnique();

            entity.Property(e => e.IdTutor).HasColumnName("idTutor");
            entity.Property(e => e.Activo)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("activo");
            entity.Property(e => e.Apellido)
                .HasMaxLength(100)
                .HasColumnName("apellido");
            entity.Property(e => e.Dni)
                .HasMaxLength(20)
                .HasColumnName("dni");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .HasColumnName("telefono");
            entity.Property(e => e.Tipo)
                .HasColumnType("enum('Padre','Madre','Tutor Legal')")
                .HasColumnName("tipo");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
