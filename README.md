# Hi-Lo Game

# Description:

HiLo is a classic guessing game where you try to discover a secret number chosen by the system within a specified range.

Gameplay:

- The system randomly selects a secret number between a defined minimum ([Min]) and maximum ([Max]) value.

- You, the player, guess a number within the same range.

- Feedback: The system provides clues based on your guess:

a) HI: The secret number is greater than your guess.

b) LO: The secret number is less than your guess.

- Repeat: Keep guessing until you correctly identify the secret number.

- The goal is to find the secret number in the minimum number of attempts.

# Technical Stack:

This project was made using C# .NET with Razor Web Apps and SignalR for the multiplayer component.
It was also containerized using docker.

Note: The single-player BOT does not have any AI mechanism and will only react based on its inputs (higher/lower).

# Structure

1 - Presentation layer:

Server / Client interaction.

2 - Domain Layer:

Layer responsible for having the models.

3 - Infrastructure layer:

Layer responsible for managing the data repositories by providing methods to add or list the data.
Note that there is no database and the data will only be available in-memory.

4 - Tests layer:

Layer responsible for having unit tests to the major classes.

# Getting Started:

Running locally using docker:

1 - On the root folder:

```
docker-compose up -d
```

or

```
docker-compose up --build
```

2 - Open the web app at: http://localhost:5263

3 - Training - Where you can see how the game works without any opponent.

4 - Single Player - Where you will play against a bot.

5 - Multi Player - Create a room in a lobby where other players can join you and duel.
