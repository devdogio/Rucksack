using UnityEngine;

namespace Devdog.Rucksack
{
    public sealed class Spin : MonoBehaviour
    {
        public Vector3 angles;

        private void Update()
        {
            transform.Rotate(angles * Time.unscaledDeltaTime);
        }
    }
}