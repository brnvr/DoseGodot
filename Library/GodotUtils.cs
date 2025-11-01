using System;
using Godot;

namespace Dose.Library
{
    public static class GodotUtils
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

        public static bool TryGetParentNode(Node node, Func<Node, bool> predicate, out Node? parentNode)
        {
            while (node != null && !predicate(node))
            {
                node = node.GetParent();
            }

            if (node == null)
            {
                parentNode = null;
                return false;
            }

            parentNode = node;
            return true;
        }

        public static bool TryGetParentNode<T>(Node node, out T? parentNode) where T : Node
        {
            while (node != null && node is not T)
            {
                node = node.GetParent();
            }

            if (node is T t)
            {
                parentNode = t;
                return true;
            }

            parentNode = null;
            return false;
        }
    }
}