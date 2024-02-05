using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class MenuNavigate : MonoBehaviour
{
    [SerializeField] private MainMenu _menu;
    [SerializeField] private PauseMenu _pauseMenu;

    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnNavigate(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        if (GameManager.Instance._eventSystem.sendNavigationEvents == true)
        {
            return;
        }

        // On récupère le focused element
        Focusable focusedElement = GameManager.Instance._currentUIDocument.rootVisualElement.focusController.focusedElement;

        // Si on va à droite ou à gauche
        if (context.ReadValue<Vector2>().x != 0)
        {
            Debug.Log("Navigate horizontal from MenuNavigate");
            if (focusedElement != null)
            {
                if (focusedElement is SliderInt)
                {
                    SliderInt slider = (SliderInt)focusedElement;
                    slider.value += (int)context.ReadValue<Vector2>().x;
                }
            }
        }

        if (context.ReadValue<Vector2>().y != 0)
        {
            Debug.Log("Movement vertical from MenuNavigate");

            if (focusedElement != null)
            {
                Debug.Log("Focused element != null");
                /*if (focusedElement is VisualElement)
                {*/
                    Debug.Log("FocusedElement est un VisualElement");

                    // Find all direct child focusable element of the parent of the focused element
                    List<VisualElement> VisualElementsArray = new List<VisualElement>();
                    foreach (VisualElement v in (focusedElement as VisualElement).parent.Children())
                    {
                        if (v.focusable == true)
                        {
                            VisualElementsArray.Add(v);
                        }
                    }
                    // var VisualElementsArray = (focusedElement as VisualElement).parent.Query(" > *").Where(b => b.focusable == true).ToList();

                    int currentFocusedElementIndex = 0;

                    int i = 0;
                    foreach (VisualElement v in VisualElementsArray)
                    {
                        if (v == focusedElement)
                        {
                            currentFocusedElementIndex = i;
                        }

                        i++;
                    }

                    Debug.Log("Current focused element index : " + currentFocusedElementIndex);

                    int nextIndex = (context.ReadValue<Vector2>().y < 0) ? 
                        currentFocusedElementIndex + 1 :
                        currentFocusedElementIndex - 1 ;

                    if (nextIndex < 0)
                    {
                        nextIndex = VisualElementsArray.Count - 1;
                    } else if (nextIndex >= VisualElementsArray.Count)
                    {
                        nextIndex = 0;
                    }

                    Debug.Log("Next index : " + nextIndex);
                    Debug.Log("VisualElementsArray.Count : " + VisualElementsArray.Count);
                    Debug.Log("VisualElementsArray[nextIndex] : " + VisualElementsArray[nextIndex]);

                    if (VisualElementsArray[nextIndex] != null)
                    {
                        VisualElementsArray[nextIndex].Focus();
                    }
                // }
            }
        }
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        // On récupère le focused element
        Focusable focusedElement = GameManager.Instance._currentUIDocument.rootVisualElement.focusController.focusedElement;
        Debug.Log("FOCUSED ELEMENT : " + focusedElement);
        // On envoie un clikckevent sur le bouton (si jamais, ça ferme le Dropdown)
        Debug.Log("Cancel event on Dropdown : " + focusedElement);

        DropdownField dropdown = focusedElement as DropdownField;
        
        /*using (var e = new NavigationSubmitEvent() { target = focusedElement })
        {
            Debug.Log("Submit event on button : " + focusedElement);
            // Submit focusedElement
            focusedElement.SendEvent(e);
        }

        using (var e = new NavigationSubmitEvent() { target = focusedElement })
        {
            Debug.Log("Submit event on button : " + focusedElement);
            // Submit focusedElement
            focusedElement.SendEvent(e);
        }*/

        // Si on est pas dans un tab
        if (_menu._currentTab == null || _menu._currentTab == "")
        {
            if (_pauseMenu != null)
            {
                Debug.Log("No panel loaded, we leave pause");

                _pauseMenu.OnResume();
            }
            else
            {
                if (_menu._currentPanel != "MainPanel")
                {
                    Debug.Log("No panel loaded, we load main panel");
                    _menu.LoadPanel("MainPanel");
                } else
                {
                    Debug.Log("No panel loaded (and we are on Main), we quit the game");
                    // On quitte l'application
                    Application.Quit();
                }
            }
        } else
        {
            _menu.UnLoadTab();
        }
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        if (GameManager.Instance._currentUIDocument == null || !context.performed) {
            return;
        }

        Debug.Log("Submit from MenuNavigate");
        // Find the element with active class
        Focusable focusedElement = GameManager.Instance._currentUIDocument.rootVisualElement.focusController.focusedElement;

        if (focusedElement == null)
        {
            return;
        }
        UnityEngine.UIElements.Button focusedButton = null;

        if (focusedElement is DropdownField)
        {
            DropdownField targetDropdown = (DropdownField)focusedElement;

            using (var e = new NavigationSubmitEvent() { target = targetDropdown })
            {
                Debug.Log("Submit event on Dropdown : " + targetDropdown);
                // Submit focusedElement
                targetDropdown.SendEvent(e);
            }

            return;
        }
        else if (focusedElement is Toggle)
        {
            Toggle targetToggle = (Toggle)focusedElement;

            using (var e = new NavigationSubmitEvent() { target = targetToggle })
            {
                Debug.Log("Submit event on Toggle : " + targetToggle);
                // Submit focusedElement
                targetToggle.SendEvent(e);
            }

            return;
        }
        else if (focusedElement is VisualElement)
        {
            // On récupère le bouton child
            VisualElement visualElement = (VisualElement)focusedElement;
            UnityEngine.UIElements.Button button = visualElement.Q<UnityEngine.UIElements.Button>();
            if (button != null)
            {
                focusedButton = button;
            }
        }
        else if (focusedElement is UnityEngine.UIElements.Button)
        {
            focusedButton = (UnityEngine.UIElements.Button)focusedElement;
        }

        if (focusedButton == null)
        {
            Debug.Log("No button found");
            return;
        }

        // focusedButton.clickable.clicked += () => Debug.Log("Clicked on button : " + focusedButton);   

        using (var e = new NavigationSubmitEvent() { target = focusedButton })
        {
            Debug.Log("Submit event on button : " + focusedButton);
            // Submit focusedElement
            focusedButton.SendEvent(e);
        }

    }
}
