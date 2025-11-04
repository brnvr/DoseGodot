using Dose.Core.Library.Control;
using Dose.Core.Library.DataStructures;
using Dose.Core.Scenes;
using Godot;

namespace Dose.Core.Library.Actors
{
    public abstract partial class BasePlayer : CharacterBody3D
    {
        protected IControlPlane? _controlPlane;

        //public Inventory? Inventory { get; protected set; }

        public BasePlayer Initialize(IControlPlane controlPlane)
        {
            _controlPlane = controlPlane;

            // if (controlPlane.ActorRegistry == null)
            // {
            //     GD.PushError($"{nameof(ControlPlane)}.{nameof(ActorRegistry)} is null.");

            //     return this;
            // }

            // Inventory = new Inventory(controlPlane.ActorRegistry);

            return this;
        }
    }
}