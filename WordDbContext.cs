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

    [RequiresUnreferencedCode("")]
    [RequiresDynamicCode("")]
    public WordDbContext(DbContextOptions options)
        : base(options)
    {
    }

    [RequiresUnreferencedCode("")]
    [RequiresDynamicCode("")]
    public WordDbContext() : base()
    {

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var envConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

        if (!string.IsNullOrEmpty(envConnectionString))
        {
            var connectionStringUri = new Uri(envConnectionString);

            var userInfo = Uri.UnescapeDataString(connectionStringUri.UserInfo).Split(':');

            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = connectionStringUri.Host,
                Username = userInfo[0],
                Password = userInfo[1],
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