
using NME.Core.Messages;
using NME.Core.Messages.Integration;

namespace NME.Core.Messages.Integration
{
    public abstract class IntegrationEvent : Event
    {
    }
}

/// <summary>
/// ========================================================================
/// SUMMARY: UsuarioRegistradoIntegrationEvent
/// ========================================================================
/// OBJETIVO: 
/// Representa um Evento de Integração (Integration Event) disparado quando um 
/// novo usuário é cadastrado com sucesso no sistema. 
/// 
/// POR QUE ESTÁ NO CORE?
/// Por se tratar de um contrato de integração inter-serviços (que vai trafegar 
/// via mensageria no RabbitMQ usando o padrão Request-Response ou EDA), esta 
/// classe e suas dependências base precisam morar no Core (Shared Kernel) 
/// para serem compartilhadas de forma limpa entre o produtor e os consumidores.
/// 
/// CONCEITOS APLICADOS:
/// - Arquitetura Orientada a Eventos (EDA): Desacopla o fluxo de cadastro 
///   permitindo que outros microsserviços reajam à criação do usuário.
/// - Imutabilidade: Propriedades possuem 'private set' e são inicializadas 
///   obrigatoriamente pelo construtor, garantindo que o evento não seja corrompido 
///   durante o trânsito na rede.
/// ===== /// </summary>
public class UsuarioRegistradoIntegrationEvent : IntegrationEvent
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; }
    public string Email { get; private set; }
    public string Cpf { get; private set; }

    // O Construtor recebe os dados e os injeta na classe de forma imutável (boa prática!)
    public UsuarioRegistradoIntegrationEvent(Guid id, string nome, string email, string cpf)
    {
        Id = id;
        Nome = nome;
        Email = email;
        Cpf = cpf;
    }
}

//Estamos totalmente dentro do ecossistema de EDA (Event-Driven Architecture / Arquitetura 
//    Orientada a Eventos) e, especificamente, implementando o padrão de comunicação que 
//    discutimos antes: o Request-Response Pattern (ou RPC - Remote Procedure Call) usando
//    filas.

//1. Estamos corretos na afirmação?
//Sim, 100%. Arquitetura Orientada a Eventos não significa apenas disparar mensagens e
//"esquecer" que elas existem (como no Pub/Sub clássico). Em sistemas corporativos, 
//existem fluxos assíncronos onde uma aplicação precisa perguntar algo para outra
//microsserviço através do RabbitMQ e esperar o retorno (a resposta) para prosseguir
//ou responder à tela do usuário. É exatamente isso que o Request-Response Pattern faz 
//utilizando o broker de mensageria.
