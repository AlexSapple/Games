using Common.Interface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class Player : IPlayer
    {
        /// <summary>
        /// A unique identifier of this player
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// The colour that represents this player.
        /// </summary>
        public Color Colour { get; }

        /// <summary>
        /// Create a player instance with Id and colour name. Note that the 
        /// colour name should come from the known list of colour pallete.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="colourName"></param>
        public Player(Guid id, string colourName)
        {
            Id = id;
            Colour = Color.FromName(colourName);
        }
    }
}
