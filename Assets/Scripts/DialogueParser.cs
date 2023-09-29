using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueParser : MonoBehaviour
{
    public delegate void ResultPass(bool res);

    private DialogueDisplayHandler display;

    private Story story;
    private ResultPass finalResult;
    private UnityEvent finishEvent;

    public void StartDialogue(TextAsset dialogueAsset, string state, ResultPass callback, UnityEvent onFinish)
    {
        story = new Story(dialogueAsset.text);

        if (state != "")
            story.ChoosePathString(state);

        finalResult = callback;
        finishEvent = onFinish;

        AdvanceDialogue();
    }

    public void AdvanceDialogue()
    {
        if (!story)
            return;

        if (!story.canContinue && story.currentChoices.Count <= 0)
        {
            OnEnd();
        }

        string text = story.Continue().Trim();
        List<Choice> choices = story.currentChoices;
    }

    private void OnEnd()
    {
        finalResult((bool)story.variablesState["result"]);
        finishEvent.Invoke();
    }
}
