using HarmonyLib;
using MelonLoader;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace VaniTAS
{
    public class Main : MelonMod
    {

        public static Game Game { get; private set; }

        private static Mode NextMode = Mode.Disabled;
        public static Mode CurrentMode = Mode.Disabled;
        internal static FrameCounter FrameCounter { get; private set; }

        public static readonly GUIStyle Style = new()
        {
            fontSize = 20,
            fontStyle = FontStyle.Bold
        };

        public override void OnApplicationLateStart()
        {
            AntiCheat.Anticheat.TriggerAnticheat();
            PatchGame();
            Game game = Singleton<Game>.Instance;

            if (game == null)
                return;

            Game = game;
            Game.OnLevelLoadComplete += OnLevelLoadComplete;

            if (RM.drifter)
                OnLevelLoadComplete();

            Style.normal.textColor = new(0f, 30f, 100f);
        }

        private void OnLevelLoadComplete()
        {
            CurrentMode = NextMode;
            FrameCounter.ResetCounter();
            if (SceneManager.GetActiveScene().name.Equals("Heaven_Environment")) return;


            if ((CurrentMode == Mode.Edit || CurrentMode == Mode.Play) && TASManager.GetTAS(Game.GetCurrentLevel().levelID) == null)
            {
                NextMode = Mode.Disabled;
                CurrentMode = Mode.Disabled;
                return;
            }

            GameObject controller = new("TAS Manager");
            FrameCounter = controller.AddComponent<FrameCounter>();

            switch (CurrentMode)
            {
                case Mode.Record:
                    controller.AddComponent<TimeController>().Initialize(TimeController.Speed.SlowMo);
                    controller.AddComponent<CharakterInfo>();
                    controller.AddComponent<Recorder>();
                    break;
                case Mode.Edit:
                    controller.AddComponent<Editor>();
                    controller.AddComponent<TimeController>().Initialize(TimeController.Speed.Frozen);
                    controller.AddComponent<CharakterInfo>();
                    break;
                case Mode.Play:
                    controller.AddComponent<Player>();
                    break;
            }
        }

        public override void OnUpdate()
        {
            if (Keyboard.current.mKey.wasPressedThisFrame)
            {
                NextMode = (Mode)((((int)NextMode) + 1) % 4);
                if (SceneManager.GetActiveScene().name.Equals("Heaven_Environment"))
                    CurrentMode = NextMode;
            }
        }

        public override void OnGUI()
        {
            GUI.Label(new Rect(10, 10, 200, 30), "Mode: " + NextMode.ToString(), Style);
        }

        public enum Mode
        {
            Disabled,
            Record,
            Edit,
            Play
        }

        private void PatchGame()
        {
            HarmonyLib.Harmony harmony = new("de.MOPSKATER.TAS");

            MethodInfo target = typeof(Game).GetMethod("OnLevelWin");
            HarmonyMethod patch = new(typeof(Recorder).GetMethod("PreOnLevelWin"));
            harmony.Patch(target, patch);

            target = typeof(GameInput).GetMethod("GetAxis");
            patch = new(typeof(Player).GetMethod("PreGetAxis"));
            harmony.Patch(target, patch);

            target = typeof(GameInput).GetMethod("GetAxisRaw");
            patch = new(typeof(Player).GetMethod("PreGetAxisRaw"));
            harmony.Patch(target, patch);

            target = typeof(GameInput).GetMethod("GetButton");
            patch = new(typeof(Player).GetMethod("PreGetButton"));
            harmony.Patch(target, patch);

            target = typeof(GameInput).GetMethod("GetButtonDown");
            patch = new(typeof(Player).GetMethod("PreGetButtonDown"));
            harmony.Patch(target, patch);

            target = typeof(GameInput).GetMethod("GetButtonUp");
            patch = new(typeof(Player).GetMethod("PreGetButtonUp"));
            harmony.Patch(target, patch);
        }
    }
}