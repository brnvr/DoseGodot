using Godot;
using Dose.Core.Library.Props;
using static Dose.Core.Library.GodotUtils;

namespace Dose.Core.Scenes
{
    public partial class Prop : RigidBody3D
    {
        public PropDefinition? Definition { get; private set; }

        public Prop Initialize(PropDefinition definition)
        {
            Definition = definition;

            var billboardMesh = GetNode<MeshInstance3D>("Mesh");
            var billboardCollision = GetNode<CollisionShape3D>("CollisionShape3D");

            if (definition.IsBillboard)
            {
                billboardMesh.SetSurfaceOverrideMaterial(0, CreateBillboardMaterial(definition.Texture));
                return this;
            }

            billboardMesh.Visible = false;
            billboardCollision.Disabled = true;

            var modelScene = definition.ModelScene
                ?? throw new System.InvalidOperationException("A model prop has no loaded model scene.");

            AddChild(modelScene.Instantiate<Node3D>());

            return this;
        }
    }
}
