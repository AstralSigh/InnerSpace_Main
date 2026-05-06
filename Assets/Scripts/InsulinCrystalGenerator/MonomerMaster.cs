using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonomerMaster : MonoBehaviour
{
    //public InsulinSavedEventTimes _savedTimes;
    public GameObject _invertedMonomer01;
    public GameObject _dimerChild01;
    public GameObject _invertedMonomer02;
    public GameObject _dimerchild02;
    public GameObject _invertedMonomer03;
    public int _index;

    public enum bindingState { crystalizedHexamer, crystalToHex, soloHexamer, hexToDim, dimer, dimToMon, monomers}
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
                //_savedTimes.bindingEventTimes.Add(newAudioEvent);
                
                this.GetComponent<InsulinMovementSystem>()._child.Add(_invertedMonomer01);
                this.GetComponent<InsulinMovementSystem>()._child.Add(_dimerChild01);
                this.GetComponent<InsulinMovementSystem>()._child.Add(_invertedMonomer02);
                this.GetComponent<InsulinMovementSystem>()._child.Add(_dimerchild02);
                this.GetComponent<InsulinMovementSystem>()._child.Add(_invertedMonomer03);
                this.GetComponent<InsulinMovementSystem>().enabled = true;
                _timer = Random.Range(_hexamerDurationMin, _hexamerDurationMax);
                _currentbindingState = bindingState.soloHexamer;
                break;

            case bindingState.soloHexamer:
                if(_timer < 0)
                {
                    _currentbindingState = bindingState.hexToDim;
                    break;
                }                
                _timer -= 0.041f;
                break;

            case bindingState.hexToDim:
                InsulinSavedEventTimes.audioEvent newAudioEventDimer = new InsulinSavedEventTimes.audioEvent();
                newAudioEventDimer.initialize(Time.time, InsulinSavedEventTimes.audioEvent.type.dimer, this.transform.name);
                //_savedTimes.bindingEventTimes.Add(newAudioEventDimer);

                //THIS DIMER PARENT
                this.GetComponent<InsulinMovementSystem>()._child.Clear();
                this.GetComponent<InsulinMovementSystem>()._child.Add(_invertedMonomer03);              
                this.GetComponent<SphereCollider>().center = new Vector3(-0.4f, 0.0f, -1.72f);
                this.GetComponent<SphereCollider>().radius = 1.8f;
                this.GetComponent<InsulinMovementSystem>()._centerPivot = this.GetComponent<SphereCollider>().center;
                //DIMER CHILD 01
                _dimerChild01.GetComponent<InsulinMovementSystem>()._child.Add(_invertedMonomer01);
                _dimerChild01.GetComponent<InsulinMovementSystem>().enabled = true;
                _dimerChild01.GetComponent<SphereCollider>().enabled = true;

                //DIMER CHILD 02    
                _dimerchild02.GetComponent<InsulinMovementSystem>()._child.Add(_invertedMonomer02);
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
                _timer -= 0.041f;
                break;

            case bindingState.dimToMon:
                InsulinSavedEventTimes.audioEvent newAudioEventMonomer = new InsulinSavedEventTimes.audioEvent();
                newAudioEventMonomer.initialize(Time.time, InsulinSavedEventTimes.audioEvent.type.monomer, this.transform.name);
                InsulinSavedEventTimes.audioEvent newAudioEventMonomer2 = new InsulinSavedEventTimes.audioEvent();
                newAudioEventMonomer2.initialize(Time.time, InsulinSavedEventTimes.audioEvent.type.monomer, this._invertedMonomer01.name);
                InsulinSavedEventTimes.audioEvent newAudioEventMonomer3 = new InsulinSavedEventTimes.audioEvent();
                newAudioEventMonomer3.initialize(Time.time, InsulinSavedEventTimes.audioEvent.type.monomer, this._invertedMonomer02.name);

                //_savedTimes.bindingEventTimes.Add(newAudioEventMonomer);
                //_savedTimes.bindingEventTimes.Add(newAudioEventMonomer2);
                //_savedTimes.bindingEventTimes.Add(newAudioEventMonomer3);
                _dimerChild01.GetComponent<InsulinMovementSystem>()._child.Clear();
                _dimerChild01.GetComponent<SphereCollider>().center = new Vector3(-.04f, 0f, 1.5f);
                _dimerChild01.GetComponent<SphereCollider>().radius = 1.18f;
                _invertedMonomer01.GetComponent<InsulinMovementSystem>().enabled = true;
                _invertedMonomer01.GetComponent<SphereCollider>().enabled = true;

                _dimerchild02.GetComponent<InsulinMovementSystem>()._child.Clear();
                _dimerchild02.GetComponent<SphereCollider>().center = new Vector3(1.42f, 0f, -0.28f);
                _dimerchild02.GetComponent<SphereCollider>().radius = 1.18f;
                _invertedMonomer02.GetComponent<InsulinMovementSystem>().enabled = true;
                _invertedMonomer02.GetComponent<SphereCollider>().enabled = true;

                _invertedMonomer03.GetComponent<InsulinMovementSystem>().enabled = true;
                _invertedMonomer03.GetComponent<SphereCollider>().enabled = true;

                this.GetComponent<InsulinMovementSystem>()._child.Clear();
                this.GetComponent<SphereCollider>().center = new Vector3(1.42f, 0f, -1.01f);
                this.GetComponent<SphereCollider>().radius = 1.18f;

                this.GetComponent<InsulinMovementSystem>()._centerPivot = this.GetComponent<SphereCollider>().center;
                _dimerChild01.GetComponent<InsulinMovementSystem>()._centerPivot = _dimerChild01.GetComponent<SphereCollider>().center;
                _dimerchild02.GetComponent<InsulinMovementSystem>()._centerPivot = _dimerchild02.GetComponent<SphereCollider>().center;
                _invertedMonomer01.GetComponent<InsulinMovementSystem>()._centerPivot = _invertedMonomer01.GetComponent<SphereCollider>().center;
                _invertedMonomer02.GetComponent<InsulinMovementSystem>()._centerPivot = _invertedMonomer02.GetComponent<SphereCollider>().center;
                _invertedMonomer03.GetComponent<InsulinMovementSystem>()._centerPivot = _invertedMonomer03.GetComponent<SphereCollider>().center;
                _currentbindingState = bindingState.monomers;
                break;
        }   

    }




}
