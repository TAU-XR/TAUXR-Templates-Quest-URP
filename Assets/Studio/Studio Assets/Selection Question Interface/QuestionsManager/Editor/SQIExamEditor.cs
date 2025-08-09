using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SQIExam))]
public class SQIExamEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector for SQIExam
        DrawDefaultInspector();

        // Reference to the SQIExam script
        SQIExam sqiExam = (SQIExam)target;

        // Initialize the children array if not already populated
        if (sqiExam.children == null || sqiExam.children.Length == 0)
        {
            sqiExam.Initialize();
        }

        if (GUILayout.Button("Next"))
        {
            sqiExam.Next();
        }

        if (GUILayout.Button("Previous"))
        {
            sqiExam.Previous();
        }

        if (GUILayout.Button("Initialize Array"))
        {
            sqiExam.Initialize();
        }
    }
}
