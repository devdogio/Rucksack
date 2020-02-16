using Devdog.Rucksack.UI;
using TMPro;

namespace Devdog.Rucksack.Integrations.TMP
{
    /// <summary>
    /// Defines extension methods for UI components that use TextMesh Pro.
    /// </summary>
    public static class TMP_UIMonoBehaviour
    {
        public static void Set(this UIMonoBehaviour ui, TMP_Text text, string str)
        {
            if (text != null)
            {
                text.text = str;
                text.gameObject.SetActive(string.IsNullOrEmpty(str) == false);
            }
        }
    }
}