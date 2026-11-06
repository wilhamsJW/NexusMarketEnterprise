using NME.Core.Messages;

namespace NME.Core.DomainObjects
{
    /// <summary>
    /// OBJETIVO: Classe base abstrata para todas as entidades do sistema.
    /// Gerencia identidade única (Id), comparações seguras e a lista de Eventos de Domínio.
    /// </summary>
    public abstract class Entity
    {
        // Identificador único da entidade (Protegido para alteração externa)
        public Guid Id { get; protected set; }

        // O Construtor: Garante que toda entidade nasce obrigatoriamente com um ID novo gerado
        protected Entity()
        {
            Id = Guid.NewGuid();
        }

        // ==========================================
        // GERENCIAMENTO DE EVENTOS DE DOMÍNIO
        // ==========================================

        // ==========================================
        // GERENCIAMENTO DE EVENTOS DE DOMÍNIO
        // ==========================================

        // Variável privada que armazena a lista de eventos. 
        // Se ninguém adicionou nada ainda, ela nasce vazia (null) para economizar memória.
        private List<Event> _notificacoes;

        /// <summary>
        /// Propriedade pública apenas para leitura (IReadOnlyCollection).
        /// - Se ninguém adicionou eventos, o operador (??=) cria uma lista vazia para evitar erros de nulo.
        /// - Se alguém já adicionou eventos, ela retorna a lista real contendo os fatos que aconteceram.
        /// - Quem chama esta propriedade é a infraestrutura (como o EF Core) no momento em que 
        ///   o banco de dados termina de salvar, para verificar se há notificações pendentes e dispará-las.
        /// </summary>
        public IReadOnlyCollection<Event> Notificacoes => (_notificacoes ??= new List<Event>()).AsReadOnly();

        /// <summary>
        /// OBJETIVO: Adiciona um novo Evento de Domínio à lista interna da entidade.
        /// </summary>
        public void AdicionarEvento(Event evento)
        {
            _notificacoes = _notificacoes ?? new List<Event>();
            _notificacoes.Add(evento);
        }

        /// <summary>
        /// OBJETIVO: Remove um evento específico da lista interna, caso necessário.
        /// </summary>
        public void RemoverEvento(Event eventItem)
        {
            _notificacoes?.Remove(eventItem);
        }

        /// <summary>
        /// OBJETIVO: Limpa todos os eventos após eles serem processados e despachados com sucesso.
        /// </summary>
        public void LimparEventos()
        {
            _notificacoes?.Clear();
        }

        #region Comparações (Equals, Operators e HashCode)

        public override bool Equals(object obj)
        {
            var compareTo = obj as Entity;

            if (ReferenceEquals(this, compareTo)) return true;
            if (ReferenceEquals(null, compareTo)) return false;

            return Id.Equals(compareTo.Id);
        }

        public static bool operator ==(Entity a, Entity b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return true;

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(Entity a, Entity b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return (GetType().GetHashCode() * 907) + Id.GetHashCode();
        }

        public override string ToString()
        {
            return $"{GetType().Name} [Id={Id}]";
        }

        #endregion
    }
}