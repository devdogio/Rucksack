using System.Collections;
using Devdog.General2;
using UnityEngine;
using UnityEngine.UI;

namespace Devdog.Rucksack.Demo.UI
{
    public sealed class DemoHintUI : MonoBehaviour
    {
        [SerializeField]
        private Text _text;

        [SerializeField]
        private Image _image;

        [SerializeField]
        private AudioClipInfo _clip;

        [SerializeField]
        private AnimationClip _showAnimation;
        
        [SerializeField]
        private AnimationClip _hideAnimation;

        [SerializeField]
        private float _letterWaitTime = 0.05f;
        
        private WaitForSeconds _textLetterWait;
        
        private Coroutine _animationCoroutine;
        private Coroutine _textCoroutine;

        private void Start()
        {
            Repaint("", null);
        }
        
        public void Repaint(string message, Sprite sprite)
        {
            _textLetterWait = new WaitForSeconds(_letterWaitTime);
            if (_text != null)
            {
                if (_textCoroutine != null)
                {
                    StopCoroutine(_textCoroutine);
                }
                
                _textCoroutine = StartCoroutine(TypeText(_text, message));
            }

            if (_image != null)
            {
                _image.sprite = sprite;
            }

            GetComponent<AudioSource>()?.Play(_clip);
            GetComponent<Animator>()?.Play(_showAnimation.name);

            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
            }
            
            _animationCoroutine = StartCoroutine(HideVisuals(5f));
        }

        private IEnumerator TypeText(Text text, string message)
        {
            text.text = "";
            for (int i = 0; i < message.Length; i++)
            {
                text.text += message[i];
                yield return _textLetterWait;
            }
        }

        private IEnumerator HideVisuals(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            GetComponent<Animator>()?.Play(_hideAnimation.name);
        }
    }
}