using Common.Enum;
using Common.Interface;
using Game.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using Game.Extensions;
using Game.GameTypeBases;
using Game.BoardGame;
using Game;

namespace SlimeWars
{
    /// <summary>
    /// Represents a single game of slime wars. Handles the logic
    /// and board updates within the game.
    /// </summary>
    public class SlimeWars : TurnBasedBoardGame, ITurnBasedBoardGame
    {
        /// <summary>
        /// The underlying game board exposed as an IBoard interface to implement <see cref="ITurnBasedBoardGame.Board"/>
        /// </summary>
        public IBoard Board => _board;

        object objectLock = new Object();
        /// <summary>
        /// The turn iterated event exposed as Event handler to implement <see cref="ITurnBasedBoardGame.TurnIterated"/>
        /// </summary>
        public event EventHandler<EventArgs> TurnIterated
        {
            add
            {
                lock (objectLock)
                {
                    _turnIterated += value;
                }
            }
            remove
            {
                lock (objectLock)
                {
                    _turnIterated -= value;
                }
            }
        }

        /// <summary>
        /// The specific colours used for the slime wars game
        /// </summary>
        private static readonly Stack<string> Colours = new Stack<string>(new List<string> { "Yellow", "Red", "Green", "Blue" });

        /// <summary>
        /// construct a slime wars game. 
        /// sizes.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="humanPlayers"></param>
        /// <param name="timeLimitPerTurnInSeconds"></param>
        public SlimeWars(int width, int height, List<Guid> players, int timeLimitPerTurnInSeconds = 60)
            : base(players, Colours, width, height, timeLimitPerTurnInSeconds)
        {
            _board.BoardChange += HandleBoardChangeEvent;
            IteratePlayerTurn();
            Status = Status.InProgress;
        }

        /// <summary>
        /// Set out the initial positions of the board based on the number
        /// of players and the static pattern of the positioning.
        /// </summary>
        protected override void OccupyInitialPositions()
        {
            if (Players.Count > 4)
                throw new ArgumentException("too many players");

            if (Players.Count >= _board._positions.Count)
                throw new ArgumentException("too many players for board size");

            if (Status != Status.Completed && Status != Status.InProgress)
            {
                foreach (Player player in Players)
                {
                    //player 1 will occupy the top right position
                    if (_board._positions[0].Occupier == null)
                    {
                        _board._positions[0].Occupier = player;
                        continue;
                    }

                    //player 2 will occupy the bottom right position
                    if (_board._positions.Last().Occupier == null)
                    {
                        _board._positions.Last().Occupier = player;
                        continue;
                    }

                    //player 3 will occupy the top right corner
                    if (_board._positions[_board.Width - 1].Occupier == null)
                    {
                        _board._positions[_board.Width - 1].Occupier = player;
                        continue;
                    }

                    //player 4 will occupy the bottom left corner
                    if (_board._positions[_board._positions.Count - _board.Width].Occupier == null)
                    {
                        _board._positions[_board._positions.Count - _board.Width].Occupier = player;
                        continue;
                    }
                }
            }
        }

        /// <summary>
        /// find and set all the "selectable" places on the board according 
        /// to the current players turn (i.e. the places that they can select)
        /// </summary>
        private void UpdateStartSelectablePositions()
        {
            if (_board.CurrentTurn != null)
            {
                //get all the positions that are occupied by the current player
                //and iterate through them to establish where the player can move
                var applicablePrimaryPositions = _board._positions.Where(p => p.Occupier == _board.CurrentTurn);
                foreach (Position position in applicablePrimaryPositions)
                {
                    position.CanStartSelect = true;
                }
            }
        }

        /// <summary>
        /// Update secondarySelecatblePositions - these are the positions that can be chosen
        /// after an initial selection has been made.
        /// </summary>
        /// <param name="position"></param>
        private void UpdateEndSelectablePositionsFromStartSelected(Position position)
        {
            if (position == null)
                return;

            if (position.IsStartSelected)
            {
                //make sure any other start selected positions are unselected at this point
                var deselectPositions = _board._positions.Where(p => p.IsStartSelected && p != position);
                foreach (Position deselectPosition in deselectPositions)
                    deselectPosition.IsStartSelected = false;

                //now locate the relevant positions surrounding our selected position
                //and mark them as canEndSelect (as long as they are not occupied and in a straight line from our position)
                var neighbours = position.GetNeighbours(_board._positions, 2);
                foreach (Position applicablePosition in neighbours)
                {
                    if (applicablePosition.Occupier == null && applicablePosition.IsStraightLineFrom(position))
                        applicablePosition.CanEndSelect = true;
                }
            }
            else
            {
                foreach (Position applicableEndPosition in _board._positions.Where(p => p.CanEndSelect))
                    applicableEndPosition.CanEndSelect = false;
            }
        }

