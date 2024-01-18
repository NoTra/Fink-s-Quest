using UnityEngine;

public class Activable : MonoBehaviour
{
    public bool _isActive = false;

    protected void Activate()
    {
        _isActive = true;
    }

    protected void Deactivate()
    {
        _isActive = false;
    }

    protected void Toggle()
    {
        if (_isActive)
            Deactivate();
        else
            Activate();
    }
}