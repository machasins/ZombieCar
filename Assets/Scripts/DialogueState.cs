using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueState : MonoBehaviour
{
    public string stateName = "";
    public UnityEvent onDialogueFinish = null;

    private static WorldComponentReference wcr;

    private DialogueController controller;
    private bool result;

    private void Awake()
    {
        if (!wcr)
            wcr = FindObjectOfType<WorldComponentReference>();

        controller = GetComponent<DialogueController>();
    }

    public void StartDialogue()
    {
        wcr.dialogueDisplay.GetComponent<DialogueParser>().StartDialogue(controller.dialogueAsset, stateName, (b) => result = b, onDialogueFinish);
    }
}