        /// <summary>
        /// Complete a players move from their end selection (i.e. the final selection that
        /// has been made).
        /// </summary>
        /// <param name="position"></param>
        private void CompleteMoveFromEndSelected(Position position)
        {
            if (position == null)
                return;

            if (position.IsEndSelected)
            {
                //1. occupy this position
                position.Occupier = Players.SingleOrDefault(p => p.Id == _board.CurrentTurn.Id);

                //2. get immediate neighbours
                var neighbours = position.GetNeighbours(_board._positions);

                //3. switch any immediate neighbours
                var neighboursToInfect = neighbours.Where(n => n.Occupier != null);
                foreach (var neighbour in neighboursToInfect)
                {
                    neighbour.Occupier = Players.SingleOrDefault(p => p.Id == _board.CurrentTurn.Id);
                }

                //4. Work out if this move is a multiply or a jump
                //if a jump, then we need to deselect (a jump can be identified by originating
                //from outside of the neighbours).
                var startSelected = _board._positions.SingleOrDefault(p => p.IsStartSelected);
                if (!neighbours.Contains(startSelected))
                    startSelected.Occupier = null;

                //5. check if this move has created a win scenario
                WinType winType = CheckForWinConditionOfCurrentPlayer();
                if (winType != WinType.NoWin)
                {
                    Status = Status.Completed;
                    Winner = _board.CurrentTurn;
                    ResetBoard();

                    if (winType == WinType.LegalMoves)
                    {
                        //for animation purposes, in this type of win, we want to occupy all the places.
                        var fillPositions = _board._positions.Where(p => p.Occupier == null)?.ToList();
                        fillPositions.ForEach(p => p.Occupier = _board.CurrentTurn);
                    }
                }

                //6. Iterate to the next player turn.
                IteratePlayerTurn();
            }
        }

        /// <summary>
        /// Move to the next available players turn.
        /// </summary>
        protected override void IteratePlayerTurn()
        {
            if (Status == Status.Completed)
                return;

            if (_board.CurrentTurn == null)
            {
                _board.CurrentTurn = Players[0];
                UpdateStartSelectablePositions();
                base.IteratePlayerTurn();
                return;
            }

            ResetBoard();

            int playerIndex = 0;
            var allOccupiedPositions = _board.Positions.Where(p => p.Occupier != null);
            foreach (var player in Players)
            {
                if (allOccupiedPositions.Any(p => p.Occupier.Id == player.Id))
                {
                    if (player.Id == _board.CurrentTurn.Id)
                    {
                        if (playerIndex < Players.Count - 1)
                            _board.CurrentTurn = Players[playerIndex + 1];
                        else
                            _board.CurrentTurn = Players[0];

                        UpdateStartSelectablePositions();

                        break;
                    }
                }
                playerIndex++;
            }

            base.IteratePlayerTurn();
        }

        /// <summary>
        /// Respond to changes happening from player moves on the board
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleBoardChangeEvent(object sender, BoardChangeEventArgs e)
        {
            if (e.PropertyName == nameof(Position.IsStartSelected))
                UpdateEndSelectablePositionsFromStartSelected(e.SourcePosition);

            if (e.PropertyName == nameof(Position.IsEndSelected))
                CompleteMoveFromEndSelected(e.SourcePosition);
        }

        /// <summary>
        /// Determine if the current player has won. This would be the case
        /// if they occupy all the occupied spaces or if no one else can make 
        /// a legal move
        /// </summary>
        /// <returns></returns>
        private WinType CheckForWinConditionOfCurrentPlayer()
        {
            //If player is the only occupier remaining, this player has won
            var allOccupiedPositions = _board.Positions.Where(p => p.Occupier != null);
            if (allOccupiedPositions.All(op => op.Occupier == _board.CurrentTurn))
                return WinType.LastManStanding;

            //If everyone else can't move, this player has won
            if (!Players.Where(p => p.Id != _board.CurrentTurn.Id).Any(p => CanMakeMove(p)))
                return WinType.LegalMoves;

            //the game should continue - there isn't a valid win condition at this time.
            return WinType.NoWin;
        }

        /// <summary>
        /// Independant of whose turn it is, check if the given
        /// player has any legal moves.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private bool CanMakeMove(Player player)
        {
            if (player == null || Status != Status.InProgress)
                return false;

            var allPlayerPositions = _board._positions.Where(p => p.Occupier?.Id == player.Id);
            if (!allPlayerPositions.Any())
                return false;

            foreach (Position position in allPlayerPositions)
            {
                var neighbours = position.GetNeighbours(_board._positions, 2)
                    .Where(n => position.IsStraightLineFrom(n))
                    .ToList();

                if (neighbours.Any(n => n.Occupier == null))
                    return true;
            }

            return false;
        }
    }
}
