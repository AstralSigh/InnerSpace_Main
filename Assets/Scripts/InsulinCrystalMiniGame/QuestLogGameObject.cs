using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using BNG;
using NUnit.Framework.Constraints;

public class QuestLogGameObject : MonoBehaviour
{
    private float lerpTime = 0.2f;
    [SerializeField] private float autoCloseDuration = 5;
    [SerializeField] float timer = 5;
    public enum MenuState { Close, Closing, Open, Opening, TemporarilyOpen };
    public MenuState currentState = MenuState.Close;

    public void Start(){
        transform.localScale = Vector3.zero;
    }

    public void Update()
    {
        //CLOSES MENU AFTER 5 SECONDS OF IT BEING OPEN
        if(currentState == MenuState.Open)
        {
            timer -= Time.deltaTime;
            if(timer < 0)
            {
                SetMenuState(false);
            }
        }

        //BUTTON INPUTS
        /*
        if (InputBridge.Instance.XButtonDown)
        {
            if(currentState == MenuState.Open)
            {
                SetMenuState(false);
            }
            else if (currentState == MenuState.Close)
            {
                SetMenuState(true);
            }
        }
        */
    }

    public void SetMenuState(bool open)
    {
        if (open)
        {
            if (currentState == MenuState.Close)
            {
                StartCoroutine(OpenMenu());
            }
        }
        else if (!open)
        {
            if (currentState == MenuState.Open)
            {
                StartCoroutine(CloseMenu());
            }
        }
    }

    IEnumerator OpenMenu()
    {
        currentState = MenuState.Opening;
        transform.DOScale(Vector3.one, lerpTime);
        yield return new WaitForSeconds(lerpTime);
        timer = autoCloseDuration;
        currentState = MenuState.Open;
    }

    IEnumerator CloseMenu()
    {
        currentState = MenuState.Closing;
        transform.DOScale(Vector3.zero, lerpTime);
        yield return new WaitForSeconds(lerpTime);
        currentState = MenuState.Close;
    }
}
