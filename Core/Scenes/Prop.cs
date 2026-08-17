using Godot;
using Dose.Core.Library.Props;
using Dose.Core.Library.Locations;
using Dose.Core.Library.Numbers;
using static Dose.Core.Library.GodotUtils;

namespace Dose.Core.Scenes
{
    public partial class Prop : RigidBody3D, IWallPositioned
    {
        public PropDefinition? Definition { get; private set; }
        public NormalizedValue MinDistanceFromWall => Definition?.MinDistanceFromWall ?? 0f;
        public NormalizedValue MaxDistanceFromWall => Definition?.MaxDistanceFromWall ?? 1f;
        public NormalizedValue MinDistanceFromCorner => Definition?.MinDistanceFromCorner ?? 0f;
        public NormalizedValue MaxDistanceFromCorner => Definition?.MaxDistanceFromCorner ?? 1f;

        public Prop Initialize(PropDefinition definition)
        {
            Definition = definition;

            var billboardMesh = GetNode<MeshInstance3D>("Mesh");
            var billboardCollision = GetNode<CollisionShape3D>("CollisionShape3D");
            var scale = Vector3.One * definition.Scale;

            if (definition.IsBillboard)
            {
                billboardMesh.Scale *= scale;
                billboardMesh.Position *= definition.Scale;
                billboardMesh.Position += Vector3.Up * definition.YOffset;
                billboardCollision.Scale *= scale;
                billboardCollision.Position *= definition.Scale;
                billboardCollision.Position += Vector3.Up * definition.YOffset;
                billboardMesh.SetSurfaceOverrideMaterial(0, CreateBillboardMaterial(definition.Texture));
                return this;
            }

            billboardMesh.Visible = false;
            billboardCollision.Disabled = true;

            var modelScene = definition.ModelScene
                ?? throw new System.InvalidOperationException("A model prop has no loaded model scene.");

            var model = modelScene.Instantiate<Node3D>();
            model.Scale *= scale;
            model.Position += Vector3.Up * definition.YOffset;
            AddChild(model);

            return this;
        }
    }
}
