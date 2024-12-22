/*
 * Author: Rickman Roedavan
 * Created: 29 September 2024
 * Desc: This script is responsible for controlling the locomotion of the avatar character. 
 *       It manages player movement, including walking, running, dashing, and jumping mechanics. 
 *       Designed to work with different types of inputs (keyboard, joystick) and locomotion modes.
 */

using UnityEngine;
using UnityEngine.UI;
public class ThirdPersonLocomotion : MonoBehaviour
{
    public enum LocomotionType
    {
        Standard,
        DirectionalTurn
    }

    public enum InputType
    {
        Keyboard,
        Joystick
    }

    [Header("Locomotion Type")]
    public LocomotionType locomotionType;
    public InputType inputType;
    private Camera MainCamera; // Camera to be assigned in the Inspector

    [Header("Input Settings")]
    public KeyCode JumpKey = KeyCode.Space;
    public KeyCode WalkKey = KeyCode.LeftShift;
    public KeyCode SprintKey = KeyCode.LeftControl;
    public KeyCode DashKey = KeyCode.LeftAlt;

    [Header("Movement Settings")]
    public float Jump = 7f;
    public float Walk = 2f; 
    public float Run = 5f;
    public float Sprint = 10f; 
    public float Rotation = 5f;
    public float Damping = 0.5f;
    public float Magnitude = 0f;
    public Vector3 velocity;

    [Header("Dash Settings")]
    public float dashSpeed = 15f;    // Kecepatan dash
    public float dashDuration = 0.2f; // Durasi dash dalam detik
    public bool canDashOnAir = false;

    [Header("Ground Detection")]
    public float Gravity = -9.81f;
    public float rayDistance = 1.1f;
    public float groundedOffset = -2f;       // Nilai untuk menjaga karakter tetap grounded
    public float directionThreshold = 0.1f;  // Threshold untuk menentukan arah gerakan
    public bool canDoubleJump = true;
    public bool isGrounded;

    private float horizontalInput;
    private float verticalInput;
    private CharacterController controller;
    private bool canUseDoubleJump = false;
    private bool touchInput = false;

    void Start()
    {
        // Inisialisasi komponen CharacterController
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        HandleInput();
        CheckGroundStatus();
        Locomotion();
        ApplyGravity();
    }

    public void SetMainCamera(Camera TargetCamera)
    {
        MainCamera = TargetCamera;
    }
    void HandleInput()
    {
        if (inputType == InputType.Keyboard)
        {
            // Mengambil input horizontal dan vertical dari pengguna
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");
        }
    }

    void CheckGroundStatus()
    {
        // Menggunakan raycast untuk memeriksa apakah karakter menyentuh tanah
        Ray ray = new Ray(transform.position, Vector3.down);
        isGrounded = Physics.Raycast(ray, rayDistance);
    }

