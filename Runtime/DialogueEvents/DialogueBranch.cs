using System;
using System.Collections.Generic;

namespace Glitch9.Game.Dialogue
{
    /// <summary>
    /// 사용자의 선택이나 이벤트의 결과에 따라 분기되는 대사들을 담는 클래스
    /// </summary>
    public class DialogueBranch
    {
        public string branchText;
        public string branchArg;
        public Action onBranchSelected;
        public List<DialogueData> dialogues;

        public DialogueBranch(string branchText, string arg)
        {
            this.branchText = branchText;
            branchArg = arg;
            dialogues = new List<DialogueData>();
        }

        public DialogueBranch(DialogueData dialog)
        {
            branchText = dialog.Line;
            branchArg = dialog.Arg;
            dialogues = new List<DialogueData>();
        }
    }
}
