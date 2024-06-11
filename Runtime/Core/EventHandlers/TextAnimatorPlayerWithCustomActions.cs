using System.Collections;
using Febucci.UI;
using UnityEngine;

namespace Glitch9.Game.Dialogue
{
    public class TextAnimatorPlayerWithCustomActions : TextAnimatorPlayer
    {
        protected override IEnumerator DoCustomAction(TypewriterAction action)
        {
            Debug.Log("TextAnimationCustomAction: " + action.actionID);

            switch (action.actionID)
            {
                case "enter":
                    foreach (string t in action.parameters)
                    {
                        DialogueManager.Instance.Enter(t);
                    }
                    break;

                case "exit":
                    foreach (string t in action.parameters)
                    {
                        DialogueManager.Instance.Exit(t);
                    }
                    break;

                case "sfx":
                    foreach (string t in action.parameters)
                    {
                        SoundManager.Instance.PlaySFX(t);
                    }
                    break;

                case "bgm":
                    foreach (string t in action.parameters)
                    {
                        if (t == "release")
                        {
                            SoundManager.Instance.StopBGM();
                        }
                        else
                        {
                            if (!int.TryParse(t, out int trackNum)) continue;
                            if (trackNum > 0) SoundManager.Instance.PlayBGM(trackNum);
                        }
                    }
                    yield return new WaitForSeconds(2);
                    break;

                case "emotion":
                    foreach (string t in action.parameters)
                    {
                        //string emotionStr = t;
                        ///* capitalize the first letter if it isn't already */
                        //emotionStr = emotionStr.Substring(0, 1).ToUpper() + emotionStr.Substring(1).ToLower();
                        //PoseType emotion = /*parse*/(PoseType)Enum.Parse(typeof(PoseType), emotionStr);
                        //CharacterManager.Instance.TriggerEmotion(emotion);
                    }
                    break;
            }
        }
    }
}