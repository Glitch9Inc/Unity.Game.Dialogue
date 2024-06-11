
using Glitch9.Apis.Google.Sheets;

namespace Glitch9.Game.Dialogue
{
    [Sheets(typeof(UnlockCondition))]
    public enum UnlockCondition
    {
        None = 0,
        StoryCompletion,
        KeyItem
    }
}