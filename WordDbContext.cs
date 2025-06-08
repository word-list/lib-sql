using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;
using WordList.Data.Sql.Models;

namespace WordList.Data.Sql;

public class WordDbContext : DbContext
{
    public DbSet<Word> Words { get; set; }
    public DbSet<WordType> Types { get; set; }

    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "This will not be used with AOT")]
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Will not be used with AOT")]
    public WordDbContext(DbContextOptions options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var envConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

        if (string.IsNullOrEmpty(envConnectionString))
        {
            throw new InvalidOperationException("DB_CONNECTION_STRING environment variable is not set.");
        }

        var connectionStringUri = new Uri(envConnectionString);

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = connectionStringUri.Host,
            Username = connectionStringUri.UserInfo.Split(':')[0],
            Password = connectionStringUri.UserInfo.Split(':')[1],
            Database = connectionStringUri.LocalPath.Trim('/'),
            Port = connectionStringUri.Port,
            Timeout = 120,
            SslMode = SslMode.VerifyFull
        };

        optionsBuilder
            .UseNpgsql(builder.ToString())
            // Fix issue introduced by locking in EF 9 - https://github.com/dotnet/efcore/issues/33731
            .ReplaceService<IHistoryRepository, NonLockingNpgsqlHistoryRepository>();
    }


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