    void Locomotion()
    {
        float currentSpeed = Run; // Default speed is running
        Magnitude = 0;

        // Menggunakan SprintKey untuk meningkatkan kecepatan menjadi sprint
        if (inputType == InputType.Keyboard && Input.GetKey(SprintKey))
        {
            currentSpeed = Sprint;
        }
        // Menggunakan WalkKey untuk mengubah kecepatan menjadi walk
        else if (inputType == InputType.Keyboard && Input.GetKey(WalkKey))
        {
            currentSpeed = Walk;
        }

        Vector3 move = Vector3.zero;

        if (MainCamera == null)
        {
            if (locomotionType == LocomotionType.Standard)
            {
                // Gerakan horizontal standar menggunakan input dan Speed
                move = transform.right * horizontalInput + transform.forward * verticalInput;
                Magnitude = (move * currentSpeed * Time.deltaTime).magnitude;
                controller.Move(move * currentSpeed * Time.deltaTime);
            }
            else if (locomotionType == LocomotionType.DirectionalTurn)
            {
                // Mengatur rotasi karakter agar menghadap ke arah gerakan
                Vector3 direction = new Vector3(horizontalInput, 0f, verticalInput).normalized;
                if (direction.magnitude >= directionThreshold)
                {
                    float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                    float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref Rotation, Damping);
                    transform.rotation = Quaternion.Euler(0f, angle, 0f);

                    move = transform.forward;
                    Magnitude = (move * currentSpeed * Time.deltaTime).magnitude;
                    controller.Move(move * currentSpeed * Time.deltaTime);
                }
            }
        }
        else
        {
            if (locomotionType == LocomotionType.Standard)
            {
                // Gerakan horizontal standar menggunakan input dan kecepatan yang sesuai
                move = MainCamera.transform.right * horizontalInput + MainCamera.transform.forward * verticalInput;
                move.y = 0; // Menjaga agar tidak ada pergerakan vertikal
                Magnitude = (move * currentSpeed * Time.deltaTime).magnitude;
                controller.Move(move * currentSpeed * Time.deltaTime);
            }
            else if (locomotionType == LocomotionType.DirectionalTurn)
            {
                // Mengatur rotasi karakter agar menghadap ke arah gerakan
                Vector3 direction = new Vector3(horizontalInput, 0f, verticalInput).normalized;
                if (direction.magnitude >= directionThreshold)
                {
                    float targetAngle = 0;
                    targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + MainCamera.transform.eulerAngles.y;
                    float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref Rotation, Damping);
                    transform.rotation = Quaternion.Euler(0f, angle, 0f);

                    move = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                    Magnitude = (move * currentSpeed * Time.deltaTime).magnitude;
                    controller.Move(move * currentSpeed * Time.deltaTime);
                }
            }
        }

        if (inputType == InputType.Keyboard && Input.GetKeyDown(DashKey))
        {
            InvokeDash();
        }
    }

    void ApplyGravity()
    {
        if (isGrounded)
        {
            if (velocity.y < 0) // Only reset y velocity when falling down or standing still
            {
                velocity.y = groundedOffset; // Maintain grounded state
            }
            canUseDoubleJump = true; // Reset double jump when grounded

            if (inputType == InputType.Keyboard && Input.GetKeyDown(JumpKey))
            {
                InvokeJump(); // Perform jump
            }
        }
        else
        {
            if (inputType == InputType.Keyboard && Input.GetKeyDown(JumpKey) && canDoubleJump && canUseDoubleJump)
            {
                InvokeJump(); // Allow double jump
            }

            velocity.y += Gravity * Time.deltaTime; // Apply gravity gradually
        }

        controller.Move(velocity * Time.deltaTime); // Move the character with gravity
    }

    public void SetInputHorizontal(Text inputText)
    {
        // Mengatur nilai horizontalInput dari UI Text
        if (inputText != null)
        {
            float.TryParse(inputText.text, out horizontalInput);
        }
    }

    public void SetInputVertical(Text inputText)
    {
        // Mengatur nilai verticalInput dari UI Text
        if (inputText != null)
        {
            float.TryParse(inputText.text, out verticalInput);
        }
    }

    public void InvokeJump()
    {
        if (isGrounded)
        {
            velocity.y = Jump; // Apply force for the first jump
            canUseDoubleJump = true; // Allow double jump after the first jump
        }
        else if (canDoubleJump && canUseDoubleJump)
        {
            velocity.y = Jump; // Apply force for double jump
            canUseDoubleJump = false; // Only allow double jump once while airborne
        }
    }

    public void InvokeDash()
    {
        // Fungsi untuk melakukan dash ketika di darat
        if (Magnitude > 0 && (isGrounded || canDashOnAir))
        {
            StartCoroutine(Dash(Vector3.forward));
        }
    }

    private System.Collections.IEnumerator Dash(Vector3 direction)
    {
        float startTime = Time.time;

        // Hitung durasi dash
        while (Time.time < startTime + dashDuration)
        {
            Vector3 dashMove = transform.forward * dashSpeed;
            controller.Move(dashMove * Time.deltaTime);
            yield return null;
        }
    }
}