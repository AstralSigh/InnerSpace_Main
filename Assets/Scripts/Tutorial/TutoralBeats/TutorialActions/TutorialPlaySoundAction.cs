using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPlaySoundAction : BaseTutorialAction_Prototyping
{
    [SerializeField] string soundString = "event:/UI Events/UI_success";
    public override void Run(TutorialBeat_Prototyping beat)
    {
        FMODUnity.RuntimeManager.PlayOneShot2D(soundString);
    }
}
