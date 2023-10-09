using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueRandomController : MonoBehaviour
{
    public TextAsset dialogueAsset;
    public float minimumTimeBetweenDialogues;
    public float timeBetweenAttempts;
    [Range(0,1)] public float dialogueChance;

    private static WorldComponentReference wcr;

    private bool canAttempt = false;
    private bool hasAIController = false;

    AIController controller;

    private void Start()
    {
        if (!wcr)
            wcr = FindObjectOfType<WorldComponentReference>();

        controller = GetComponent<AIController>();
        hasAIController = controller != null;

        General.StartCallbackAfterTime(this, () => canAttempt = false, Random.Range(0, minimumTimeBetweenDialogues), () => canAttempt = true);
    }

    private void Update()
    {
        if (canAttempt && (!hasAIController || controller.GetCurrentState() != AIController.State.passive))
        {
            if (Random.value <= dialogueChance)
            {
                StartDialogue();
                General.StartCallbackAfterTime(this, () => canAttempt = false, minimumTimeBetweenDialogues + timeBetweenAttempts, () => canAttempt = true);
            }
            else
                General.StartCallbackAfterTime(this, () => canAttempt = false, timeBetweenAttempts, () => canAttempt = true);
        }
    }

    public void StartDialogue()
    {
        wcr.dialogueRandomDisplay.GetComponent<DialogueRandomParser>().StartDialogue(dialogueAsset, null);
    }
}
