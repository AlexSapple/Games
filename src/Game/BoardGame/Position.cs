using Game.Interface;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Game.BoardGame
{
    /// <summary>
    /// A concrete implementation of a game board position
    /// </summary>
    public class Position : IBoardPosition, INotifyPropertyChanged
    {
        /// <summary>
        /// Identifier of the current occupier of this position. 
        /// Null if this position is not currently occupied.
        /// </summary>
        /// <value></value>
        public IPlayer Occupier { get; set; }

        /// <summary>
        /// The x coordinate position of this
        /// </summary>
        /// <value></value>
        public int XCoordinate { get; }

        /// <summary>
        /// The y coordinate position of this
        /// </summary>
        /// <value></value>
        public int YCoordinate { get; }

        /// <summary>
        /// Indicates that this position is selectable
        /// </summary>
        /// <value></value>
        public bool CanStartSelect { get; set; }

        private bool _isStartSelected;
        /// <summary>
        /// Indicates that this position has been selected as a 
        /// primary selected position (that is a starting selection)
        /// </summary>
        public bool IsStartSelected { 
            get => _isStartSelected;
            set
            {
                _isStartSelected = value;
                OnPropertyChanged();
            } 
        }

        public bool _canEndSelect;
        /// <summary>
        /// Indicates that this position is eligible as an end position
        /// (that is a position to complete a players turn). Note that
        /// we would expect CanEndSelect to only ever be true where a parent
        /// board contains a true value for <see cref="IsStartSelected"/>
        /// </summary>
        public bool CanEndSelect {
            get => _canEndSelect; 
            set 
            {
                _canEndSelect = value;
                OnPropertyChanged();
            }
        }

        private bool _isEndSelected;
        /// <summary>
        /// Indicates that this position has been selected as a 
        /// secondary or "end" position (that is to complete a move).
        /// </summary>
        public bool IsEndSelected
        {
            get => _isEndSelected;
            set
            {
                _isEndSelected = value;
                OnPropertyChanged();
            }
        }

        #region property change event notification
        //some properties on this class will notify a change event
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        /// <summary>
        /// Construct a board position. Coordinates must be provided.
        /// The x and y coordinates will always be treated as positive ints
        /// </summary>
        /// <param name="xCoordinate"></param>
        /// <param name="yCoordinate"></param>
        public Position(int xCoordinate, int yCoordinate)
        {
            XCoordinate = Math.Abs(xCoordinate);
            YCoordinate = Math.Abs(yCoordinate);
        }
    }
}
