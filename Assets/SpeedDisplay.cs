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

    private void Awake()
    {
        if (!wcr)
            wcr = FindObjectOfType<WorldComponentReference>();

        rotate = DOTween.To(() => transform.rotation.eulerAngles.z - 180, x => transform.rotation = Quaternion.Euler(new Vector3(0, 0, x)), rotationLimits.x, lerpTime).SetAutoKill(false);
    }

    private void Update()
    {
        Rigidbody2D rb = wcr.player.GetComponent<Rigidbody2D>();
        rotate.ChangeValues(transform.rotation.eulerAngles.z - 180, Mathf.Lerp(rotationLimits.x, rotationLimits.y, rb.velocity.magnitude / maximumPossibleSpeed), lerpTime);
    }
}
