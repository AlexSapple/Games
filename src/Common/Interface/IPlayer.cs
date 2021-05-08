using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interface
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
