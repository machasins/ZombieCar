using Ink.Runtime;
using System.Collections.Generic;
using UnityEngine;
using Progression;
using TMPro;
using UnityEngine.UI;
using KoganeUnityLib;
using DG.Tweening;
using UnityEngine.InputSystem;
using System.Collections;

public class DialogueRandomParser : MonoBehaviour
{
    public TMP_Text nameText;
    public Image characterSprite;
    public float typewriterSpeed;
    public float completedAdvanceTime;
    public float dialogueCooldownTime;

    private Story story;
    private QuestAsset quest;

    private bool canAcceptDialogue = true;
    private Sprite originalCharacterSprite;

    TMP_Typewriter typewriter;
    CanvasGroup group;

    private void Awake()
    {
        typewriter = GetComponentInChildren<TMP_Typewriter>();
        group = GetComponent<CanvasGroup>();

        originalCharacterSprite = characterSprite.sprite;
    }

    public void StartDialogue(TextAsset dialogueAsset, QuestAsset questAsset)
    {
        if (!canAcceptDialogue)
            return;

        group.DOFade(1.0f, 0.25f).SetUpdate(true);
        canAcceptDialogue = false;

        story = new Story(dialogueAsset.text);
        quest = questAsset;

        if (questAsset)
        {
            ProgressionPointAsset[] points = quest.GetProgressionPoints();

            for (int i = 0; i < points.Length; ++i)
                story.variablesState["quest_" + i] = points[i].IsComplete();
        }

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

        if (!story.canContinue && story.currentChoices.Count <= 0)
            OnEnd();

        if (story.canContinue)
        {
            string text = story.Continue().Trim();
            typewriter.Play(text, typewriterSpeed, DelayAdvanceDialogue, false);

            string characterName = (string)story.variablesState["char_name"];
            nameText.text = characterName;

            Sprite characterImage = Resources.Load<Sprite>("Characters/" + (string)story.variablesState["char_sprite"]);
            characterSprite.sprite = (characterImage != null) ? characterImage : originalCharacterSprite;
        }
        else if (story.currentChoices.Count > 0)
            story.ChooseChoiceIndex(Random.Range(0, story.currentChoices.Count));
    }

    public void DelayAdvanceDialogue()
    {
        IEnumerator func()
        {
            yield return new WaitForSeconds(completedAdvanceTime);
            AdvanceDialogue();
        }

        StartCoroutine(func());
    }

    private void OnEnd()
    {
        if (quest)
        {
            ProgressionPointAsset[] points = quest.GetProgressionPoints();

            for (int i = 0; i < points.Length; ++i)
            {
                if (!points[i].IsComplete() && (bool)story.variablesState["quest_" + i])
                    points[i].Complete();
            }
        }

        group.DOFade(0.0f, 0.25f).SetUpdate(true);

        General.StartCallbackAfterTime(this, ()=> { }, dialogueCooldownTime, () => canAcceptDialogue = true);
    }
}
