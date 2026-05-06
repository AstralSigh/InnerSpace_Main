using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InsulinSavedEventTimes", menuName = "ScriptableObjects/InsulinSavedEventTimes")]
public class InsulinSavedEventTimes : ScriptableObject
{
    [System.Serializable]
    public class audioEvent
    {
    public enum type { monomer, dimer, hexamer, unassigned }
    public float _time;
    public type _bindingType = type.unassigned;
    public string _locationName;

        public void initialize(float time, type bindingType, string location)
        {
            _time = time;
            _bindingType = bindingType;
            _locationName = location;
        }
    }

    public List<audioEvent> bindingEventTimes = new List<audioEvent>();
    public float _animationDuration;
    
}
