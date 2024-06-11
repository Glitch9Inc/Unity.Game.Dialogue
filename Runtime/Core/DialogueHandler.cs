using Cysharp.Threading.Tasks;
using Glitch9.Routina.Tutorial;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glitch9.Game.Dialogue
{
    public class DialogueHandler
    {
        private const string EVENT_NAME_NARRATION = "NarrationEvent";
        private const string EVENT_NAME_EVENT = "DialogueEvent";
        private const string EVENT_NAME_GETITEM = "GetItemEvent";
        private const string EVENT_NAME_INPUTFIELD = "InputFieldEvent";
        private const string PLAYER_ID = "Master";

        private ICharacter _currentCharacter;
        public ICharacterPrefab CurrentCharacterPrefab { get; set; }
        public Episode CurrentEpisode => DialogueManager.Instance.CurrentEpisode;
        public int CurrentIndex { get; private set; } = 0;
        public VoiceMode SoundMode { get; set; }


        public bool IsStarted => CurrentEpisode != null && CurrentIndex > 0;
        public bool IsFinished => CurrentEpisode == null || (CurrentEpisode != null && CurrentIndex >= CurrentEpisode.Dialogues.Count);
        public bool IsEventStackNotEmpty => DialogueManager.Instance.IsEventActive;

        private IDialogueBox _dialogueBox => DialogueManager.Instance.CurrentDialogueBox;
        private Stack<DialogueData> _eventDialogues => DialogueManager.Instance.EventDialogues;


        /// <summary>
        /// 다음 대사로 넘어갑니다.
        /// </summary>
        /// <param name="delay">초 단위의 시간입니다</param>
        public async void ToNextDialogue(float delay = 0, bool hideBoxBeforeDelay = false)
        {
            if (_dialogueBox.LogIfNull()) return;

            if (delay > 0)
            {
                if (hideBoxBeforeDelay) DialogueManager.Instance.HideDialogueBox();
                await UniTask.WaitForSeconds(delay);
                if (hideBoxBeforeDelay) DialogueManager.Instance.ShowDialogueBox();
            }

            // TODO : 여기 체크하는 부분에 구조적 문제가 있는거 같습니다 ( 2024.02.19 )
            // EventDialogue가 비어있지 않으면 EventDialogue를 처리하고 리턴하는데
            // EventDialogue 추가 하는 코드가 각 이벤트 안에 있어서 현재 구조상 여기 if문에 걸리지 못합니다
            if (IsEventStackNotEmpty)
            {
                ToNextEventDialogue();
                return;
            }

            if (CurrentEpisode != null && CurrentEpisode.Dialogues != null)
            {
                CurrentIndex++;
                if (CurrentEpisode.Dialogues.ContainsKey(CurrentIndex))
                {
                    GoTo(CurrentIndex);
                    return;
                }
            }

            GNLog.Info("다음 대사가 없습니다. 다이얼로그를 종료합니다.");
            DialogueManager.Instance.FinishDialogue();
        }

        public void ToNextEventDialogue()
        {
            GNLog.Info("다음 이벤트 대사로 넘어갑니다.");

            if (_eventDialogues.Count == 0)
            {
                ToNextDialogue();
                return;
            }

            DialogueData dialog = _eventDialogues.Peek();
            HandleDialogue(dialog);
            _eventDialogues.Pop();
        }


        public void GoTo(int index)
        {
            int indexCounter = 0;

            if (CurrentEpisode.Dialogues.IsNullOrEmpty())
            {
                Game.DisplayError(this, "Dialogue Block is null");
                DialogueManager.Instance.FinishDialogue();
                return;
            }

            // Skipping null keys
            if (!CurrentEpisode.Dialogues.ContainsKey(index))
            {
                GNLog.Warning($"Dialogue index {index} not found. Skipping...");
                while (!CurrentEpisode.Dialogues.ContainsKey(index))
                {
                    if (indexCounter > 500)
                    {
                        Game.DisplayError(this, "Infinite loop detected:" + index);
                        break;
                    }
                    indexCounter++;
                    index++;
                }
            }

            // Skipping event related stuff      
            if (IsEventStackNotEmpty && CurrentEpisode.Dialogues[index].EventIndex != 0)
            {
                GNLog.Warning($"Dialogue index {index} is an event. Skipping...");
                while (CurrentEpisode.Dialogues[index].EventIndex != 0)
                {
                    if (indexCounter > 500)
                    {
                        Game.DisplayError(this, "Infinite loop detected:" + index);
                        break;
                    }
                    indexCounter++;
                    index++;
                }
            }

            CurrentIndex = index;

            if (CurrentEpisode.Dialogues.ContainsKey(CurrentIndex))
            {
                HandleDialogue(CurrentEpisode.Dialogues[CurrentIndex]);
            }
            else
            {
                GNLog.Error($"Dialogue index {index} not found.");
            }
        }

        private void ShowDialogueBoxAndNext()
        {
            DialogueManager.Instance.ShowDialogueBox();
            ToNextDialogue();
        }

        private void HandleDialogue(DialogueData dialog)
        {
            DialogueType type = dialog.Type;
            GNLog.Info($"<color=blue>[ {type} ]</color> {dialog.Line}");

            string line = dialog.Line;
            string characterId = dialog.CharacterId;
            string arg = dialog.Arg;
            string sfxId = dialog.SFXId;
            string bgmId = dialog.BGMId;

            switch (type)
            {
                // 대사 출력
                case DialogueType.Line: HandleLine(dialog); break;

                // 이벤트 시작
                case DialogueType.Narration: HandleEvent(EVENT_NAME_NARRATION, dialog); break;
                case DialogueType.Event: HandleEvent(EVENT_NAME_EVENT, dialog); break;
                case DialogueType.GetItem: HandleEvent(EVENT_NAME_GETITEM, dialog); break;
                case DialogueType.InputField: HandleEvent(EVENT_NAME_INPUTFIELD, dialog); break;

                // 사운드 or 배경음악 재생
                case DialogueType.Sfx: PlaySfx(sfxId); break;
                case DialogueType.Bgm: PlayBGM(int.Parse(bgmId)); break;

                // 캐릭터 입장 or 퇴장
                case DialogueType.Enter: Enter(characterId, true); break;
                case DialogueType.Exit: Exit(characterId); break;

                // 어시스트모드(대화창에 비서 얼굴나오는 모드) 시작 or 종료
                case DialogueType.AssistStart: DialogueManager.Instance.StartAssistMode(characterId); break;
                case DialogueType.AssistEnd: DialogueManager.Instance.EndAssistMode(); break;

                // 특수 함수 실행
                case DialogueType.Finish: DialogueManager.Instance.FinishDialogue(); break; // 스토리 종료
                case DialogueType.Empty: ToNextDialogue(1.2f, true); break;

                case DialogueType.Click:
                    DialogueManager.Instance.HideDialogueBox();
                    TutorialManager.Instance.SetBackgroundEnabled(false);
                    TutorialManager.Instance.SetTutorial(characterId, 0, ShowDialogueBoxAndNext);
                    break;

                case DialogueType.Wait:
                    DialogueManager.Instance.HideDialogueBox();
                    TutorialManager.Instance.SetBackgroundEnabled(false);
                    if (!string.IsNullOrEmpty(characterId))
                    {
                        float waitTime = float.Parse(characterId);

                        IEnumerator WaitAndShowNextTutorialLine(float delay)
                        {
                            yield return new WaitForSeconds(delay);
                            ShowDialogueBoxAndNext();
                            TutorialManager.Instance.SetBackgroundEnabled(true);
                        }

                        CoroutineOwner.StartCoroutine(WaitAndShowNextTutorialLine(waitTime));
                    }
                    break;

                case DialogueType.ShowCutScene: DialogueManager.Instance.EventHandler.ShowCutscene?.Invoke(this, arg); break;
                case DialogueType.HideCutScene: DialogueManager.Instance.EventHandler.HideCutscene?.Invoke(this, arg); break;
                case DialogueType.Questionnaire: DialogueManager.Instance.HandleQuestionnaire(dialog); break;      // 2024-1-16 추가
            }
        }

        private void HandleLine(DialogueData dialog)
        {
            if (_dialogueBox.LogIfNull()) return;

            string characterId = dialog.CharacterId;
            string line = dialog.Line;

            if (characterId != PLAYER_ID)
            {
                ICharacter character = null;//TODO
                _dialogueBox.CurrentCharacter = character;

                if (SoundMode == VoiceMode.Voiced)
                {
                    //bool isAssistDialogue = _dialogueBox is DialogueBox_Companion;
                    //string voiceFilePath = dialog.GetVoiceFilePath(isAssistDialogue);
                    //if (!string.IsNullOrWhiteSpace(voiceFilePath)) _dialogueBox.PlayVoice(voiceFilePath);
                }
            }

            string parsed = ApplyTags(line);
            _dialogueBox.ShowText(parsed);
        }

        /// <summary>
        /// 더이상 함수를 나누지 말고 모든 이벤트를 여기서 처리하자
        /// </summary>
        private void HandleEvent(string eventName, DialogueData dialog)
        {
            DialogueManager.Instance.HideDialogueBox();

            if (eventName == EVENT_NAME_NARRATION)
            {
                DialogueManager.Instance.Logger.Info(eventName, dialog.Line);
                string line = dialog.Line;
                DialogueManager.Instance.EventHandler.ShowNarration?.Invoke(this, new Narration() { content = line, onComplete = ShowDialogueBoxAndNext });
                return;
            }

            if (eventName == EVENT_NAME_INPUTFIELD)
            {
                string inputFieldEventCode = dialog.Arg;
                DialogueManager.Instance.Logger.Info(eventName, inputFieldEventCode);
                DialogueManager.Instance.EventHandler.ShowInputFieldPopup?.Invoke(this, inputFieldEventCode);
                return;
            }

            if (eventName == EVENT_NAME_GETITEM)
            {
                DialogueManager.Instance.Logger.Info(eventName, dialog.ItemId + " " + dialog.ItemQuantity);
                int itemId = dialog.ItemId;
                int quantity = dialog.ItemQuantity;
                ItemReward data = ItemReward.FromItemIndex(itemId, quantity);
                data.TryClaimAsync((success) => ShowDialogueBoxAndNext()).Forget();
                return;
            }

            int eventId = dialog.EventIndex;

            if (!CurrentEpisode.Events.ContainsKey(eventId))
            {
                DialogueManager.Instance.Logger.Warning($"EventIndex {eventId} is not found in CurrentEpisode.Events");
                ToNextDialogue();
                return;
            }

            if (eventName == EVENT_NAME_EVENT)
            {
                string eventCode = dialog.Arg;
                DialogueManager.Instance.Logger.Info(eventName, eventCode);
                DialogueManager.Instance.EventHandler.HandleEvent?.Invoke(this, eventCode);
            }
        }


        #region SFX & BGM
        public void PlaySfx(string id) => PlaySfxAsync(id, DialogueSettings.SfxDelay, DialogueSettings.SfxWait);

        private async void PlaySfxAsync(string addressableName, float delay, float wait)
        {
            DialogueManager.Instance.HideDialogueBox();
            await UniTask.Delay((int)(delay * 1000));
            SoundManager.Instance.PlaySFX(addressableName);
            await UniTask.Delay((int)(wait * 1000));
            ToNextDialogue();
            await UniTask.Delay(1000);
            DialogueManager.Instance.ShowDialogueBox();
        }

        public void PlayBGM(int bgmId)
        {
            BGMManager.Instance.StartPreview(bgmId);
            ToNextDialogue();
        }


        #endregion

        #region DEBUGGING (운영자)
        public void SkipToNextEvent(int agoLine = 1)
        {
            int eventLine = 0;
            foreach (KeyValuePair<int, DialogueEvent> choiceEvent in CurrentEpisode.Events)
            {
                if (choiceEvent.Value.StartIndex > CurrentIndex)
                {
                    eventLine = choiceEvent.Value.StartIndex - agoLine;
                    CurrentIndex = eventLine;
                    Debug.Log("EventLine found :" + choiceEvent.Value.StartIndex);
                    break;
                }
            }

            if (eventLine == 0)
            {
                CurrentIndex = CurrentEpisode.Dialogues.Count - agoLine;
            }

            ToNextDialogue();
        }
        #endregion

        #region 특수 : Character Enter & Exit // DialogueScript (구글스프레드시트)와 데이터 연결하여 CustomTextAnimatorPlayer.cs 통해 여기로 연결됨

        /// <summary>
        /// 캐릭터가 바에 등장 (주의: 함수 이름을 변경하면 안됩니다)
        /// </summary>
        public void Enter(string value, bool automaticallyShowNextLineAfterDelay = false)
        {
            if (CurrentCharacterPrefab != null) CurrentCharacterPrefab.Show();
            if (automaticallyShowNextLineAfterDelay) ToNextDialogue(1.2f, true);
        }

        /// <summary>
        /// 캐릭터가 바에서 퇴장 (주의: 함수 이름을 변경하면 안됩니다)
        /// </summary>
        public void Exit(string value)
        {
            if (CurrentCharacterPrefab != null) CurrentCharacterPrefab.Hide();
        }

        #endregion

        private string ApplyTags(string text)
        {
            //if (text.Contains("[username]")) text = text.Replace("[username]", $"<color=#00DDFF>{User.Player.DisplayName}</color>");
            //if (text.Contains("[barname]")) text = text.Replace("[barname]", $"<color=#00DDFF>{User.BarSaveData.BarName}</color>");
            //if (text.Contains("[totaldays]")) text = text.Replace("[totaldays]", $"<color=#00DDFF>{User.Time.GetTotalDaysPlayed()}</color>");
            return text;
        }
    }
}