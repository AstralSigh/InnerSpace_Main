using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuToggle : MonoBehaviour
{
    // TODO: DELETE THIS CLASS.
    public enum menuState { open, closed, transitioning};
    [SerializeField] public menuState _currentState = menuState.closed;
    [Tooltip("Menu items that will collapse and condense")]
    [SerializeField] public GameObject[] _menuItems;
    [Tooltip("Time it takes for items to collapse and condense")]
    [SerializeField] public float _menuToggleDuration = .25f;
    [Tooltip("_menuItems are set up based on what is in the inspector")]
    [SerializeField] bool _setupThroughInspector;
    [SerializeField] bool _automaticallyToggleNextButton;
    //STORED VARIABLES
    private GameObject[] _placeHolderButtons;

    //Debug Bool
    public bool _toggleMenuItem = false;

    void Awake()
    {
        if (_setupThroughInspector)
        {
            setupMenu();
        }
    }

    private void Update()
    {
        if (_toggleMenuItem) //Inspector Debug
        {
            ToggleMenuItems();
            _toggleMenuItem = false;
        }
    }

    public void setupMenu()
    {
        _placeHolderButtons = new GameObject[_menuItems.Length];
        switch (_currentState)
        {

            case menuState.open:
                for (int x = 0; x < _menuItems.Length; x++)
                {
                    _placeHolderButtons[x] = Instantiate(_menuItems[x], _menuItems[x].transform.position, _menuItems[x].transform.rotation);
                    _placeHolderButtons[x].transform.SetParent(this.transform, false);
                    _placeHolderButtons[x].SetActive(false);
                    _placeHolderButtons[x].TryGetComponent<SphereCollider>(out SphereCollider collider);
                    Destroy(collider);

                    MonoBehaviour[] scripts = _placeHolderButtons[x].GetComponents<MonoBehaviour>(); 
                    for(int y = 0; y < scripts.Length; y++)
                    {                        
                        Destroy(scripts[y]);
                    }
                    _menuItems[x].SetActive(true);
                }
                break;

            case menuState.closed:
                for (int x = 0; x < _menuItems.Length; x++)
                {
                    _placeHolderButtons[x] = Instantiate(_menuItems[x], this.transform.position, this.transform.rotation);
                    _placeHolderButtons[x].transform.SetParent(this.transform, false);
                    _placeHolderButtons[x].SetActive(false);
                    _placeHolderButtons[x].TryGetComponent<SphereCollider>(out SphereCollider collider);
                    Destroy(collider);

                    MonoBehaviour[] scripts = _placeHolderButtons[x].GetComponents<MonoBehaviour>();
                    for (int y = 0; y < scripts.Length; y++)
                    {
                        Destroy(scripts[y]);
                    }
                    _menuItems[x].SetActive(false);
                }
                break;
        }
    }

    public void ToggleMenuItems()
    {
        switch (_currentState)
        {
            case menuState.closed:
                StartCoroutine(openMenu());
                _currentState = menuState.transitioning;
                //this.GetComponent<SphereCollider>().enabled = false;
                break;

            case menuState.open:
                StartCoroutine(closeMenu());
                _currentState = menuState.transitioning;
                //this.GetComponent<SphereCollider>().enabled = false;
                break;
        }
    }

    public void ToggleOpen()
    {
        if(_currentState != menuState.open)
        {
            StartCoroutine(openMenu());
            _currentState = menuState.transitioning;
        }
    }

    public void ToggleClose()
    {
        if (_currentState != menuState.closed)
        {
            StartCoroutine(closeMenu());
            _currentState = menuState.transitioning;
        }
    }

    public IEnumerator openMenu()
    {
        for(int x = 0; x < _menuItems.Length; x++)
        {
            _menuItems[x].SetActive(false);
            StartCoroutine(lerpAnimation(_placeHolderButtons[x], this.transform.position, _menuItems[x].transform.position));
        }
        yield return new WaitForSeconds(_menuToggleDuration);

        for(int x = 0; x < _menuItems.Length; x++)
        {
            _menuItems[x].SetActive(true);
            
            if (_automaticallyToggleNextButton)
            {
                if (_menuItems[x].GetComponent<MenuToggle>()._currentState == menuState.closed)
                {
                    _menuItems[x].GetComponent<MenuToggle>().ToggleMenuItems();
                    yield return new WaitForSeconds(_menuToggleDuration);
                }
            }
        }
        _currentState = menuState.open;
        //this.GetComponent<SphereCollider>().enabled = true;
    }

    public List<int> checkOpenStatesInMenuItems()
    {
        List<int> indexWithChildren = new List<int>();

        for (int x = 0; x < _menuItems.Length; x++)
        {
            if(_menuItems[x].GetComponent<MenuToggle>() != null)
            {
                if(_menuItems[x].GetComponent<MenuToggle>()._currentState == menuState.open)
                {
                    indexWithChildren.Add(x);
                }
            }
        }
        return indexWithChildren;
    }

    public IEnumerator closeMenu()
    {
        List<int> theseItems = checkOpenStatesInMenuItems();
        if (theseItems.Count > 0)
        {
            for (int x = 0; x < theseItems.Count; x++)
            {
                List<int> theseItemsItems = _menuItems[theseItems[x]].GetComponent<MenuToggle>().checkOpenStatesInMenuItems();
                if (theseItemsItems.Count > 0)
                {
                    for (int y = 0; y < theseItemsItems.Count; y++)
                    {
                        _menuItems[theseItems[x]].GetComponent<MenuToggle>()._menuItems[theseItemsItems[y]].GetComponent<MenuToggle>().ToggleMenuItems();
                        float theseItemsItemsDuration = _menuItems[theseItems[x]].GetComponent<MenuToggle>()._menuItems[theseItemsItems[y]].GetComponent<MenuToggle>()._menuToggleDuration;
                        yield return new WaitForSeconds(theseItemsItemsDuration);
                    }
                }
                _menuItems[theseItems[x]].GetComponent<MenuToggle>().ToggleMenuItems();
                float theseItemsDuration = _menuItems[theseItems[x]].GetComponent<MenuToggle>()._menuToggleDuration;
                yield return new WaitForSeconds(_menuToggleDuration);
            }
        }

        for (int x = 0; x < _menuItems.Length; x++)
        {
            _menuItems[x].TryGetComponent<BasicButton>(out BasicButton button);
            if(button != null)
            {
                //button.forceExit();
            }
            _menuItems[x].SetActive(false);
            StartCoroutine(lerpAnimation(_placeHolderButtons[x], _menuItems[x].transform.position, this.transform.position));
        }
        yield return new WaitForSeconds(_menuToggleDuration);

        _currentState = menuState.closed;
        //this.GetComponent<SphereCollider>().enabled = true;
    }

    public IEnumerator lerpAnimation(GameObject lerpedItem, Vector3 start, Vector3 end)
    {
        lerpedItem.SetActive(true);
        float timer = 0;
        while (timer < _menuToggleDuration)
        {
            float lerpedTime = timer / _menuToggleDuration;
            lerpedItem.transform.position = Vector3.Lerp(start, end, lerpedTime);
            timer += Time.deltaTime;
            yield return null;
        }
        lerpedItem.SetActive(false);
    }
}
