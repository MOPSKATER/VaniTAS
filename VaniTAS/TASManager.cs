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
        }

        public static List<InputAction> GetTAS(string levelID)
        {
            if (levelID == _levelID) return _currentTAS;

            return null; // TODO load TAS
        }
    }
}
