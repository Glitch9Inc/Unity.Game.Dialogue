namespace Glitch9.Game.Dialogue
{
    public class DialogueLogger : ILogger
    {
        private const string TAG = "DialogueSystem";
        
        public void Info(string message)
        {
            GNLog.Info(TAG, message);
        }

        public void Warning(string message)
        {
            GNLog.Warning(TAG, message);
        }

        public void Error(string message)
        {
            GNLog.Error(TAG, message);
        }

        public void Info(string tag, string message)
        {
            GNLog.Info(tag, message);
        }

        public void Warning(string tag, string message)
        {
            GNLog.Warning(tag, message);
        }

        public void Error(string tag, string message)
        {
            GNLog.Error(tag, message);
        }
    }
}