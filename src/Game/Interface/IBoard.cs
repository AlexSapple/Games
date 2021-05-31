using System.Collections.ObjectModel;

namespace Game.Interface
{
    /// <summary>
    /// Represents a game board
    /// </summary>
    public interface IBoard
    {
        /// <summary>
        /// The positions of the board 
        /// </summary>
        /// <value></value>
        ReadOnlyCollection<IBoardPosition> Positions { get; }

        /// <summary>
        /// The number of positions of the board in the x axis
        /// </summary>
        int Width { get; }

        /// <summary>
        /// The number of positions of the board in the y axis
        /// </summary>
        int Height { get; }

        /// <summary>
        /// The current players turn, may be null.
        /// </summary>
        IPlayer CurrentTurn { get; }
    }
}
