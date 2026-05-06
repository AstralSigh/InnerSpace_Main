using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ui_bus : MonoBehaviour
{
    FMOD.Studio.Bus uiBus;
    [SerializeField] //[Range(-80f, 10f)]
    private float busVolume;

    // Start is called before the first frame update
    void Start()
    {
        uiBus = FMODUnity.RuntimeManager.GetBus("bus:/ UI Bus");
    }

    // Update is called once per frame
    void Update()
    {
        //busVolume.setVolume(DecibelToLinear(busVolume));
    }

    private float DecibelToLinear(float db)
    {
        float linear = Mathf.Pow(10.0f, db / 20f);
        return linear;
    }
}
