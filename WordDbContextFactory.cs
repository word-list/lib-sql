using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace WordList.Data.Sql;

public class WordDbContextFactory : IDesignTimeDbContextFactory<WordDbContext>
{
    public WordDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<WordDbContext>();
        optionsBuilder.UseNpgsql();

        return new WordDbContext(optionsBuilder.Options);
    }
}