namespace HiLoGame.Domain
{
    public static class Constants
    {
        public static class HubConstants
        {
            public const string Rooms = nameof(Rooms);
            public const string PlayerJoined = nameof(PlayerJoined);
            public const string GameUpdate = nameof(GameUpdate);
        }

        public static class GameConstants
        {
            public const string HIGHER_NUMBER = "HI: the mystery number is higher than the player's guess";
            public const string LOWER_NUMBER = "LO: the mystery number is lower than the player's guess";
            public const string CORRECT = nameof(CORRECT);

            public const string Player1 = "Player 1";
            public const string Player2 = "Player 2";
        }
    }
}