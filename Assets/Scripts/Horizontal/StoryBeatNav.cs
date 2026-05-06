using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class StoryBeatNav : MonoBehaviour
{
    public int storyIndex = 0;
    public GameObject playerRoot;
    public NarrationPlaylist sceneNarration;

    public List<UnityEvent> storyBeats;
    private void Start()
    {
        
    }

    private void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.Space) && storyIndex < storyBeats.Count)
        {
            AdvanceStory();
        }
        */

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            AdvanceStory();
        }

    }
    public void AdvanceStory()
    {
        if (storyIndex < storyBeats.Count)
        {
            storyBeats[storyIndex].Invoke();
            storyIndex++;
        }
    }

    public void CenterPlayer()
    {
        iTween.MoveTo(playerRoot, iTween.Hash("position", this.gameObject.transform.position, "time", 1.5f, "easetype", iTween.EaseType.easeOutSine));
    }
}
