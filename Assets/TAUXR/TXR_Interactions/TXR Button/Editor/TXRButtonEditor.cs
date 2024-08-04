using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TXRButton))]
public class TXRButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector first
        DrawDefaultInspector();

        // Get the target script
        TXRButton txrButton = (TXRButton)target;

        // Ensure that TXRButtonReferences, Backface, and Stroke are not null
        if (txrButton.References != null && txrButton.References.Backface != null && txrButton.References.Stroke != null && txrButton.References.ButtonInput.transform != null)
        {
            // Begin a change check
            EditorGUI.BeginChangeCheck();

            // Draw float fields for the width, height, and stroke offset properties
            float newWidth = EditorGUILayout.FloatField("Width", txrButton.References.Backface.Width);
            float newHeight = EditorGUILayout.FloatField("Height", txrButton.References.Backface.Height);
            string buttonText = EditorGUILayout.TextField("Text", txrButton.References.Text.text);
            float textSize = EditorGUILayout.FloatField("Text Size", txrButton.References.Text.fontSize);
            Color activeColor = EditorGUILayout.ColorField("Active Color", txrButton.References.Backface.FillColorEnd);

            // If there are any changes
            if (EditorGUI.EndChangeCheck())
            {
                // Record the changes for undo functionality
                Undo.RecordObject(txrButton.References.Backface, "Change Backface Dimensions");
                Undo.RecordObject(txrButton.References.Stroke, "Change Stroke Dimensions");
                Undo.RecordObject(txrButton.References.ButtonInput.transform, "Change Input Dimensions");
                Undo.RecordObject(txrButton.References.Text, "Change Text");

                // Apply the new width and height values to Backface
                txrButton.References.Backface.Width = newWidth;
                txrButton.References.Backface.Height = newHeight;
                txrButton.References.Backface.FillColorEnd = activeColor;

                // Apply the new width and height values to Stroke, including the stroke offset
                txrButton.References.Stroke.Width = newWidth + 0.005f;
                txrButton.References.Stroke.Height = newHeight + 0.005f;

                // Apply input collider size change
                txrButton.References.ButtonInput.transform.localScale = new Vector3(newWidth + 0.025f, newHeight + 0.025f, 0.08f);

                // Apply button text
                txrButton.References.Text.text = buttonText;
                txrButton.References.Text.fontSize = textSize;
                txrButton.References.Text.rectTransform.sizeDelta = new Vector2(newWidth, newHeight);


                EditorUtility.SetDirty(txrButton.References.Backface);
                EditorUtility.SetDirty(txrButton.References.Stroke);
            }
        }
        else
        {
            EditorGUILayout.HelpBox("References, Backface, or Stroke is not assigned.", MessageType.Warning);
        }
    }
}
