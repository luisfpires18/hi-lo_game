namespace HiLoGame.Hubs
{
    using System.Threading.Tasks;
    using HiLoGame.Domain;
    using HiLoGame.Domain.Interfaces;
    using Microsoft.AspNetCore.SignalR;
    using static HiLoGame.Domain.Constants;

    public class GameHub : Hub
    {
        private readonly int minValue;

        private readonly int maxValue;

        private readonly IGameRoomRepository gameRoomRepository;

        public GameHub(
            IGameRoomRepository gameRoomRepository,
            IRangeValuesConfiguration rangeValuesConfiguration)
        {
            ArgumentNullException.ThrowIfNull(gameRoomRepository, nameof(gameRoomRepository));
            ArgumentNullException.ThrowIfNull(rangeValuesConfiguration, nameof(rangeValuesConfiguration));

            this.gameRoomRepository = gameRoomRepository;
            this.minValue = rangeValuesConfiguration.MinValue;
            this.maxValue = rangeValuesConfiguration.MaxValue;
        }

        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"Player: {this.Context.ConnectionId} has connected.");

            await this.Clients.Caller.SendAsync(HubConstants.Rooms, this.gameRoomRepository.GetAll());
        }

        public async Task<GameRoom?> CreateRoom(string roomName, string playerName)
        {
            var roomId = Guid.NewGuid().ToString();

            var room = new GameRoom(roomId, roomName);
            this.gameRoomRepository.Insert(room);

            var player = new Player(this.Context.ConnectionId, playerName);

            this.gameRoomRepository.TryAddPlayer(room, player);

            await this.Groups.AddToGroupAsync(this.Context.ConnectionId, roomId);

            await this.Clients.All.SendAsync(HubConstants.Rooms, this.gameRoomRepository.GetAll());

            return room;
        }

        public async Task<GameRoom?> JoinRoom(string roomId, string playerName)
        {
            var room = this.gameRoomRepository.GetRoomById(roomId);

            if (room is not null)
            {
                var player = new Player(this.Context.ConnectionId, playerName);

                if (this.gameRoomRepository.TryAddPlayer(room, player))
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
            var room = this.gameRoomRepository.GetRoomById(roomId);

            if (room is not null)
            {
                room.HiLoGame?.StartGame(this.minValue, this.maxValue);

                await this.Clients.Group(roomId).SendAsync(HubConstants.GameUpdate, room);
            }
        }

        public async Task<GameRoom?> MakeMove(string roomId, int guess, string playerId)
        {
            var room = this.gameRoomRepository.GetRoomById(roomId);

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