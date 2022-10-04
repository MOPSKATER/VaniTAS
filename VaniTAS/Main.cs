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
            GameDataManager.powerPrefs.dontUploadToLeaderboard = true;
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
                    controller.AddComponent<TimeController>().Initialize(TimeController.Speed.Frozen);
                    controller.AddComponent<CharakterInfo>();
                    controller.AddComponent<Editor>();
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

        #region Patching stuff

        private void PatchGame()
        {
            HarmonyLib.Harmony harmony = new("de.MOPSKATER.TAS");

            MethodInfo target = typeof(LevelStats).GetMethod("UpdateTimeMicroseconds");
            HarmonyMethod patch = new(typeof(Main).GetMethod("PreventNewScore"));
            harmony.Patch(target, patch);

            target = typeof(Game).GetMethod("OnLevelWin");
            patch = new(typeof(Main).GetMethod("PreventNewGhost"));
            harmony.Patch(target, patch);

            target = typeof(LevelRush).GetMethod("IsCurrentLevelRushScoreBetter", BindingFlags.NonPublic | BindingFlags.Static);
            patch = new(typeof(Main).GetMethod("PreventNewBestLevelRush"));
            harmony.Patch(target, patch);

            target = typeof(Game).GetMethod("OnLevelWin");
            patch = new(typeof(Recorder).GetMethod("PreOnLevelWin"));
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

        public static bool PreventNewScore(LevelStats __instance, ref long newTime)
        {
            if (newTime < __instance._timeBestMicroseconds)
            {
                if (__instance._timeBestMicroseconds == 999999999999L)
                    __instance._timeBestMicroseconds = 600000000;
                __instance._newBest = true;
            }
            else
                __instance._newBest = false;
            __instance._timeLastMicroseconds = newTime;
            return false;
        }

        public static bool PreventNewGhost(Game __instance)
        {
            __instance.winAction = null;
            return true;
        }

        public static bool PreventNewBestLevelRush(ref bool __result)
        {
            __result = false;
            return false;
        }

        #endregion
    }
}