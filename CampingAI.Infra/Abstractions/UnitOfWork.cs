using System.Data;
using System.Data.Common;

namespace CampingAI.Infra.Abstractions;

public sealed class UnitOfWork : IUnitOfWork {
    #region Dependencies
    readonly Configuration.Factories.Interfaces.ISqlConnectionFactory _sqlConnectionFactory;
    #endregion

    IDbConnection? _connection;
    IDbTransaction? _transaction;

    public UnitOfWork(Configuration.Factories.Interfaces.ISqlConnectionFactory sqlConnectionFactory) {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public IDbConnection Connection => _connection ??= _sqlConnectionFactory.CreateConnection();

    public IDbTransaction? CurrentTransaction => _transaction;

    public async Task BeginTransactionAsync() {
        if (_transaction is not null)
            throw new InvalidOperationException("Ya existe una transacción activa en esta unidad de trabajo.");

        var connection = Connection;
        if (connection.State != ConnectionState.Open) {
            if (connection is DbConnection asyncConnection)
                await asyncConnection.OpenAsync();
            else
                connection.Open();
        }

        _transaction = connection.BeginTransaction();
    }

    public Task CommitAsync() {
        if (_transaction is null)
            throw new InvalidOperationException("No hay ninguna transacción activa que confirmar.");

        try {
            _transaction.Commit();
        }
        catch {
            _transaction.Rollback();
            throw;
        }
        finally {
            DisposeTransaction();
        }

        return Task.CompletedTask;
    }

    public Task RollbackAsync() {
        if (_transaction is null)
            return Task.CompletedTask;

        try {
            _transaction.Rollback();
        }
        finally {
            DisposeTransaction();
        }

        return Task.CompletedTask;
    }

    private void DisposeTransaction() {
        _transaction?.Dispose();
        _transaction = null;
    }

    public void Dispose() {
        _transaction?.Dispose();
        _connection?.Dispose();
        _transaction = null;
        _connection = null;
    }
}
