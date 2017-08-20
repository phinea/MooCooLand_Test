using UnityEngine;

namespace MooCooEngine.Game
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        public static GameManager Instance { get { return _instance; } }

        public string SceneName_MainMenu;
        public string SceneName_Settings;
        public string SceneName_LevelHandler;
        public string SceneName_Highscores;

        //public string UserName;
        //public User CurrentUser;
        //public string StartLevelName;

        // Use this for initialization
        void Start()
        {
            if (_instance == null)
                _instance = this;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void GoTo_MainMenu()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneName_MainMenu);
        }

        public void GoTo_Settings()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneName_Settings);
        }

        public void GoTo_ActualGame()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneName_LevelHandler);
        }

        public void GoTo_Highscore()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneName_Highscores);
        }
    }
}