using Dose.Library.LevelMapping;
using Godot;
using System;
using Dose.Library.Math;

public partial class Wall : MeshInstance3D
{
    public override void _Ready()
    {
        var mapGenerator = new CorridorMapGenerator(
            horizontalCells: 50,
            verticalCells: 50,
            probabilityToContinueGenerating: new Probability(0.75),
            probabilityToBifurcate: new Probability(0.75),
            probabilityToBuildDoors: new Probability(0.5)
        );

        var map = mapGenerator.GenerateMap();

        var meshBuilder = new MapMeshBuilder(map, 4);

        Mesh = meshBuilder.Build();
    }
}