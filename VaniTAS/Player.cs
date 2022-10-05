using System.Reflection;
using UnityEngine;

namespace VaniTAS
{
    internal class Player : MonoBehaviour
    {
        protected List<InputAction> _inputs;
        protected static InputAction _currentAction;
        public static Player CurrentPlayer { get; private set; }

        protected readonly FieldInfo Xinfo = typeof(MouseLook).GetField("rotationX", BindingFlags.NonPublic | BindingFlags.Instance);
        protected readonly FieldInfo Yinfo = typeof(MouseLook).GetField("rotationY", BindingFlags.NonPublic | BindingFlags.Instance);


        protected void Awake()
        {
            CurrentPlayer = this;
            _inputs = TASManager.GetTAS(Main.Game.GetCurrentLevel().levelID);
        }

        void FixedUpdate()
        {
            if (!Trigger()) return;
            _currentAction = _inputs[FrameCounter.CurrentFrameCount - 1];
            Xinfo.SetValue(RM.drifter.mouseLookX, _currentAction.Pitch);
            Yinfo.SetValue(RM.drifter.mouseLookY, _currentAction.Yaw);
        }

        public static bool PreGetAxis(ref float __result, GameInput.GameActions axis, GameInput.InputType inputType = GameInput.InputType.Game)
        {
            if (!Trigger(inputType)) return true;


            if (Main.CurrentMode == Main.Mode.Edit)
            {
                if (!Editor.Continue) return false;
            }

            switch (axis)
            {
                case GameInput.GameActions.MoveHorizontal:
                    {
                        __result = _currentAction.Horizontal;
                        return false;
                    }
                case GameInput.GameActions.MoveVertical:
                    {
                        __result = _currentAction.Vertical;
                        return false;
                    }
                case GameInput.GameActions.SwapCard:
                    {
                        __result = _currentAction.Swap ? 1f : 0f;
                        return false;
                    }
                default: return true;
            }
        }

        public static bool PreGetAxisRaw(ref float __result, GameInput.GameActions axis, GameInput.InputType inputType = GameInput.InputType.Game)
        {
            if (!Trigger(inputType)) return true;


            if (Main.CurrentMode == Main.Mode.Edit)
            {
                if (!Editor.Continue) return false;
            }

            switch (axis)
            {
                case GameInput.GameActions.MoveHorizontal:
                    {
                        __result = _currentAction.Horizontal;
                        return false;
                    }
                case GameInput.GameActions.MoveVertical:
                    {
                        __result = _currentAction.Vertical;
                        return false;
                    }
                default: return true;
            }
        }

        public static bool PreGetButton(ref bool __result, GameInput.GameActions button, GameInput.InputType inputType = GameInput.InputType.Game)
        {
            if (!Trigger(inputType) || button == GameInput.GameActions.Restart) return true;


            if (Main.CurrentMode == Main.Mode.Edit)
            {
                if (!Editor.Continue) return false;
            }

            switch (button)
            {
                case GameInput.GameActions.FireCard:
                    {
                        __result = _currentAction.Fire;
                        return false;
                    }
                case GameInput.GameActions.FireCardAlt:
                    {
                        __result = _currentAction.Ability;
                        return false;
                    }
                case GameInput.GameActions.Jump:
                    {
                        __result = _currentAction.Jump;
                        return false;
                    }
                default: return true;
            }
        }

        public static bool PreGetButtonDown(ref bool __result, GameInput.GameActions button, GameInput.InputType inputType = GameInput.InputType.Game)
        {
            if (!Trigger(inputType) || button == GameInput.GameActions.Restart) return true;


            if (Main.CurrentMode == Main.Mode.Edit)
            {
                if (FrameCounter.CurrentFrameCount < 2) return false;
                if (!Editor.Continue) return false;
            }
            else if (FrameCounter.CurrentFrameCount < 2) return true;

            switch (button)
            {
                case GameInput.GameActions.FireCard:
                    {
                        __result = _currentAction.Fire && !CurrentPlayer._inputs[FrameCounter.CurrentFrameCount - 2].Fire;
                        return false;
                    }
                case GameInput.GameActions.FireCardAlt:
                    {
                        __result = _currentAction.Ability && !CurrentPlayer._inputs[FrameCounter.CurrentFrameCount - 2].Ability;
                        return false;
                    }
                case GameInput.GameActions.Jump:
                    {
                        __result = _currentAction.Jump && !CurrentPlayer._inputs[FrameCounter.CurrentFrameCount - 2].Jump;
                        return false;
                    }
                default: return true;
            }
        }

        public static bool PreGetButtonUp(ref bool __result, GameInput.GameActions button, GameInput.InputType inputType = GameInput.InputType.Game)
        {
            if (!Trigger(inputType) || button == GameInput.GameActions.Restart) return true;


            if (Main.CurrentMode == Main.Mode.Edit)
            {
                if (FrameCounter.CurrentFrameCount < 2) return false;
                if (!Editor.Continue) return false;
            }
            else if (FrameCounter.CurrentFrameCount < 2) return true;

            switch (button)
            {
                case GameInput.GameActions.FireCard:
                    {
                        __result = !_currentAction.Fire && CurrentPlayer._inputs[FrameCounter.CurrentFrameCount - 2].Fire;
                        return false;
                    }
                case GameInput.GameActions.FireCardAlt:
                    {
                        __result = !_currentAction.Ability && CurrentPlayer._inputs[FrameCounter.CurrentFrameCount - 2].Ability;
                        return false;
                    }
                case GameInput.GameActions.Jump:
                    {
                        __result = !_currentAction.Jump && CurrentPlayer._inputs[FrameCounter.CurrentFrameCount - 2].Jump;
                        return false;
                    }
                case GameInput.GameActions.Restart:
                    {
                        __result = false; // !frame_restart_pressed && last_frame_restart_pressed;
                        return false;
                    }
                default: return true;
            }
        }
        protected static bool Trigger() =>
            CurrentPlayer != null &&
                (Main.CurrentMode == Main.Mode.Play || (Main.CurrentMode == Main.Mode.Edit && Editor.Continue)) &&
                FrameCounter.CurrentFrameCount < CurrentPlayer._inputs.Count;

        private static bool Trigger(GameInput.InputType inputType)
        {
            return Trigger() && inputType == GameInput.InputType.Game;
        }
    }
}
