using Dose.Library.LevelMapping;
using Godot;
using System;
using Dose.Library.Math;

public partial class Location : MeshInstance3D
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

        var meshBuilder = new MapMeshBuilder(map, 4);

        Mesh = meshBuilder.Build();
    }
}