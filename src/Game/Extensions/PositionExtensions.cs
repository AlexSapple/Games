using System;
using System.Collections.Generic;
using System.Linq;
using Game.BoardGame;

namespace Game.Extensions
{
    /// <summary>
    /// Position extensions
    /// </summary>
    public static class PositionExtensions
    {
        /// <summary>
        /// Determine if the position and the candiate position are situated on a straight line
        /// axis from one another. This means you could draw a straight line from one to the other
        /// </summary>
        /// <param name="position"></param>
        /// <param name="candidatePosition"></param>
        /// <returns></returns>
        public static bool IsStraightLineFrom(this Position position, Position candidatePosition)
        {
            if (candidatePosition == null)
                return false;

            //the positions share an x coordinate, therefore a straight line can be drawn vertically between them.
            if (position.XCoordinate == candidatePosition.XCoordinate)
                return true;

            //the positions share a y coordinate, therefore a stright line can be drawn horizontally between them
            if (position.YCoordinate == candidatePosition.YCoordinate)
                return true;

            //now for diagonals - slighlty more complex, but not really. How far apart on each axis and 
            //if equdistance apart on each axis, then a straight line can be drawn in a diagonal.
            int xDegreeOfSeperation = Math.Abs(position.XCoordinate - candidatePosition.XCoordinate);
            int yDegreeOfSeperation = Math.Abs(position.YCoordinate - candidatePosition.YCoordinate);

            return xDegreeOfSeperation == yDegreeOfSeperation;
        }

        public static List<Position> GetNeighbours(this Position position, List<Position> candidatePositions, int degreeOfSeperation = 1, List<Position> neighbours = null)
        {
            neighbours ??= new List<Position>();

            if (position == null || candidatePositions == null)
                return neighbours;

            while (degreeOfSeperation > 0)
            {
                neighbours.AddRange(GetNeighboursByOrientation(position, candidatePositions, NeighbourOrientation.Top, degreeOfSeperation));
                neighbours.AddRange(GetNeighboursByOrientation(position, candidatePositions, NeighbourOrientation.Right, degreeOfSeperation));
                neighbours.AddRange(GetNeighboursByOrientation(position, candidatePositions, NeighbourOrientation.Bottom, degreeOfSeperation));
                neighbours.AddRange(GetNeighboursByOrientation(position, candidatePositions, NeighbourOrientation.Left, degreeOfSeperation));

                degreeOfSeperation--;
                neighbours.AddRange(GetNeighbours(position, candidatePositions, degreeOfSeperation, neighbours));
            }

            return neighbours.Distinct().ToList();
        }

        /// <summary>
        /// Determine all neighbours of a given position by orientation.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private static List<Position> GetNeighboursByOrientation(Position position, List<Position> candidatePositions, NeighbourOrientation orientation, int degreeOfSeperation = 1)
        {
            if (position == null || candidatePositions == null)
                return new List<Position>();

            List<Position> positions = new List<Position>();

            //start with axis 1 (which is the immediate orientation of the line)
            positions = orientation switch
            {
                NeighbourOrientation.Left => candidatePositions.Where(p => p.XCoordinate == position.XCoordinate - degreeOfSeperation).ToList(),
                NeighbourOrientation.Right => candidatePositions.Where(p => p.XCoordinate == position.XCoordinate + degreeOfSeperation).ToList(),
                NeighbourOrientation.Top => candidatePositions.Where(p => p.YCoordinate == position.YCoordinate - degreeOfSeperation).ToList(),
                NeighbourOrientation.Bottom => candidatePositions.Where(p => p.YCoordinate == position.YCoordinate + degreeOfSeperation).ToList(),
                _ => throw new ArgumentException("direction not defined", nameof(orientation))
            };

            //then axis 2 (which will chop the line to our boundaries based on the opposite axis.
            positions = orientation switch
            {
                var verticalLine when (orientation == NeighbourOrientation.Left || orientation == NeighbourOrientation.Right)
                    => positions.Where(p => p.YCoordinate >= position.YCoordinate - degreeOfSeperation
                                        && p.YCoordinate <= position.YCoordinate + degreeOfSeperation).ToList(),

                var horizontalLine when (orientation == NeighbourOrientation.Top || orientation == NeighbourOrientation.Bottom)
                    => positions.Where(p => p.XCoordinate >= position.XCoordinate - degreeOfSeperation
                                        && p.XCoordinate <= position.XCoordinate + degreeOfSeperation).ToList(),

                _ => throw new ArgumentException("direction not defined", nameof(orientation))
            };

            return positions;
        }

        /// <summary>
        /// Enum for use with the neighbour calculations
        /// </summary>
        private enum NeighbourOrientation
        {
            Left,
            Top,
            Right,
            Bottom
        }
    }
}
