using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetraTransparentParticleEmitter : MonoBehaviour
{
    [SerializeField] ParticleSystem _particleSystem;
    [SerializeField] float _particleFadeInTime = 1f;

    Material mat;
    Color finalColor;
    string colorPropId = "_FinalColor2";

    void Awake() {
        mat = _particleSystem.GetComponent<ParticleSystemRenderer>().material;
        finalColor = mat.GetColor(colorPropId);
        mat.SetColor(colorPropId, Color.clear);
    }

    public void FadeInParticles() {
        StartCoroutine(DoFadeInParticles(_particleFadeInTime));
    }

    IEnumerator DoFadeInParticles(float maxTime) {
        float time = 0;
        while (time < maxTime) {
            yield return null;
            time = Mathf.MoveTowards(time, maxTime, Time.deltaTime);
            float t = time / maxTime;
            mat.SetColor(colorPropId,Color.Lerp(Color.clear, finalColor, t));
        }
    }

    public void RemovePrewarmStopPlane() {
        _particleSystem.collision.RemovePlane(0);
    }
}
