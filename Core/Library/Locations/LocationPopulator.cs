using System.Collections.Generic;
using System.Linq;
using System;
using Dose.Core.Library.Numbers;
using Dose.Core.Library.Props;
using Dose.Core.Library.Randomness;
using Godot;
using static Dose.Core.Library.GeoPositioning;

namespace Dose.Core.Library.Locations
{
    public abstract class LocationPopulator(
        IBuildableLocation location,
        Probability probabilityPopulate,
        int blockSize
    )
    {
        const int WALL_POSITION_SUBDIVISIONS = 2;

        readonly record struct PopulationPosition(Vector2I Cell, Vector2 Offset);

        public IBuildableLocation Location { get; } = location;
        public Probability ProbabilityPopulate { get; } = probabilityPopulate;
        public int BlockSize { get; } = blockSize;

        public List<Node3D> Populate(
            HashSet<Vector2I> populatedPositions,
            out HashSet<Vector2I> populatedPositionsOut
        )
        {
            populatedPositionsOut = [.. populatedPositions];
            List<Node3D> targets = [];

            if (Location.Map == null)
            {
                GD.PushWarning("Location.Map is null. Cannot populate location.");
                return targets;
            }

            var decider = new RandomDecider(ProbabilityPopulate);

            while (decider.Roll())
            {
                var target = CreateTarget();

                if (target == null)
                {
                    return targets;
                }

                if (TryAddTarget(target))
                {
                    targets.Add(target);
                    continue;
                }

                var availablePositions = GetAvailablePositions(target, populatedPositionsOut);

                if (availablePositions.Count == 0)
                {
                    target.Free();
                    return targets;
                }

                var position = new RandomSelector<PopulationPosition>(availablePositions).Pick();

                populatedPositionsOut.Add(position.Cell);
                AddTarget(target, position);
                targets.Add(target);
            }

            return targets;
        }

        List<PopulationPosition> GetAvailablePositions(
            Node3D target,
            HashSet<Vector2I> populatedPositions
        )
        {
            var positions = Location.Map?.GetFilledPositions().Except(populatedPositions) ?? [];

            if (target is not IWallPositioned wallPositioned)
            {
                return positions
                    .Select(position => new PopulationPosition(position, Vector2.Zero))
                    .ToList();
            }

            return positions
                .SelectMany(GetSubdividedPositions)
                .Where(position => IsWithinWallDistance(
                    position.Cell,
                    position.Offset,
                    wallPositioned.MinDistanceFromWall,
                    wallPositioned.MaxDistanceFromWall,
                    wallPositioned.MinDistanceFromCorner,
                    wallPositioned.MaxDistanceFromCorner
                ))
                .ToList();
        }

        static IEnumerable<PopulationPosition> GetSubdividedPositions(Vector2I cell)
        {
            for (var x = 0; x < WALL_POSITION_SUBDIVISIONS; x++)
            {
                for (var y = 0; y < WALL_POSITION_SUBDIVISIONS; y++)
                {
                    var offset = new Vector2(
                        (x + 0.5f) / WALL_POSITION_SUBDIVISIONS - 0.5f,
                        (y + 0.5f) / WALL_POSITION_SUBDIVISIONS - 0.5f
                    );

                    yield return new PopulationPosition(cell, offset);
                }
            }
        }

        bool IsWithinWallDistance(
            Vector2I position,
            Vector2 offset,
            NormalizedValue minimumDistance,
            NormalizedValue maximumDistance,
            NormalizedValue minimumCornerDistance,
            NormalizedValue maximumCornerDistance
        )
        {
            var horizontalDistance = GetNormalizedDistanceToClosestWall(
                position,
                offset,
                CardinalDirection.East,
                CardinalDirection.West
            );
            var verticalDistance = GetNormalizedDistanceToClosestWall(
                position,
                offset,
                CardinalDirection.North,
                CardinalDirection.South
            );
            if (horizontalDistance == null || verticalDistance == null)
            {
                return false;
            }

            var closestWallDistance = Mathf.Min(horizontalDistance.Value, verticalDistance.Value);
            var closestCornerDistance = Mathf.Sqrt(
                horizontalDistance.Value * horizontalDistance.Value
                + verticalDistance.Value * verticalDistance.Value
            ) / Mathf.Sqrt(2f);

            return closestWallDistance >= minimumDistance
                && closestWallDistance <= maximumDistance
                && closestCornerDistance >= minimumCornerDistance
                && closestCornerDistance <= maximumCornerDistance;
        }

        float? GetNormalizedDistanceToClosestWall(
            Vector2I position,
            Vector2 offset,
            CardinalDirection direction,
            CardinalDirection oppositeDirection
        )
        {
            var distance = GetDistanceToWall(position, offset, direction);
            var oppositeDistance = GetDistanceToWall(position, offset, oppositeDirection);

            if (distance == null || oppositeDistance == null)
            {
                return null;
            }

            var distanceToClosestWall = Mathf.Min(distance.Value, oppositeDistance.Value);
            var distanceBetweenWalls = distance.Value + oppositeDistance.Value;
            var minimumSubdividedDistance = 0.5f / WALL_POSITION_SUBDIVISIONS;
            var maximumSubdividedDistance = distanceBetweenWalls / 2f - minimumSubdividedDistance;

            if (maximumSubdividedDistance <= minimumSubdividedDistance)
            {
                return 0f;
            }

            return (distanceToClosestWall - minimumSubdividedDistance)
                / (maximumSubdividedDistance - minimumSubdividedDistance);
        }

        float? GetDistanceToWall(
            Vector2I position,
            Vector2 offset,
            CardinalDirection direction
        )
        {
            if (Location.Map == null)
            {
                return null;
            }

            var currentPosition = position;
            var distance = direction switch
            {
                CardinalDirection.North => 0.5f + offset.Y,
                CardinalDirection.South => 0.5f - offset.Y,
                CardinalDirection.East => 0.5f - offset.X,
                CardinalDirection.West => 0.5f + offset.X,
                _ => 0.5f
            };

            while (Location.Map.TryGet(currentPosition, out MapCell? cell) && cell != null)
            {
                if (cell.GetWall(direction) != null)
                {
                    return distance;
                }

                currentPosition = MoveTo(currentPosition, direction);
                distance += 1f;
            }

            return null;
        }

        protected void AddTarget(Node3D target, Vector3 position)
        {
            if (Location is Node locationNode)
            {
                locationNode.AddChild(target);
            }

            target.Position = position;
        }

        void AddTarget(Node3D target, PopulationPosition position)
        {
            if (Location.Map == null)
            {
                return;
            }

            AddTarget(
                target,
                new Vector3(
                    position.Cell.X + position.Offset.X - Location.Map.CenterX,
                    0,
                    position.Cell.Y + position.Offset.Y - Location.Map.CenterY
                ) * BlockSize
            );
        }

        protected virtual bool TryAddTarget(Node3D target)
        {
            return false;
        }

        protected abstract Node3D? CreateTarget();
    }

