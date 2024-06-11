using System.Collections.Generic;

namespace Glitch9.Game.Dialogue
{
    public class DialogueUtils
    {
        public static void StartTempDialogue(ICharacter character)
        {
            Episode dialogue = CreateEmptyRewardDialogue(character.Id);
            EpisodeData chapterInfo = new()
            {
                Id = "0_0"
            };
            DialogueManager.Instance.StartEpisode(character, chapterInfo, dialogue);
        }

        /// <summary>
        /// TODO : 스크립트가 없을 때 보여줄 임시 대화
        /// </summary>
        public static Episode CreateEmptyRewardDialogue(string characterKey)
        {
            ICharacter character = null;//ICharacter.Get(characterKey);
            if (character.LogIfNull()) return null;

            Dictionary<int, DialogueData> entries = new();
            Dictionary<int, DialogueEvent> events = new();

            string charKey = character.Id;

            DialogueData dialogue2 = new(2, DialogueType.Line, 0, 0, null, charKey, "현재 대본 제작중입니다. \n업데이트까지 조금만 기다려주세요.");
            DialogueData dialogue3 = new(3, DialogueType.Line, 0, 0, null, charKey, "칵테일 이벤트를 진행하시면, 보상을 얻으실 수 있습니다.");
            DialogueData dialogue4 = new(4, DialogueType.Line, 0, 0, null, charKey, "칵테일 이벤트를 진행합니다.");
            DialogueData dialogue5 = new(5, DialogueType.Cocktail, 1, 0, null, charKey, "");

            DialogueData dialogue6 = new(6, DialogueType.Line, 1, 0, null, charKey, "");
            DialogueData dialogue7 = new(7, DialogueType.Line, 1, 2, null, charKey, "");
            DialogueData dialogue8 = new(8, DialogueType.Line, 1, 3, null, charKey, "");

            //CocktailEvent cocktailEvent = new(1, 5, "", "");
            //cocktailEvent.AddBranchDialogue((int)DialogEventResult.SuperSuccess, dialogue6);
            //cocktailEvent.AddBranchDialogues(DialogEventResult.Success, new DialogueBranch("", "TODO Arg"));
            //cocktailEvent.AddBranchDialogue((int)DialogEventResult.Success, dialogue7);
            //cocktailEvent.AddBranchDialogues(DialogEventResult.Fail, new DialogueBranch("", "TODO Arg"));
            //cocktailEvent.AddBranchDialogue((int)DialogEventResult.Fail, dialogue8);


            DialogueData dialogue9 = new(20, DialogueType.Line, 0, 0, null, charKey, "칵테일 이벤트를 종료합니다.");

            entries.Add(2, dialogue2);
            entries.Add(3, dialogue3);
            entries.Add(4, dialogue4);
            entries.Add(5, dialogue5);
            entries.Add(20, dialogue9);
            //events.Add(1, cocktailEvent);

            Episode dialogueBlock = new(new EpisodeId(0, 0), entries, events);
            return dialogueBlock;
        }
    }
}
