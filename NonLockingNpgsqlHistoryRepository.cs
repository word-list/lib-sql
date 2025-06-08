using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Migrations.Internal;

namespace WordList.Data.Sql;

#pragma warning disable EF1001
public class NonLockingNpgsqlHistoryRepository(HistoryRepositoryDependencies dependencies)
    : NpgsqlHistoryRepository(dependencies)
{
    public override IMigrationsDatabaseLock AcquireDatabaseLock()
        => new NoopMigrationsDatabaseLock(this);

    public override Task<IMigrationsDatabaseLock> AcquireDatabaseLockAsync(
        CancellationToken cancellationToken = default)
        => Task.FromResult<IMigrationsDatabaseLock>(new NoopMigrationsDatabaseLock(this));

    class NoopMigrationsDatabaseLock(NpgsqlHistoryRepository historyRepository) : IMigrationsDatabaseLock
    {
        public void Dispose() { }
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
        public IHistoryRepository HistoryRepository => historyRepository;
    }
}
#pragma warning restore EF1001