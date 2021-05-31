using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Collections.Generic;
using System;
using System.Linq;
using Game.Interface;
using Game.Enum;

namespace Game.GameTypeBases
{
    public abstract class Game : INotifyPropertyChanged
    {
        /// <summary>
        /// The players of this game
        /// </summary>
        protected readonly List<Player> Players = new List<Player>();

        /// <summary>
        /// The winner, this will be null during gameplay and will only be populated
        /// on conclusion of the game. If it remains null after this point (denoted by 
        /// <see cref="Status"/> then the game concluded in a draw
        /// </summary>
        public IPlayer Winner { get; protected set; }

        private Status _status;
        /// <summary>
        /// The current status of the game.
        /// </summary>
        /// <value></value>
        public Status Status
        {
            get => _status;
            set
            {
                _status = value;
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
        /// construct a Game engine, with the given player Ids and colour codes.
        /// </summary>
        /// <param name="playerIds"></param>
        /// <param name="playerColourPool"></param>
        public Game (List<Guid> playerIds, Stack<string> playerColourPool)
        {
            initPlayers(playerIds, playerColourPool);
            if (!Players.Any())
                throw new ArgumentException("No players for this game");
        }

        /// <summary>
        /// initialize the players in to the game. Override this in a derived class if specific functionality
        /// is needed.
        /// </summary>
        /// <param name="playerIds">Id's for required players</param>
        /// <param name="playerColourPool">the coloyr names to apply to the players</param>
        protected virtual void initPlayers(List<Guid> playerIds, Stack<string> playerColourPool)
        {
            if (playerColourPool.Count < playerIds.Count)
                throw new ArgumentException("Not enough colours for the number of players", nameof(playerColourPool));

            if (playerIds != null)
            {
                foreach (Guid humanPlayer in playerIds)
                {
                    Players.Add(new Player(humanPlayer, playerColourPool.Pop()));
                }
            }

            //and reorder the players to create a random player order
            ShufflePlayers();
        }

        /// <summary>
        /// Shuffle the players in to a near random order.
        /// </summary>
        private void ShufflePlayers()
        {
            if (Status == Status.Completed || Status == Status.InProgress)
                return;

            if (!Players.Any())
                return;

            Random rng = new Random();
            int range = Players.Count;
            while (range > 1)
            {
                range--;
                int k = rng.Next(range + 1);
                var value = Players[k];
                Players[k] = Players[range];
                Players[range] = value;
            }
        }
    }
}
