using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System.Data;

namespace CampingAI.Infra;
public class UnitOfWork : Abstractions.IUnitOfWork {
    private readonly Models.REDARBOR_DB.REDARBOR_TTContext _context;
    private readonly ILogger<UnitOfWork> _logger;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(Models.REDARBOR_DB.REDARBOR_TTContext context,
                      ILogger<UnitOfWork> logger) {
        _context = context;
        _logger = logger;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {
        try {
            ApplyAuditInformation();
            var result = await _context.SaveChangesAsync(cancellationToken);

            _logger.LogDebug("Guardados {ChangesCount} cambios", result);

            return result;
        } catch (DbUpdateConcurrencyException ex) {
            _logger.LogError(ex, "Error de concurrencia al guardar cambios");
            throw new Exception("Los datos han sido modificados por otro usuario", ex);
        } catch (DbUpdateException ex) {
            _logger.LogError(ex, "Error al guardar cambios en la base de datos");
            throw new Exception("Error al guardar en la base de datos", ex);
        }
    }

    public async Task BeginTransactionAsync() {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync() {
        if (_transaction != null) {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync() {
        if (_transaction != null) {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    private void ApplyAuditInformation() {
        var entries = _context.ChangeTracker.Entries()
            .Where(e => e.Entity is Domain.Abstractions.Entities.IAuditableEntity &&
                       (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries) {
            var auditableEntity = (Domain.Abstractions.Entities.IAuditableEntity)entry.Entity;

            if (entry.State == EntityState.Added) {
                auditableEntity.Created();
            }

            if (entry.State == EntityState.Modified) {
                auditableEntity.Updated();
            }
        }
    }



    public void Dispose() {
        _transaction?.Dispose();
        _context?.Dispose();
    }
}