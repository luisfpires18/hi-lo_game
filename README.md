# HiLo Game

# Description:

HiLo is a classic guessing game where you try to discover a secret number chosen by the system within a specified range. With this implementation, you can enjoy the game in three modes:

Training: 
Single Player: Challenge yourself against a BOT that reacts based on your guesses.
Multi Player: Create a room in a lobby and invite friends to join you for a real-time guessing duel.
Gameplay:

The system randomly selects a secret number between a defined minimum ([Min]) and maximum ([Max]) value.
You, the player, guess a number within the same range.
Feedback: The system provides clues based on your guess:
HI: The secret number is greater than your guess.
LO: The secret number is less than your guess.
Repeat: Keep guessing until you correctly identify the secret number.
The goal is to find the secret number in the minimum number of attempts.

#Technical Stack:

This project was made using C# .NET with Razor Web Apps and SignalR for the multiplayer component.
It was also containerized using docker.

Note: The single-player BOT does not have any AI mechanism and will only react based on its inputs (higher/lower).

#Getting Started:

Running locally using docker:
1 - On the root folder:

```
docker-compose up -d
```

2 - Open the web app at: http://localhost:5263

3 - Training - Where you can see how the game works without any opponent.

4 - Single Player - Where you will play against a bot.

5 - Multi Player - Create a room in a lobby where other players can join you and duel.
