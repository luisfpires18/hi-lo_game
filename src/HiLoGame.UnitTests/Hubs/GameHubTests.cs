namespace HiLoGame.UnitTests
{
    using AutoFixture;
    using FluentAssertions;
    using HiLoGame.Domain;
    using HiLoGame.Domain.Interfaces;
    using HiLoGame.Hubs;
    using Microsoft.AspNetCore.SignalR;
    using Moq;

    public class GameHubTests
    {
        private readonly Mock<IGameRoomRepository> mockRepository;
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

        private const string RoomName = "LP_ROOM";
        private const string PlayerId = "18";
        private const string PlayerName = "LP";

        public GameHubTests()
        {
            this.fixture = new Fixture();
            this.mockRepository = new Mock<IGameRoomRepository>();

            this.mockRepository
                .Setup(r => r.TryAddPlayer(It.IsAny<GameRoom>(), It.IsAny<Player>()))
                .Returns(true);

            var rooms = this.GenerateRooms();

            var room = rooms.First();

            this.mockRepository
                .Setup(r => r.GetAll())
                .Returns(rooms);

            this.mockRepository
                .Setup(r => r.GetRoomById(It.IsAny<string>()))
                .Returns(room);

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

            this.gameHub = new GameHub(this.mockRepository.Object, this.mockRangeValuesConfiguration.Object)
            {
                Context = this.mockHubContext.Object,
                Groups = this.mockGroupManager.Object,
                Clients = this.mockHubCallerClients.Object,
            };
        }

        [Fact]
        public void GameHub_NullRepository_ShouldThrowArgumentNullException()
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
            var gameHub = () => new GameHub(this.mockRepository.Object, null);

            // Assert
            gameHub.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task GameHub_CreateRoom_ShouldReturnCreatedRoom()
        {
            // Arrange && Act
            var createdRoom = await this.gameHub.CreateRoom(RoomName, PlayerName);

            // Assert
            createdRoom.Should().NotBeNull();
            createdRoom!.Name.Should().Be(RoomName);

            this.mockRepository
                .Verify(
                    x => x.Insert(It.IsAny<GameRoom>()),
                    Times.Once);

            this.mockRepository
                .Verify(
                    x => x.TryAddPlayer(It.IsAny<GameRoom>(), It.IsAny<Player>()),
                    Times.Once);
        }

        [Fact]
        public async Task GameHub_CreateAndJoinRoom_ShouldJoinRoom()
        {
            // Arrange
            var player2 = this.fixture.Create<string>();

            // Act
            var createdRoom = await this.gameHub.CreateRoom(RoomName, PlayerName);

            this.mockHubContext
                .SetupGet(context => context.ConnectionId)
                .Returns(this.fixture.Create<string>());

            createdRoom.Should().NotBeNull();

            var result = await this.gameHub.JoinRoom(createdRoom!.Id, player2);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be(RoomName);

            this.mockRepository
                .Verify(
                    x => x.Insert(It.IsAny<GameRoom>()),
                    Times.Once);

            this.mockRepository
                .Verify(
                    x => x.GetRoomById(It.IsAny<string>()),
                    Times.Once);

            this.mockRepository
                .Verify(
                    x => x.TryAddPlayer(It.IsAny<GameRoom>(), It.IsAny<Player>()),
                    Times.Exactly(2));
        }

        [Fact]
        public async Task GameHub_StartGameAndMakeMove_ShouldWinPlayer1()
        {
            // Arrange
            var player2 = this.fixture.Create<string>();

            this.mockHubContext
                .SetupGet(context => context.ConnectionId)
                .Returns(PlayerName);

            // Act
            var room = await this.gameHub.CreateRoom(RoomName, PlayerName);

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

            room = await this.gameHub.MakeMove(roomId, playerGuess, PlayerName);

            // Assert
            room.Should().NotBeNull();
            room!.Name.Should().Be(RoomName);
            room.HiLoGame!.Winner.Should().NotBeNull();
            room.HiLoGame.Winner.Key.Should().Be(PlayerName);

            this.mockRepository
                .Verify(
                    x => x.Insert(It.IsAny<GameRoom>()),
                    Times.Once);

            this.mockRepository
                .Verify(
                    x => x.GetRoomById(It.IsAny<string>()),
                    Times.AtLeastOnce);

            this.mockRepository
                .Verify(
                    x => x.TryAddPlayer(It.IsAny<GameRoom>(), It.IsAny<Player>()),
                    Times.Exactly(2));
        }

        [Fact]
        public async Task GameHub_StartGameAndMakeMove_ShouldBePlayer2Turn()
        {
            // Arrange
            var player2 = this.fixture.Create<string>();

            this.mockHubContext
                .SetupGet(context => context.ConnectionId)
                .Returns(PlayerName);

            // Act
            var room = await this.gameHub.CreateRoom(RoomName, PlayerName);

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

            room = await this.gameHub.MakeMove(roomId, playerGuess, PlayerName);

            // Assert
            room.Should().NotBeNull();
            room!.Name.Should().Be(RoomName);

            this.mockRepository
                .Verify(
                    x => x.Insert(It.IsAny<GameRoom>()),
                    Times.Once);

            this.mockRepository
                .Verify(
                    x => x.GetRoomById(It.IsAny<string>()),
                    Times.AtLeastOnce);

            this.mockRepository
                .Verify(
                    x => x.TryAddPlayer(It.IsAny<GameRoom>(), It.IsAny<Player>()),
                    Times.Exactly(2));
        }

        private List<GameRoom> GenerateRooms()
        {
            return new List<GameRoom>
            {
                new GameRoom(this.fixture.Create<string>(), RoomName)
                {
                    Players = new List<Player>
                    {
                        new Player(PlayerId, PlayerName)
                    },
                    HiLoGame = new HiLoGame
                    {
                        CurrentPlayerId = PlayerId,
                        Player1 = PlayerId,
                        Winner = new KeyValuePair<string, string>(PlayerName, PlayerId),
                    }
                }
            };
        }
    }
}