using System;
using System.Collections.Generic;
using System.Timers;
using Game.Enum;

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
        /// The timer per turn. This timer controls the sample rate, on it's elapsed
        /// event, the time per turn is evaluated to determine if it has expired
        /// </summary>
        protected readonly Timer _turnTimer;

        #region turn iterated event handler
        protected event EventHandler<EventArgs> _turnIterated;

        /// <summary>
        /// The Time remaining on this turn. This value is sampled at the refreshRateMilliseconds passed in construction
        /// there is no point in sampling faster than this rate. For the same reason, this value should not be 
        /// considered highly precice. 
        /// </summary>
        protected TimeSpan _turnTimeRemaining;

        /// <summary>
        /// The sample rate for the countdown. Used in <see cref="_turnTimeRemaining" calculation and to determing if the turn has entirely elapsed/>
        /// </summary>
        public int SampleRateMilliseconds { get; }

        /// <summary>
        /// class level reference to the initial time limit per turn (for resetting purposes)
        /// </summary>
        private readonly TimeSpan _timeLimitPerTurn; 

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
        /// <param name="timeLimitPerTurn">A timespan representing the maximum duration of a turn</param>
        /// <param name="sampleRateMilliseconds">The level of precision needed for querying remaining time, for example if you want to query this rapidly, you may need it to update faster than once per second</param>
        public TurnBasedBoardGame(List<Guid> playerIds, Stack<string> playerColourPool, int width, int height, TimeSpan? timeLimitPerTurn = null, int sampleRateMilliseconds = 1000)
            : base(playerIds, playerColourPool, width, height)
        {
            if (timeLimitPerTurn.HasValue && timeLimitPerTurn.Value.TotalSeconds < 0)
                throw new ArgumentException("Turn based game must specify a time limit per turn of at least 1 second. Pass null if timer not needed"
                    , nameof(timeLimitPerTurn));

            if (sampleRateMilliseconds < 0)
                throw new ArgumentException("sample rate cannot be less than 1 milliseconds", nameof(sampleRateMilliseconds));

            if(timeLimitPerTurn.HasValue)
            {
                _timeLimitPerTurn = timeLimitPerTurn.Value;
                _turnTimeRemaining = timeLimitPerTurn.Value;
                SampleRateMilliseconds = sampleRateMilliseconds;
                _turnTimer = new Timer(sampleRateMilliseconds);
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
        /// If the turn timer elapses, we need to check if the overall time has elapsed and iterate the player turn.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnTurnTimeElapsed(Object source, ElapsedEventArgs e)
        {
            //always update the remaining time and check if we need
            //to update the actual player
            if (Status == Status.Completed)
            {
                _turnTimer.Enabled = false;
                _turnTimeRemaining = TimeSpan.Zero;
                return;
            }

            _turnTimeRemaining = _turnTimeRemaining.Subtract(new TimeSpan(0, 0, 0, 0, SampleRateMilliseconds));
            
            if(_turnTimeRemaining <= new TimeSpan())
            {
                _turnTimer.Stop();
                IteratePlayerTurn();
                _turnTimer.Start();
            }
        }

        /// <summary>
        /// Method to move from one turn to the next. 
        /// </summary>
        protected virtual void IteratePlayerTurn()
        {
            //this base class just fires the event. Derived classes need
            //to override this virtual method with game specific logic.
            OnTurnIterated(EventArgs.Empty);
            _turnTimeRemaining = _timeLimitPerTurn;
        }
    }
}
