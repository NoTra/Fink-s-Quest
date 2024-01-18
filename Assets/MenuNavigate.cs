using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using System.Xml.Linq;

public class MenuNavigate : MonoBehaviour
{
    private Button _focusedElement;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*public void OnNavigate()
    {
        Debug.Log("Navigate from MenuNavigate");
        var uiDocument = GameManager.Instance._currentUIDocument;
    }*/

    public void OnSubmit()
    {
        Debug.Log("Submit from MenuNavigate");
        // Find the element with active class
        Focusable focusedElement = GameManager.Instance._currentUIDocument.rootVisualElement.focusController.focusedElement;
        if (focusedElement != null)
        {
            Button focusedButton = null;

            Debug.Log("Click on button found : " + focusedElement);

            if (focusedElement is VisualElement)
            {
                // On récupère le bouton child
                VisualElement visualElement = (VisualElement)focusedElement;
                Button button = visualElement.Q<Button>();
                if (button != null)
                {
                    focusedButton = button;
                }
            } else if (focusedElement is Button)
            {
                // On récupère le bouton child
                Button button = (Button)focusedElement;
                if (button != null)
                {
                    focusedButton = button;
                }
            }

            if (focusedButton == null)
            {
                Debug.Log("No button found");
                return;
            }

            focusedButton.clicked += () => Debug.Log("Clicked on button");

            using (var e = new NavigationSubmitEvent() { target = focusedButton })
                // Submit focusedElement
                focusedButton.SendEvent(e);
        }
    }
}
