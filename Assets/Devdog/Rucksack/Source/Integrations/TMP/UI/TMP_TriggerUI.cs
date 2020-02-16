using System.Linq;
using Devdog.General2;
using Devdog.General2.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Devdog.Rucksack.Integrations.TMP
{
    [RequireComponent(typeof(UIWindow))]
    public partial class TMP_TriggerUI : MonoBehaviour
    {
        /// <summary>
        /// Bind an KeyCode to a sprite.
        /// </summary>
        [System.Serializable]
        public class KeyIconBinding
        {
            public KeyCode keyCode;
            public Sprite sprite;
        }

        public Image imageIcon;
        public TMP_Text shortcutText;
        public bool moveToTriggerLocation = true;

        public KeyIconBinding[] keyIconBindings = new KeyIconBinding[0];

        private ITrigger _currentTrigger;
        private UIWindow _window;

        public Player player
        {
            get { return PlayerManager.currentPlayer; }
        }

        protected virtual void Awake()
        {
            _window = GetComponent<UIWindow>();
        }

        protected void LateUpdate()
        {
            if (player != null)
            {
                if (player.bestTriggerInRange != _currentTrigger)
                {
                    _currentTrigger = player.bestTriggerInRange;
                    BestTriggerChanged(_currentTrigger);
                }

                _currentTrigger = player.bestTriggerInRange;
            }
        }
        
        private void BestTriggerChanged(ITrigger newBest)
        {
            if (newBest != null)
            {
                _window.Show();
                Repaint(newBest);
                if (moveToTriggerLocation)
                {
                    UpdatePosition(newBest);
                }
            }
            else
            {
                _window.Hide();
            }
        }

        protected virtual Sprite GetIcon(ITriggerInputHandler inputHandler)
        {
            var binding = keyIconBindings.FirstOrDefault(o => o.keyCode.ToString() == inputHandler.ToString());
            if (binding == null)
            {
                DevdogLogger.LogWarning("Couldn't find binding for keycode : " + inputHandler.ToString(), this);
                return null;
            }

            return binding.sprite;
        }
        
        protected virtual void UpdatePosition(ITrigger trigger)
        {
            var behavior = trigger as Behaviour;
            if (behavior != null)
            {
                transform.position = Camera.main.WorldToScreenPoint(behavior.transform.position);
            }
        }

        public virtual void Repaint(ITrigger trigger)
        {
            _window.Show();

            Sprite icon = null;
            string actionName = "";

            var behaviour = trigger as Behaviour;
            if (behaviour != null)
            {
                var input = behaviour.GetComponent<ITriggerInputHandler>();
                if (input != null)
                {
                    icon = GetIcon(input);
                    actionName = input.ToString();
                }
            }

            if (imageIcon != null)
            {
                imageIcon.sprite = icon;
            }

            if (shortcutText != null)
            {
                shortcutText.text = actionName;
            }
        }
    }    
}
