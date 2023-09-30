using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Progression;
using System.Security.Cryptography.X509Certificates;
using UnityEngine.InputSystem.XR;

public class DialogueController : MonoBehaviour
{
    public TextAsset dialogueAsset;
    public QuestAsset questAsset;

    private static WorldComponentReference wcr;

    private bool isInteractingWithPlayer = false;

    private void Awake()
    {
        if (!wcr)
            wcr = FindObjectOfType<WorldComponentReference>();
    }

    private void Update()
    {
        if (isInteractingWithPlayer && wcr.interactInput.action.WasPressedThisFrame())
            StartDialogue();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            isInteractingWithPlayer = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            isInteractingWithPlayer = false;
    }

    public void StartDialogue()
    {
        wcr.dialogueDisplay.GetComponent<DialogueParser>().StartDialogue(dialogueAsset, questAsset);
    }
}
