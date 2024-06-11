namespace Glitch9.Game.Dialogue
{
    public class ChoiceEvent : DialogueEvent
    {
        private const string TIMED_KEY = "timed";
        public string Question { get; private set; }
        public int TimeLimit = 0;

        public ChoiceEvent(int eventIndex, int startIndex, string rawArg, string line) : base(eventIndex, startIndex)
        {
            HandleTimedEvent(rawArg, line);
        }

        private void HandleTimedEvent(string rawArg, string line)
        {
            bool isTimed = rawArg.ToLower().Contains(TIMED_KEY);
            if (isTimed)
            {
                /* extract number from _arg */
                string[] args = rawArg.Split('=');
                foreach (string arg in args)
                {
                    if (int.TryParse(arg, out TimeLimit)) break;
                }
                Question = line;
            }
        }
    }
}