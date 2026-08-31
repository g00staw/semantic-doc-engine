using SemanticDocEngine.Api.Domain;
using SemanticDocEngine.Api.Features.Documents;

namespace SemanticDocEngine.Api.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    public DbSet<Document> Documents { get; set; }
    public DbSet<DocumentChunk> DocumentChunks { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasPostgresExtension("vector");
        
        modelBuilder.Entity<Document>()
            .HasMany<DocumentChunk>()
            .WithOne()
            .HasForeignKey(chunk => chunk.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Document>()
            .Property(d => d.Title)
            .HasMaxLength(255);
        modelBuilder.Entity<Document>()
            .Property(d => d.Status)
            .HasConversion<string>();
        
        modelBuilder.Entity<DocumentChunk>()
            .Property(p => p.Embedding)
            .HasColumnType("vector(768)");
        

    }
    
}