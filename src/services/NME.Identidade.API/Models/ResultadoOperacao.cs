namespace NME.Identidade.API.Models
{
    public class ResultadoOperacao<T>
    {
        public bool Sucesso { get; private set; }
        public T Dados { get; private set; }
        public IEnumerable<string> Erros { get; private set; }

        private ResultadoOperacao(bool sucesso, T dados, IEnumerable<string> erros)
        {
            Sucesso = sucesso;
            Dados = dados;
            Erros = erros ?? Array.Empty<string>();
        }

        public static ResultadoOperacao<T> CriarSucesso(T dados)
        {
            return new ResultadoOperacao<T>(true, dados, null);
        }

        public static ResultadoOperacao<T> CriarFalha(params string[] erros)
        {
            return new ResultadoOperacao<T>(false, default, erros);
        }

        public static ResultadoOperacao<T> CriarFalhas(IEnumerable<string> erros)
        {
            return new ResultadoOperacao<T>(false, default, erros);
        }
    }
}