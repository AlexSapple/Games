using System.ComponentModel;

namespace Game.Interface
{
    /// <summary>
    /// Represents a position of a game board
    /// </summary>
    public interface IBoardPosition
    {
        /// <summary>
        /// Identifier of the current occupier of this position. 
        /// Null if this position is not currently occupied.
        /// </summary>
        IPlayer Occupier { get; }

        /// <summary>
        /// The x coordinate position of this
        /// </summary>
        /// <value></value>
        int XCoordinate { get; }

        /// <summary>
        /// The y coordinate position of this
        /// </summary>
        /// <value></value>
        int YCoordinate { get; }

        /// <summary>
        /// Indicates that this position is start selectable
        /// </summary>
        /// <value></value>
        bool CanStartSelect { get; }

        /// <summary>
        /// Indicates that this position has been selected as a 
        /// primary selected position (that is a starting selection)
        /// </summary>
        public bool IsStartSelected { get; set; }

        /// <summary>
        /// Indicates that this position is eligible as an end position
        /// (that is a position to complete a players turn). Note that
        /// we would expect CanEndSelect to only ever be true where a parent
        /// board contains a true value for <see cref="IsStartSelected"/>
        /// </summary>
        public bool CanEndSelect { get; }

        /// <summary>
        /// Indicates that this position has been selected as a 
        /// secondary or "end" position (that is to complete a move).
        /// </summary>
        public bool IsEndSelected { get; set; }

        /// <summary>
        /// Event handler for the board change event, this will trigger when various 
        /// board position changes occur.
        /// </summary>
        event PropertyChangedEventHandler PropertyChanged;
    }
}