    public class ItemLocationPopulator(
        IBuildableLocation location,
        Func<Node3D?> targetFactory,
        Probability probabilityPopulate,
        int blockSize,
        IEnumerable<IItemSurface> props
    ) : LocationPopulator(location, probabilityPopulate, blockSize)
    {
        readonly List<IItemSurface> _availableSurfaceProps =
        [
            .. props.Where(prop => prop.CanPlaceItems)
        ];

        protected override Node3D? CreateTarget() => targetFactory();

        protected override bool TryAddTarget(Node3D target)
        {
            while (_availableSurfaceProps.Count > 0)
            {
                var prop = new RandomSelector<IItemSurface>(_availableSurfaceProps).Pick();
                _availableSurfaceProps.Remove(prop);

                if (Location is not Node3D locationNode)
                {
                    return false;
                }

                if (!prop.TryGetSurfacePosition(locationNode, out var surfacePosition))
                {
                    continue;
                }

                AddTarget(target, surfacePosition);

                if (target is PhysicsBody3D targetBody && prop is PhysicsBody3D propBody)
                {
                    targetBody.AddCollisionExceptionWith(propBody);
                    propBody.AddCollisionExceptionWith(targetBody);
                }

                return true;
            }

            return false;
        }
    }

    public class TargetLocationPopulator(
        IBuildableLocation location,
        Func<Node3D?> targetFactory,
        Probability probabilityPopulate,
        int blockSize
    ) : LocationPopulator(location, probabilityPopulate, blockSize)
    {
        public Func<Node3D?> TargetFactory { get; } = targetFactory;

        protected override Node3D? CreateTarget() => TargetFactory();
    }
}
