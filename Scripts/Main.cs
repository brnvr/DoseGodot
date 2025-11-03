using Dose.Actors.Npcs;
using Godot;
using System;

namespace Dose.Scripts
{
    public partial class Main : Node3D
	{
		public override void _Ready()
		{
			// Create a MeshInstance3D node
			var plane = new MeshInstance3D();

			// Create a PlaneMesh
			var mesh = new PlaneMesh();
			mesh.Size = new Vector2(4, 4); // Width and height of the plane

			// Assign mesh to the MeshInstance3D
			plane.Mesh = mesh;

			// Optionally create a material
			var material = new StandardMaterial3D();
			material.AlbedoColor = new Color(0.2f, 0.8f, 0.3f); // Green
			plane.MaterialOverride = material;

			// Rotate plane if you want it vertical
			// plane.RotateX(Mathf.DegToRad(-90));

			// Add the plane to the scene
			AddChild(plane);

			var light = new DirectionalLight3D();
			light.RotationDegrees = new Vector3(-45, 0, 0);
			AddChild(light);










			var ui = GetNode<UI>("UI");
			var npc = GetNode<Npc>("Npc");
			var player = GetNode<Player>("Player");

			var controlPlane = GetNode<ControlPlane>("ControlPlane").Initialize(ui, player, npc);
			

		}
	}

}