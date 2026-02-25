using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dose.Core.Library.Locations;
using Godot;

class LocationQueue(ILocationCreator ILocationCreator, int maxLocationsPerType)
{
    readonly Dictionary<(LocationType, ClockDirection), ConcurrentQueue<ILocation>> _nextLocations = [];

    public bool IsInitialized { get; private set; } = false;
    public int MaxLocationsPerType { get; } = maxLocationsPerType;
    public ILocationCreator ILocationCreator { get; } = ILocationCreator;

    public void Initialize()
    {
        if (IsInitialized)
        {
            GD.PushError("LocationQueue has already been initialized.");
        }

        foreach (var type in Enum.GetValues<LocationType>())
        {
            foreach (var direction in Enum.GetValues<ClockDirection>())
            {
                _nextLocations[(type, direction)] = new ConcurrentQueue<ILocation>();

                while (_nextLocations.TryGetValue((type, direction), out ConcurrentQueue<ILocation>? value) && value.Count < MaxLocationsPerType)
                {
                    var newLocation = ILocationCreator.CreateLocation(type, direction);

                    if (newLocation == null)
                    {
                        GD.PushError($"Failed to create location of type {type} and direction {direction} during LocationQueue initialization.");
                        continue;
                    }

                    _nextLocations[(type, direction)].Enqueue(newLocation);
                }
            }
        }

        IsInitialized = true;
    }

    public bool IsEmpty(LocationType type, ClockDirection direction)
    {
        return _nextLocations.TryGetValue((type, direction), out ConcurrentQueue<ILocation>? queue) && queue.IsEmpty;
    }

    public ILocation? PopNextLocation(LocationType type, ClockDirection direction)
    {
        var queue = _nextLocations[(type, direction)];

        if (queue.IsEmpty)
        {
            GD.Print($"No pre-generated locations of type {type} and direction {direction} are available. Generating one on the fly...");
            return ILocationCreator.CreateLocation(type, direction);
        }

        _ = Refill(type, direction);

        queue.TryDequeue(out var nextLocation);

        if (nextLocation == null)
        {
            GD.PushError($"Failed to dequeue next location of type {type} and direction {direction}.");
        }

        return nextLocation;
    }

    public void Free()
    {
        foreach (var queue in _nextLocations.Values)
        {
            while (queue.TryDequeue(out var location))
            {
                if (location is Node locationNode)
                {
                    locationNode.QueueFree();
                }
            }
        }
    }

    async Task Refill(LocationType type, ClockDirection direction)
    {
        await Task.Run(async () =>
        {
            //await Task.Delay(5000);
            var newLocation = ILocationCreator.CreateLocation(type, direction);

            if (newLocation == null)
            {
                GD.PushError($"Failed to create location of type {type} and direction {direction}.");
                return;
            }

            _nextLocations[(type, direction)].Enqueue(newLocation);
        });
    }
}