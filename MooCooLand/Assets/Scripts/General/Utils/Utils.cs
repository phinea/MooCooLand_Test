using UnityEngine;

namespace MooCooEngine.Utils
{
    public static class General
    {
        public static string GetValidFilename(string unvalidatedFilename)
        {
            char[] invalidChars = new char[] { '/', ':', '*', '?', '"', '<', '>', '|', '\\'}; 
            string formattedFilename = unvalidatedFilename;
            
            foreach (char invalidChar in invalidChars)
            {
                formattedFilename = formattedFilename.Replace((invalidChar.ToString()), ""); 
            }
                
            return formattedFilename;
        }

        public static string GetFullName(GameObject go)
        {
            string goName = go.name;
            Transform g = go.transform;
            while (g.parent != null)
            {
                goName = g.parent.name + "\\" + goName;
                g = g.transform.parent;
            }
            return goName;
        }

        public static float Clamp(float value, float min, float max)
        {
            if (value > max)
                return max;
            if (value < min)
                return min;
            return value;
        }

        public static float Normalize(float value, float min, float max)
        {
            if (value > max)
                return 1;
            if (value < min)
                return 0;

            return (value - min) / (max - min);
        }

        public static void GameObject_ChangeTransparency(GameObject gobj, float newTransparency)
        {
            float origTransp = 0; // just a dummy variable to reuse the following function
            Utils.General.GameObject_ChangeTransparency(gobj, newTransparency, ref origTransp);
        }

        /// <summary>
        /// Change the transparency of game object "gobj" with a transparency value between 0 and 255;
        /// </summary>
        /// <param name="gobj"></param>
        /// <param name="transparency">Expected values range from 0 (fully transparent) to 1 (fully opaque).</param>
        /// <param name="originalTransparency">Input "-1" if you don't know the original transparency yet.</param>
        public static void GameObject_ChangeTransparency(GameObject gobj, float transparency, ref float originalTransparency)
        {
            try
            {
                //# Go through renderers in main object
                Renderers_ChangeTransparency(gobj.GetComponents<Renderer>(), transparency, ref originalTransparency);

                //# Go through renderers in children objects
                Renderers_ChangeTransparency(gobj.GetComponentsInChildren<Renderer>(), transparency, ref originalTransparency);
            }
            catch (MissingReferenceException)
            {
                // Just ignore; Usually happens after the game object already got destroyed, but the update sequence had already be started
            }
        }

        private static void Renderers_ChangeTransparency(Renderer[] renderers, float transparency, ref float originalTransparency)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                Material[] mats = renderers[i].materials;
                foreach (Material mat in mats)
                {
                    Color c = mat.color;

                    if (originalTransparency == -1)
                        originalTransparency = c.a;

                    if (transparency < 1)
                    {
                      //  ChangeRenderMode.ChangeRenderModes(mat, ChangeRenderMode.BlendMode.Transparent);
                    }
                    else
                    {
                        //ChangeRenderMode.ChangeRenderModes(mat, ChangeRenderMode.BlendMode.Opaque);
                    }

                    mat.color = new Color(c.r, c.g, c.b, transparency);
                }
            }
        }

        /// <summary>
        /// Change the color of game object "gobj".
        /// </summary>
        /// <param name="gobj"></param>
        /// <param name="newColor"></param>
        public static void GameObject_ChangeColor(GameObject gobj, Color newColor)
        {
            Color? col;
            col = Color.black;
            GameObject_ChangeColor(gobj, newColor, ref col);
        }

        /// <summary>
        /// Change the color of game object "gobj".
        /// </summary>
        /// <param name="gobj"></param>
        /// <param name="newColor"></param>
        /// <param name="originalColor">Enter "null" in case you're passing the original object and want to save the original color.</param>
        public static void GameObject_ChangeColor(GameObject gobj, Color newColor, ref Color? originalColor)
        {
            try
            {
                Renderer[] renderers = gobj.GetComponents<Renderer>();
                for (int i = 0; i < renderers.Length; i++)
                {
                    Material[] mats = renderers[i].materials;
                    foreach (Material mat in mats)
                    {
                        Color c = mat.color;

                        if (!originalColor.HasValue)
                            originalColor = c;

                        mat.color = newColor;
                    }
                }

                renderers = gobj.GetComponentsInChildren<Renderer>();
                for (int i = 0; i < renderers.Length; i++)
                {
                    Material[] mats = renderers[i].materials;
                    foreach (Material mat in mats)
                    {
                        mat.color = newColor;
                    }
                }
            }
            catch (MissingReferenceException)
            {
                // Just ignore; Usually happens after the game object already got destroyed, but the update sequence had already be started
            }
        }
    }
}
