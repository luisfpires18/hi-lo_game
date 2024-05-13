namespace HiLoGame.Domain
{
    using System.Collections.Generic;
    using System.Linq;

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

        private const int MAX_NUM_PLAYERS = 2;

        public bool TryAddPlayer(Player player)
        {
            var playerIsNotOnRoom = this.Players.Any(p => p.Id == player.Id);

            if (this.Players.Count < MAX_NUM_PLAYERS && !playerIsNotOnRoom && this.HiLoGame is not null)
            {
                this.Players.Add(player);

                if (this.IsPlayerWaitingOpponent())
                {
                    this.HiLoGame.Player1 = player.Id;
                }
                else if (this.IsRoomFull())
                {
                    this.HiLoGame.Player2 = player.Id;
                }

                return true;
            }

            return false;
        }

        public bool IsPlayerWaitingOpponent()
        {
            return this.Players.Count == 1;
        }

        public bool IsRoomFull()
        {
            return this.Players.Count == MAX_NUM_PLAYERS;
        }
    }
}