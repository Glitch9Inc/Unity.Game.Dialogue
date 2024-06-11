using System.Collections.Generic;
using Glitch9.Apis.OpenAI;
using Glitch9.Apis.OpenAI.Tools.EditorGPT;
using Glitch9.Internal;
using UnityEditor;
using UnityEngine;

namespace Glitch9.Company.AIDialogGenerator
{
    public class AIDialogGenerator : EditorGPTWindow<AIDialogGenerator>
    {

        [MenuItem(UnityMenu.AIDialogGenerator.PATH_AI_DIALOG_GENERATOR, priority = UnityMenu.AIDialogGenerator.PRIORITY_AI_DIALOG_GENERATOR)]
        public static void ShowWindow() => Initialize();

        private const string PREFSKEY_MIN_LINES = "EditorGPTBase.MinLines";
        protected int MinLines
        {
            get => EditorPrefs.GetInt(PREFSKEY_MIN_LINES, 3);
            set => EditorPrefs.SetInt(PREFSKEY_MIN_LINES, value);
        }

        private string characterInfo;
        private string chapterInfo;
        private string storyProgression;
        private string tableFields;
        private string notes;

        
        protected override void OnGUIMid()
        {
            GUILayout.BeginVertical();

            // character info
            GUILayout.Label("Character Info");
            characterInfo = GUILayout.TextArea(characterInfo, GUILayout.MaxWidth(2000), GUILayout.Height(120));
            if (GUILayout.Button("Clear")) characterInfo = "";

            // chapter info
            GUILayout.Label("Chapter Info");
            chapterInfo = GUILayout.TextArea(chapterInfo, GUILayout.MaxWidth(2000), GUILayout.Height(120));
            if (GUILayout.Button("Clear")) chapterInfo = "";

            // story progression
            GUILayout.Label("Story Progression");
            storyProgression = GUILayout.TextArea(storyProgression, GUILayout.MaxWidth(2000), GUILayout.Height(120));
            if (GUILayout.Button("Clear")) storyProgression = "";

            // table fields
            GUILayout.Label("Table Fields");
            tableFields = GUILayout.TextArea(tableFields, GUILayout.MaxWidth(2000), GUILayout.Height(120));
            if (GUILayout.Button("Clear")) tableFields = "";

            // notes
            GUILayout.Label("Notes");
            notes = GUILayout.TextArea(notes, GUILayout.MaxWidth(2000), GUILayout.Height(120));
            if (GUILayout.Button("Clear")) notes = "";

            // generate button
            if (GUILayout.Button("Generate"))
            {
                if (string.IsNullOrEmpty(characterInfo) || string.IsNullOrEmpty(chapterInfo) || string.IsNullOrEmpty(storyProgression) || string.IsNullOrEmpty(tableFields) || string.IsNullOrEmpty(notes))
                {
                    Debug.LogError("Text is null or empty");
                    return;
                }

                RequestWithCurrentPrompt();
            }

            GUILayout.EndVertical();
        }

        public override void RequestWithCurrentPrompt()
        {

        }


        protected List<Message> GetMessages()
        {
            List<Message> list = new();

            string cInfo = characterInfo.Replace('/', '#').Replace("\r\n", "\\n").Replace("\r", "\\n").Replace("\n", "\\n");
            string chInfo = chapterInfo.Replace('/', '#').Replace("\r\n", "\\n").Replace("\r", "\\n").Replace("\n", "\\n");
            string sProgression = storyProgression.Replace('/', '#').Replace("\r\n", "\\n").Replace("\r", "\\n").Replace("\n", "\\n");
            string tFields = tableFields.Replace('/', '#').Replace("\r\n", "\\n").Replace("\r", "\\n").Replace("\n", "\\n");
            string n = notes.Replace('/', '#').Replace("\r\n", "\\n").Replace("\r", "\\n").Replace("\n", "\\n");

            Message userMessage = new(ChatRole.User, "Can you help me with writing story dialogues?");
            Message assistantMessage = new(ChatRole.Assistant, "Of course! I'm a bot trained to generate Text based on your prompts. What kind of story are you working on?");
            Message promptMessage = new(ChatRole.User, "It's a cyberpunk-themed game story where the player runs a cocktail bar, engaging with NPC customers(Main Characters). Each main character has its own storylines.");
            Message promptMessage2 = new(ChatRole.Assistant, "Great! Tell me about the main character you want to write about.");
            Message promptMessage3 = new(ChatRole.User, cInfo);
            Message promptMessage4 = new(ChatRole.Assistant, "What is the chapter about?");
            Message promptMessage5 = new(ChatRole.User, chInfo);
            Message promptMessage6 = new(ChatRole.Assistant, "What is the story progression?");
            Message promptMessage7 = new(ChatRole.User, sProgression);
            Message promptMessage8 = new(ChatRole.Assistant, "What are the table fields?");
            Message promptMessage9 = new(ChatRole.User, tFields);
            Message promptMessage10 = new(ChatRole.Assistant, "Is there anything important you want to note?");
            Message promptMessage11 = new(ChatRole.User, n);
            Message promptMessage12 = new(ChatRole.Assistant, "How many dialogues does this chapter have?");
            Message promptMessage13 = new(ChatRole.User, "At least " + MinLines + " dialogues.");

            list.Add(userMessage);
            list.Add(assistantMessage);
            list.Add(promptMessage);
            list.Add(promptMessage2);
            list.Add(promptMessage3);
            list.Add(promptMessage4);
            list.Add(promptMessage5);
            list.Add(promptMessage6);
            list.Add(promptMessage7);
            list.Add(promptMessage8);
            list.Add(promptMessage9);
            list.Add(promptMessage10);
            list.Add(promptMessage11);
            list.Add(promptMessage12);
            list.Add(promptMessage13);

            return list;
        }


    }
}