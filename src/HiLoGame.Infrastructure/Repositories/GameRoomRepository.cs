namespace HiLoGame.Infrastructure.Repositories
{
    using System.Collections.Generic;
    using HiLoGame.Domain;
    using HiLoGame.Domain.Interfaces;
    using static HiLoGame.Domain.Constants;

    public class GameRoomRepository : IGameRoomRepository
    {
        protected static readonly List<GameRoom> collection = [];

        public void Insert(GameRoom entity) => collection.Add(entity);

        public IEnumerable<GameRoom> GetAll() => collection;

        public GameRoom? GetRoomById(string roomId)
        {
            var rooms = this.GetAll();

            return rooms.FirstOrDefault(r => r.Id == roomId);
        }

        public bool TryAddPlayer(GameRoom room, Player player)
        {
            var playerIsNotOnRoom = room.Players.Any(p => p.Id == player.Id);

            if (room.Players.Count < GameConstants.MAX_NUM_PLAYERS && !playerIsNotOnRoom && room.HiLoGame is not null)
            {
                room.Players.Add(player);

                if (room.IsPlayerWaitingOpponent())
                {
                    room.HiLoGame.Player1 = player.Id;
                }
                else if (room.IsRoomFull())
                {
                    room.HiLoGame.Player2 = player.Id;
                }

                return true;
            }

            return false;
        }
    }
}