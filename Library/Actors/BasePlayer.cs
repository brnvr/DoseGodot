using Dose.Library.Control;
using Dose.Library.DataStructures;
using Dose.Scripts;
using Godot;

namespace Dose.Library.Actors
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