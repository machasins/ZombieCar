using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MapMover : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IScrollHandler
{
    public float movementInterpTime = 0.25f;
    public Vector2 movementMax;

    public float zoomInterpTime = 0.25f;
    public float zoomDefault = 2.5f;
    public float zoomMin = 0.75f;
    public float zoomMax = 4.0f;
    public float zoomAmount = 0.1f;

    private RectTransform t;

    private Vector2 prevCursorPosition;
    private Vector3 prevPosition;
    private Vector3 targetPosition;

    private Tweener movement;
    private Tweener zoom;

    private float zoomLevel;
    private float zoomLevelPrev;

    private void Awake()
    {
        t = GetComponent<RectTransform>();

        prevCursorPosition = t.position;
        targetPosition = prevCursorPosition;
        movement = t.DOMove(targetPosition, movementInterpTime).SetAutoKill(false);
        prevPosition = targetPosition;

        zoomLevel = zoomDefault;
        zoomLevelPrev = zoomDefault;
        t.localScale = zoomLevel * Vector3.one;
        zoom = t.DOScale(zoomLevel * Vector3.one, zoomInterpTime).SetAutoKill(false);
    }

    private void Update()
    {
        if (targetPosition != prevPosition)
        {
            movement.ChangeEndValue(targetPosition, true).Restart();
            prevPosition = targetPosition;
        }

        if (zoomLevel != zoomLevelPrev)
        {
            zoom.ChangeEndValue(zoomLevel * Vector3.one, true).Restart(true);
            zoomLevelPrev = zoomLevel;
        }
    }

    public void OnBeginDrag(PointerEventData data)
    {
        prevCursorPosition = GetPoint(data);
    }

    public void OnDrag(PointerEventData data)
    {
        Vector2 point = GetPoint(data);

        targetPosition += (Vector3)(point - prevCursorPosition);

        targetPosition = new(
            Mathf.Clamp(targetPosition.x, -movementMax.x * zoomLevel, movementMax.x * zoomLevel), 
            Mathf.Clamp(targetPosition.y, -movementMax.y * zoomLevel, movementMax.y * zoomLevel), 
            targetPosition.z
        );

        Debug.Log(targetPosition);
        prevCursorPosition = point;
    }

    public void OnEndDrag(PointerEventData data)
    {

    }

    public void OnScroll(PointerEventData data)
    {
        float scroll = data.scrollDelta.y;

        zoomLevel = Mathf.Clamp(zoomLevel + scroll * zoomAmount, zoomMin, zoomMax);

        SetPivotInWorldSpace(t, GetPoint(data));
    }

    public Vector2 GetPoint(PointerEventData data)
    {
        //RectTransform t = GetComponent<RectTransform>();
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(t, data.pointerCurrentRaycast.screenPosition, data.pressEventCamera, out Vector2 point);

        return data.pointerCurrentRaycast.worldPosition;
    }

    // Set the RectTransform's pivot point in world coordinates, without moving the position.
    // This is like dragging the pivot handle in the editor.
    public void SetPivotInWorldSpace(RectTransform source, Vector3 pivot)
    {
        // Strip scaling and rotations.
        pivot = source.InverseTransformPoint(pivot);
        Vector2 pivot2 = new(
            (pivot.x - source.rect.xMin) / source.rect.width,
            (pivot.y - source.rect.yMin) / source.rect.height);

        // Now move the pivot, keeping and restoring the position which is based on it.
        Vector2 offset = pivot2 - source.pivot;
        offset.Scale(source.rect.size);
        Vector3 worldPos = source.position + source.TransformVector(offset);
        source.pivot = pivot2;
        source.position = worldPos;
        targetPosition = worldPos;
    }
}
