using UnityEngine;
using UnityEngine.InputSystem;

namespace TAS
{
    internal class TimeController : MonoBehaviour
    {
        private int _currentScaleIndex = 0;
        private readonly float[] _speedValues = new float[] { 0f, 0.2f, 0.5f, 1f };
        private bool _frameAdvance = false;

        private int _targetFrame = -1;

        public void Initialize(Speed speedMode) => Initialize((int)speedMode);
        public void Initialize(int initialSpeedIndex)
        {
            _currentScaleIndex = initialSpeedIndex;
            RM.time.SetTargetTimescale(_speedValues[initialSpeedIndex]);
        }

        void Update()
        {
            if (_targetFrame == FrameCounter.CurrentFrameCount - 1)
            {
                RM.time.SetTargetTimescale(0f);
                _currentScaleIndex = 0;
                return;
            }

            if (Keyboard.current.upArrowKey.wasPressedThisFrame && _currentScaleIndex < 3)
                RM.time.SetTargetTimescale(_speedValues[++_currentScaleIndex]);

            else if (Keyboard.current.downArrowKey.wasPressedThisFrame && _currentScaleIndex > 0)
                RM.time.SetTargetTimescale(_speedValues[--_currentScaleIndex]);

            else if (Keyboard.current.numpad0Key.wasPressedThisFrame)
            {
                _currentScaleIndex = (int)Speed.Frozen;
                RM.time.SetTargetTimescale(_speedValues[_currentScaleIndex]);
            }

            else if (Keyboard.current.numpad1Key.wasPressedThisFrame)
            {
                _currentScaleIndex = (int)Speed.SlowMo;
                RM.time.SetTargetTimescale(_speedValues[_currentScaleIndex]);
            }

            else if (Keyboard.current.numpad2Key.wasPressedThisFrame)
            {
                _currentScaleIndex = (int)Speed.Half;
                RM.time.SetTargetTimescale(_speedValues[_currentScaleIndex]);
            }

            else if (Keyboard.current.numpad3Key.wasPressedThisFrame)
            {
                _currentScaleIndex = (int)Speed.Normal;
                RM.time.SetTargetTimescale(_speedValues[_currentScaleIndex]);
            }

            else if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
            {
                _frameAdvance = true;
                RM.time.SetTargetTimescale(_speedValues[1]);
            }
        }

        void FixedUpdate()
        {
            if (_frameAdvance)
            {
                _frameAdvance = false;
                RM.time.SetTargetTimescale(0);
            }
        }

        public void GoToFrame(int frame)
        {
            if (frame < 1 || _targetFrame <= FrameCounter.CurrentFrameCount)
                _targetFrame = frame;
        }

        public enum Speed
        {
            Frozen = 0,
            SlowMo = 1,
            Half = 2,
            Normal = 3
        }
    }
}
