using Dose.Library.Locations;
using Godot;
using Dose.Library.Math;

public partial class Location : Node3D
{
    const int BLOCK_SIZE = 4;

    public override void _Ready()
    {
        var mapGenerator = new CorridorMapGenerator(
            horizontalCells: 500,
            verticalCells: 500,
            probabilityToExtendCorridor: new Probability(0.7),
            probabilityToBifurcate: new Probability(0.25),
            probabilityToBuildDoors: new Probability(0.5)
        );

        var map = mapGenerator.GenerateMap();
        var meshBuilder = new LocationMeshBuilder(this, map, BLOCK_SIZE);
        var staticBody = new StaticBody3D();

        var doorsProviderProcessor = new MapProcessor(map, new LocationDoorsProvider(this, BLOCK_SIZE));

        AddChild(staticBody);

        var meshInstance = new MeshInstance3D
        {
            Mesh = meshBuilder.Build()
        };

        staticBody.AddChild(meshInstance);

        meshInstance.CreateTrimeshCollision();

        doorsProviderProcessor.Process();
    }
}