using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCircleRenderer : MonoBehaviour
{
    public LineRenderer circleRenderer;

    public int renderSteps = 100;

    public float renderRadius = 1;


    void Start()
    {
        DrawCircle(renderSteps, renderRadius);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DrawCircle(int steps, float radius)
    {
        circleRenderer.positionCount = steps;

        for (int currentStep = 0; currentStep < steps; currentStep++)
        {
            float circumferenceProgress = (float)currentStep / steps;

            float currentRadian = circumferenceProgress * 2 * Mathf.PI;

            float xScaled = Mathf.Cos(currentRadian);
            float yScaled = Mathf.Sin(currentRadian);

            float x = xScaled * radius;
            float y = yScaled * radius;

            Vector3 currentPostion = new Vector3(x, y, 0);

            circleRenderer.SetPosition(currentStep, currentPostion);
        }

    }
}
