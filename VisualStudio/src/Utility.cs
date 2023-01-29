namespace PastimeReading
{
    public class Utility
    {
        public static bool IsScenePlayable()
        {
            return !(string.IsNullOrEmpty(GameManager.m_ActiveScene) || GameManager.m_ActiveScene.Contains("MainMenu") || GameManager.m_ActiveScene == "Boot" || GameManager.m_ActiveScene == "Empty");
        }

        public static bool IsScenePlayable(string scene)
        {
            return !(string.IsNullOrEmpty(scene) || scene.Contains("MainMenu") || scene == "Boot" || scene == "Empty");
        }

        public static void Log(ConsoleColor color, string message)
        {
            if (Settings.options.debugLog)
            {
                MelonLogger.Msg(color, message);
            }
        }

    }
}
