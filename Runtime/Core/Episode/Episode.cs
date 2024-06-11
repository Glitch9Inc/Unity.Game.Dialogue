using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Glitch9.Game.Dialogue
{
    public class Episode
    {
        private const int MAX_LINE_LETTER_COUNT = 120;
        public static Episode Current => DialogueManager.Instance.CurrentEpisode;
        public static void Start(ICharacter character, EpisodeData chapterInfo) => DialogueManager.Instance.StartEpisodeAsync(character, chapterInfo);

        public EpisodeId Id { get; private set; }
        public ICharacter Character { get; private set; }
        public Dictionary<int, DialogueData> Dialogues { get; private set; }
        public Dictionary<int, DialogueEvent> Events { get; private set; }

        /// <summary>
        /// 구버전의 Loader를 사용할 때 사용
        /// </summary>
        public Episode(EpisodeId id, IDictionary<int, DialogueData> dialogs, IDictionary<int, DialogueEvent> events)
        {
            Id = id;
            Dialogues = new Dictionary<int, DialogueData>(dialogs);
            Events = new Dictionary<int, DialogueEvent>(events);
        }

        /// <summary>
        /// LoaderV2를 사용할 때 사용
        /// </summary>
        public Episode(EpisodeId id, Dictionary<int, DialogueData> dialogs)
        {
            Id = id;
            ParseRowEntries(dialogs);
        }

        private void ParseRowEntries(Dictionary<int, DialogueData> entries)
        {
            Dialogues = new Dictionary<int, DialogueData>();
            Events = new Dictionary<int, DialogueEvent>();

            foreach (KeyValuePair<int, DialogueData> entry in entries)
            {
                if (entry.Value.IsEvent)
                {
                    DialogueType type = entry.Value.Type;
                    int eventIndex = entry.Value.EventIndex;

                    if (!Events.ContainsKey(eventIndex))
                    {
                        if (type == DialogueType.Event)
                        {
                            ChoiceEvent newEvent = new(eventIndex, entry.Key, entry.Value.Arg, entry.Value.Line);
                            Events.Add(eventIndex, newEvent);
                            Dialogues.Add(entry.Key, entry.Value);
                        }
                        else if (type == DialogueType.Cocktail)
                        {
                            //CocktailEvent newEvent = new(eventIndex, entry.Key, entry.Value.Arg, entry.Value.Line);
                            //Events.Add(eventIndex, newEvent);
                            //Dialogs.Add(entry.Key, entry.Value);
                        }
                    }

                    Events[eventIndex].AddBranchDialogue(entry.Key, entry.Value);
                }
                else
                {
                    Dialogues.Add(entry.Key, entry.Value);
                }

                // 이벤트가 아닌 경우에도 Entries에 추가 ? ( 2024.02.19 )
                // 추가 안하면 NextDialogue() 호출시 다음 순서의 Dialogue Index 가 없으면 바로 종료됨
                // 예) 1, 2, 3, 4, 5, ( 다음이 6이 아니여서 종료 ) 9, 10, 11
                // Entries.Add(entry.Key, entry.Value);
            }
        }

        public Episode() { }

        #region Factory Methods

        public static Episode Create(List<DialogueData> list)
        {
            Episode episode = new()
            {
                Dialogues = new Dictionary<int, DialogueData>(),
                Events = new Dictionary<int, DialogueEvent>(),
            };

            for (int i = 0; i < list.Count; i++)
            {
                episode.Dialogues.Add(i, list[i]);
            }

            return episode;
        }

        public static Episode Create(string characterId, VoiceFile voice)
        {
            Episode episode = new()
            {
                Dialogues = new Dictionary<int, DialogueData>(),
                Events = new Dictionary<int, DialogueEvent>(),
            };

            episode.Dialogues.Add(0, voice.ToDialogue(characterId));
            return episode;
        }

        public static Episode CreateQuestion(string characterId, string question, string dialogue, params Choice[] choices)
        {
            Episode episode = new()
            {
                Id = EpisodeId.Question,
                Dialogues = new Dictionary<int, DialogueData>(),
                Events = new Dictionary<int, DialogueEvent>(),
            };

            // question string이 너무 길면 잘라서 dialogue로 만들어줌
            // ., ?, ! 등의 구두점을 기준으로 자름
            // 자른 문장의 길이가 MaxLineLetterCount를 넘어가면 자름
            // .... 같은건 자르면 안됨 (구두점이 연달아 나오는 경우)
            string[] sentences = Regex.Split(dialogue, @"(?<=[.!?])\s+");

            foreach (string sentence in sentences)
            {
                int start = 0;
                while (start < sentence.Length)
                {
                    int count = Math.Min(MAX_LINE_LETTER_COUNT, sentence.Length - start);
                    string subString = sentence.Substring(start, count);

                    // 연속된 구두점 처리
                    if (start + count < sentence.Length && !char.IsPunctuation(sentence[start + count]))
                    {
                        int nextPunchIndex = -1;
                        for (int i = start + count; i < sentence.Length; i++)
                        {
                            if (char.IsPunctuation(sentence[i]))
                            {
                                if (nextPunchIndex == -1 || i == nextPunchIndex + 1)
                                {
                                    nextPunchIndex = i;
                                }
                                else break;
                            }
                        }

                        if (nextPunchIndex != -1 && nextPunchIndex - start <= MAX_LINE_LETTER_COUNT)
                        {
                            count = nextPunchIndex - start + 1;
                            subString = sentence.Substring(start, count);
                        }
                    }

                    // subString을 이용하여 Episode 객체 생성 및 추가
                    // 예시: episodes.Add(new Episode { ... });
                    int index = episode.Dialogues.Count;
                    DialogueData entry = DialogueData.Create(index, DialogueType.Line, 0, characterId, subString);
                    episode.Dialogues.Add(index, entry);
                    start += count;
                }
            }

            // 선택지 추가 (choice event)
            int startIndex = episode.Dialogues.Count;
            ChoiceEvent choiceEvent = new(0, startIndex, "", question);

            for (int i = 0; i < choices.Length; i++)
            {
                int index = episode.Dialogues.Count;
                int choiceIndex = i;
                DialogueData entry = DialogueData.Create(index, DialogueType.Event, 0, characterId, choices[i].content);

                if (i == 0) episode.Dialogues.Add(index, entry); // ChoiceEvent 트리거로 Entries에도 하나 추가해준다.         

                choiceEvent.AddBranchDialogue(choiceIndex, entry);
                choiceEvent.AddListener(choiceIndex, new Action(choices[choiceIndex].onClick));
                GNLog.Error($"BranchResult {i} added");
            }

            choiceEvent.ImmediatelyFinishAfterEvent = true;
            episode.Events.Add(0, choiceEvent);
            return episode;
        }

        #endregion
    }
}