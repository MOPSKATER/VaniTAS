using System.Reflection;
using UnityEngine;

namespace VaniTAS
{
    internal class Editor : Player
    {
        private readonly GameInput _gameInput = Singleton<GameInput>.Instance;
        private readonly MethodInfo GetInputActionForGameAction = typeof(GameInput).GetMethod("GetInputActionForGameAction", BindingFlags.NonPublic | BindingFlags.Instance);


        public static bool Continue { get; set; } = false;

        public new void Awake()
        {
            base.Awake();
            _currentAction = _inputs[0];
        }

        void FixedUpdate()
        {
            if (FrameCounter.CurrentFrameCount >= _inputs.Count) return;
            _currentAction = _inputs[FrameCounter.CurrentFrameCount - 1];
            if (!Trigger()) return;
            Xinfo.SetValue(RM.drifter.mouseLookX, _currentAction.Pitch);
            Yinfo.SetValue(RM.drifter.mouseLookY, _currentAction.Yaw);
        }

        void Update()
        {
            if (Continue) return;

            Vector2 vector = GetAxisRaw();

            MouseLook mouseLookX = RM.drifter.mouseLookX;
            MouseLook mouseLookY = RM.drifter.mouseLookY;

            mouseLookX.SetRotationX(mouseLookX.RotationX + vector.x);
            mouseLookY.SetRotationY(mouseLookY.RotationY - vector.y);

            RM.drifter.m_cameraRotationX = RM.drifter.m_motorRotation;


            if (GetButtonDown(GameInput.GameActions.Jump))
                _currentAction.Jump = !_currentAction.Jump;

            if (GetButtonDown(GameInput.GameActions.FireCard))
                _currentAction.Fire = !_currentAction.Fire;

            if (GetButtonDown(GameInput.GameActions.FireCardAlt))
                _currentAction.Ability = !_currentAction.Ability;

            if (GetButtonDown(GameInput.GameActions.SwapCard))
                _currentAction.Swap = !_currentAction.Swap;
        }

        void OnGUI()
        {
            GUI.Label(new Rect(10, 160, 150, 150), _currentAction.ToString(), Main.Style);
        }

        private bool GetButtonDown(GameInput.GameActions button)
        {
            UnityEngine.InputSystem.InputAction inputActionForGameAction = (UnityEngine.InputSystem.InputAction)GetInputActionForGameAction.Invoke(_gameInput, new object[] { button });
            return inputActionForGameAction != null && inputActionForGameAction.WasPressedThisFrame();
        }

        public Vector2 GetAxisRaw()
        {
            UnityEngine.InputSystem.InputAction inputLookHorizontal = (UnityEngine.InputSystem.InputAction)GetInputActionForGameAction.Invoke(_gameInput, new object[] { GameInput.GameActions.LookHorizontal });
            UnityEngine.InputSystem.InputAction inputLookVertical = (UnityEngine.InputSystem.InputAction)GetInputActionForGameAction.Invoke(_gameInput, new object[] { GameInput.GameActions.LookVertical });
            if (inputLookHorizontal == null || inputLookVertical == null) return Vector2.zero;

            return new Vector2(inputLookHorizontal.ReadValue<Vector2>().x, inputLookHorizontal.ReadValue<Vector2>().y);
        }
    }
}
