using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

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
            Debug.Log("Da load scene " + sceneName);

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
            Debug.LogError("khong thay scene " + sceneName + " hoac chua add vào Build Settings");
        }
    }

    public void ReloadAdditiveScene(string sceneName)
    {
        StartCoroutine(ReloadSceneCoroutine(sceneName));
    }
    public void ExitAdditiveScene(string sceneName)
    {
        SceneManager.UnloadSceneAsync(sceneName);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private IEnumerator ReloadSceneCoroutine(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        if (!scene.isLoaded)
        {
            Debug.LogWarning("Scene " + sceneName + " chuaa load additive.");
            yield break;
        }

        AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(sceneName);
        yield return unloadOp;

        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        yield return loadOp;

        Debug.Log("Reload xong scene additive: " + sceneName);
    }
}
