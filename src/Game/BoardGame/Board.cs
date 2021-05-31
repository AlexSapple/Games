using Game.Event;
using Game.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Game.BoardGame
{
    /// <summary>
    /// A concrete implementation of a game board.
    /// </summary>
    public class Board : IBoard
    {
        /// <summary>
        /// The number of positions of the board in the x axis
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// The number of positions of the board in the y axis
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// The current players turn, may be null if not applicable
        /// </summary>
        public IPlayer CurrentTurn { get; set; }

        /// <summary>
        /// The data representation of each place within the board
        /// </summary>
        /// <typeparam name="SlimeBoardPosition"></typeparam>
        /// <returns></returns>
        public ReadOnlyCollection<IBoardPosition> Positions =>
            _positions.Cast<IBoardPosition>().ToList().AsReadOnly();

        public readonly List<Position> _positions = new List<Position>();

        /// <summary>
        /// Event handler for the board change event, this will trigger when various 
        /// board position changes occur.
        /// </summary>
        public event EventHandler<BoardChangeEventArgs> BoardChange;

        /// <summary>
        /// Event handler for the completion of the board change event, this will trigger
        /// after the completion of all logic that triggers the <see cref="BoardChange"/> event
        /// </summary>
        public event EventHandler<EventArgs> BoardChangeCompleted;

        protected void OnBoardChange(BoardChangeEventArgs e)
        {
            BoardChange?.Invoke(this, e);
        }

        /// <summary>
        /// Create a Board. The width and height will always be treated as positive ints
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Thrown when <see cref="width"/> or <see cref="height"/> are 0
        /// </exception>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Board(int width, int height)
        {
            if (width == 0)
                throw new ArgumentException(nameof(width), "Must be positive");

            if (height == 0)
                throw new ArgumentException(nameof(height), "Must be positive");

            Width = Math.Abs(width);
            Height = Math.Abs(height);

            InitializePositions();
        }

        /// <summary>
        /// Create all the slime board positions for this board in their initial state
        /// which will all be unoccupied.
        /// </summary>
        private void InitializePositions()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Position position = new Position(x, y);
                    position.PropertyChanged += PositionChange;
                    _positions.Add(position);
                }
            }
        }

        private void PositionChange(object sender, PropertyChangedEventArgs e)
        {
            BoardChangeEventArgs args = new BoardChangeEventArgs();
            args.SourcePosition = sender as Position;
            args.PropertyName = e.PropertyName;
            BoardChange?.Invoke(sender, args);
        }
    }
}
