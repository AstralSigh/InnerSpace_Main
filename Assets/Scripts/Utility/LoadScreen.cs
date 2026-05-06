using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadScreen : MonoBehaviour
{
    [SerializeField] private Image progressBar;

    // Start is called before the first frame update
    void Update()
    {
        transform.position = Camera.main.transform.position + Camera.main.transform.forward;
        transform.forward = (Camera.main.transform.position - transform.position).normalized;
    }

    private void Start()
    {
        StartCoroutine(LoadSceneAsyn());
    }
    IEnumerator LoadSceneAsyn()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(LoadingData.sceneToLoad);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            progressBar.fillAmount = operation.progress * 1.11f;
            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
