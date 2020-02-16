using UnityEngine;
using UnityEngine.SceneManagement;

namespace Devdog.Rucksack.Demo
{
    public sealed class DemoSceneLoader : MonoBehaviour
    {
        public string sceneName = "SurvivalUI_Partial";
        public LoadSceneMode mode = LoadSceneMode.Additive;
        
        private void Start()
        {
            var active = SceneManager.GetSceneByName(sceneName);
            if (active.isLoaded == false)
            {
                SceneManager.LoadScene(sceneName, mode);
            }
        }
    }
}