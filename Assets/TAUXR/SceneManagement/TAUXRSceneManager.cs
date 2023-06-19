using UnityEditor;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

public class TAUXRSceneManager : TAUXRSingleton<TAUXRSceneManager>
{

    [Header("All scenes")]      // declare by name each scene and make it public so it can be accessed easily from other scripts.
    [SerializeField] public string BaseSceneName = "Base Scene";
    [SerializeField] public string FirstLoadedSceneName = "TAUXR Entry Scene";

    private float FADETOBLACKDURATION = 2.5f;
    private float FADETOCLEARDURATION = 1.5f;



    private string currentSceneName;
    public string CurrentSceneName => currentSceneName;

    public void Init()
    {
        // the script assumes that Base Scene is always the first in the build order.
        if (Application.isEditor)
        {
            InitializeInEditor();
        }
        else
        {
            InitializeInBuild();
        }
    }

    private void InitializeInEditor()
    {
        // do nothing if scene count is not bigger than 1
        if (SceneManager.sceneCount < 2)
        {
            Debug.LogWarning("Scene count is less than 2, no scene initialization");
            return;
        }

        RepositionPlayer();
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

    private void InitializeInBuild()
    {
        TAUXRPlayer.Instance.FadeToColor(Color.black, 0).Forget();
        currentSceneName = BaseSceneName;
        // make sure to launch your starting scene here
        LoadActiveScene(FirstLoadedSceneName).Forget();
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
        RepositionPlayer();

        await TAUXRPlayer.Instance.FadeToColor(Color.clear, FADETOCLEARDURATION);
    }

    private void RepositionPlayer()
    {
        Transform playerScenePositioner = FindObjectOfType<PlayerScenePositioner>().transform;
        TAUXRPlayer.Instance.RepositionPlayer(playerScenePositioner.position, playerScenePositioner.rotation);
    }
}
