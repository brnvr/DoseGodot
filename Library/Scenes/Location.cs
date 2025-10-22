using Dose.Library.LevelMapping;
using Godot;
using Dose.Library.Math;

public partial class Location : Node3D
{
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

        var meshBuilder = new MapMeshBuilder(this, map, 4);
        var mesh = meshBuilder.Build();

        var staticBody = new StaticBody3D();
        AddChild(staticBody);

        var meshInstance = new MeshInstance3D();
        meshInstance.Mesh = mesh;
        staticBody.AddChild(meshInstance);

        meshInstance.CreateTrimeshCollision();
    }
}