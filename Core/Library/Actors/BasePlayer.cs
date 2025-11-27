using Dose.Core.Library.Control;
using Dose.Core.Library.DataStructures;
using Dose.Core.Library.Interactivity;
using Dose.Core.Scenes;
using Godot;

namespace Dose.Core.Library.Actors
{
    public abstract partial class BasePlayer : CharacterBody3D, ITalker, IPicker
    {
        protected enum MoveState
        {
            Locked,
            Looking,
            Focused,
            Free
        }

        protected enum InteractionState
        {
            Curious,
            Talking
        }

        protected Node3D? _head;
        protected Camera3D? _view;
        protected IActor<Player>? _currentTarget;
        protected IControlPlane? _controlPlane;
        protected MoveState _moveState = MoveState.Locked;
        protected InteractionState _interactionState;

        public DialogueTravesser? DialogueTravesser { get; private set; }
        public Inventory? Inventory { get; protected set; }
        public InventoryController? InventoryController {get; protected set; }
        public ItemContext? SelectedItem { get; protected set; }

        public BasePlayer Initialize(IControlPlane controlPlane)
        {
            _controlPlane = controlPlane;

            return this;
        }

        public void OnDialogueBegin(DialogueTravesser dialogueTravesser)
        {
            DialogueTravesser = dialogueTravesser;
            
            if (dialogueTravesser.Dialogue is Dialogue)
            {
                _moveState = MoveState.Focused;
                _interactionState = InteractionState.Talking;
            }
            
            UpdateDialogue();
        }

        public bool CanMove()
        {
            return _moveState == MoveState.Free;
        }

        public bool CanLook()
        {
            return _moveState == MoveState.Free || _moveState == MoveState.Looking;
        }

        public void ProgressDialogue()
        {
            if (DialogueTravesser == null)
            {
                GD.PushWarning("Dialogue Travesser is null. Can't progress dialogue.");
                _moveState = MoveState.Free;
                _interactionState = InteractionState.Curious;
                _controlPlane?.OnDialogueEnd();

                return;
            }

            if (!DialogueTravesser.GoToNextNode())
            {
                DialogueTravesser = null;
                _moveState = MoveState.Free;
                _interactionState = InteractionState.Curious;
                _controlPlane?.OnDialogueEnd();

                UpdateTarget();

                return;
            }

            UpdateDialogue();
        }

        protected void UpdateDialogue()
        {
            if (DialogueTravesser == null) return;

            _controlPlane?.OnDialogueChange(DialogueTravesser.GetCurrentNode());
        }

        protected void UpdateTarget()
        {
            _controlPlane?.OnTargetChange(_currentTarget);
        }

        protected void LookAt(Node3D node, double delta)
        {
            if (_head == null)
            {
                GD.PushWarning("View not defined. Skipping LookAt.");
                return;
            }

            Vector3 from = _head.GlobalPosition;
            Vector3 to = node.GlobalPosition + new Vector3(0, 2, 0);

            // Compute desired rotation
            Basis targetBasis = Basis.LookingAt(to - from, Vector3.Up);

            // Smooth rotation
            Basis currentBasis = _head.GlobalTransform.Basis;
            Basis newBasis = currentBasis.Slerp(targetBasis, (float)(delta * 5f));

            _head.GlobalTransform = new Transform3D(newBasis, from);
        }
    }
}