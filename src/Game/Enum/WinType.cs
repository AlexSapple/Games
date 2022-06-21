namespace Game.Enum
{
    /// <summary>
    /// The type of win. This can be used where a win requires some 
    /// contextual type (such as how the win occured)
    /// </summary>
    public enum WinType
    {
        /// <summary>
        /// Indicates that a win has not occurred
        /// </summary>
        NoWin,

        /// <summary>
        /// the only player with remaining valid positions
        /// </summary>
        LastManStanding,

        /// <summary>
        /// The current player is the only player
        /// who has legal moves.
        /// </summary>
        LegalMoves,

        /// <summary>
        /// The board is completed, and a score based win is required
        /// </summary>
        Score
    }
}
