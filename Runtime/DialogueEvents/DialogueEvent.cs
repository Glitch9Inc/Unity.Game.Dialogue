using System;
using System.Collections.Generic;

namespace Glitch9.Game.Dialogue
{
    public abstract class DialogueEvent
    {
        public Dictionary<int, DialogueBranch> Branches { get; private set; } = new();
        public DialogueBranch TimeOutBranch { get; private set; }
        public int EventIndex { get; private set; }
        public int StartIndex { get; private set; }
        public bool ImmediatelyFinishAfterEvent { get; set; } = false;

        protected DialogueEvent(int eventIndex, int startIndex)
        {
            EventIndex = eventIndex;
            StartIndex = startIndex;
            if (startIndex == 0) GNLog.Warning("이벤트 시작 인덱스가 0입니다.");
        }

        public void AddBranchDialogue(int branchIndex, DialogueData dialog)
        {
            if (branchIndex == -1) // 인덱스가 -1이면 사용자가 선택지를 고르지 못하고 제한시간이 초과되었을 때 나오는 대사
            {
                if (TimeOutBranch == null)
                {
                    TimeOutBranch = new DialogueBranch(dialog.Line, dialog.Arg);
                }
                else
                {
                    TimeOutBranch.dialogues.Add(dialog);
                }
                return;
            }

            if (Branches.ContainsKey(branchIndex))
            {
                Branches[branchIndex].dialogues.Add(dialog);
            }
            else
            {
                Branches.Add(branchIndex, new DialogueBranch(dialog));
            }
        }

        public void SortBranchDialogues()
        {
            foreach (var branch in Branches)
            {
                branch.Value.dialogues.Sort((a, b) => a.Index.CompareTo(b.Index));
            }
        }

        public void AddListener(int branchIndex, Action callback)
        {
            if (Branches.TryGetValue(branchIndex, out DialogueBranch branch))
            {
                branch.onBranchSelected = callback;
            }
            else
            {
                Branches.Add(branchIndex, new DialogueBranch("", "")
                {
                    onBranchSelected = callback
                });
            }
        }
        public void RunCallback(int branchIndex)
        {
            if (Branches.ContainsKey(branchIndex))
            {
                Branches[branchIndex].onBranchSelected?.Invoke();
            }
        }
    }
}