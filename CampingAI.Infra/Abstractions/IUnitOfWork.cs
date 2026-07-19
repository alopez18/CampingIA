using System.Data;

namespace CampingAI.Infra.Abstractions;

/// <summary>
/// Unidad de trabajo que comparte una única conexión y una transacción opcional
/// entre los repositorios de escritura, permitiendo confirmar o revertir de forma
/// atómica varias operaciones dentro del mismo ámbito (scope).
/// </summary>
public interface IUnitOfWork : IDisposable {
    /// <summary>Conexión compartida durante el ámbito actual (se crea de forma perezosa).</summary>
    IDbConnection Connection { get; }

    /// <summary>Transacción activa, o <c>null</c> si no se ha iniciado ninguna.</summary>
    IDbTransaction? CurrentTransaction { get; }

    /// <summary>Inicia una nueva transacción sobre la conexión compartida.</summary>
    Task BeginTransactionAsync();

    /// <summary>Confirma la transacción activa. Revierte automáticamente si el commit falla.</summary>
    Task CommitAsync();

    /// <summary>Revierte la transacción activa. No hace nada si no hay ninguna activa.</summary>
    Task RollbackAsync();
}
