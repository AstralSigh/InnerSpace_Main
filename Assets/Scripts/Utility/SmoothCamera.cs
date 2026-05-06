using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNG;

public class SmoothCamera : MonoBehaviour
{
    [SerializeField]
    GameObject _target;
    [SerializeField]
    bool _smoothRotation = true;
    [SerializeField]
    bool _lockZAxis = true;
    [SerializeField]
    bool _freezeCamera = false;
    Quaternion _targetRotation;   
    public float smoothTime = 0.3F;
    public float rotateTime = 0.3f;
    private Vector3 velocity = Vector3.zero;

    private ControllerBinding bButton = ControllerBinding.BButton;

    private void Start()
    {

    }

    void Update()
    {
        if (!_freezeCamera)
        {
            lerpToDestination();
        }

        if (bButton.GetDown())
        {
            _freezeCamera = !_freezeCamera;
        }
    }


    void lerpToDestination()
    {
        transform.position = Vector3.SmoothDamp(transform.localPosition, _target.transform.position, ref velocity, smoothTime);

        if (_smoothRotation)
        {
            _targetRotation = _target.transform.rotation;
            Quaternion lerpTarget = Quaternion.Lerp(transform.rotation, _targetRotation, Time.time * rotateTime);
            if (_lockZAxis)
            {
                Quaternion tempTarget = Quaternion.identity;
                tempTarget.eulerAngles = new Vector3(lerpTarget.eulerAngles.x, lerpTarget.eulerAngles.y, 0);
                transform.rotation = tempTarget;
            }
            else
            {
                transform.rotation = lerpTarget;
            }
        } 
    }
    

}
