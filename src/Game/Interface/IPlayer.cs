using System;
using System.Drawing;

namespace Game.Interface
{
    /// <summary>
    /// Represents a player
    /// </summary>
    public interface IPlayer
    {
        /// <summary>
        /// A unique identifier of this player
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// The colour that represents this player.
        /// </summary>
        public Color Colour { get; }
    }
}
