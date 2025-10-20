using Godot;

namespace Dose.Library
{
    public static class Utils
    {
        public static Mesh LoadMeshFromScene(string path, int childIndex = 0)
        {
            var packedScene = GD.Load<PackedScene>(path);

            var sceneInstance = packedScene.Instantiate();
            var meshInstance = sceneInstance.GetChild<MeshInstance3D>(childIndex);
            var mesh = meshInstance.Mesh;

            sceneInstance.Free();

            return mesh;
        }
    }
}