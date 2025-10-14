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
            probabilityToContinueGenerating: new Probability(0.5),
            probabilityToBifurcate: new Probability(0.75),
            probabilityToBuildDoors: new Probability(0.5)
        );
        var map = mapGenerator.GenerateMap();

        for (var i = 0; i < 50; i++)
        {
            var line = "";
            for (var j = 0; j < 50; j++)
            {
                line += map.Get(new Vector2I(j, i)) == null ? "0" : "1";
            }
            Console.WriteLine(line);
        }

        var meshBuilder = new MapMeshBuilder(map, 1);

        var mesh = new ArrayMesh();

        // Define vertex data
        var vertices = new Vector3[]
            {
                new(0, 1, 0),
                new(5, 1, 0),
                new(5, 0, 0),
                new(5, 0, 0),
                new(0, 0, 0),
                new(0, 1, 0)
            };

        var normals = new Vector3[]
        {
            Vector3.Forward,
            Vector3.Forward,
            Vector3.Forward,
            Vector3.Forward,
            Vector3.Forward,
            Vector3.Forward
        };

        // var uvs = new Vector2[]
        // {
        //     new Vector2(0.5f, 0),
        //     new Vector2(1, 1),
        //     new Vector2(0, 1),
        // };

        var indices = new int[] { 0, 1, 2, 3, 4, 5 };

        // Prepare array structure
        var arrays = new Godot.Collections.Array();
        arrays.Resize((int)Mesh.ArrayType.Max);
        arrays[(int)Mesh.ArrayType.Vertex] = vertices;
        arrays[(int)Mesh.ArrayType.Normal] = normals;
        //arrays[(int)Mesh.ArrayType.TexUV] = uvs;
        arrays[(int)Mesh.ArrayType.Index] = indices;

        // Add surface to mesh
        mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);

        // Assign mesh to this MeshInstance3D
        Mesh = meshBuilder.Build();
    }
}