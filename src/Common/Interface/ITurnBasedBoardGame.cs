using System;

namespace Common.Interface
{
    /// <summary>
    /// Represents a turn based board game
    /// </summary>
    public interface ITurnBasedBoardGame
    {
        /// <summary>
        /// The board
        /// </summary>
        IBoard Board { get; }

        /// <summary>
        /// The turn iterated event.
        /// </summary>
        event EventHandler<EventArgs> TurnIterated;
    }
}
