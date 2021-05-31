using System;
using System.Collections.Generic;
using System.Timers;
using Common.Enum;

namespace Game.GameTypeBases
{
    /// <summary>
    /// Represents a turn based board game with a turn timer. This abstract class will
    /// handler a timer if required and fire the dervived <see cref="IteratePlayerTurn"/>
    /// when the timer elapses. It will start the timer when the <see cref="Status"/> is set
    /// to in progress. It will stop the timer when the <see cref="Status"/> is set to anything that
    /// does not represent a game in progress status.
    /// </summary>
    public abstract class TurnBasedBoardGame : BoardGame
    {
        /// <summary>
        /// The timer per turn.
        /// </summary>
        protected readonly Timer _turnTimer;

        #region turn iterated event handler
        protected event EventHandler<EventArgs> _turnIterated;

        protected void OnTurnIterated(EventArgs e)
        {
            _turnIterated?.Invoke(this, e);
        }
        #endregion

        /// <summary>
        /// construct a Turn based game with optional timer. Pass null if no time limit needed. 
        /// </summary>
        /// <param name="playerIds"></param>
        /// <param name="playerColourPool"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="timeLimitPerTurnInSeconds"></param>
        public TurnBasedBoardGame(List<Guid> playerIds, Stack<string> playerColourPool, int width, int height, int? timeLimitPerTurnInSeconds = null)
            : base(playerIds, playerColourPool, width, height)
        {
            if (timeLimitPerTurnInSeconds.HasValue && timeLimitPerTurnInSeconds < 1)
                throw new ArgumentException("Turn based game must specify a time limit per turn of at least 1 second. Pass null if timer not needed"
                    , nameof(timeLimitPerTurnInSeconds));

            if(timeLimitPerTurnInSeconds.HasValue)
            {
                _turnTimer = new Timer(timeLimitPerTurnInSeconds.Value * 1000);
                _turnTimer.Elapsed += OnTurnTimeElapsed;
                _turnTimer.Stop();

                //if the status of the game changes, and that change results
                //in the game being in progress, then make sure we start the 
                //timer.
                PropertyChanged += (s, e) => {
                    if (!_turnTimer.Enabled && Status == Status.InProgress)
                        _turnTimer.Start();

                    if (Status != Status.InProgress)
                        _turnTimer.Stop();
                };
            }
        }

        /// <summary>
        /// If the turn timer elapses, we need to iterate the player turn
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnTurnTimeElapsed(Object source, ElapsedEventArgs e)
        {
            if (Status == Status.Completed)
            {
                _turnTimer.Enabled = false;
                return;
            }

            _turnTimer.Stop();
            IteratePlayerTurn();
            _turnTimer.Start();
        }

        /// <summary>
        /// Method to move from one turn to the next. 
        /// </summary>
        protected virtual void IteratePlayerTurn()
        {
            //this base class just fires the event. Derived classes need
            //to override this virtual method with game specific logic.
            OnTurnIterated(EventArgs.Empty);
        }
    }
}
