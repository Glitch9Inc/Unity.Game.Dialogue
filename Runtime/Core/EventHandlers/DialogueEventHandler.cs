using System;

namespace Glitch9.Game.Dialogue
{
    public class DialogueEventHandler : IGameEventHandler
    {
        public EventHandler<Episode> OnEpisodeStart { get; set; }
        public EventHandler<Episode> OnDialogueEnd { get; set; }

        public EventHandler<Narration> ShowNarration { get; set; }
        public EventHandler<string> ShowInputFieldPopup { get; set; }
        public EventHandler<string> ShowCutscene { get; set; }
        public EventHandler<string> HideCutscene { get; set; }
        public EventHandler<string> HandleEvent { get; set; }
    }
}