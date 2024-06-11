using TMPro;
using UnityEngine;

namespace Glitch9.Routina
{
    public class ClickHere : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _text;
        [SerializeField] ParticleSystem _effect;
        [SerializeField] float _offset = 300;

        public void SetClickHere(string text, Vector3 targetPos)
        {
            if (!string.IsNullOrEmpty(text))
            {
                _text.text = text;
                _text.GetComponent<RectTransform>().position = targetPos - new Vector3(0, _offset, 0);
                _text.gameObject.SetActive(true);
            }
            else _text.gameObject.SetActive(false);

            _effect.GetComponent<RectTransform>().position = targetPos;
            _effect.gameObject.SetActive(true);
            _effect.Play();
        }
        
        public void Remove()
        {
            _effect.Stop();
            _text.gameObject.SetActive(false);
            _effect.gameObject.SetActive(false);
        }
    }
}