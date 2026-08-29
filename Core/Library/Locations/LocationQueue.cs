using System;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dose.Core.Library.Locations;
using Godot;

class LocationQueue(int maxLocationsPerType, Dictionary<string, Func<ClockDirection, ILocation?>> categoryFuncMap)
{
    readonly Dictionary<(string, ClockDirection), ConcurrentQueue<ILocation>> _nextLocations = [];
    readonly FrozenDictionary<string, Func<ClockDirection, ILocation?>> categoryFuncMap = categoryFuncMap.ToFrozenDictionary();

    public bool IsInitialized { get; private set; } = false;
    public int MaxLocationsPerType { get; } = maxLocationsPerType;
    

    public LocationQueue Initialize()
    {
        if (IsInitialized)
        {
            GD.PushError("LocationQueue has already been initialized.");
        }

        foreach (var category in categoryFuncMap.Keys)
        {
            foreach (var direction in Enum.GetValues<ClockDirection>())
            {
                _nextLocations[(category, direction)] = new ConcurrentQueue<ILocation>();

                while (_nextLocations.TryGetValue((category, direction), out ConcurrentQueue<ILocation>? value) && value.Count < MaxLocationsPerType)
                {
                    var newLocation = categoryFuncMap[category](direction);

                    if (newLocation == null)
                    {
                        GD.PushError($"Failed to create location for category {category} and direction {direction} during LocationQueue initialization.");
                        continue;
                    }

                    _nextLocations[(category, direction)].Enqueue(newLocation);
                }
            }
        }

        IsInitialized = true;

        return this;
    }

    public bool IsEmpty(string category, ClockDirection direction)
    {
        return _nextLocations.TryGetValue((category, direction), out ConcurrentQueue<ILocation>? queue) && queue.IsEmpty;
    }

    public ILocation? PopNextLocation(string category, ClockDirection direction)
    {
        var queue = _nextLocations[(category, direction)];

        if (queue.IsEmpty)
        {
            GD.Print($"No pre-generated locations for category {category} and direction {direction} are available. Generating one on the fly...");
            return categoryFuncMap[category](direction);
        }

        _ = Refill(category, direction);

        queue.TryDequeue(out var nextLocation);

        if (nextLocation == null)
        {
            GD.PushError($"Failed to dequeue next location for category {category} and direction {direction}.");
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

    Task Refill(string category, ClockDirection direction)
    {
        return Task.Run(() =>
        {
            //await Task.Delay(5000);
            var newLocation = categoryFuncMap[category](direction);

            if (newLocation == null)
            {
                GD.PushError($"Failed to create location of type {category} and direction {direction}.");
                return;
            }

            _nextLocations[(category, direction)].Enqueue(newLocation);
        });
    }
}
