namespace Glitch9.Game.Dialogue
{
    /// <summary>
    /// 평범하게도 사용하지만, 
    /// 네이티브 플러그인과 통신하기 위한 Model 클래스로도 사용된다.
    /// Model은 Json으로 변환되어 네이티브 플러그인으로 전달된다.
    /// </summary>
    public class VoiceFile
    {
        // 보통 클래스와 차이점을 주기위해 필드명을 소문자로 시작한다.
        // Model 구조가 Java플러그인 내의 GNTaskNotification.java과 동일해야 한다.
        public string filePath;
        public string dialogue;
        public int alarmType;
    }


    public static class VoiceFileExtensions
    {
        public static DialogueData ToDialogue(this VoiceFile voiceFile, string charKey) => DialogueData.CreateLine(0, charKey, voiceFile.dialogue, voiceFile.filePath);
    }
}