using Azure;
using Core.Services.Abstraction;
using Microsoft.AspNetCore.SignalR;
using Shared.Dtos.PlayerDtos;
using Shared.Helpers;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;

namespace Guess_Word_Backend.Hubs
{
   

    public class GameHub : Hub
    {
        private readonly IServiceManager _serviceManager;

        public GameHub(IServiceManager serviceManager)
        {
           
            this._serviceManager = serviceManager;
        }

        public override async Task OnConnectedAsync()
        {
            string connectionId = this.Context.ConnectionId;

            var http = this.Context.GetHttpContext();
            string userId = http?.Request.Query["userId"]!;
            string name = http?.Request.Query["name"] ?? "player";

            PlayerDto? newPlayer = await _serviceManager.PlayerService.OnPlayerConnected(connectionId, userId, name);
            //! var newPlayer = _onlinePlayersService.AddPlayer(userId, connectionId, name);


            //await Clients.AllExcept([connectionId]).SendAsync("ReceiveOnlineUser", connectionId);
            //foreach (PlayerDto player in _onlinePlayersService.GetPlayersRange(p=>p.ConnectionId != null))
            //{
            //    if (player.ConnectionId == connectionId)
            //    {
            //        continue;
            //    }
            //    await Clients.Caller.SendAsync("ReceiveOnlineUser",player.ConnectionId);

            //}
            var parameters = new PagedListRequestParameters
            {
                PageNumber = 1,
                PageSize = 50
            };
            var response = await _serviceManager.PlayerService.GetOnlinePlayers(parameters);
            foreach (PlayerDto player in response.Players)
            {
                if (player.ConnectionId == connectionId)
                {
                    continue;
                }
                await Clients.Caller.SendAsync("ReceiveOnlineUser", player.ConnectionId);

            }
            System.Console.WriteLine($"User {userId} Connected");
            await Clients.Others.SendAsync("OnNewPlayerConnected", newPlayer);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var http = Context.GetHttpContext();
            string? userId = http?.Request.Query["userId"].ToString();
            await _handelOpponentDisconnected();
            await base.OnDisconnectedAsync(exception);
        }

      
        
        private async Task _handelOpponentDisconnected()
        {
            PlayerDto player = await _serviceManager.PlayerService.
                OnPlayerDisconnected(Context.ConnectionId);
            await Clients.Others.SendAsync("OnPlayerDisconnected", player);
        }


        
    }
}
