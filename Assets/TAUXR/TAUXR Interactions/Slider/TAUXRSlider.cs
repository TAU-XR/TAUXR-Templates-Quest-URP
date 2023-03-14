using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TAUXRSlider : MonoBehaviour
{
    [SerializeField] Transform lineStart;
    [SerializeField] Transform lineEnd;
    LineRenderer sliderLine;

    [SerializeField] VRButtonTouch touchButton;         // need to get a referece to the touchButton to get the toucher transform.
    Transform node;
    Vector3 nodePositionTarget;

    [SerializeField] TMPro.TextMeshPro valueText;

    [Range(0f, 1f)]
    [SerializeField] float valueStart = .5f;
    [SerializeField] float stepSize = .01f;
    [SerializeField] float nodeLerpSpeed = 5f;
    [SerializeField] float detachmentDistanceThreshold = .05f;

    // the Transform touching the slider node.
    Transform toucher;
    bool isNodeTouched = false;

    float valueCurrent = 0;
    public float Value => valueCurrent;

    TAUXRSliderSignifier signifier;
    void Start()
    {
        sliderLine = GetComponentInChildren<LineRenderer>();
        signifier = GetComponent<TAUXRSliderSignifier>();

        node = touchButton.transform;
        touchButton.Pressed.AddListener(OnNodeTouched);

        valueStart = RoundToStepSize(stepSize, valueStart);

        node.position = GetPointOnLineFromNormalizedValue(lineStart.position, lineEnd.position, valueStart);
        nodePositionTarget = node.position;
        valueCurrent = valueStart;

        signifier.IdlePreRating.Invoke();

        UpdateValueText(valueStart);
    }

    void Update()
    {
        SetLineRendererPositions(lineStart.position, lineEnd.position, sliderLine);

        if (isNodeTouched)
        {
            nodePositionTarget = GetClosestPointOnLine(lineStart.position, lineEnd.position, toucher.position);

            if (ShouldDetachNode(toucher.position, nodePositionTarget, detachmentDistanceThreshold))
            {
                DetachNode();
            }

            // round value to step size
            valueCurrent = GetNormalizedValueFromPointOnLine(lineStart.position, lineEnd.position, nodePositionTarget);
            valueCurrent = RoundToStepSize(stepSize, valueCurrent);
            nodePositionTarget = GetPointOnLineFromNormalizedValue(lineStart.position, lineEnd.position, valueCurrent);

            DebugShowDetachmentLine();
            UpdateValueText(valueCurrent);
        }

        // this line is out of isNodeTouch to allow node continue moving after finger left button (especially in cases the node is placed on 0/100).
        node.position = Vector3.Lerp(node.position, nodePositionTarget, nodeLerpSpeed * Time.deltaTime);

    }
    void UpdateValueText(float value)
    {
        if (valueText != null)
        {
            //valueText.text = (value * 100f).ToString();
            valueText.text = (Mathf.CeilToInt(value * 100f)).ToString();
        }
    }

    private bool ShouldDetachNode(Vector3 toucherPosition, Vector3 linePoint, float threshold)
    {
        float toucherToLineDistance = (toucherPosition - linePoint).magnitude;

        return (toucherToLineDistance > threshold);
    }

    void DebugShowDetachmentLine()
    {
        Vector3 toucherDirection = (toucher.position - nodePositionTarget).normalized;
        Vector3 detachmentPoint = nodePositionTarget + toucherDirection * detachmentDistanceThreshold;
        Debug.DrawLine(nodePositionTarget, detachmentPoint, Color.red);

        Debug.DrawLine(lineStart.position, nodePositionTarget, Color.green);
    }

    Vector3 GetClosestPointOnLine(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
    {
        Vector3 lineDirection = lineEnd - lineStart;
        float magnitudeMax = lineDirection.magnitude;
        lineDirection = lineDirection.normalized;

        Vector3 pointVector = point - lineStart;
        float projectionLength = Vector3.Dot(pointVector, lineDirection);
        projectionLength = Mathf.Clamp(projectionLength, 0, magnitudeMax);
        return lineStart + lineDirection * projectionLength;
    }

    void SetLineRendererPositions(Vector3 start, Vector3 end, LineRenderer line)
    {
        line.transform.position = start;
        line.transform.rotation = Quaternion.LookRotation(end - start, line.transform.up);
        line.SetPosition(1, new Vector3(0,0,(end - start).magnitude));
    }

    // gets a normilized target position and returns a world-position.
    Vector3 GetPointOnLineFromNormalizedValue(Vector3 lineStart, Vector3 lineEnd, float valueNormalized)
    {
        Vector3 lineVec = lineEnd - lineStart;
        valueNormalized = Mathf.Clamp01(valueNormalized);

        return lineStart + lineVec * valueNormalized;
    }

    float GetNormalizedValueFromPointOnLine(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
    {
        Vector3 lineVector = lineEnd - lineStart;
        Vector3 pointVector = point - lineStart;

        return pointVector.magnitude/lineVector.magnitude;
    }

    float RoundToStepSize(float stepSize, float clampedValue)
    {
        stepSize = Mathf.Clamp01(stepSize);
        float multiplicand = Mathf.Round(clampedValue / stepSize);
        return stepSize * multiplicand;
    }

    public void OnNodeTouched()
    {
        if (isNodeTouched) return;

        signifier.Sign(SliderSignifier.ButtonPress);
        signifier.DuringRating.Invoke();

        toucher = touchButton.ActiveToucher;
        isNodeTouched = true;

    }

    public void DetachNode()
    {
        Debug.Log("slider node detached");
        signifier.Sign(SliderSignifier.ButtonRelease);
        signifier.IdlePostRating.Invoke();

        isNodeTouched = false;
    }
}
