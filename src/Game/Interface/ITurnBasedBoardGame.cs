using System;

namespace Game.Interface
{
    /// <summary>
    /// Represents a turn based board game
    /// </summary>
    public interface ITurnBasedBoardGame
    {
        /// <summary>
        /// The board
        /// </summary>
        IBoard Board { get; }

        /// <summary>
        /// The turn iterated event.
        /// </summary>
        event EventHandler<EventArgs> TurnIterated;

        /// <summary>
        /// The sample rate for the countdown - do not attempt to refresh UI components faster than this sample rate.
        /// </summary>
        int SampleRateMilliseconds { get; }

        /// <summary>
        /// a representation of the remaining duration of the current turn as a timespan.
        /// </summary>
        TimeSpan TurnTimeRemaining { get; }
    }
}
