namespace Glitch9.Game.Dialogue
{
    public class DialogueData : IListEntry
    {
        public int Index { get; set; }
        public DialogueType Type { get; private set; }
        public int EventIndex { get; private set; }
        public string Line { get; private set; }
        public string VoicePath { get; private set; }
        public int ChoiceIndex { get; private set; }

        public int ItemId => EventIndex;
        public int ItemQuantity => ChoiceIndex;
        public bool IsEvent => Type == DialogueType.Event || Type == DialogueType.Cocktail;

        public string Arg { get; private set; } // 캐릭터 Id or 사운드 이름 or BGM 이름
        public string CharacterId => Arg;

        // Sounds
        public string SFXId => Arg;
        public string BGMId => Arg;

        // Questionnaire
        public string QuestionId => Arg;
        public int AnswerIndex => ChoiceIndex;

        /// <summary>
        /// 에디터에서 인덱스를 바꿀때 사용
        /// </summary>
        public void ChangeIndex(int index) => Index = index;
        public void ChangeLine(string line) => Line = line;

        public DialogueData() { }

        /// <summary>
        /// 로더V2에서 사용하는 생성자
        /// </summary>
        public DialogueData(int index, DialogueType type, int eventId, int choiceId, string voicePath, string arg, string line)
        {
            Index = index;
            Type = type;
            EventIndex = eventId;
            ChoiceIndex = choiceId;
            Arg = arg;
            Line = line;
            VoicePath = voicePath;
        }

        public string GetVoiceFilePath(bool isAssistDialogue = false)
        {
            if (isAssistDialogue)
            {
                if (string.IsNullOrWhiteSpace(Arg)) return null;
                return $"Assist/{Arg}.mp3";
            }

            return VoicePath;
        }

        public static DialogueData Create(int index, DialogueType type, int eventIndex, string characterId, string line)
        {
            var entry = new DialogueData
            {
                Index = index,
                Type = type,
                EventIndex = eventIndex,
                Arg = characterId,
                Line = line
            };
            return entry;
        }

        public static DialogueData CreateLine(int index, string characterId, string line, string voicePath = null)
        {
            var entry = new DialogueData
            {
                Index = index,
                Arg = characterId,
                Line = line,
                VoicePath = voicePath,
                Type = DialogueType.Line
            };
            return entry;
        }
    }
}

