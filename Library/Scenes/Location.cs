using Dose.Library.Locations;
using Godot;
using System;

public partial class Location() : MeshInstance3D
{
    public Map Map { get; private set; }
    public int BlockSize { get; private set; }
    public IMapGenerator MapGenerator { get; private set; }

    public Location Init(IMapGenerator mapGenerator, int blockSize)
    {
        MapGenerator = mapGenerator;
        BlockSize = blockSize;
        
        if (Map != null)
        {
            throw new Exception("Location has already been initiated");
        }

        Map = MapGenerator.GenerateMap();
        Mesh = new LocationMeshBuilder(Map, BlockSize).Build();

        CreateTrimeshCollision();

        return this;
    }
}
