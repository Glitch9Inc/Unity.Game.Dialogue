using Glitch9.Game.Dialogue;
using Glitch9.Tweener;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;


namespace Glitch9.Routina.Tutorial
{
    public class TutorialManager : MonoSingleton<TutorialManager>
    {
        public static Action onTutorialEnd;

        [SerializeField] private GameObject background;
        [SerializeField] private Transform container;
        [SerializeField] private ClickHere clickHere;
        [SerializeField] private Sprite emptySprite;
        [SerializeField, SerializeReference] private IDialogueBox dialogueBox;

        public TutorialEntry CurrentEntry { get; set; }
        public Action OnClick { get; set; }
        public List<TutorialEntry> Tutorials { get; set; }


        private CanvasGroup _backgroundCanvasGroup;


        public void Initialize(IList<TutorialEntry> tutorials)
        {
            Tutorials = new List<TutorialEntry>(tutorials);
            if (background != null) _backgroundCanvasGroup = background.GetComponent<CanvasGroup>();
        }

        public void SetTutorial(string arg, float delay = 0, Action onClick = null)
        {
            StartCoroutine(SetOnClickCoroutine(arg, delay, onClick));
        }

        private IEnumerator SetOnClickCoroutine(string arg, float delay = 0, Action onClick = null)
        {
            if (Tutorials == null)
            {
                GNLog.Error("튜토리얼 오브젝트가 없습니다 ");
                yield break;
            }

            if (arg == "SwipeRoutine")
            {
                SetBackgroundEnabled(true);
                StartCoroutine(ExecuteAfterDelay(onClick, 5f));
                yield break;
            }

            yield return new WaitForSeconds(delay);

            GameObject obj = null;
            foreach (TutorialEntry item in Tutorials)
            {
                if (item.key == arg)
                {
                    obj = GameObject.Find(item.objectName);
                    this.OnClick = item.action;
                    this.OnClick += onClick;
                    CurrentEntry = item;
                    break;
                }
            }

            if (CurrentEntry == null)
            {
                GNLog.Error("튜토리얼 오브젝트가 없습니다: " + arg);
                yield break;
            }

            SetHighlight(obj);
        }

        private IEnumerator ExecuteAfterDelay(Action action, float delay)
        {
            yield return new WaitForSeconds(delay);
            action?.Invoke();
        }

        public void SetHighlight(GameObject obj)
        {
            SetBackgroundEnabled(true);

            Vector3 targetPos = obj.GetComponent<RectTransform>().position;
            Vector2 targetSize = obj.GetComponent<RectTransform>().sizeDelta;

            GameObject clone = Instantiate(obj, container);
            clone.GetComponent<RectTransform>().position = targetPos;

            GameObject empty = Instantiate(new GameObject("btn"), container);
            RectTransform rectTransform = empty.AddComponent<RectTransform>();
            rectTransform.sizeDelta = targetSize;
            rectTransform.position = targetPos;

            Image image = empty.AddComponent<Image>();
            image.sprite = emptySprite;
            Button button = empty.AddComponent<Button>();
            button.onClick.AddListener(() => Finish());
            clickHere.SetClickHere(null, targetPos);
        }


        public void SetBackgroundEnabled(bool active)
        {
            if (background.activeSelf == active) return;

            if (active)
            {
                background.gameObject.SetActive(true);
                _backgroundCanvasGroup.FadeIn(0.5f);
            }
            else
            {
                _backgroundCanvasGroup.FadeOut(0.5f);
            }
        }

        private void Finish()
        {
            if (container.childCount > 0)
                foreach (Transform child in container)
                    Destroy(child.gameObject);

            clickHere.Remove();
            if (CurrentEntry.vibrate) Vibrator.Tick();
            OnClick?.Invoke();
        }
    }
}