using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MooCooEngine
{
    public class Button_StartScene : Target
    {
        public string SceneToBeLoaded = "";

        public new void OnSelect()
        {
            if (SceneToBeLoaded != "")
            {
                StartCoroutine(LoadNewScene());
            }
            else
            {
                Debug.LogError(">> Unsupported menu button.");
            }
        }

        IEnumerator LoadNewScene()
        {
            Debug.Log(">> ********************************  Load new scene  ****************************************");
            yield return new WaitForSeconds(0.5f);
            SceneManager.LoadScene(SceneToBeLoaded);
        }
    }
}
