using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoNodeMenuManager : MonoBehaviour
{
    enum direction { north, south, east, west };
    [SerializeField] direction _spawnDirection = direction.east;
    [SerializeField] private GameObject _nodeSlotPrefab;
    [SerializeField] private float _radius = 0.5f;
    [SerializeField] Transform menuContainer;
    int slotCount;
    GameObject[] _menuItems = new GameObject[0];
    List<string> letters = new List<string> { "a", "b", "c", "d", "e", "f", "g" };

    void SetupMenu(int slotCount)
    {
        // Delete any previous info node menu items.
        foreach(GameObject infoNodeMenuItem in _menuItems)
        {
            Destroy(infoNodeMenuItem);
        }

        _menuItems = new GameObject[slotCount];

        for (int x = 0; x < _menuItems.Length; x++)
        {
            float angle = 0;
            if(_menuItems.Length > 1)
            {
                angle = 2 * Mathf.PI * ((float)x / (_menuItems.Length - 1));
            }
                  
            float targetAngle = angle / 2;

            switch (_spawnDirection)
            {
                case direction.south:
                    break;

                case direction.west:
                    targetAngle += Mathf.PI / 2f;
                    break;

                case direction.north:
                    targetAngle += Mathf.PI;
                    break;

                case direction.east:
                    targetAngle += Mathf.PI * (3f / 2f);
                    break;
            }

            string nodeLetter = letters[x];
            
            //Create slots
            Vector3 targetOffset = new Vector3(Mathf.Cos(targetAngle) * _radius, 0, Mathf.Sin(targetAngle) * _radius);
            _menuItems[x] = Instantiate(_nodeSlotPrefab, this.transform.localPosition + targetOffset, Quaternion.Euler(180, 0, 180));
            _menuItems[x].transform.SetParent(menuContainer, false);

            //Get targetNode 
            InfoNodeHeadV2 targetInfoNode = InfoNodeManager.Instance.GetInfoNodesByCons(SelectedConstituentManager.Instance.GetCurrentData())[x]; 

            //Set up slot
            _menuItems[x].transform.GetComponent<InfoNodeSlot_DEPRECATED>().initialize(nodeLetter, targetInfoNode);
            _menuItems[x].name = "Teleport Slot " + x;

            _menuItems[x].SetActive(true);
        }
    }
}
