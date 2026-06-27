using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;

namespace Achievement.Hubs;

/// <summary>
/// Analisa a presença de utilizadores online e envia atualizações em tempo real para todos os clientes conectados.
/// https://learn.microsoft.com/en-us/aspnet/core/tutorials/signalr?view=aspnetcore-10.0&tabs=visual-studio
/// </summary>
public class PresenceHub : Hub
{
    // chave da pessoa -> nº de ligações abertas (separadores).
    private static readonly ConcurrentDictionary<string, int> People = new();

    /// <summary>
    /// Para cada ligação aberta, incrementa o contador de pessoas online e envia a contagem atualizada para todos os clientes conectados.
    /// </summary>
    /// <returns></returns>
    public override async Task OnConnectedAsync()
    {
        var key = GetKey();
        Context.Items["key"] = key;
        People.AddOrUpdate(key, 1, (_, c) => c + 1);
        await BroadcastCount();
        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Para cada ligação fechada, decrementa o contador de pessoas online e envia a contagem atualizada para todos os clientes conectados.
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (Context.Items["key"] is string key && People.AddOrUpdate(key, 0, (_, c) => c - 1) <= 0)
            People.TryRemove(key, out _);
        await BroadcastCount();
        await base.OnDisconnectedAsync(exception);
    }

    // Autenticado: Evita ligações duplicadas por userId. Anónimo: conta por ligação.
    private string GetKey() => Context.UserIdentifier ?? Context.ConnectionId;

    private Task BroadcastCount() => Clients.All.SendAsync("online", People.Count);
}
