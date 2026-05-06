using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneHeader : MonoBehaviour
{
    [SerializeField] private GameObject headerText;
    [SerializeField] private GameObject subHeaderText;
    [SerializeField] private GameObject dividerBarImage;

    private void OnEnable()
    {
        DisplayHeader();
    }

    void DisplayHeader()
    {
        //Show Header
        iTween.ScaleTo(headerText, iTween.Hash("scale", Vector3.one, "time", 2.0f, "easetype", iTween.EaseType.easeInSine));
        //Animate Bar
        iTween.ScaleTo(dividerBarImage, iTween.Hash("scale", Vector3.one, "time", 2.0f, "easetype", iTween.EaseType.easeInSine, "delay", 6.0f));
        //Show sub header
        iTween.ScaleTo(subHeaderText, iTween.Hash("scale", Vector3.one, "time", 2.0f, "easetype", iTween.EaseType.easeInSine, "delay", 3.0f));
        //Revert and disable header items
        StartCoroutine(RevertHeader());
    }

    IEnumerator RevertHeader()
    {

        yield return new WaitForSeconds(8f);

        iTween.ScaleTo(headerText, iTween.Hash("scale", new Vector3(0,1,1), "time", 1.0f, "easetype", iTween.EaseType.easeOutExpo));
        iTween.ScaleTo(dividerBarImage, iTween.Hash("scale", new Vector3(0, 1.0f, 1.0f), "time", 1.0f, "easetype", iTween.EaseType.easeOutExpo));
        iTween.ScaleTo(subHeaderText, iTween.Hash("scale", new Vector3(0, 1, 1), "time", 1.0f, "easetype", iTween.EaseType.easeOutExpo));

        StartCoroutine(DisableHeader());
    }

    IEnumerator DisableHeader()
    {

        yield return new WaitForSeconds(2.0f);

        this.gameObject.SetActive(false);
    }

    public void SetHeaderText(string header)
    {
        headerText.GetComponent<Text>().text = header;
    }

    public void SetSubHeaderText(string subHeader)
    {
        subHeaderText.GetComponent<Text>().text = subHeader;
    }
}
