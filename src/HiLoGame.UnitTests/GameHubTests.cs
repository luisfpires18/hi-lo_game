namespace HiLoGame.UnitTests
{
    using AutoFixture;
    using FluentAssertions;
    using HiLoGame.Hubs;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.Extensions.Configuration;
    using Moq;

    public class GameHubTests
    {
        private readonly Mock<IConfiguration> mockConfiguration;
        private readonly Mock<IRangeValuesConfiguration> mockRangeValuesConfiguration;
        private readonly Mock<IHubCallerClients> mockHubCallerClients;
        private readonly Mock<IGroupManager> mockGroupManager;
        private readonly Mock<IClientProxy> mockClientProxy;
        private readonly Mock<ISingleClientProxy> mockSingleClientProxy;
        private readonly Mock<HubCallerContext> mockHubContext;
        private readonly GameHub gameHub;

        private readonly IFixture fixture;

        private const int MinValue = 1;
        private const int MaxValue = 100;

        public GameHubTests()
        {
            this.fixture = new Fixture();
            this.mockConfiguration = new Mock<IConfiguration>();
            this.mockRangeValuesConfiguration = new Mock<IRangeValuesConfiguration>();

            this.mockRangeValuesConfiguration
                .Setup(x => x.MinValue)
                .Returns(MinValue);

            this.mockRangeValuesConfiguration
                .Setup(x => x.MaxValue)
                .Returns(MaxValue);

            this.mockGroupManager = new Mock<IGroupManager>();
            this.mockHubContext = new Mock<HubCallerContext>();

            this.mockHubContext
                .SetupGet(context => context.ConnectionId)
                .Returns(this.fixture.Create<string>());

            this.mockClientProxy = new Mock<IClientProxy>();

            this.mockSingleClientProxy = new Mock<ISingleClientProxy>();

            this.mockHubCallerClients = new Mock<IHubCallerClients>();

            this.mockHubCallerClients
                .SetupGet(c => c.All)
                .Returns(this.mockClientProxy.Object);

            this.mockHubCallerClients
                .SetupGet(c => c.Caller)
                .Returns(this.mockSingleClientProxy.Object);

            this.mockHubCallerClients
                .Setup(c => c.Group(It.IsAny<string>()))
                .Returns(this.mockClientProxy.Object);

            this.gameHub = new GameHub(this.mockConfiguration.Object, this.mockRangeValuesConfiguration.Object)
            {
                Context = this.mockHubContext.Object,
                Groups = this.mockGroupManager.Object,
                Clients = this.mockHubCallerClients.Object,
            };
        }

        [Fact]
        public void GameHub_NullConfiguration_ShouldThrowArgumentNullException()
        {
            // Arrange && Act
            var gameHub = () => new GameHub(null, this.mockRangeValuesConfiguration.Object);

            // Assert
            gameHub.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GameHub_NullRangeValues_ShouldThrowArgumentNullException()
        {
            // Arrange && Act
            var gameHub = () => new GameHub(this.mockConfiguration.Object, null);

            // Assert
            gameHub.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task GameHub_CreateRoom_ShouldReturnCreatedRoom()
        {
            // Arrange
            var playerName = this.fixture.Create<string>();
            var roomName = this.fixture.Create<string>();

            // Act
            var createdRoom = await this.gameHub.CreateRoom(roomName, playerName);

            // Assert
            createdRoom.Should().NotBeNull();
            createdRoom!.Name.Should().Be(roomName);
            createdRoom.Players.Should().NotBeNullOrEmpty().And.HaveCount(1);
            var player = createdRoom.Players.First();
            player.Name.Should().Be(playerName);
        }

        [Fact]
        public async Task GameHub_CreateAndJoinRoom_ShouldJoinRoom()
        {
            // Arrange
            var player1 = this.fixture.Create<string>();
            var player2 = this.fixture.Create<string>();
            var roomName = this.fixture.Create<string>();

            // Act
            var createdRoom = await this.gameHub.CreateRoom(roomName, player1);

            this.mockHubContext
                .SetupGet(context => context.ConnectionId)
                .Returns(this.fixture.Create<string>());

            createdRoom.Should().NotBeNull();

            var result = await this.gameHub.JoinRoom(createdRoom!.Id, player2);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be(roomName);
        }

        [Fact]
        public async Task GameHub_StartGameAndMakeMove_ShouldWinPlayer1()
        {
            // Arrange
            var player1 = this.fixture.Create<string>();
            var player2 = this.fixture.Create<string>();
            var roomName = this.fixture.Create<string>();

            this.mockHubContext
                .SetupGet(context => context.ConnectionId)
                .Returns(player1);

            // Act
            var room = await this.gameHub.CreateRoom(roomName, player1);

            this.mockHubContext
                .SetupGet(context => context.ConnectionId)
                .Returns(player2);

            room.Should().NotBeNull();

            var roomId = room!.Id;
            const int playerGuess = 10;

            room = await this.gameHub.JoinRoom(room!.Id, player2);

            await this.gameHub.StartGame(roomId);

            room.Should().NotBeNull();
            room!.HiLoGame.Should().NotBeNull();
            room.HiLoGame!.MysteryNumber = playerGuess;

            room = await this.gameHub.MakeMove(roomId, playerGuess, player1);

            // Assert
            room.Should().NotBeNull();
            room!.Name.Should().Be(roomName);
            room.HiLoGame!.Winner.Should().NotBeNull();
            room.HiLoGame.Winner.Key.Should().Be(player1);
        }

        [Fact]
        public async Task GameHub_StartGameAndMakeMove_ShouldBePlayer2Turn()
        {
            // Arrange
            var player1 = this.fixture.Create<string>();
            var player2 = this.fixture.Create<string>();
            var roomName = this.fixture.Create<string>();

            this.mockHubContext
                .SetupGet(context => context.ConnectionId)
                .Returns(player1);

            // Act
            var room = await this.gameHub.CreateRoom(roomName, player1);

            this.mockHubContext
                .SetupGet(context => context.ConnectionId)
                .Returns(player2);

            room.Should().NotBeNull();

            var roomId = room!.Id;
            const int playerGuess = 10;

            room = await this.gameHub.JoinRoom(room!.Id, player2);

            await this.gameHub.StartGame(roomId);

            room.Should().NotBeNull();
            room!.HiLoGame.Should().NotBeNull();

            room = await this.gameHub.MakeMove(roomId, playerGuess, player1);

            // Assert
            room.Should().NotBeNull();
            room!.Name.Should().Be(roomName);
            room.HiLoGame!.CurrentPlayerId.Should().NotBeNull().And.NotBe(player1);
        }
    }
}