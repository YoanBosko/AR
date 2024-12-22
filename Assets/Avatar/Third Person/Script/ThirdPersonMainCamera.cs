using UnityEngine;

public class ThirdPersonMainCamera : MonoBehaviour
{
    public enum CameraMovementType
    {
        SmoothFollow,
        TopDownFollow,
        OrbitFollow
    }

    [Header("Main Camera Settings")]
    public Camera MainCamera; // Camera to be assigned in the Inspector
    public CameraMovementType cameraMovementType;

    [Header("Smooth Follow Settings")]
    public float followDamping = 0.1f;
    public float followDistance = 5f;   // Distance behind the target
    public float followHeight = 2f;     // Height offset from the target
    public float followSideOffset = 0f; // Horizontal offset from the target
    public float minFollowDistance = 2f;

    [Header("Top Down Settings")]
    public Vector3 topDownOffset = new Vector3(0, 10, -10);
    public float topDownDamping = 15f;  // Ketinggian kamera dari target dalam mode Top Down
    public float topDownLookAngle = 45f; // Sudut pandang kamera dari ata

    [Header("Orbit Follow Settings")]
    public float orbitSpeed = 100f;
    public float orbitHeightOffset = 2f;
    public float orbitFromTarget = 5f;
    public float orbitRotationLimit = 80f;
    public float orbitDampingAuto = 5f; // Damping untuk rotasi otomatis
    public float orbitDampingMouse = 2f; // Damping untuk rotasi menggunakan mouse

    [Header("Zoom Settings")]
    public float zoomSpeed = 2f;        // Kecepatan zoom dengan scroll wheel
    public float minZoomDistance = 2f;  // Jarak minimum kamera dari target
    public float maxZoomDistance = 10f; // Jarak maksimum kamera dari target

    private Vector3 currentVelocity;
    ThirdPersonLocomotion thirdPersonLocomotion;

    private void Start()
    {
        thirdPersonLocomotion = GetComponent<ThirdPersonLocomotion>();
        thirdPersonLocomotion.SetMainCamera(MainCamera);
    }

    void LateUpdate()
    {
        switch (cameraMovementType)
        {
            case CameraMovementType.SmoothFollow:
                SmoothFollow();
                break;

            case CameraMovementType.TopDownFollow:
                TopDownFollow();
                break;

            case CameraMovementType.OrbitFollow:
                OrbitFollow();
                break;
        }
    }

    void SmoothFollow()
    {
        // Desired position with offset behind, above, and optionally to the side of the character's transform
        Vector3 desiredPosition = transform.position
                                  - transform.forward * followDistance
                                  + Vector3.up * followHeight
                                  + transform.right * followSideOffset;

        // Menghitung jarak antara kamera dan target
        float currentDistance = Vector3.Distance(MainCamera.transform.position, transform.position);

        // Jika jarak antara kamera dan target lebih besar dari jarak minimum, lakukan smooth follow
        if (currentDistance > minFollowDistance)
        {
            // Smoothly move to the desired position
            Vector3 smoothedPosition = Vector3.SmoothDamp(MainCamera.transform.position, desiredPosition, ref currentVelocity, followDamping);
            MainCamera.transform.position = smoothedPosition;
        }
        else
        {
            // Ketika terlalu dekat, hanya perbaiki rotasi tanpa merubah posisi
            MainCamera.transform.position = MainCamera.transform.position; // Tetap di posisi saat ini
        }

        // Rotate the camera to face the same direction as the character
        Quaternion targetRotation = Quaternion.LookRotation(transform.position - MainCamera.transform.position);
        MainCamera.transform.rotation = Quaternion.Lerp(MainCamera.transform.rotation, targetRotation, followDamping);
    }

    void TopDownFollow()
    {
        // Set desired position with an offset above the character's transform
        Vector3 desiredPosition = transform.position + topDownOffset;

        // Smoothly move to the desired position using Lerp
        MainCamera.transform.position = Vector3.Lerp(MainCamera.transform.position, desiredPosition, topDownDamping * Time.deltaTime);

        // Set rotation looking at the character
        Quaternion topDownRotation = Quaternion.Euler(topDownLookAngle, 0, 0);
        MainCamera.transform.rotation = Quaternion.Lerp(MainCamera.transform.rotation, topDownRotation, topDownDamping * Time.deltaTime);
    }

    void OrbitFollow()
    {
        if (MainCamera == null) return;

        // Mengambil input dari mouse untuk rotasi horizontal dan vertikal
        float horizontalInput = Input.GetAxis("Mouse X") * orbitSpeed * Time.deltaTime;
        float verticalInput = -Input.GetAxis("Mouse Y") * orbitSpeed * Time.deltaTime;

        // Update rotasi horizontal menggunakan RotateAround untuk mengelilingi karakter
        MainCamera.transform.RotateAround(transform.position + Vector3.up * orbitHeightOffset, Vector3.up, horizontalInput);

        // Menghitung sudut vertikal baru dengan membatasi rotasi agar tidak berputar terlalu jauh ke atas/bawah
        float currentAngleX = MainCamera.transform.eulerAngles.x + verticalInput;
        currentAngleX = Mathf.Clamp(currentAngleX, 10f, orbitRotationLimit);

        // Terapkan sudut vertikal yang diperbarui
        Vector3 currentEulerAngles = MainCamera.transform.eulerAngles;
        currentEulerAngles.x = currentAngleX;
        MainCamera.transform.eulerAngles = new Vector3(currentAngleX, currentEulerAngles.y, currentEulerAngles.z);

        // Mengatur zoom menggunakan scroll wheel
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        orbitFromTarget -= scrollInput * zoomSpeed;
        orbitFromTarget = Mathf.Clamp(orbitFromTarget, minZoomDistance, maxZoomDistance); // Membatasi jarak zoom

        // Mengatur posisi kamera untuk menjaga jarak tetap dari target dengan tinggi yang ditentukan
        Vector3 direction = (MainCamera.transform.position - (transform.position + Vector3.up * orbitHeightOffset)).normalized;
        MainCamera.transform.position = (transform.position + Vector3.up * orbitHeightOffset) + direction * orbitFromTarget;

        // Mengatur agar kamera selalu menghadap ke karakter
        MainCamera.transform.LookAt(transform.position + Vector3.up * orbitHeightOffset);
    }

    // Public functions to activate specific camera movement types
    public void SetSmoothFollow()
    {
        cameraMovementType = CameraMovementType.SmoothFollow;
    }

    public void SetTopDownFollow()
    {
        cameraMovementType = CameraMovementType.TopDownFollow;
    }

    public void SetOrbitFollow()
    {
        cameraMovementType = CameraMovementType.OrbitFollow;
    }
}