using Dose.Core.Library.Locations;
using Dose.Core.Library.Randomness;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Dose.Core.Scenes
{
    public abstract partial class Location : Node3D, ILocation
    {
        public NavigationRegion3D? NavigationRegion { get; private set; }

        public enum LocationType
        {
            Room, Corridor
        }

        public abstract LocationType? Type { get; }

        public Location InitializeNavigationRegion(string floorNodeName = "FloorMeshInstance")
        {
            var floorNode = FindChild(floorNodeName, owned: false);
            
            if (floorNode == null) 
            {
                GD.PushWarning($"Child not found: {floorNodeName}");
                return this;
            }

            if (NavigationRegion != null)
            {
                GD.PushWarning($"{nameof(NavigationRegion)} has already been initialized.");
                return this;
            }
    
            NavigationRegion = new NavigationRegion3D
            { 
                NavigationMesh = new NavigationMesh
                {
                    GeometryParsedGeometryType = NavigationMesh.ParsedGeometryType.StaticColliders
                }
            };

            AddChild(NavigationRegion);
            
            var sourceData = new NavigationMeshSourceGeometryData3D();

            NavigationServer3D.ParseSourceGeometryData(
                NavigationRegion.NavigationMesh,
                sourceData,
                floorNode
            );

            NavigationServer3D.BakeFromSourceGeometryData(
                NavigationRegion.NavigationMesh,
                sourceData
            );

                GD.Print($"Source has data: {sourceData.HasData()}");
                GD.Print($"Source vertices: {sourceData.GetVertices().Count()}");
                GD.Print($"Source indices: {sourceData.GetIndices().Count()}");

            return this;
        }

        public TPortal SelectRandomPortal<TPortal>(Func<TPortal, bool>? predicate = null) where TPortal : IPortal
        {
            var portals = GetChildren().OfType<TPortal>();

            if (predicate != null)
            {
                portals = portals.Where(predicate);
            }

            return new RandomSelector<TPortal>(portals).Pick();
        }

        public void Clear(int keepLevels)
        {
            Clear(keepLevels, null);
        }

        void Clear(int keepLevels, Location? previous)
        {
            var doors = GetChildren().OfType<Door>();
            var stairs = GetChildren().OfType<Stairs>();

            Clear(doors, keepLevels, previous);
            Clear(stairs, keepLevels, previous);
        }

        void Clear(IEnumerable<IPortal> portals, int keepLevels, Location? previous)
        {
            foreach (var portal in portals)
            {
                var otherLocation = portal.Linked?.Location;
                
                if (otherLocation is Location _otherLocation && _otherLocation != previous)
                {
                    _otherLocation.Clear(keepLevels-1, this); 

                    if (keepLevels <= 0)
                    {
                        portal.Unlink();
                        _otherLocation.QueueFree();
                    }
                }
            }
        }
    }

}