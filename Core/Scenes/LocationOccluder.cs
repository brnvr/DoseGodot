using Dose.Core.Library.Locations;
using Godot;
namespace Dose.Core.Scenes {
    public partial class LocationOccluder : Node3D, ILocationOccluder
    {
        public LocationOccluder Initialize(Mesh wallMesh, Mesh doorwayMesh, Mesh floorMesh, Mesh ceilingMesh, int blockSize, StandardMaterial3D material)
        {
            var meshInstance = GetNode<MeshInstance3D>("MeshInstance3D");

            var st = new SurfaceTool();
            st.Begin(Mesh.PrimitiveType.Triangles);

            var baseTransform = Transform3D.Identity.Scaled(new Vector3(blockSize, blockSize, blockSize));

            st.AppendFrom(doorwayMesh, 0, baseTransform);
            st.AppendFrom(wallMesh, 0, baseTransform.Translated(new Vector3(0, 0, -blockSize)).Rotated(Vector3.Up, Mathf.DegToRad(180)));
            st.AppendFrom(wallMesh, 0, baseTransform.Translated(new Vector3(-blockSize * .5f, 0, -blockSize * .5f)).Rotated(Vector3.Up, Mathf.DegToRad(90)));
            st.AppendFrom(wallMesh, 0, baseTransform.Translated(new Vector3(blockSize * .5f, 0, -blockSize * .5f)).Rotated(Vector3.Up, Mathf.DegToRad(270)));
            st.AppendFrom(floorMesh, 0, baseTransform.Translated(new Vector3(0, 0, blockSize * .5f)));
            st.AppendFrom(ceilingMesh, 0, baseTransform.Translated(new Vector3(0, 0, blockSize * .5f)));

            meshInstance.Mesh = st.Commit();
            meshInstance.MaterialOverride = material;

            return this;
        }

        ILocationOccluder ILocationOccluder.Initialize(Mesh wallMesh, Mesh doorwayMesh, Mesh floorMesh, Mesh ceilingMesh, int blockSize, StandardMaterial3D material)
        {
            return Initialize(wallMesh, doorwayMesh, floorMesh, ceilingMesh, blockSize, material);
        }
    }
}