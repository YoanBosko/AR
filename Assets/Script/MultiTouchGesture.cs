using UnityEngine;

public class MultiTouchGesture : MonoBehaviour
{
    private float initialDistance;
    private Vector3 initialScale;
    private Vector2 initialTouchPosition;
    private Vector3 initialPosition;
    private float rotationSpeed = 0.1f;
    private float panSpeed = 0.01f;

    void Update()
    {
        if (Input.touchCount == 1) // Pan atau geser dengan satu jari
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                initialTouchPosition = touch.position;
                initialPosition = transform.position;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector2 touchDelta = touch.position - initialTouchPosition;
                Vector3 newPosition = new Vector3(touchDelta.x * panSpeed, touchDelta.y * panSpeed, 0);
                transform.position = initialPosition + newPosition;
            }
        }
        else if (Input.touchCount == 2) // Pinch-to-zoom atau rotasi dengan dua jari
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            // Pinch-to-zoom
            if (touch0.phase == TouchPhase.Moved || touch1.phase == TouchPhase.Moved)
            {
                float currentDistance = Vector2.Distance(touch0.position, touch1.position);
                if (initialDistance == 0)
                {
                    initialDistance = currentDistance;
                    initialScale = transform.localScale;
                }
                else
                {
                    float scaleFactor = currentDistance / initialDistance;
                    transform.localScale = initialScale * scaleFactor;
                }
            }

            // Rotasi
            Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
            Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;

            float previousAngle = Vector2.SignedAngle(touch0PrevPos, touch1PrevPos);
            float currentAngle = Vector2.SignedAngle(touch0.position, touch1.position);
            float angleDifference = currentAngle - previousAngle;

            transform.Rotate(Vector3.up, -angleDifference * rotationSpeed, Space.World);
        }
        else
        {
            // Reset initial values saat tidak ada touch
            initialDistance = 0;
        }
    }
}
