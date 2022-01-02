using System;
using System.Collections.Generic;
using Game.BoardGame;

namespace Game.GameTypeBases
{
    /// <summary>
    /// Represents a board game, that is a game that has a board representation
    /// </summary>
    public abstract class BoardGame : Game
    {
        /// <summary>
        /// the underlying board
        /// </summary>
        protected readonly Board _board;

        /// <summary>
        /// Construct a board game.
        /// </summary>
        /// <param name="playerIds"></param>
        /// <param name="playerColourPool"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public BoardGame(List<Guid> playerIds, string[] playerColourPool, int width, int height)
            : base(playerIds, playerColourPool)
        {
            _board = new Board(width, height);
            OccupyInitialPositions();
        }

        /// <summary>
        /// Apply any default positions to players within the board
        /// </summary>
        protected abstract void OccupyInitialPositions();

        /// <summary>
        /// Resest the values of the board - this is teh meta data 
        /// and does not reset any occupied positions.
        /// </summary>
        protected void ResetBoard()
        {
            foreach (Position position in _board.Positions)
            {
                position.CanStartSelect = false;
                position.CanEndSelect = false;
                position.IsStartSelected = false;
                position.IsEndSelected = false;
            }
        }

        /// <summary>
        /// Convenience method to reset the board game
        /// </summary>
        protected void ResetBoardGame()
        {
            ResetBoard();

            foreach (var position in _board._positions)
                position.Occupier = null;

            _board.CurrentTurn = null;

            ResetGame();
            OccupyInitialPositions();
        }
    }
}
