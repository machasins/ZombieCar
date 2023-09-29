using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    public TextAsset dialogueAsset;

    private Dictionary<string, DialogueState> states;
    private DialogueState currentState;

    private void Awake()
    {
        DialogueState[] possibleStates = GetComponents<DialogueState>();

        foreach (DialogueState s in possibleStates)
        {
            states.Add(s.stateName, s);
            s.enabled = false;
        }

        if (!states.TryGetValue("", out currentState))
            currentState = possibleStates[0];

        currentState.enabled = true;
    }

    public void StartDialogue()
    {
        currentState.StartDialogue();
    }

    public void ChangeState(string stateName)
    {
        currentState.enabled = false;

        currentState = states[stateName];
        currentState.enabled = true;
    }
}
