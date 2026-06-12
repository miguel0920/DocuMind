using DocuMind.Domain;
using Microsoft.EntityFrameworkCore;

namespace DocuMind.Infrastructure.Persistence.Contexts;

public class DocuMindDbContext(DbContextOptions<DocuMindDbContext> options) : DbContext(options)
{
    public DbSet<DocumentChunk> DocumentChunks => Set<DocumentChunk>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 1. Le decimos a Postgres que registre la extensión de vectores de forma explícita
        modelBuilder.HasPostgresExtension("vector");

        // 2. Configuramos nuestra entidad DocumentChunk
        modelBuilder.Entity<DocumentChunk>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DocumentName).IsRequired().HasMaxLength(250);
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.ChunkIndex).IsRequired();

            entity.Property(e => e.Embedding)
                .HasColumnType("vector(768)");

            entity.HasGeneratedTsVectorColumn(e => e.SearchVector, 
                "spanish", 
                e => new { e.DocumentName, e.Content }) // Columnas que se van a indexar
                .HasIndex(e => e.SearchVector)
                .HasMethod("GIN");
        });
    }
}