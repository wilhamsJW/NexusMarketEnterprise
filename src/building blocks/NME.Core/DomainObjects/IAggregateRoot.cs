namespace NME.Core.DomainObjects
{
    /// <summary>
    /// Interface de Marcação (Marker Interface).
    /// Não possui métodos porque serve exclusivamente para "etiquetar"
    /// quais Entidades do sistema são Raízes de Agregação no DDD.
    /// Exemplo: Produto, Cliente e Pedido assinam esta interface.
    /// </summary>
    public interface IAggregateRoot
    {
    }
}