using UnityEngine;

namespace TAS
{
    internal class Recorder : MonoBehaviour
    {
        public static Recorder CurrentRecorder;
        private List<InputAction> _actionList = new(2000000);
        private readonly GameInput _gameInput = Singleton<GameInput>.Instance;

        private static bool _listen = true;

        void Awake() => CurrentRecorder = this;

        void FixedUpdate()
        {
            if (_listen) Record(); new Task(Record).Start();
        }

        private void Record()
        {
            float inputX = Singleton<GameInput>.Instance.GetAxis(GameInput.GameActions.MoveHorizontal, GameInput.InputType.Game);
            float inputY = Singleton<GameInput>.Instance.GetAxis(GameInput.GameActions.MoveVertical, GameInput.InputType.Game);

            _actionList.Add(new()
            {
                Vertical = inputY,
                Horizontal = inputX,
                Pitch = RM.drifter.mouseLookX.RotationX,
                Yaw = RM.drifter.mouseLookY.RotationY,
                Jump = _gameInput.GetButton(GameInput.GameActions.Jump, GameInput.InputType.Game),
                Fire = _gameInput.GetButton(GameInput.GameActions.FireCard, GameInput.InputType.Game),
                Ability = _gameInput.GetButton(GameInput.GameActions.FireCardAlt, GameInput.InputType.Game),
                Swap = _gameInput.GetButton(GameInput.GameActions.SwapCard, GameInput.InputType.Game)
            });
        }

        public static void PreOnLevelWin()
        {
            if (Main.CurrentMode == Main.Mode.Record)
            {
                _listen = false;
                TASManager.SetTAS(Main.Game.GetCurrentLevel().levelID, CurrentRecorder._actionList);
            }
        }
    }
}
