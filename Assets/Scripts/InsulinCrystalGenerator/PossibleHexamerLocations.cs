using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "PossibleHexamerLocations", menuName = "ScriptableObjects/PossibleHexamerLocations")]

public class PossibleHexamerLocations : ScriptableObject
{
    public List<Vector3> possibleHexamerLocations = new List<Vector3>();

    public List<Vector3> sortedList = new List<Vector3>();
    public string newList;
    //public string toRecreateInBlender;
    public int locationCount = 0;
    public int listIndex = 0;
    public List<string> toRecreateInBlender;


    public void exportToBlender()
    {
        newList = "hexamerList = [";
        for (int x = 98; x < 300; x++)
        { 
            newList += possibleHexamerLocations[x] + ",";
        }
        newList += "]";
        /*
        toRecreateInBlender.Clear();
        toRecreateInBlender.Add("hexamerList = [");
            for (int x = 0; x < possibleHexamerLocations.Count; x++)
            {

                if(locationCount > 2000)
                {
                    toRecreateInBlender[listIndex] += "]";
                    listIndex++;
                    locationCount = 0;
                    toRecreateInBlender.Add("hexamerList = [");
                }

                toRecreateInBlender[listIndex] += "(" + -possibleHexamerLocations[x].x + "," + possibleHexamerLocations[x].z + "," + possibleHexamerLocations[x].y + ")";

                if (locationCount < 2000)
                {
                    toRecreateInBlender[listIndex] += ", ";
                }
                locationCount++;

            }
            */
    }
}

