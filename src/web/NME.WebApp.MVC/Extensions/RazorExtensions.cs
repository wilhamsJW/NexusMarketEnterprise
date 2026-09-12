using Microsoft.AspNetCore.Mvc.Razor;
using System.Globalization;

namespace NME.WebApp.MVC.Extensions;

public static class RazorExtensions
{
    // Método para formatar valores monetários em Real (R$)
    public static string FormatoMoeda(this RazorPage page, decimal valor)
    {
        return valor.ToString("C", new CultureInfo("pt-BR"));
    }

    // Método para exibir mensagem amigável de estoque
    public static string MensagemEstoque(this RazorPage page, int quantidade)
    {
        return quantidade > 0
            ? $"Apenas {quantidade} em estoque!"
            : "Produto indisponível no momento";
    }
}