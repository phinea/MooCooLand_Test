using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MooCooEngine
{
    public class Button_ToggleMenu : Target
    {
        public GameObject MenuRootGameObject;
        public Texture TextureToMinimize;
        public Texture TextureToMaximize;
        public bool startMaximized = false;
        private bool isMaximized = false;

        protected override void Start()
        {
            isMaximized = startMaximized;
            UpdateMenu();
        }

        public new void OnSelect()
        {
            Debug.Log("Button_ToggleMenu: OnSelect!");
            isMaximized = !isMaximized;
            UpdateMenu();
        }
        
        private void UpdateMenu()
        {
            if (MenuRootGameObject != null)
            {
                MenuRootGameObject.SetActive(isMaximized);

                if (isMaximized)
                    UpdateTexture(TextureToMinimize);
                else
                    UpdateTexture(TextureToMaximize);
            }
            else
            {
                Debug.LogErrorFormat("[{0}] Menu root game object has not been specified.", this.name);
            }
        }

        private void UpdateTexture(Texture tex)
        {
            //Texture2D tex = (Texture2D)Resources.Load(texName, typeof(Texture2D));
            this.GetComponent<Renderer>().material.mainTexture = tex;
        }
    }
}
