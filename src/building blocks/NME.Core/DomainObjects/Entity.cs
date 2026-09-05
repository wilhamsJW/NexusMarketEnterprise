using System;

namespace NME.Core.DomainObjects
{
    // 'abstract': Significa que esta classe é um "molde". 
    // Você NUNCA fará 'new Entity()'. Você só fará 'new Produto()', 'new Cliente()', etc.
    public abstract class Entity
    {
        // 1. A IDENTIDADE ÚNICA (Tipo um RG)
        // Toda classe que herdar de Entity ganha essa propriedade automaticamente.
        public Guid Id { get; set; }

        // 2. A SOBRESCRITA DO EQUALS (A regra do DDD)
        // Este método é chamado AUTOMATICAMENTE por listas (.Remove, .Contains) do C#.
        public override bool Equals(object obj)
        {
            // Tenta converter o objeto recebido para o tipo 'Entity'
            var compareTo = obj as Entity;

            // CHECAGEM 1: Os dois apontam para a MESMA posição de memória RAM?
            // Se sim, nem perde tempo comparando propriedade por propriedade. Já retorna 'true'.
            if (ReferenceEquals(this, compareTo)) return true;

            // CHECAGEM 2: O objeto que veio para comparar é NULO?
            // Se o outro lado for nulo, eles com certeza são diferentes. Retorna 'false'.
            if (ReferenceEquals(null, compareTo)) return false;

            // CHECAGEM 3: O PULO DO GATO!
            // Esquece a memória RAM! Compara apenas se o ID de um é igual ao ID do outro.
            return Id.Equals(compareTo.Id);
        }

        // 3. SOBRECARGA DO OPERADOR '=='
        // Ensina o C# o que fazer quando você digita: 'if (produtoA == produtoB)'
        public static bool operator ==(Entity a, Entity b)
        {
            // Se os DOIS lados do '==' forem NULOS (null == null), eles são iguais.
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return true;

            // Se APENAS UM dos lados for NULO (null == objeto), eles são diferentes.
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;

            // Se nenhum for nulo, chama o método Equals() lá de cima para comparar os IDs!
            return a.Equals(b);
        }

        // 4. SOBRECARGA DO OPERADOR '!='
        // Ensina o C# o que fazer quando você digita: 'if (produtoA != produtoB)'
        public static bool operator !=(Entity a, Entity b)
        {
            // É simplesmente o inverso (o negação '!') do operador '=='
            return !(a == b);
        }

        // 5. CÓDIGO HASH PARA BUSCAS RÁPIDAS
        // Chamado AUTOMATICAMENTE por 'Dictionary<Key, Value>' e 'HashSet<T>'.
        public override int GetHashCode()
        {
            // Pega o tipo da classe (ex: Produto) e multiplica por 907 (um número primo)
            // e soma com o código do ID.
            // Motivo: Cria um "número de etiqueta" único para o .NET achar esse objeto 
            // instantaneamente dentro de listas de alta performance sem varrer a lista toda.
            return (GetType().GetHashCode() * 907) + Id.GetHashCode();
        }

        // 6. IMPRESSÃO DE DIAGNÓSTICO (DEBUG)
        // Chamado AUTOMATICAMENTE quando você dá Console.WriteLine(produto) ou no Visual Studio.
        public override string ToString()
        {
            // Em vez de imprimir "NME.Core.DomainObjects.Produto",
            // vai imprimir algo limpo como: "Produto [Id=f3a2b1...]"
            return $"{GetType().Name} [Id={Id}]";
        }
    }
}