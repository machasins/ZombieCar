using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedDisplay : MonoBehaviour
{
    public float maximumPossibleSpeed;
    public Vector2 rotationLimits;
    public float lerpTime;

    private static WorldComponentReference wcr;

    private Tweener rotate;
    private float angle;

    private void Awake()
    {
        if (!wcr)
            wcr = FindObjectOfType<WorldComponentReference>();

        rotate = DOTween.To(() => angle, x => angle = x, rotationLimits.x, lerpTime).SetAutoKill(false);
    }

    private void Update()
    {
        Rigidbody2D rb = wcr.player.GetComponent<Rigidbody2D>();
        rotate.ChangeValues(angle, Mathf.Lerp(rotationLimits.x, rotationLimits.y, rb.velocity.magnitude / maximumPossibleSpeed), lerpTime);

        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
