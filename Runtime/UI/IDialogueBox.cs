using System;

namespace Glitch9.Game.Dialogue
{
    public interface IDialogueBox
    {
        string Id { get; }
        DialogueBoxType Type { get; }
        bool AllLettersShown { get; }
        bool AnyLetterVisible { get; }
        bool AutoMode { get; set; }
        bool IsVisible { get; }
        ICharacter CurrentCharacter { get; set; }


        void Show();
        void Hide();
        void ShowText(string text);
        void Complete(Action callback);

    }
}