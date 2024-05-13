namespace HiLoGame.Client
{
    using Microsoft.Extensions.Configuration;

    public class RangeValues
    {
        private readonly IConfiguration configuration;

        public int MinValue { get; }
        public int MaxValue { get; }

        public RangeValues(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.MinValue = this.configuration.GetValue<int>("RangeValues:MinValue");
            this.MaxValue = this.configuration.GetValue<int>("RangeValues:MaxValue");
        }
    }
}