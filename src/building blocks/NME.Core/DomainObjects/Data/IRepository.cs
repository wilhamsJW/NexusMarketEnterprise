using System;

namespace NME.Core.DomainObjects.Data
{
    /// <summary>
    /// Contrato genérico base para todos os Repositórios da solução.
    /// </summary>
    /// <typeparam name="T">A Entidade de Domínio manipulada pelo repositório.</typeparam>
    /// <remarks>
    /// - Herda 'IDisposable' para garantir a liberação de recursos de banco da memória.
    /// - A cláusula 'where T : IAggregateRoot' garante que APENAS Entidades Raízes
    ///   de Agregação possam possuir um repositório próprio.
    /// </remarks>
    public interface IRepository<T> : IDisposable where T : IAggregateRoot
    {
    }
}