using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DimerMaster : MonoBehaviour
{
    public InsulinSavedEventTimes _savedTimes;
    public GameObject _dimerChild01;
    public GameObject _dimerchild02;
    public int _index;

    public enum bindingState { crystalizedHexamer, crystalToHex, soloHexamer, hexToDim, dimer, dimToMon, monomers }
    public bindingState _currentbindingState = bindingState.crystalizedHexamer;

    [SerializeField]
    private float _timer = 0;
    [SerializeField]
    private float _hexamerDurationMin = 1;
    [SerializeField]
    private float _hexamerDurationMax = 3;
    [SerializeField]
    private float _dimerDurationMin = 3;
    [SerializeField]
    private float _dimerDurationMax = 6;

    public void Update()
    {
        switch (_currentbindingState)
        {
            case bindingState.crystalToHex:
                InsulinSavedEventTimes.audioEvent newAudioEvent = new InsulinSavedEventTimes.audioEvent();
                newAudioEvent.initialize(Time.time, InsulinSavedEventTimes.audioEvent.type.hexamer, this.transform.name);
                _savedTimes.bindingEventTimes.Add(newAudioEvent);

                this.GetComponent<InsulinMovementSystem>()._child.Add(_dimerChild01);
                this.GetComponent<InsulinMovementSystem>()._child.Add(_dimerchild02);
                this.GetComponent<InsulinMovementSystem>().enabled = true;
                _timer = Random.Range(_hexamerDurationMin, _hexamerDurationMax);
                _currentbindingState = bindingState.soloHexamer;
                break;

            case bindingState.soloHexamer:
                if (_timer < 0)
                {
                    _currentbindingState = bindingState.hexToDim;
                    break;
                }
                _timer -= Time.deltaTime;
                break;

            case bindingState.hexToDim:
                InsulinSavedEventTimes.audioEvent newAudioEventDimer = new InsulinSavedEventTimes.audioEvent();
                newAudioEventDimer.initialize(Time.time, InsulinSavedEventTimes.audioEvent.type.dimer, this.transform.name);
                _savedTimes.bindingEventTimes.Add(newAudioEventDimer);

                //THIS DIMER PARENT
                this.GetComponent<InsulinMovementSystem>()._child.Clear();
                this.GetComponent<SphereCollider>().center = new Vector3(-1.1f, 0.01f, 0.9f);
                this.GetComponent<SphereCollider>().radius = 1.6f;
                this.GetComponent<InsulinMovementSystem>()._centerPivot = this.GetComponent<SphereCollider>().center;
                //DIMER CHILD 01
                _dimerChild01.GetComponent<InsulinMovementSystem>().enabled = true;
                _dimerChild01.GetComponent<SphereCollider>().enabled = true;

                //DIMER CHILD 02    
                _dimerchild02.GetComponent<InsulinMovementSystem>().enabled = true;
                _dimerchild02.GetComponent<SphereCollider>().enabled = true;

                this.GetComponent<InsulinMovementSystem>()._centerPivot = this.GetComponent<SphereCollider>().center;
                _dimerChild01.GetComponent<InsulinMovementSystem>()._centerPivot = _dimerChild01.GetComponent<SphereCollider>().center;
                _dimerchild02.GetComponent<InsulinMovementSystem>()._centerPivot = _dimerchild02.GetComponent<SphereCollider>().center;
                _timer = Random.Range(_dimerDurationMin, _dimerDurationMax);
                _currentbindingState = bindingState.dimer;
                break;

            case bindingState.dimer:
                if (_timer < 0)
                {
                    _currentbindingState = bindingState.dimToMon;
                    break;
                }
                _timer -= Time.deltaTime;
                break;
        }

    }

}
