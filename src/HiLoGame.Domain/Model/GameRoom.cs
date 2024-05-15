namespace HiLoGame.Domain
{
    using System.Collections.Generic;
    using static global::HiLoGame.Domain.Constants;

    public class GameRoom
    {
        public GameRoom(string id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        public string Id { get; private set; } = string.Empty;

        public string Name { get; private set; } = string.Empty;

        public List<Player> Players { get; set; } = new List<Player>();

        public HiLoGame? HiLoGame { get; set; } = new HiLoGame();

        public bool IsPlayerWaitingOpponent()
        {
            return this.Players.Count == 1;
        }

        public bool IsRoomFull()
        {
            return this.Players.Count == GameConstants.MAX_NUM_PLAYERS;
        }
    }
}