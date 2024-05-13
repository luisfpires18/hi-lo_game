namespace HiLoGame.Domain
{
    using static global::HiLoGame.Domain.Constants;

    public class HiLoGame
    {
        public int MinNumber { get; set; }

        public int MaxNumber { get; set; }

        public string? Player1 { get; set; }

        public string? Player2 { get; set; }

        public string? CurrentPlayerId { get; set; }

        public string PlayerTurn => this.CurrentPlayerId == Player1 ? GameConstants.Player1 : GameConstants.Player2;

        public int MysteryNumber { get; set; }

        public GameStatus GameStatus { get; set; } = GameStatus.WaitingToStart;

        public string Result { get; set; } = string.Empty;

        public KeyValuePair<string, string> Winner { get; set; } = new KeyValuePair<string, string>();

        public int GenerateMysteryNumber()
        {
            var random = new Random();

            return random.Next(this.MinNumber, this.MaxNumber + 1);
        }

        public string CheckGuess(int guess)
        {
            return guess.CompareTo(this.MysteryNumber) switch
            {
                < 0 => GameConstants.HIGHER_NUMBER,
                > 0 => GameConstants.LOWER_NUMBER,
                _ => GameConstants.CORRECT,
            };
        }

        public void StartGame(int minNumber, int maxNumber)
        {
            this.GameStatus = GameStatus.GameStarted;
            this.CurrentPlayerId = this.Player1;

            this.MinNumber = minNumber;
            this.MaxNumber = maxNumber;

            this.MysteryNumber = this.GenerateMysteryNumber();
            Console.WriteLine($"Mystery Number is: {this.MysteryNumber}");
        }

        public void ChangePlayer()
        {
            this.CurrentPlayerId = (this.CurrentPlayerId == Player1) ? Player2 : Player1;
        }

        public bool IsGameOver()
        {
            return this.GameStatus == GameStatus.GameOver;
        }
    }
}