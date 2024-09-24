using TMPro;
using UnityEngine;

public class InteractionPromptUI : MonoBehaviour
{
    [SerializeField] private TMP_Text promptText;

    private void OnEnable()
    {
        InteractionEvent.Instance.onInteractionPromptChanged.AddListener(UpdatePrompt);
    }

    private void OnDisable()
    {
        InteractionEvent.Instance.onInteractionPromptChanged.RemoveListener(UpdatePrompt);
    }

    private void UpdatePrompt(string prompt)
    {
        promptText.text = prompt;
    }
}