namespace NME.Core.Messages.Integration
{
    /// <summary>
    /// OBJETIVO: Representa a mensagem de resposta padronizada (Response Message) 
    /// utilizada no padrão Request-Response (RPC) sobre o RabbitMQ.
    /// POR QUE ESTÁ NO CORE? Porque precisa ser compartilhada entre os microsserviços 
    /// produtores e consumidores que participam dessa comunicação assíncrona síncrona na ponta.
    /// </summary>
    public class ResponseMessage : Message
    {
        public bool ValidationResult { get; private set; }
        public string Message { get; private set; }

        // Construtor para inicializar a resposta do processamento remoto
        public ResponseMessage(bool validationResult, string message = null)
        {
            ValidationResult = validationResult;
            Message = message;
        }
    }
}
