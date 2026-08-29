using Dose.Core.ActorArgs;
using Dose.Core.Interactions;
using Dose.Core.Library.Actors;
using Dose.Core.Library.Interactivity;
using Dose.Core.Library.Locations;
using Dose.Core.Library;
using Godot;
using System;
using System.Linq;

namespace Dose.Core.Scenes
{
    public abstract partial class Stairs : Node3D, IActor<Player>, IStairs, IInitializableActor<StairsContext, StairsArgs>
    {
        public abstract StairsDirection Direction { get; }   
        public IStairs? Linked { get; private set; }
        public ILocation? Location { get; private set; }
        public Vector2I MapPosition { get; set; }
        public GeoPositioning.CardinalDirection MapDirection { get; set; }
        public StairsContext? Context { get; private set; }

        IActorContext? IActor.Context => Context;

        IPortal? IPortal.Linked => Linked;


        public void Link(Stairs other)
        {
            if (Linked == other)
            {
                return;
            }

            Linked = other;
            other.Link(this);
        }

        public IInteraction<Player> GetDefaultInteraction(Player interactor)
        {
            return new UseStairsInteraction<Player>(interactor);
        }

        public void OnInteract(IInteraction<Player> interaction)
        {
            interaction.Perform(this);
        }

        void IPortal.Link(IPortal other) => Link((Stairs)other);

        public void Unlink()
        {
            Linked = null;
        }

        public void Initialize(StairsContext context, StairsArgs args)
        {
            Context = context;
            Location = args.Location;

            foreach (var meshInstance in FindChildren("*", "MeshInstance3D", true, false)
                .OfType<MeshInstance3D>())
            {
                var mesh = meshInstance.Mesh;

                if (mesh == null)
                {
                    continue;
                }

                for (var surfaceIndex = 0; surfaceIndex < mesh.GetSurfaceCount(); surfaceIndex++)
                {
                    var importedMaterial = mesh.SurfaceGetMaterial(surfaceIndex);
                    var activeMaterial = meshInstance.GetActiveMaterial(surfaceIndex);
                    var surfaceName = mesh is ArrayMesh arrayMesh
                        ? arrayMesh.SurfaceGetName(surfaceIndex)
                        : null;

                    bool HasName(string name) =>
                        string.Equals(surfaceName, name, StringComparison.OrdinalIgnoreCase)
                        || string.Equals(
                            importedMaterial?.ResourceName,
                            name,
                            StringComparison.OrdinalIgnoreCase
                        )
                        || string.Equals(
                            activeMaterial?.ResourceName,
                            name,
                            StringComparison.OrdinalIgnoreCase
                        );

                    if (HasName("Walls"))
                    {
                        meshInstance.SetSurfaceOverrideMaterial(surfaceIndex, args.WallMaterial);
                    }
                    else if (HasName("Black"))
                    {
                        meshInstance.SetSurfaceOverrideMaterial(surfaceIndex, args.BlackMaterial);
                    }
                }
            }
        }
    }
}
