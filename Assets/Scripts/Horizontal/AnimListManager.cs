using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimListManager : MonoBehaviour
{
    public int animIndex = 0;
    public List<GameObject> animTargets;
    public List<Transform> animDestinations;
    public iTween.EaseType animEasing;

    public GameObject cameraRoot;
    public List<Transform> camPoses;

    public Text conTitle;
    public Text conSubTitle;
    public Text descText;

    public List<ConstituentData> ConDatas;
    [TextArea(3,33)]
    public List<string> ProcessDesc;

    public void StepAnimForwards()
    {
       
        if(animIndex < camPoses.Count)
        {
            if (!animTargets[animIndex].activeSelf)
                animTargets[animIndex].SetActive(true);

            if (animTargets[animIndex].GetComponent<Animator>())
            {
                animTargets[animIndex].GetComponent<Animator>().SetTrigger("Activated");
            }
            else
            {
                iTween.MoveTo(animTargets[animIndex], iTween.Hash("position", animDestinations[animIndex].position, "time", 1.0f, "easetype", animEasing));
            }

            iTween.MoveTo(cameraRoot, iTween.Hash("position", camPoses[animIndex].position, "time", 1.0f, "easetype", animEasing));

            SetHUD(animIndex);

            this.animIndex++;
        }
        
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            StepAnimForwards();
        }
    }

    void SetHUD(int animStepIndex)
    {
        conTitle.text = ConDatas[animStepIndex].conHeader;
        descText.text = ProcessDesc[animStepIndex];
        conSubTitle.text = ConDatas[animStepIndex].conSubHeader;
    }
}
