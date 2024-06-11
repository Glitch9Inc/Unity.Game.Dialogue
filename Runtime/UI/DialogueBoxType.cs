using System;

namespace Glitch9.Game.Dialogue
{
    [Flags]
    public enum DialogueBoxType
    {
        Default = 1 << 0,
        Assist = 1 << 1,
    }
}