using UnityEngine;
using UnityEngine.EventSystems;

public class test : MonoBehaviour
{
    void Update()
    {
        // Check if the mouse is over a UI element
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            // Get the mouse position in screen space
            Vector2 mousePosition = Input.mousePosition;

            // Check if the mouse position is within the bounds of a UI element
            if (RectTransformUtility.RectangleContainsScreenPoint(GetComponent<RectTransform>(), mousePosition))
            {
                Debug.Log("Mouse is over UI element");
            }
        }
    }
}
