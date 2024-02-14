using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FinksQuest
{
    public class HoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
    {
        [SerializeField] private SVGImage _stroke;

        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("Pointer Enter in : " + name);
            _stroke.gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.Log("Pointer Exit of : " + name);
            _stroke.gameObject.SetActive(false);
        }

        public void OnSelect(BaseEventData eventData)
        {
            Debug.Log("Selected : " + name);
            _stroke.gameObject.SetActive(true);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            Debug.Log("Deselected : " + name);
            _stroke.gameObject.SetActive(false);
        }
    }
}
