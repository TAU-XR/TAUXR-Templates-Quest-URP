using UnityEngine;

public class SQIExam : MonoBehaviour
{
    public GameObject[] children;
    public int currentIndex = 0;

    private void Awake()
    {
        Initialize();
    }

    // Initializes the array of children and sets the first child active
    public void Initialize()
    {
        int childCount = transform.childCount;
        children = new GameObject[childCount];

        // Populate array and disable all children
        for (int i = 0; i < childCount; i++)
        {
            children[i] = transform.GetChild(i).gameObject;
            children[i].SetActive(false);
        }

        // Activate the first child, if any exist
        if (children.Length > 0)
        {
            currentIndex = 0;
            children[currentIndex].SetActive(true);
        }
    }

    public void Next()
    {
        if (children == null || children.Length == 0) return;

        // Disable current child
        children[currentIndex].SetActive(false);

        // Increment index and wrap if necessary
        currentIndex++;
        if (currentIndex >= children.Length)
        {
            currentIndex = 0;
        }

        // Activate the new child
        children[currentIndex].SetActive(true);
    }

    public void Previous()
    {
        if (children == null || children.Length == 0) return;

        // Disable current child
        children[currentIndex].SetActive(false);

        // Decrement index and wrap if necessary
        currentIndex--;
        if (currentIndex < 0)
        {
            currentIndex = children.Length - 1;
        }

        // Activate the new child
        children[currentIndex].SetActive(true);
    }
}
