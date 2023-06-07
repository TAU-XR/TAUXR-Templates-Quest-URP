
public class GameManager : TAUXRSingleton<GameManager>
{
    TAUXRSceneManager sceneManager;

    private void Start()
    {
        sceneManager = TAUXRSceneManager.Instance;
        sceneManager.Init();
    }

}
