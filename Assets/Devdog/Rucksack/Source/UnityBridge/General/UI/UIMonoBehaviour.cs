using UnityEngine;
using UnityEngine.UI;

namespace Devdog.Rucksack.UI
{
    public abstract class UIMonoBehaviour : MonoBehaviour
    {
        protected void SetActive(Behaviour uiBehaviour, bool a)
        {
            if (uiBehaviour != null)
            {
                SetActive(uiBehaviour.gameObject, a);
            }
        }
        
        protected void SetActive(GameObject obj, bool a)
        {
            if (obj != null)
            {
                obj.SetActive(a);
            }
        }
        
        protected void SetActive(Transform t, bool a)
        {
            if (t != null)
            {
                t.gameObject.SetActive(a);
            }
        }

        protected void Set(Text text, string str)
        {
            if (text != null)
            {
                text.text = str;
                text.gameObject.SetActive(string.IsNullOrEmpty(str) == false);
            }
        }
        
        protected void Set(Image img, Sprite sprite)
        {
            Set(img, sprite, Color.white);
        }
        
        protected void DestroyChildren(Transform t)
        {
            foreach (Transform child in t)
            {
                Destroy(child.gameObject);
            }
        }
        
        protected void Set(Image img, Sprite sprite, Color color)
        {
            if (img != null)
            {
                img.sprite = sprite;
                img.color = color;
                img.gameObject.SetActive(sprite != null);
            }
        }
    }
}