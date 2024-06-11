using System;
using Glitch9.Routina.Tutorial;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Glitch9.Game.Dialogue
{
    public class DialogueManager : MonoSingleton<DialogueManager>
    {
        [SerializeField, SerializeReference] private List<IDialogueBox> dialogueBoxes;
        [SerializeField] private AudioClip masterClip;

 
        public bool AutoMode => CurrentDialogueBox != null && CurrentDialogueBox.AutoMode;
        public bool IsTalking(string characterId)
        {
            return IsDialogueStarted
                   && CharacterPrefab != null
                   && CurrentDialogueBox != null
                   && !CurrentDialogueBox.AllLettersShown
                   && CurrentDialogueBox.CurrentCharacter != null
                   && CurrentDialogueBox.CurrentCharacter.Id == characterId
                   && CurrentDialogueBox.AnyLetterVisible;
        }

        public ICharacter Character { get; set; }
        public ICharacterPrefab CharacterPrefab { get; set; }
        public Episode CurrentEpisode { get; private set; }
        public EpisodeId CurrentEpisodeId { get; private set; }
        public IDialogueBox CurrentDialogueBox { get; private set; }
        public Stack<DialogueData> EventDialogues { get; private set; }
        public EpisodeType Mode
        {
            get
            {
                if (CurrentEpisodeId == null) return EpisodeType.Unknown;
                if (CurrentEpisodeId.IsQuestionnaire) return EpisodeType.SmallTalk;
                if (CurrentEpisodeId.IsMainEpisode) return EpisodeType.Main;
                if (CurrentEpisodeId.IsSideEpisode) return EpisodeType.Side;
                return EpisodeType.SmallTalk;
            }
        }
        public DialogueEventHandler EventHandler { get; set; }
        public ILogger Logger { get; set; }

        public bool IsDialogueStarted => _dialogueHandler?.IsStarted ?? false;
        public bool IsDialogueFinished => _dialogueHandler?.IsFinished ?? false;
        public bool IsEventActive => EventDialogues.IsValid();

        private IEpisodeLoader _episodeLoader;
        private DialogueHandler _dialogueHandler;
        private QuestionnaireHandler _questionnaireHandler;
        private bool _isInitialized = false;

        public void Initialize(IEpisodeLoader episodeLoader, ILogger customLogger = null)
        {
            if (_isInitialized) return;
            _isInitialized = true;

            _episodeLoader = episodeLoader;
            Logger = customLogger ?? new DialogueLogger();
            _dialogueHandler = new DialogueHandler();
            _questionnaireHandler = new QuestionnaireHandler();
            EventHandler = new DialogueEventHandler();
        }


        public void StartEventDialogues(List<DialogueData> eventDialogues)
        {
            EventDialogues = eventDialogues.ToStack();
            _dialogueHandler.ToNextEventDialogue();
        }


        public async void StartEpisodeAsync(ICharacter character, EpisodeData data)
        {
            if (character.LogIfNull() || data.LogIfNull()) return;

            GNLog.Info($"에피소드 시작: {character.Id} - {data.Id}");
            Episode episode = await _episodeLoader.LoadEpisodeAsync(character.Id, data.Id);
            HandleRegularEpisode(character, data, episode);
        }
        public void StartEpisode(ICharacter character, EpisodeData data, Episode episode) => HandleRegularEpisode(character, data, episode);

        private void HandleRegularEpisode(ICharacter character, EpisodeData data, Episode episode)
        {
            EventDialogues = new();

            if (episode == null)
            {
                MyGame.DisplayIssue(this, Issue.MissingContent);
                GNLog.Error("에피소드를 로드 완료했으나 값이 null입니다.");
                return;
            }

            Character = character;
            CurrentEpisodeId = data.Id;

            // if (data.BGMId != 0) BGMHelper.StartPreview(data.BGMId);

            void startDialogue() => OnStartEpisode(character, episode, data.IsVoiced == 1);
            // if (data.IsNarration) MenuNarration.Show(data.Title, startDialogue);
            //else OnStartEpisode(character, episode, data.IsVoiced == 1);
        }

        private void OnStartEpisode(ICharacter character, Episode episode, bool isVoiced)
        {
            if (character.LogIfNull()) return;

            if (!CurrentDialogueBox.IsVisible)
                InitiateDefaultMode(character);

            EventHandler.OnEpisodeStart?.Invoke(this, episode);
            CurrentEpisode = episode;
            VoiceMode mode = VoiceMode.SFX;
            if (isVoiced) mode = VoiceMode.Voiced;
            _dialogueHandler.SoundMode = mode;
            _dialogueHandler.GoTo(0);
        }


        public void PlayOneDialogue(ICharacter character, VoiceFile singleLine)
        {
            _dialogueHandler.SoundMode = VoiceMode.Voiced;
            DialogueData entry = singleLine.ToDialogue(character.Id);
            PlayOneDialogue(character, entry);
        }

        /// <summary>
        /// 보이스알림을 프리뷰할때 사용된다
        /// </summary>
        public void PlayOneDialogue(ICharacter character, string singleStringLine)
        {
            if (character.LogIfNull()) return;
            if (singleStringLine.LogIfNullOrEmpty()) return;

            _dialogueHandler.SoundMode = VoiceMode.Silent;
            DialogueData entry = DialogueData.CreateLine(0, character.Id, singleStringLine);
            PlayOneDialogue(character, entry);
        }

        private void PlayOneDialogue(ICharacter character, DialogueData dialogueData)
        {
            GNLog.Info($"PlayOneDialogue: {character.Id}, {dialogueData}");

            if (!CurrentDialogueBox.IsVisible)
            {
                //CurrentDialogueBox = dialogueBoxCompanion;
                //dialogueBoxCompanion.gameObject.SetActive(true);
                //dialogueBoxCompanion.Initialize(character);
            }

            EventDialogues = new();
            EventDialogues.Push(dialogueData);
            _dialogueHandler.ToNextEventDialogue();
        }

        public void InitiateDefaultMode(ICharacter character)
        {
            if (character.LogIfNull()) return;
            //CurrentDialogueBox = dialogueBoxMy;
            CurrentDialogueBox.Show();
            CurrentDialogueBox.CurrentCharacter = character;
        }

        public void StartAssistMode(string characterId)
        {
            throw new NotImplementedException();
        }
        public void EndAssistMode(bool startDefaultMode = true)
        {
            CurrentDialogueBox.Complete(() =>
            {
                CurrentDialogueBox.Hide();
                TutorialManager.Instance.SetBackgroundEnabled(false);
                if (startDefaultMode)
                {
                    _dialogueHandler.ToNextDialogue();
                }
            });
        }

        public void HandleQuestionnaire(DialogueData dialog)
        {
            _questionnaireHandler ??= new QuestionnaireHandler();
            _questionnaireHandler.SetAnswer(dialog.QuestionId, dialog.AnswerIndex);
        }

        public void FinishDialogue()
        {
            HideDialogueBox();

            if (IsDialogueFinished) return; // 여기 문제 있을 수 있다. 스토리 종료시 문제 생기면 체크

            if (_questionnaireHandler != null && _questionnaireHandler.IsQuestionnaireAvailable)
            {
                // TODO : 설문 결과 처리
            }

            CharacterPrefab?.Hide();
            EventHandler.OnDialogueEnd?.Invoke(this, CurrentEpisode);

            CurrentEpisode = null;
            CurrentEpisodeId = null;

            Logger.Info("Dialogue Finished");
        }

        public void ShowDialogueBox()
        {
            if (CurrentDialogueBox.LogIfNull()) return;
            CurrentDialogueBox.Show();
        }

        public void HideDialogueBox()
        {
            if (CurrentDialogueBox.LogIfNull()) return;
            CurrentDialogueBox.Complete(CurrentDialogueBox.Hide);
        }

        public void PlayTypewriterSound() => SoundManager.Instance.PlaySFX("타자기소리");
        public void ToNextDialogue(float delay = 0) => _dialogueHandler.ToNextDialogue(delay);
        public void GoTo(int index) => _dialogueHandler.GoTo(index);
        public void SkipToNextEvent(int agoLine = 1) => _dialogueHandler.SkipToNextEvent(agoLine);
        public void Enter(string value, bool automaticallyShowNextLineAfterDelay = false) => _dialogueHandler.Enter(value, automaticallyShowNextLineAfterDelay);
        public void Exit(string value) => _dialogueHandler.Exit(value);


        // 특수 다이얼로그
        // 2024-01-16 추가
        public void Ask(ICharacter character, string question, string dialogue, params Choice[] choices)
        {
            // 지금 실행중인 다이얼로그가 있으면 그것을 강제로 종료시킨다.
            if (IsDialogueStarted) FinishDialogue();

            if (character.LogIfNull()) return;
            if (choices == null || choices.Length == 0) return;
            if (choices.Length > 4)
            {
                GNLog.Error("선택지는 최대 4개까지 가능합니다.");
                return;
            }

            //InitiateAssistMode(character.Id);
            Episode tempEpisode = Episode.CreateQuestion(character.Id, question, dialogue, choices);

            // manually start episode
            CurrentEpisodeId = tempEpisode.Id;
            CurrentEpisode = tempEpisode;
            _dialogueHandler = new DialogueHandler();
            _dialogueHandler.SoundMode = VoiceMode.SFX;
            _dialogueHandler.GoTo(0);
        }

        public VoiceMode GetDialogueHandlerMode() => _dialogueHandler?.SoundMode ?? VoiceMode.Unset;

        private IDialogueBox GetDialogueBox(DialogueBoxType boxType)
        {
            if (dialogueBoxes.IsNullOrEmpty()) return null;
            IDialogueBox result = dialogueBoxes.Find(box => box.Type.HasFlag(boxType));
            if (result != null) return result;
            return dialogueBoxes.FirstOrDefault();
        }



        //private void OnMainEpisodeStart()
        //{
        //    if (ICharacterManager.Instance) ICharacterManager.Instance.StopSpawning();
        //    NavigationBar.Instance.DisableAllButtons();
        //    BarManager.Instance.PanTo(CameraZoom.ZoomedIn);
        //}

        //private void OnMainEpisodeEnd()
        //{
        //    if (GameManager.Instance.TutorialIsNotCompleted) ICharacterManager.Instance.StartSpawning();
        //    if (TutorialManager.Instance) TutorialManager.Instance.SetBGEnabled(false);
        //    NavigationBar.Instance.EnableAllButtons();
        //    BarManager.Instance.PanTo(CameraZoom.ZoomedOut);
        //    BGMHelper.StopPreview();
        //}

    }
}