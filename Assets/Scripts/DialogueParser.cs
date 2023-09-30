using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Progression;
using TMPro;
using UnityEngine.UI;
using KoganeUnityLib;
using DG.Tweening;
using UnityEngine.InputSystem;
using System;

public class DialogueParser : MonoBehaviour
{
    public TMP_Text nameText;
    public Image characterSprite;
    public float typewriterSpeed;

    public Transform buttonContainer;
    public Transform buttonPrefab;

    private static WorldComponentReference wcr;

    private Story story;
    private QuestAsset quest;

    TMP_Typewriter typewriter;
    CanvasGroup group;

    PlayerInput input;

    private void Awake()
    {
        if (!wcr)
            wcr = FindObjectOfType<WorldComponentReference>();

        input = wcr.cameraPosition.GetComponent<PlayerInput>();
        typewriter = GetComponentInChildren<TMP_Typewriter>();
        group = GetComponent<CanvasGroup>();
    }

    public void StartDialogue(TextAsset dialogueAsset, QuestAsset questAsset)
    {
        group.DOFade(1.0f, 0.25f).SetUpdate(true);
        Time.timeScale = 0.0f;

        input.SwitchCurrentActionMap("UI");
        input.actions["Click"].performed += AdvanceDialogue;

        RemoveChildren(buttonContainer);

        story = new Story(dialogueAsset.text);
        quest = questAsset;

        ProgressionPointAsset[] points = quest.GetProgressionPoints();

        for (int i = 0; i < points.Length; ++i)
            story.variablesState["quest_" + i] = points[i].IsComplete();

        AdvanceDialogue();
    }

    public void AdvanceDialogue(InputAction.CallbackContext action)
    {
        if (action.action.WasPressedThisFrame())
            AdvanceDialogue();
    }

    public void AdvanceDialogue()
    {
        if (!story)
            return;

        if (typewriter.IsActive())
        {
            typewriter.Skip();
            return;
        }

        if (!story.canContinue && story.currentChoices.Count <= 0)
            OnEnd();

        if (story.canContinue)
        {
            string characterName = (string)story.variablesState["char_name"];
            nameText.text = characterName;

            string text = story.Continue().Trim();
            typewriter.Play(text, typewriterSpeed, null, true);

            RemoveChildren(buttonContainer);

            List<Choice> choices = story.currentChoices;
            if (story.currentChoices.Count > 0)
            {
                for (int i = 0; i < story.currentChoices.Count; i++)
                {
                    Choice choice = story.currentChoices[i];
                    Button button = CreateChoiceView(choice.text.Trim());
                    // Tell the button what to do when we press it
                    button.onClick.AddListener(delegate
                    {
                        OnClickChoiceButton(choice);
                    });

                    if (i == 0)
                        button.Select();
                }
            }
        }
    }

    void OnClickChoiceButton(Choice choice)
    {
        story.ChooseChoiceIndex(choice.index);
        AdvanceDialogue();
    }

    Button CreateChoiceView(string text)
    {
        // Creates the button from a prefab
        Transform choice = Instantiate(buttonPrefab, buttonContainer, false);

        // Gets the text from the button prefab
        choice.GetComponentInChildren<TMP_Text>().text = text;

        return choice.GetComponent<Button>();
    }

    void RemoveChildren(Transform t)
    {
        int childCount = t.childCount;
        for (int i = childCount - 1; i >= 0; --i)
            Destroy(t.GetChild(i).gameObject);
    }

    private void OnEnd()
    {
        ProgressionPointAsset[] points = quest.GetProgressionPoints();

        for (int i = 0; i < points.Length; ++i)
        {
            if (!points[i].IsComplete() && (bool)story.variablesState["quest_" + i])
                points[i].Complete();
        }

        group.DOFade(0.0f, 0.25f).SetUpdate(true);
        Time.timeScale = 1.0f;

        input.actions["Click"].performed -= AdvanceDialogue;
        input.SwitchCurrentActionMap("Player");
    }
}
