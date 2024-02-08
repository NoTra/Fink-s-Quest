using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaskFollowPlayer : MonoBehaviour
{
    [SerializeField] private Image circle;
    [SerializeField] private Transform player;
    private RectTransform rectTransform;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = circle.GetComponent<RectTransform>();
    }

    public void CenterCircleOnPlayer()
    {
        // Change pivot to player position
        rectTransform.position = Camera.main.WorldToScreenPoint(player.position);
    }
}
