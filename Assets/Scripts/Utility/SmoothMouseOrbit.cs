using UnityEngine;
using UnityEngine.InputSystem;

public class SmoothMouseOrbit : MonoBehaviour
{

    private Transform _XForm_Camera;
    public Transform _XForm_Parent;

    public float fMaxCamDistance = 5;

    private Vector3 _LocalRotation;
    private float _CameraDistance = 3f;

    public float fMouseSensitivity = 1f;
    public float fScrollSensitivity = 1f;
    public float fOrbitDampening = 10f;
    public float fScrollDampening = 6f;

    public bool bCameraDisabled = false;

    public bool bMouseRotation = false;

    private HorizontalControls _horizontalControlActions;
    private InputAction _orbitActionRotateH;
    private InputAction _orbitActionRotateV;
    private InputAction _zoomCamera;

    private void Awake()
    {
        _horizontalControlActions = new HorizontalControls();
    }

    private void OnEnable()
    {
        _orbitActionRotateH = _horizontalControlActions.Player.OrbitHorz;
        _orbitActionRotateH.Enable();
        _orbitActionRotateV = _horizontalControlActions.Player.OrbitVert;
        _orbitActionRotateV.Enable();
        _zoomCamera = _horizontalControlActions.Player.CamZoom;
        _zoomCamera.Enable();
    }

    private void OnDisable()
    {
        _orbitActionRotateH.Disable();
        _orbitActionRotateV.Disable();
        _zoomCamera.Disable();

    }

    void Start()
    {
        this._XForm_Camera = this.transform; //transform of camera object; make sure script is ATTACHED TO CAMERA, not parent
        //this._XForm_Parent = this.transform.parent; //from original code, comment out if setting parent in realtime
    }

    void LateUpdate()
    {
        float orbitH = _orbitActionRotateH.ReadValue<float>();
        float orbitV = _orbitActionRotateV.ReadValue<float>();
        float zoomC = _zoomCamera.ReadValue<float>();

        if (!bCameraDisabled)
        {
            if (!bMouseRotation)
            {
                //Rotation of Camera based on Axis Input
                if ( orbitH != 0 || orbitV != 0)  //change everything here to Mouse X and Mouse Y to make it mouse controls
                {
                    _LocalRotation.x += orbitH * fMouseSensitivity;
                    _LocalRotation.y -= orbitV * fMouseSensitivity;

                    //Clamp the y rotation to the horizon and full north
                    _LocalRotation.y = Mathf.Clamp(_LocalRotation.y, -60f, 60f); //Consider resetting variables or making them public
                }
            } else
            {
                
                /*
                //Rotation of Camera based on Mouse Coordinates
                if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)  //change everything here to Mouse X and Mouse Y to make it mouse controls
                {
                    _LocalRotation.x += Input.GetAxis("Mouse X") * fMouseSensitivity;
                    _LocalRotation.y -= Input.GetAxis("Mouse Y") * fMouseSensitivity;

                    //Clamp the y rotation to the horizon and full north
                    _LocalRotation.y = Mathf.Clamp(_LocalRotation.y, -70f, 70f); //Consider resetting variables or making them public
                }
                */
            }
            
            /*
            //Zooming input from Mouse Scroll Input
            if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                float fScrollAmount = Input.GetAxis("Mouse ScrollWheel") * fScrollSensitivity;

                //Make camera zoom faster the further away it is from the target
                fScrollAmount *= (this._CameraDistance * 0.3f);

                this._CameraDistance += fScrollAmount * -1f;

                //Clamp min and max distance away from target
                this._CameraDistance = Mathf.Clamp(this._CameraDistance, 0.25f, fMaxCamDistance); //Also consider making these public?
            }
            */

            //Zooming input from Key Input
            if (zoomC != 0)
            {
                float fScrollAmount = zoomC * fScrollSensitivity;

                //Make camera zoom faster the further away it is from the target
                fScrollAmount *= (this._CameraDistance * 0.3f);

                this._CameraDistance += fScrollAmount * -1f;

                //Clamp min and max distance away from target
                this._CameraDistance = Mathf.Clamp(this._CameraDistance, 0.25f, fMaxCamDistance); //Also consider making these public?
            }

            //Actual Camera Rig Transformations
            Quaternion QT = Quaternion.Euler(_LocalRotation.y, _LocalRotation.x, 0);
            this._XForm_Parent.rotation = Quaternion.Lerp(this._XForm_Parent.rotation, QT, Time.deltaTime * fOrbitDampening);

            if (this._XForm_Camera.localPosition.z != this._CameraDistance * -1f)
            {
                this._XForm_Camera.localPosition = new Vector3(0f, 0f, Mathf.Lerp(this._XForm_Camera.localPosition.z, this._CameraDistance * -1f, Time.deltaTime * fScrollDampening));
            }
        }

    }

    public void ResetTransform()
    {
        _XForm_Camera.localRotation = Quaternion.identity;
        _XForm_Camera.localPosition = Vector3.zero;
    }
}
