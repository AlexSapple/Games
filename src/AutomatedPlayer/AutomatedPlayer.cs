using Game.Interface;
using System;
using System.Threading.Tasks;

namespace AutomatedPlayer
{
    public abstract class AutomatedPlayer
    {
        protected readonly Guid MyPlayerId;

        protected readonly ITurnBasedBoardGame Game;

        private bool _myTurnInProgress = false;

        protected AutomatedPlayer(Guid myPlayerId, ITurnBasedBoardGame game)
        {
            MyPlayerId = myPlayerId;
            if (MyPlayerId == Guid.Empty)
                throw new ArgumentException(nameof(myPlayerId));

            Game = game;
            Game.TurnIterated += (s, e) => onTurnIterated();

            //and fire the handler immediately because our automated player
            //may be playing first (without any board change event firing)
            onTurnIterated();
        }

        private void onTurnIterated()
        {
            if (Game.Board.CurrentTurn?.Id == MyPlayerId && !_myTurnInProgress)
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
        protected abstract Task MakeMove();
    }
}
