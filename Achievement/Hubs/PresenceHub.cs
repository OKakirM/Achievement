using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;

namespace Achievement.Hubs;

public class PresenceHub : Hub
{
    // chave da pessoa -> nº de ligações abertas (separadores).
    // ponytail: estado em memória, chega para 1 instância. Usar Redis backplane se escalar para várias.
    private static readonly ConcurrentDictionary<string, int> People = new();

    public override async Task OnConnectedAsync()
    {
        var key = GetKey();
        Context.Items["key"] = key;
        People.AddOrUpdate(key, 1, (_, c) => c + 1);
        await BroadcastCount();
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (Context.Items["key"] is string key && People.AddOrUpdate(key, 0, (_, c) => c - 1) <= 0)
            People.TryRemove(key, out _);
        await BroadcastCount();
        await base.OnDisconnectedAsync(exception);
    }

    // Autenticado: dedupe separadores por userId. Anónimo: conta por ligação.
    private string GetKey() => Context.UserIdentifier ?? Context.ConnectionId;

    private Task BroadcastCount() => Clients.All.SendAsync("online", People.Count);
}
