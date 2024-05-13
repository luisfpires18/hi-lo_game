HiLo Game - A Guessing Game with Real-Time Multiplayer

Description:

HiLo is a classic guessing game where you try to discover a secret number chosen by the system within a specified range. With this implementation, you can enjoy the game in three modes:

Training: Practice your skills and learn the game mechanics without any opponent.
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
Technical Stack:

Frontend: Razor Web Apps
Real-time Communication: SignalR
(Optional) Additional Libraries/Technologies (list any specific ones used)
Features:

Multi-player support for fun and competitive play (if applicable)
Responsive web design for a seamless experience across devices
Note:

The single-player BOT currently lacks AI and reacts directly to player inputs.
Getting Started:

Running locally using docker:
1 - On the root folder:

docker-compose up -d

2 - Open the web app at: http://localhost:5263
