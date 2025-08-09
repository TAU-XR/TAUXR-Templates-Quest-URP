using NaughtyAttributes;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PositioningHandle : MonoBehaviour
{
    private Transform pinchingTransform;

    [Header("Attached Objects")]
    public List<Transform> objectsToAdjust; // Original handle logic
    public Transform connectedObject; // The flat window or other object to rotate

    public float lerpSpeed = 8f;
    public float startMovingDistance = 0.1f;

    [SerializeField] private float _minimalHeightFromFloor;

    private Vector3 initialTargetPosition;
    private Dictionary<Transform, Vector3> initialPositions = new Dictionary<Transform, Vector3>();

    private bool isAdjusting = false;
    private bool hasMovedOnce = false;
    private PinchDetector _input;
    private PositioningHandleVisuals _visuals;

    private Camera mainCamera;

    private void Awake()
    {
        _visuals = GetComponentInChildren<PositioningHandleVisuals>();
        _visuals.SetActive(false);
        _input = GetComponentInChildren<PinchDetector>();
        _input.PinchEnter += OnPinchEnter;
        _input.PinchExit += OnPinchExit;
        _input.PinchHoverEnter += OnHoverEnter;
        _input.PinchHoverExit += OnHoverExit;

        mainCamera = Camera.main; // Get the main camera (user's head position)
    }

    private void OnHoverEnter(PinchManager pinchManager)
    {
        if (_input.IsPinched) return;
        _visuals.SetHover();
    }

    private void OnHoverExit(PinchManager pinchManager)
    {
        if (_input.IsPinched) return;
        _visuals.SetActive();
    }

    private void OnPinchEnter(PinchManager pinchManager)
    {
        if (isAdjusting) return;

        pinchingTransform = pinchManager.Pincher.transform;
        StartAdjustment();
        _visuals.SetPinched();
    }

    private void OnPinchExit()
    {
        pinchingTransform = null;
        StopAdjustment();
        if (_input.IsHovered)
            _visuals.SetHover();
        else
            _visuals.SetActive();
    }

    private void LateUpdate()
    {
        if (isAdjusting)
        {
            AdjustPositions();
            UpdateConnectedObject(); // Update the connected object's rotation
            if(transform.position.y < _minimalHeightFromFloor)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y+ 0.05f, transform.position.z);
                objectsToAdjust[0].position = new Vector3(objectsToAdjust[0].position.x, objectsToAdjust[0].position.y + 0.05f, objectsToAdjust[0].position.z);
                isAdjusting = false;
            }
        }
    }

    [Button]
    public void StartAdjustment()
    {
        StoreInitialPositions();
        isAdjusting = true;
        hasMovedOnce = false;
    }

    [Button]
    public void StopAdjustment()
    {
        isAdjusting = false;
    }

    private void StoreInitialPositions()
    {
        if (pinchingTransform == null)
        {
            Debug.LogError("Target Object is not assigned for Height Handle Script.");
            return;
        }

        initialTargetPosition = pinchingTransform.position;

        initialPositions.Clear();

        initialPositions[transform] = transform.position;

        foreach (Transform obj in objectsToAdjust)
        {
            if (obj == null) continue;
            initialPositions[obj] = obj.position;
        }
    }

    private void AdjustPositions()
    {
        Vector3 currentTargetPosition = pinchingTransform.position;
        Vector3 deltaPosition = currentTargetPosition - initialTargetPosition;

        if (!hasMovedOnce && deltaPosition.magnitude < startMovingDistance) return;

        hasMovedOnce = true;

        foreach (Transform obj in initialPositions.Keys)
        {
            if (obj == null) continue;

            Vector3 newPosition = initialPositions[obj] + deltaPosition;
            obj.position = Vector3.Lerp(obj.position, newPosition, lerpSpeed * Time.deltaTime);
        }
    }

    private void UpdateConnectedObject()
    {        
        if (connectedObject == null) return;

        // Calculate the direction vector from the connected object to the camera
        Vector3 direction = TXRPlayer.Instance.PlayerHead.transform.position - connectedObject.position;

        // Ensure the connected object faces the camera, accounting for height differences
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // Apply a 180-degree offset to flip the object
        targetRotation *= Quaternion.Euler(0, 180, 0);

        // Smoothly interpolate the object's rotation towards the target rotation
        objectsToAdjust[0].rotation = Quaternion.Slerp(objectsToAdjust[0].rotation, targetRotation, lerpSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lerpSpeed * Time.deltaTime);
    }



}