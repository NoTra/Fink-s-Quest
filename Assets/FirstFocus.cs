using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FirstFocus : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        FocusFirstElement();
    }

    public void FocusFirstElement()
    {
        GetComponent<UIDocument>().rootVisualElement.
            Q<Button>().parent.Focus();
    }
}
