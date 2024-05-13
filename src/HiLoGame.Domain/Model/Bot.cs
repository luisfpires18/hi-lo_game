namespace HiLoGame.Domain
{
    using System;

    public class Bot
    {
        private static string[] Prefixes = { "Alpha", "Beta", "Gamma", "Delta" };

        public string Name { get; init; }

        public Bot()
        {
            var random = new Random();

            string prefix = Prefixes[random.Next(Prefixes.Length)];

            this.Name = $"{prefix} Bot";
        }
    }
}