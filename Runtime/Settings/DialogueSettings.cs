using UnityEngine;

namespace Glitch9.Game.Dialogue
{
    [CreateAssetMenu(fileName = "DialogueSettings", menuName = "Glitch9/Routina/Dialogue Settings", order = 0)]
    public class DialogueSettings : ScriptableResource<DialogueSettings>
    {
        [SerializeField] private float defaultTextSpeed = 0.06f;
        [SerializeField] private float doubleTextSpeed = 0.02f;
        [SerializeField] private float nextDialogueDelay = .2f; // 다음 대사로 넘어가기까지의 딜레이
        [SerializeField] private float sfxDelay = 1f;
        [SerializeField] private float sfxWait = 2f;

        [SerializeField] private Color userBoxColor;
        [SerializeField] private Color characterBoxColor;
        [SerializeField] private Color assistantBoxColor;
        [SerializeField] private Color userTextColor;
        [SerializeField] private Color characterTextColor;
        [SerializeField] private Color assistantTextColor;



        // Dialogue Settings
        public static float DefaultTextSpeed => Instance.defaultTextSpeed;
        public static float DoubleTextSpeed => Instance.doubleTextSpeed;
        public static float NextDialogueDelay => Instance.nextDialogueDelay;
        public static float SfxDelay => Instance.sfxDelay;
        public static float SfxWait => Instance.sfxWait;


        // Box Colors
        public static Color UserBoxColor => Instance.userBoxColor;
        public static Color CharacterBoxColor => Instance.characterBoxColor;
        public static Color AssistantBoxColor => Instance.assistantBoxColor;
        public static Color UserTextColor => Instance.userTextColor;
        public static Color CharacterTextColor => Instance.characterTextColor;
        public static Color AssistantTextColor => Instance.assistantTextColor;

    }
}
