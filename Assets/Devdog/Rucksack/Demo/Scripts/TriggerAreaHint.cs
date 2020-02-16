using Devdog.General2;
using Devdog.Rucksack.Demo.UI;
using UnityEngine;

namespace Devdog.Rucksack.Demo
{
    [RequireComponent(typeof(SphereCollider))]
    public sealed class TriggerAreaHint : MonoBehaviour
    {
        [SerializeField]
        private string _message = "";

        [SerializeField]
        private Sprite _sprite;
        
        private DemoHintUI _hintUI;
        private bool _used = false;
        private void Start()
        {
            _hintUI = FindObjectOfType<DemoHintUI>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_used)
            {
                return;
            }
            
            var player = other.GetComponentInParent<Player>();
            if (player != null)
            {
                _hintUI?.Repaint(_message, _sprite);
                _used = true;
            }
        }
    }
}