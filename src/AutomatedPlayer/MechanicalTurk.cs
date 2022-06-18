using AutomatedPlayer.Enum;
using Game.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutomatedPlayer
{
    /// <summary>
    /// A trivial automaton. There isn't really any AI going on here,
    /// it's largely just smoke and mirrors. This player simply knows
    /// how to make a valid move and does so. There is no clever stuff, 
    /// it's just random fun. 
    /// </summary>
    public class MechanicalTurk : AutomatedPlayer
    {
        private readonly ThinkingTime _thinkingTime;
        public MechanicalTurk(Guid myPlayerId, ITurnBasedBoardGame game, ThinkingTime thinkingTime = ThinkingTime.Fast)
            :base(myPlayerId, game)
        {
            _thinkingTime = thinkingTime;
        }

        protected override async Task MakeMove()
        {
            //we want a recursive make move method that the abstract 
            //signature doesn't allow (which is why this is written as such)
            await MakeMove(null);
        }

        private async Task MakeMove(List<IBoardPosition> startPositionsAlreadyAttempted, int? overrideThinkingTimeMillisecond = null)
        {
            await Task.Delay(GetThinkingTimeMilliseconds(overrideThinkingTimeMillisecond));

            startPositionsAlreadyAttempted ??= new List<IBoardPosition>();

            //get my positions, but only take those which have not already been attempted
            var myPositions = Game.Board?.Positions?.Where(p => p.Occupier?.Id == MyPlayerId 
                  && p.CanStartSelect
                  && !startPositionsAlreadyAttempted.Contains(p));

            if (!myPositions.Any())
                return;

            var rand = new Random();

            //from the available start select positions that belong to the turk, select one.
            int randomStartPositionIndex = rand.Next(0, myPositions.Count() - 1);
            var startPosition = myPositions.ElementAt(randomStartPositionIndex);
            startPositionsAlreadyAttempted.Add(startPosition);
            startPosition.IsStartSelected = true;

            //now the board should have a load of end select positions (updated based on the start selection above).
            //if not, recursively call this method to try another position excluding already attempted.
            //When doing so - also avoid additional thinking time by overriding with 0
            var endSelectablePositions = Game.Board.Positions.Where(p => p.CanEndSelect);
            if (!endSelectablePositions.Any())
                await MakeMove(startPositionsAlreadyAttempted, 0);
            else
            {
                int randomEndPositionIndex = rand.Next(0, endSelectablePositions.Count() - 1);

                var endPosition = endSelectablePositions.ElementAt(randomEndPositionIndex);
                endPosition.IsEndSelected = true;
            }
        }

        private int GetThinkingTimeMilliseconds(int? overrideMillisecond = null)
        {
            if (overrideMillisecond.HasValue)
                return overrideMillisecond.Value;

            int maxThinkingtime = _thinkingTime switch
            {
                ThinkingTime.None => 0,
                ThinkingTime.Slow => 2000,
                ThinkingTime.Fast => 500,
                _ => throw new NotImplementedException(),
            };

            if (maxThinkingtime == 0)
                return 0;

            Random rnd = new Random();
            return rnd.Next(0, maxThinkingtime);
        }
    }
}
