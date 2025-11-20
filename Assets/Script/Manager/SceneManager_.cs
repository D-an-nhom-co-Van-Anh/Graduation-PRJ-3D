using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager_ : Singleton<SceneManager_>
{
  
    public void LoadSceneByName(string sceneName, bool lockCursor = false, bool showCursor = true)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Tên scene r?ng ho?c null");
            return;
        }

        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            Debug.Log("?ã load scene " + sceneName);

            if (lockCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = showCursor;   
            }
        }
        else
        {
            Debug.LogError("Không th?y scene: " + sceneName + " ho?c ch?a add vào Build Settings");
        }
    }
}
