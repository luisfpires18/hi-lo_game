namespace HiLoGame.Hubs
{
    using System.Threading.Tasks;
    using HiLoGame.Client;
    using HiLoGame.Domain;
    using Microsoft.AspNetCore.SignalR;
    using static HiLoGame.Domain.Constants;

    public class GameHub : Hub
    {
        private readonly int MinNumber;

        private readonly int MaxNumber;

        public GameHub(IConfiguration configuration, IRangeValuesConfiguration rangeValuesConfiguration)
        {
            ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));
            ArgumentNullException.ThrowIfNull(rangeValuesConfiguration, nameof(rangeValuesConfiguration));

            this.MinNumber = rangeValuesConfiguration.MinValue;
            this.MaxNumber = rangeValuesConfiguration.MaxValue;
        }

        private static readonly List<GameRoom> rooms = new List<GameRoom>();

        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"Player: {this.Context.ConnectionId} has connected.");

            await this.Clients.Caller.SendAsync(HubConstants.Rooms, rooms);
        }

        public async Task<GameRoom?> CreateRoom(string roomName, string playerName)
        {
            var roomId = Guid.NewGuid().ToString();

            var room = new GameRoom(roomId, roomName);
            rooms.Add(room);

            var player = new Player(this.Context.ConnectionId, playerName);

            room.TryAddPlayer(player);

            await this.Groups.AddToGroupAsync(this.Context.ConnectionId, roomId);

            await this.Clients.All.SendAsync(HubConstants.Rooms, rooms);

            return room;
        }

        public async Task<GameRoom?> JoinRoom(string roomId, string playerName)
        {
            var room = rooms.FirstOrDefault(r => r.Id == roomId);

            if (room is not null)
            {
                var player = new Player(this.Context.ConnectionId, playerName);

                if (room.TryAddPlayer(player))
                {
                    await this.Groups.AddToGroupAsync(this.Context.ConnectionId, roomId);

                    await this.Clients.Group(roomId).SendAsync(HubConstants.PlayerJoined, player);

                    return room;
                }
            }

            return null;
        }

        public async Task StartGame(string roomId)
        {
            var room = rooms.FirstOrDefault(r => r.Id == roomId);

            if (room is not null)
            {
                room.HiLoGame?.StartGame(this.MinNumber, this.MaxNumber);

                await this.Clients.Group(roomId).SendAsync(HubConstants.GameUpdate, room);
            }
        }

        public async Task<GameRoom?> MakeMove(string roomId, int guess, string playerId)
        {
            var room = rooms.FirstOrDefault(r => r.Id == roomId);

            if (room?.HiLoGame is null)
            {
                return null;
            }

            var result = room.HiLoGame.CheckGuess(guess);

            if (result == GameConstants.CORRECT)
            {
                room.HiLoGame.GameStatus = GameStatus.GameOver;

                var currentPlayer = room.Players.FirstOrDefault(p => p.Id == playerId);

                if (currentPlayer is not null)
                {
                    room.HiLoGame.Winner = new KeyValuePair<string, string>(currentPlayer.Id, currentPlayer.Name);
                }
            }

            room.HiLoGame.Result = result;

            room.HiLoGame.ChangePlayer();

            await this.Clients.Group(roomId).SendAsync(HubConstants.GameUpdate, room);

            return room;
        }
    }
}