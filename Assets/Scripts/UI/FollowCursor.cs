using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FollowCursor : MonoBehaviour
{
    public Camera mainCam;
    public float defaultZ = 0;
    public float followTime = 0.1f;

    private Vector3 lastPosition;
    private Tweener movement;

    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        movement = transform.DOMove(Vector3.forward * defaultZ, followTime).SetAutoKill(false);
        lastPosition = Vector3.forward * defaultZ;
    }
    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    private void Update()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();

        mousePos.z = defaultZ;
        mousePos = mainCam.ScreenToWorldPoint(mousePos);
        mousePos.z = defaultZ;

        if (lastPosition == mousePos) return;

        movement.ChangeEndValue(mousePos, true).Restart();
        lastPosition = mousePos;
    }
}
