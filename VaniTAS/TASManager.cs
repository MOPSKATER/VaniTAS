using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace VaniTAS
{
    internal static class TASManager
    {
        private static string _levelID;
        private static List<InputAction> _currentTAS;

        public static void SetTAS(string levelID, List<InputAction> actions)
        {
            _levelID = levelID;
            _currentTAS = actions;
            SaveToFile();
        }

        public static List<InputAction> GetTAS(string levelID)
        {
            if (levelID == _levelID) return _currentTAS;
            LoadFromFile(levelID);
            return _currentTAS;
        }

        private static void SaveToFile()
        {
            string path = "";
            if (!GhostUtils.GetPath(Main.Game.GetCurrentLevel().levelID, GhostUtils.GhostType.PersonalGhost, ref path)) return;

            path += "\\Vani.TAS";

            Stream stream = File.Open(path, FileMode.Create);
            BinaryFormatter formatter = new();
            formatter.Serialize(stream, _currentTAS);
            stream.Close();
        }

        private static void LoadFromFile(string levelID)
        {
            _currentTAS = null;
            string path = "";
            if (!GhostUtils.GetPath(levelID, GhostUtils.GhostType.PersonalGhost, ref path)) return;

            path += "\\Vani.TAS";

            try
            {
                Stream stream = File.Open(path, FileMode.Open);
                BinaryFormatter formatter = new();
                _currentTAS = (List<InputAction>)formatter.Deserialize(stream);
                stream.Close();
            }
            catch (FileNotFoundException fileNotFound)
            {
                Debug.Log(fileNotFound.Message);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Debug.LogError("Following TAS file might be broken: " + path);
            }
        }
    }
}
