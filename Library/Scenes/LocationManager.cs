using Dose.Library.Locations;
using Godot;
using Dose.Library.Math;
using System.Collections.Generic;

public partial class LocationManager : Node3D
{
    const int BLOCK_SIZE = 4;

    readonly static PackedScene _locationScene = GD.Load<PackedScene>("res://Scenes/Location.tscn");

    public override void _Ready()
    {
        var mapGenerator = new CorridorMapGenerator(
            horizontalCells: 500,
            verticalCells: 500,
            probabilityToExtendCorridor: new Probability(0.7),
            probabilityToBifurcate: new Probability(0.25),
            probabilityToBuildDoors: new Probability(0.5)
        );

        var location = _locationScene.Instantiate<Location>().Init(mapGenerator, BLOCK_SIZE);

        AddChild(location);

        var processors = new List<LocationMapProcessor>
        {
            new LocationDoorsProvider(location, BLOCK_SIZE) 
        };

        foreach (var processor in processors)
        {
            processor.Process();
        }
    }
}