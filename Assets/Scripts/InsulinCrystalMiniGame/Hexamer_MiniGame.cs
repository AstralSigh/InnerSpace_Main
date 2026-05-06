using BNG;
using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Hexamer_MiniGame : MonoBehaviour
{
    public enum HexamerColor { BW, Desaturated, Normal }
    public enum HexamerState { Idle, Alligning, Lerping, Placed, Detaching }
    public HexamerState currentHexamerState = HexamerState.Idle;

    [Tooltip("The time it takes for hexamer to rotate into slot.")]
    [SerializeField] private float lerpTime = .5f;
    [SerializeField] private MeshRenderer[] monomers;
    [SerializeField] private MeshRenderer[] zincs;
    [SerializeField] private HexamerFormationColorProfiles colorProfile;
    private bool previousState = false;

    private void OnEnable()
    {
        SetColor(HexamerColor.BW);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boundry"))
        {
            transform.parent.GetComponent<Rigidbody>().velocity = Vector3.zero;
            transform.parent.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            CrystalManager.Instance.AddToBoundaryCount();
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Slot") && !other.GetComponent<Slot>().IsFilled() && other.GetComponent<Slot>().currentSlotState == Slot.SlotState.Available)
        {
            switch (currentHexamerState)
            {
                case HexamerState.Idle:
                    currentHexamerState = HexamerState.Alligning;
                    break;

                case HexamerState.Alligning:
                    if (!transform.parent.GetComponent<Grabbable>().BeingHeld)
                    {
                        StartCoroutine(PlaceHexamer(other.transform));
                    }
                    break;
            }
        }
    }

    public void Update()
    {   
        if (currentHexamerState != HexamerState.Placed)
        {
            PlayGrabReleaseSounds();
        }
    }


    //TODO: REMOVE THIS AND ADD THIS TO A DIFFERENT MANAGER. 
    public void PlayGrabReleaseSounds()
    {
        bool currentState = transform.parent.GetComponent<Grabbable>().BeingHeld;

        //TO DO CHANGE THIS LATER
        if (currentState != previousState)
        {
            //If held
            if (currentState)
            {
                FMODUnity.RuntimeManager.PlayOneShot("event:/UI Events/grab", this.transform.position);
            }
            //If released
            else
            {
                FMODUnity.RuntimeManager.PlayOneShot("event:/UI Events/text_close", this.transform.position);
            }
            previousState = currentState;
        }
    }

        IEnumerator PlaceHexamer(Transform targetSlot)
    {
        currentHexamerState = HexamerState.Lerping;
        SetGrabbable(false);
        transform.parent.DOMove(targetSlot.transform.position, lerpTime);
        transform.parent.DORotate(GetClosestRotation(transform.parent.rotation, targetSlot).eulerAngles, lerpTime);
        yield return new WaitForSeconds(lerpTime);
        targetSlot.GetComponent<Slot>().AttachHexamer(this);
        transform.parent.transform.SetParent(targetSlot.transform);
        currentHexamerState = HexamerState.Placed;
    } 

    public void SetColor(HexamerColor hexamerColor)
    {
        switch (hexamerColor)
        {
            case HexamerColor.Normal:
                foreach(MeshRenderer m in monomers)
                {
                    m.SetMaterials(colorProfile.hexamer3);
                }
                break;
            case HexamerColor.Desaturated:
                foreach (MeshRenderer m in monomers)
                {
                    m.SetMaterials(colorProfile.hexamer2);
                }
                break;
            case HexamerColor.BW:
                foreach (MeshRenderer m in monomers)
                {
                    m.SetMaterials(colorProfile.hexamer1);
                }
                break;
        }
    }

    public void DetatchHexamer()
    {
        SetColor(HexamerColor.Normal);
        StartCoroutine(DetachSequence());
    }

    IEnumerator DetachSequence()
    {
        currentHexamerState = HexamerState.Detaching;
        SetGrabbable(true);
        transform.parent.GetComponent<Rigidbody>().AddForceAtPosition(new Vector3(30f, 30f, 30f), transform.parent.position);
        float time = 0; 
        while(time < 2)
        {
            time += Time.deltaTime;
            if(transform.parent.GetComponent<Grabbable>().BeingHeld == true)
            {
                time = 2;
            }
            yield return null;
        }
        currentHexamerState = HexamerState.Idle;
        transform.parent.GetComponent<Rigidbody>().velocity = Vector3.zero;
        transform.parent.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }

    public void SetGrabbable(bool state)
    {
        //transform.GetComponent<BoxCollider>().enabled = state;
        transform.parent.GetComponent<BNG.Grabbable>().enabled = state;
        transform.parent.GetComponent<Rigidbody>().isKinematic = !state;
    }

    private Quaternion GetClosestRotation(Quaternion currentRotation, Transform targetSlot)
    {
        // Define the target rotations
        Quaternion targetRotation1 = targetSlot.rotation * Quaternion.Euler(0f, 0f, 0f);
        Quaternion targetRotation2 = targetSlot.rotation * Quaternion.Euler(0f, 120f, 0f);
        Quaternion targetRotation3 = targetSlot.rotation * Quaternion.Euler(0f, 240f, 0f);
        Quaternion targetRotation4 = targetSlot.rotation * Quaternion.Euler(0f, 360f, 0f);

        // Calculate the differences between current rotation and target rotations
        float diff1 = Quaternion.Angle(targetRotation1, currentRotation);
        float diff2 = Quaternion.Angle(targetRotation2, currentRotation);
        float diff3 = Quaternion.Angle(targetRotation3, currentRotation);
        float diff4 = Quaternion.Angle(targetRotation4, currentRotation);

        // Determine which target rotation it is closest to
        if (diff1 <= diff2 && diff1 <= diff3 && diff1 <= diff4)
            return targetRotation1;
        else if (diff2 <= diff3 && diff2 <= diff4)
            return targetRotation2;
        else if (diff3 <= diff4)
            return targetRotation3;
        else
            return targetRotation4;
    }
}
