using System;
using System.Collections.Generic;
using Godot;

namespace Dose.Core.Library
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

        public static ImageTexture CreatePaletteTexture(List<Color> colors)
        {
            var image = Image.CreateEmpty(
                colors.Count,
                1,
                false,
                Image.Format.Rgba8
            );

            for (int i = 0; i < colors.Count; i++)
            {
                image.SetPixel(i, 0, colors[i]);
            }

            return ImageTexture.CreateFromImage(image);
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

        public static StandardMaterial3D CreatePlainMaterial(Texture2D texture)
		{
			return new StandardMaterial3D
			{
				AlbedoTexture = texture,
				ShadingMode = BaseMaterial3D.ShadingModeEnum.PerVertex,
				VertexColorUseAsAlbedo = false,
				TextureFilter = BaseMaterial3D.TextureFilterEnum.Nearest
			};
		}

        public static StandardMaterial3D CreateBillboardMaterial(Texture2D? texture)
        {
            return new StandardMaterial3D
            {
                AlbedoTexture = texture,
                BillboardMode = BaseMaterial3D.BillboardModeEnum.FixedY,
                Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
                TextureFilter = BaseMaterial3D.TextureFilterEnum.Nearest,
                ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
            };
        }

        public static Mesh? FlipNormals(Mesh original)
        {
            if (original is ArrayMesh arrayMesh)
            {
                var newMesh = new ArrayMesh();
                int surfaceCount = arrayMesh.GetSurfaceCount();

                for (int s = 0; s < surfaceCount; s++)
                {
                    var arrays = arrayMesh.SurfaceGetArrays(s);

                    Vector3[] normals  = arrays[(int)Mesh.ArrayType.Normal].As<Vector3[]>();
                    int[] indices      = arrays[(int)Mesh.ArrayType.Index].As<int[]>();

                    if (normals != null)
                    {
                        for (int i = 0; i < normals.Length; i++)
                            normals[i] = -normals[i];

                        arrays[(int)Mesh.ArrayType.Normal] = normals;
                    }

                    if (indices != null && indices.Length % 3 == 0)
                    {
                        for (int i = 0; i < indices.Length; i += 3)
                        {
                            (indices[i + 2], indices[i]) = (indices[i], indices[i + 2]);
                        }

                        arrays[(int)Mesh.ArrayType.Index] = indices;
                    }

                    newMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);
                }

                return newMesh;
            }

            var st = new SurfaceTool();
            st.CreateFrom(original, 0);
            st.Index();
            var committed = st.Commit();
            if (committed != null)
                return FlipNormals(committed);

            return null;
        }
    }
}