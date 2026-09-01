using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Polly;

namespace NME.Core
{
    public static class ResilienceExtensions
    {
        /// <summary>
        /// Aplica a pipeline padrão de resiliência HTTP com Polly v8 do Nexus Market Enterprise.
        /// </summary>
        public static IHttpStandardResiliencePipelineBuilder AddNmeStandardResilience(this IHttpClientBuilder builder)
        {
           return builder.AddStandardResilienceHandler(options =>
            {
                // === 1. TOTAL REQUEST TIMEOUT ===
                // Teto global da operação. Se a requisição inteira (somando o tempo da 1ª chamada + retentativas + esperas) 
                // ultrapassar 30 segundos, o Polly aborta tudo e lança TimeoutRejectedException.
                options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(30);

                // === 2. ATTEMPT TIMEOUT ===
                // Tempo limite máximo permitido para CADA TENTATIVA INDIVIDUAL HTTP.
                // Se a API externa não responder em 5 segundos nesta tentativa, a tentativa é cancelada e o Polly aciona a retentativa.
                options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(5);

                // === 3. RETRY (RETENTATIVAS) ===
                // Número máximo de tentativas adicionais caso a requisição inicial falhe com erros transitórios (5xx, timeouts, exceções de rede).
                options.Retry.MaxRetryAttempts = 3;

                // Tempo de espera inicial base antes de realizar a primeira retentativa.
                options.Retry.Delay = TimeSpan.FromSeconds(2);

                // Algoritmo de incremento do tempo de espera.
                // Exponential faz o tempo crescer exponencialmente a cada falha (ex: 2s -> 4s -> 8s) para dar tempo de a API destino se recuperar.
                options.Retry.BackoffType = DelayBackoffType.Exponential;

                // Adiciona uma variação aleatória de milissegundos ao tempo de espera (Jitter).
                // Evita o problema de "Thundering Herd" (múltiplas instâncias tentando reconectar exatamente no mesmo milissegundo, derrubando o servidor).
                options.Retry.UseJitter = true;

                // === 4. CIRCUIT BREAKER (DISJUNTOR) ===
                // Taxa de falha exigida para abrir o disjuntor (0.5 = 50%).
                // Se 50% das requisições falharem na janela de amostragem, o disjuntor ABRE e bloqueia chamadas imediatamente sem sobrecarregar a API.
                options.CircuitBreaker.FailureRatio = 0.5;

                // Volume mínimo de requisições necessárias dentro da janela para que o disjuntor comece a calcular a taxa de falhas.
                // Evita que o disjuntor abra com poucas requisições isoladas (ex: 1 falha em 2 chamadas no total).
                options.CircuitBreaker.MinimumThroughput = 5;

                // Janela de tempo contínua em que o Polly monitora a taxa de erros e o volume de requisições.
                options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(30);

                // Tempo em que o disjuntor permanece ABERTO impedindo novas chamadas.
                // Passados os 15 segundos, ele entra em estado "Meio-Aberto" (Half-Open) e testa uma nova chamada para verificar se o serviço recuperou.
                options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(15);
            });
        }
    }
}