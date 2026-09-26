using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using Dose.Core.Library.Locations;
using Godot;

class LocationQueue(int maxLocationsPerType, Dictionary<string, Func<ClockDirection, ILocation?>> categoryFuncMap)
{
    const ulong REFILL_INTERVAL_MILLISECONDS = 500;

    readonly Dictionary<(string, ClockDirection), Queue<ILocation>> _nextLocations = [];
    readonly Queue<(string Category, ClockDirection Direction)> _pendingRefills = [];
    readonly HashSet<(string Category, ClockDirection Direction)> _pendingRefillSet = [];
    readonly FrozenDictionary<string, Func<ClockDirection, ILocation?>> _categoryFuncMap =
        categoryFuncMap.ToFrozenDictionary();
    ulong _nextRefillAt;

    public bool IsInitialized { get; private set; }
    public int MaxLocationsPerType { get; } = maxLocationsPerType;

    public LocationQueue Initialize()
    {
        if (IsInitialized)
        {
            GD.PushError($"{nameof(LocationQueue)} has already been initialized.");
            return this;
        }

        foreach (var category in _categoryFuncMap.Keys)
        {
            foreach (var direction in Enum.GetValues<ClockDirection>())
            {
                var queue = new Queue<ILocation>();
                _nextLocations[(category, direction)] = queue;

                while (queue.Count < MaxLocationsPerType)
                {
                    var newLocation = _categoryFuncMap[category](direction);

                    if (newLocation == null)
                    {
                        GD.PushError($"Failed to create location for category {category} and direction {direction} during {nameof(LocationQueue)} initialization.");
                        break;
                    }

                    queue.Enqueue(newLocation);
                }
            }
        }

        IsInitialized = true;
        return this;
    }

    public bool IsEmpty(string category, ClockDirection direction) =>
        _nextLocations.TryGetValue((category, direction), out var queue) && queue.Count == 0;

    public ILocation? PopNextLocation(string category, ClockDirection direction)
    {
        var queue = _nextLocations[(category, direction)];
        ILocation? nextLocation;

        if (queue.Count == 0)
        {
            GD.Print($"No pre-generated locations for category {category} and direction {direction} are available. Generating one on the fly...");
            nextLocation = _categoryFuncMap[category](direction);
        }
        else
        {
            nextLocation = queue.Dequeue();
        }

        RequestRefill(category, direction);
        return nextLocation;
    }

    public void ProcessPendingRefill()
    {
        if (_pendingRefills.Count == 0 || Time.GetTicksMsec() < _nextRefillAt)
        {
            return;
        }

        var key = _pendingRefills.Dequeue();
        _pendingRefillSet.Remove(key);
        var queue = _nextLocations[key];

        if (queue.Count < MaxLocationsPerType)
        {
            var newLocation = _categoryFuncMap[key.Category](key.Direction);

            if (newLocation == null)
            {
                GD.PushError($"Failed to create location of type {key.Category}.");
            }
            else
            {
                queue.Enqueue(newLocation);
            }
        }

        if (queue.Count < MaxLocationsPerType)
        {
            RequestRefill(key.Category, key.Direction);
        }

        _nextRefillAt = Time.GetTicksMsec() + REFILL_INTERVAL_MILLISECONDS;
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

        _pendingRefills.Clear();
        _pendingRefillSet.Clear();
    }

    void RequestRefill(string category, ClockDirection direction)
    {
        var key = (category, direction);

        if (!_pendingRefillSet.Add(key))
        {
            return;
        }

        _pendingRefills.Enqueue(key);

        if (_pendingRefills.Count == 1)
        {
            _nextRefillAt = Time.GetTicksMsec() + REFILL_INTERVAL_MILLISECONDS;
        }
    }
}
