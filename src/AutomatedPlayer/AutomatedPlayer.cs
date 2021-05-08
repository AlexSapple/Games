using Common.Interface;
using System;

namespace AutomatedPlayer
{
    public abstract class AutomatedPlayer
    {
        protected readonly Guid MyPlayerId;

        protected readonly IBoard Board;

        private bool _myTurnInProgress = false;

        protected AutomatedPlayer(Guid myPlayerId, IBoard board)
        {
            MyPlayerId = myPlayerId;
            if (MyPlayerId == Guid.Empty)
                throw new ArgumentException(nameof(myPlayerId));

            Board = board;
            Board.BoardChangeCompleted += (s, e) => onBoardChangeCompleted(e);
            
            //and fire the handler immediately because our automated player
            //may be playing first (without any board change event firing)
            onBoardChangeCompleted(EventArgs.Empty);
        }

        private void onBoardChangeCompleted(EventArgs args)
        {
            if (Board.CurrentTurn?.Id == MyPlayerId && !_myTurnInProgress)
            {
                _myTurnInProgress = true;
                MakeMove();
                _myTurnInProgress = false;
            }
        }

        /// <summary>
        /// make an automated player turn. This method will be called
        /// at the correct time and so derived instances do not need
        /// to check if it is there turn.
        /// </summary>
        protected abstract void MakeMove();
    }
}
