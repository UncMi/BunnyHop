using UnityEngine;
using UnityEngine.Events;

public class InteractionEvent : MonoBehaviour
{
    public UnityEvent<string> onInteractionPromptChanged; 

    private static InteractionEvent _instance;

    public static InteractionEvent Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<InteractionEvent>();
            }
            return _instance;
        }
    }
}