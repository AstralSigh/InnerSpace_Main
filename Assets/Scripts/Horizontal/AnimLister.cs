using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class AnimLister : MonoBehaviour
{
    public List<AnimBeat> localAnimBeats;
    public List<Transform> objectStarts;

    public int animIndex = -1;

    public GameObject camRoot;

    public Text conTitle;
    public Text conSubTitle;
    public Text descText;

    public UnityEvent onCompleteScene;

    public bool autoPlay;

    void Start()
    {
        if (autoPlay)
        {
            InvokeRepeating("AdvanceAnim", 1f, 1f);
        }
    }

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame && animIndex < localAnimBeats.Count - 1)
        {
            AdvanceAnim();
        }


    }

    public void AdvanceAnim()
    {
        this.animIndex++;

        if (animIndex < localAnimBeats.Count)
        {
            if (!localAnimBeats[animIndex].animTarget.activeSelf)
                localAnimBeats[animIndex].animTarget.SetActive(true);

            if (localAnimBeats[animIndex].animTarget.GetComponent<Animator>())
            {
                localAnimBeats[animIndex].animTarget.GetComponent<Animator>().SetTrigger("Activated");
            }
            else
            {
                iTween.MoveTo(localAnimBeats[animIndex].animTarget, iTween.Hash("position", localAnimBeats[animIndex].refTarget.position, "time", 1.0f, "easetype", localAnimBeats[animIndex].animEasing));
            }

            if(!autoPlay)
            {
                iTween.MoveTo(camRoot, iTween.Hash("position", localAnimBeats[animIndex].camTarget.position, "time", 1.0f, "easetype", localAnimBeats[animIndex].camEasing));

                SetHUD(animIndex);
            }
            

            StartCoroutine(EnableTargets(0));
            StartCoroutine(DisableTargets(1.0f));

           
        }

        if (animIndex == localAnimBeats.Count)
        {
            onCompleteScene.Invoke();
            StartCoroutine(EnableTargets(0));
            StartCoroutine(DisableTargets(1.0f));
        }


    }

    IEnumerator EnableTargets(float delay)
    {

        yield return new WaitForSeconds(delay);

        if (animIndex < localAnimBeats.Count && localAnimBeats[animIndex].enableTargets.Count > 0)
        {
            foreach (GameObject go in localAnimBeats[animIndex].enableTargets)
            {
                go.SetActive(true);
            }
        }

    }

    IEnumerator DisableTargets(float delay)
    {

        yield return new WaitForSeconds(delay);

        if (animIndex <= localAnimBeats.Count && localAnimBeats[animIndex].disableTargets.Count > 0)
        {
            foreach (GameObject go in localAnimBeats[animIndex].disableTargets)
            {
                go.SetActive(false);
            }
        }
    }

    void SetHUD(int animStepIndex)
    {
        //conTitle.text = ConDatas[animStepIndex].conHeader;
        if(localAnimBeats[animStepIndex].animDesc != "")
        {
            descText.text = localAnimBeats[animStepIndex].animDesc;
        }
        else
        {
            descText.text = "";
        }
        //conSubTitle.text = ConDatas[animStepIndex].conSubHeader;
    }
}

[System.Serializable]
public class AnimBeat
{
    public GameObject animTarget;

    public Transform refTarget;

    public Transform camTarget;

    public List<GameObject> enableTargets;

    public List<GameObject> disableTargets;

    public iTween.EaseType animEasing = iTween.EaseType.easeInOutCirc;
    public iTween.EaseType camEasing = iTween.EaseType.easeInOutSine;

    [TextArea(3, 33)]
    public string animDesc;
}
