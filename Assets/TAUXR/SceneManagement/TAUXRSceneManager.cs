using UnityEditor;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

public class TAUXRSceneManager : TAUXRSingleton<TAUXRSceneManager>
{

    [Header("All scenes")]      // declare by name each scene and make it public so it can be accessed easily from other scripts.
    [SerializeField] public string BaseSceneName;
    [SerializeField] public string FirstLoadedSceneName;
   
    private float FADETOBLACKDURATION = 2.5f;
    private float FADETOCLEARDURATION = 1.5f;



    private string currentSceneName;
    public string CurrentSceneName => currentSceneName;

    public void Init()
    {
        // the script assumes that Base Scene is always the first in the build order.
        if (Application.isEditor)
        {
            // we assume that when played in editor we'll be in the right scene combination.
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                // Get the scene at the specified index
                Scene scene = SceneManager.GetSceneAt(i);

                if (scene.name != BaseSceneName)
                {
                    currentSceneName = scene.name;
                    return;
                }

            }
            // if we got here it means we only have the base scene (for 1 scene projects) and it should be the current
            currentSceneName = BaseSceneName;

        }
        else
        {
            // make sure to launch your starting scene here
            LoadActiveScene(FirstLoadedSceneName).Forget();
        }
    }

    async public UniTask SwitchActiveScene(string sceneName)
    {
        if (currentSceneName == sceneName)
        {
            Debug.LogWarning($"Tried to load {sceneName} scene but its already loaded");
            return;
        }

        await UnloadActiveScene();

        await LoadActiveScene(sceneName);

    }

    async private UniTask UnloadActiveScene()
    {
        await TAUXRPlayer.Instance.FadeToColor(Color.black, FADETOBLACKDURATION);

        await SceneManager.UnloadSceneAsync(currentSceneName);
    }

    async private UniTask LoadActiveScene(string sceneName)
    {
        if (currentSceneName == sceneName)
        {
            Debug.LogWarning($"Tried to load {sceneName} scene but its already loaded");
            return;
        }

        await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        currentSceneName = sceneName;

        // reposition player accordingly to new scene
        Transform playerScenePositioner = FindObjectOfType<PlayerScenePositioner>().transform;
        TAUXRPlayer.Instance.RepositionPlayer(playerScenePositioner.position, playerScenePositioner.rotation);

        await TAUXRPlayer.Instance.FadeToColor(Color.clear, FADETOCLEARDURATION);
    }
}
