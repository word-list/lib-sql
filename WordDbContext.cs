using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using WordList.Data.Sql.Models;

namespace WordList.Data.Sql;

public class WordDbContext : DbContext
{
    public DbSet<Word> Words { get; set; }
    public DbSet<WordType> Types { get; set; }

    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "This will not be used with AOT")]
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Will not be used with AOT")]
    public WordDbContext(DbContextOptions options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Word>()
            .HasKey(word => word.Text)
            .HasName("words");

        modelBuilder.Entity<Word>()
            .HasMany(w => w.WordTypes)
            .WithMany(wt => wt.Words);

        modelBuilder.Entity<WordType>()
            .HasKey(type => type.Text)
            .HasName("types");

        modelBuilder.Entity<WordType>()
            .HasMany(wt => wt.Words)
            .WithMany(w => w.WordTypes);
    }
}