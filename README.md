# Games
A set of projects to create simple games in C# and blazor based around a 2d board structure.

This project is intended for experimentation and learning. It consists of a solution containing a 
.Net 5 Blazor web application with a component library and various C# projects. The application layer
is essentially a demonstration of the components and logic. 

The structure is such that the board is general purpose element that has a great deal of reuse. The 
board itself doesn't contain any logic beyond interaction. A typical game is made up of a set logical rules, 
players and the board. Both internal logic, and automated or human players can update the state of the board.

The commonality of a 2d board in various games (think chess, draughts, scrabble even! and if not games, then mathematical simulations too)
along with the "dumbness" of the board element allow for this solution to quickly create playable games with a completely re-useable
board that doesn't need to understand the game being played. 

##To run the project locally
debug from your preferred IDE GUI (with WebApp as startup project) or using Dot net CLI;

    /src/WebApp/ dotnet run

