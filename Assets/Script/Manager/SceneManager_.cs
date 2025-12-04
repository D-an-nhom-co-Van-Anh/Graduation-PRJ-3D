using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneManager_ : Singleton<SceneManager_>
{
    public void LoadSceneByName(string sceneName, bool lockCursor = false, bool showCursor = true)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("ten scene rong hoac null");
            return;
        }
        if (SceneManager.sceneCount > 1)
        {
            Debug.LogWarning("Dang co scene additive → khong the load scene moi: " + sceneName);
            return;
        }
        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            StartCoroutine(LoadSceneAsyncHidden(sceneName, lockCursor, showCursor));
        }
        else
        {
            Debug.LogError("Ko thay scene " + sceneName + " hoac chua add Build Settings");
        }
    }

    private IEnumerator LoadSceneAsyncHidden(string sceneName, bool lockCursor, bool showCursor)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        asyncLoad.allowSceneActivation = false;

        Debug.Log("bat dau load " + sceneName);

        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }
        asyncLoad.allowSceneActivation = true;

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
            Debug.LogWarning("Scene " + sceneName + " chua load additive.");
            yield break;
        }

        AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(sceneName);
        yield return unloadOp;

        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        loadOp.allowSceneActivation = false;

        while (loadOp.progress < 0.9f)
            yield return null;

        loadOp.allowSceneActivation = true;
        Debug.Log("Reload xong scene additive: " + sceneName);
    }
}
