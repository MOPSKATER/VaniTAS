using UnityEngine;

namespace VaniTAS
{
    internal class FrameCounter : MonoBehaviour
    {
        public static int CurrentFrameCount { get; private set; } = 0;

        void OnGUI()
        {
            GUI.Label(new Rect(10, 40, 200, 30), "Frame: " + CurrentFrameCount, Main.Style);
        }

        void FixedUpdate()
        {
            if (CurrentFrameCount != int.MaxValue)
                CurrentFrameCount++;
        }

        public static void ResetCounter()
        {
            CurrentFrameCount = 0;
        }
    }
}
