using Cysharp.Threading.Tasks;
using UnityEngine;

public class ExperimentStartingPosition : MonoBehaviour
{

    public Collider leftHandPositionCollider;
    public Collider rightHandPositionCollider;
    public Collider headPositionCollider;
    public Animator modelAnimator;

    private TXRPlayer player;
    private Transform playerHead;
    private Transform rightHandAnchor;
    private Transform leftHandAnchor;

    [Tooltip("The amount of time to wait for the player to reach the starting position. In seconds")]
    public float timeout = 300f;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        player = TXRPlayer.Instance;
        playerHead = player.PlayerHead;
        rightHandAnchor = player.RightHand;
        leftHandAnchor = player.LeftHand;
    }


    public async UniTask WaitForPlayerToBeInStartingPosition()
    {
        Init();

        float startTime = Time.time;
        while (true)
        {
            // safty check to prevent infinite loop
            if (Time.time - startTime > timeout)
            {
                Debug.LogWarning("Timeout waiting for player to reach starting position.");
                break;
            }
            // escape loop if escape key is pressed
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log("Exiting loop manually.");
                break;
            }

            // Check if the player's head and hands are in the colliders
            bool isHeadInCollider = headPositionCollider.bounds.Contains(playerHead.position);
            bool isLeftHandInCollider = leftHandPositionCollider.bounds.Contains(leftHandAnchor.position);
            bool isRightHandInCollider = rightHandPositionCollider.bounds.Contains(rightHandAnchor.position);

            // If all conditions are met, break out of the loop
            if (isHeadInCollider && isLeftHandInCollider && isRightHandInCollider)
            {
                modelAnimator.SetTrigger("FadeOut");
                await WaitForAnimationToEnd(modelAnimator);
                break;
            }

            await UniTask.Yield();
        }

        TXRDataManager.Instance.LogLineToFile("Player got to starting position. Starting the experiment");
        // Destroy the game object once everything is done
        Destroy(gameObject);
    }

    private async UniTask WaitForAnimationToEnd(Animator animator)
    {
        // Wait until the Animator stops playing any animations
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        while (animator.IsInTransition(0) || stateInfo.normalizedTime < 1f)
        {
            await UniTask.Yield();
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        }
    }




}
