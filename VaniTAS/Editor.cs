﻿using System.Reflection;
using UnityEngine;

namespace TAS
{
    internal class Editor : Player
    {
        private readonly GameInput _gameInput = Singleton<GameInput>.Instance;
        private readonly MethodInfo GetInputActionForGameAction = typeof(GameInput).GetMethod("GetInputActionForGameAction", BindingFlags.NonPublic | BindingFlags.Instance);
        public static bool Continue { get; set; } = false;

        void Awake()
        {
            base.Awake();
        }

        void FixedUpdate()
        {
            _currentAction = _inputs[FrameCounter.CurrentFrameCount - 1];
            if (!Trigger() || !Continue) return;
            Xinfo.SetValue(RM.drifter.mouseLookX, _currentAction.Pitch);
            Yinfo.SetValue(RM.drifter.mouseLookY, _currentAction.Yaw);
        }

        void Update()
        {
            if (GetButtonDown(GameInput.GameActions.Jump, GameInput.InputType.Game))
            {
                _currentAction.Jump = !_currentAction.Jump;
            }
            if (GetButtonDown(GameInput.GameActions.FireCard, GameInput.InputType.Game))
            {
                _currentAction.Fire = !_currentAction.Fire;
            }
            if (GetButtonDown(GameInput.GameActions.FireCardAlt, GameInput.InputType.Game))
            {
                _currentAction.Ability = !_currentAction.Ability;
            }
            if (GetButtonDown(GameInput.GameActions.SwapCard, GameInput.InputType.Game))
            {
                _currentAction.Swap = !_currentAction.Swap;
            }
        }

        void OnGUI()
        {
            GUI.Label(new Rect(10, 160, 150, 150), _currentAction.ToString(), Main.Style);
        }

        private bool GetButtonDown(GameInput.GameActions button, GameInput.InputType inputType)
        {
            UnityEngine.InputSystem.InputAction inputActionForGameAction = (UnityEngine.InputSystem.InputAction) GetInputActionForGameAction.Invoke(_gameInput, new object[] { button });
            return inputActionForGameAction != null && inputActionForGameAction.WasPressedThisFrame();
        }
    }
}