using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public string npcName;
    public EmotionState CurrentState = EmotionState.Calm;

    private List<NPC> observers = new List<NPC>();

    private void Start()
    {
        if(npcName == "") {npcName = gameObject.name;} // Assign the name of the GameObject to npcName if not assigned
        RegisterWithOthers();
    }

    private void RegisterWithOthers()
    {
        // Find all NPCs in the scene and subscribe to them (excluding self)
         NPC[] allNpcs = FindObjectsByType<NPC>(FindObjectsSortMode.None);
        foreach (var npc in allNpcs)
        {
            if (npc != this)
                npc.Subscribe(this); // I observe other NPCs
        }
    }

    public void Subscribe(NPC observer)
    {
        if (!observers.Contains(observer))
            observers.Add(observer);
    }

    public void Unsubscribe(NPC observer)
    {
        if (observers.Contains(observer))
            observers.Remove(observer);
    }

    public void ChangeEmotion(EmotionState newState)
    {
        if (CurrentState == newState) return;

        Debug.Log($"<color=green>{npcName} changed state to {newState}</color>");
        CurrentState = newState;
        NotifyObservers();
    }

    private void NotifyObservers()
    {
        foreach (var observer in observers)
        {
            observer.OnEmotionChanged(this, CurrentState);
        }
    }

    public void OnEmotionChanged(NPC source, EmotionState newEmotion)
    {
        // Define reaction rules based on observed emotion change
        if (newEmotion == EmotionState.Scared && CurrentState != EmotionState.Scared)
        {
            Debug.Log($"{npcName} panicked because {source.npcName} got scared!");
            ChangeEmotion(EmotionState.Scared);
        }
        else if (newEmotion == EmotionState.Alert && CurrentState == EmotionState.Curious)
        {
            Debug.Log($"{npcName} became alert because {source.npcName} was alerted!");
            ChangeEmotion(EmotionState.Alert);
        }
        else if (newEmotion == EmotionState.Curious && CurrentState == EmotionState.Calm)
        {
            Debug.Log($"{npcName} got curious because {source.npcName} noticed something!");
            ChangeEmotion(EmotionState.Curious);
        }
    }

    // You can manually trigger states via right-click context menu in the editor
    [ContextMenu("Trigger Scared")]
    public void TriggerScared() => ChangeEmotion(EmotionState.Scared);

    [ContextMenu("Trigger Alert")]
    public void TriggerAlert() => ChangeEmotion(EmotionState.Alert);

    [ContextMenu("Trigger Curious")]
    public void TriggerCurious() => ChangeEmotion(EmotionState.Curious);
}
