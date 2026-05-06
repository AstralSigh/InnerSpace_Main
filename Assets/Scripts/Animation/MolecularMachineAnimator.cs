using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using NUnit.Framework;
using System.Linq;
//using UnityEditor.ShaderGraph.Internal;
using System.Threading;
using System.Runtime.CompilerServices;

public class MolecularMachineAnimator : MonoBehaviour
{
    public class Tetra
    {
        Vector3 positionOffset;
        GameObject mesh;
        float lerpTime;
        private Vector3 prePose;
        private Vector3 pose;
        private Vector3 postPose;

        public Tetra(Vector3 positionOffest, GameObject mesh, float lerpTime)
        {
            this.positionOffset = positionOffest;
            this.mesh = mesh;
            this.lerpTime = lerpTime;

            AssignPoses();
            SetPrePose();
        }

        public void AssignPoses()
        {
            pose = mesh.transform.position;
            prePose = mesh.transform.position + new Vector3(Random.Range(-positionOffset.x, positionOffset.x), Random.Range(-positionOffset.y, positionOffset.y), Random.Range(-positionOffset.z, positionOffset.z));
            postPose = mesh.transform.position + new Vector3(Random.Range(-positionOffset.x, positionOffset.x), Random.Range(-positionOffset.y, positionOffset.y), Random.Range(-positionOffset.z, positionOffset.z));
        }

        public void SetPrePose()
        {
            mesh.transform.position = prePose;
            mesh.SetActive(false);
        }

        public void LerpToPose()
        {
            mesh.SetActive(true);
            mesh.transform.DOMove(pose, lerpTime);
        }

        public void LerpToPostPose()
        {
            mesh.transform.DOMove(postPose, lerpTime);
            mesh.transform.DOScale(Vector3.zero, lerpTime);
        }
    }

    [Tooltip("Plugin all the individual meshes for this animation")]
    [SerializeField] private List<GameObject> meshes;
    [Tooltip("How far too offset the random positions for pre-post and post-pose")]
    [SerializeField] private Vector3 positionOffset;
    [Tooltip("How long it takes for object to lerp from pre-pose to pose and pose to post-pose")]
    [SerializeField] private float lerpTime;
    [Tooltip("How long the objects stay at pose")]
    [SerializeField] private float poseDuration;
    private List<Tetra> tetraList;
    private List<float> timeList;
    [Tooltip("How long objects should spawn at pre-pose before lerping to pose")]
    [SerializeField] private float duration;

    public void Start()
    {
        InstantiateList();
        CalculateTime();
        StartCoroutine(RunAnimation());
    }

    public void InstantiateList()
    {
        tetraList = new List<Tetra>();

        foreach(GameObject m in meshes)
        {
            tetraList.Add(new Tetra(positionOffset, m, lerpTime));
        }

        tetraList = tetraList.OrderBy(x => Random.value).ToList();
    }

    IEnumerator RunAnimation()
    {
        int index = 0;
        foreach(Tetra t in tetraList)
        {
            t.LerpToPose();
            yield return new WaitForSeconds(timeList[index]);
            index++;
        }

        yield return new WaitForSeconds(poseDuration + lerpTime);

        foreach(Tetra t in tetraList)
        {
            t.LerpToPostPose();
        }
    }

    public void CalculateTime()
    {
        if (tetraList.Count <= 1) return;

        timeList = new List<float>();

        float averageTime = duration / tetraList.Count;   

        for (int x = 0; x < tetraList.Count; x++)
        {
            timeList.Add(averageTime);
        }
        for(int x = 0; x < (tetraList.Count /2) -1; x+=1)
        {
            float randomValue = Random.Range(-averageTime, averageTime);
            timeList[x] += randomValue;
            timeList[x+1] -= randomValue;
        }
    }
   
}
