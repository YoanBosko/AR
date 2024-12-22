/*
 * Author: Rickman Roedavan
 * Created: 29 September 2024
 * Desc: This script manages the avatar's animations. 
 *       It synchronizes animation states with the character's movement and actions. 
 *       Supports blending between different animations, such as idle, walk, run, and jump, to ensure a smooth transition.
 */
using UnityEngine;

public class ThirdPersonAnimation : MonoBehaviour
{
    [Header("Animator Settings")]
    public Animator targetAnimator;

    [Header("Parameter Names")]
    public string inputMagnitudeParam = "inputMagnitude";
    public string isSprintingParam = "isSprinting";
    public string isGroundedParam = "IsGrounded";
    public string jumpStateName = "Jump";

    [Header("Magnitude Setting")]
    public float inputMagnitude;
    public float Multiply;
    public bool isGrounded;

    // Referensi ke script AvatarLocomotion
    private ThirdPersonLocomotion thirdPersonLocomotion;

    // Nama state untuk animasi Jump

    // Variable untuk mendeteksi loncatan
    private bool isJumping;

    public void UpdateAnimator(GameObject Avatar)
    {
        targetAnimator = Avatar.GetComponent<Animator>();
    }

    void Start()
    {
        thirdPersonLocomotion = GetComponent<ThirdPersonLocomotion>();
    }

    void Update()
    {
        UpdateAnimatorParameters();
        HandleJump();
    }

    void LateUpdate()
    {
        targetAnimator.SetBool(isGroundedParam, true);
    }

    void UpdateAnimatorParameters()
    {
        if (targetAnimator == null || thirdPersonLocomotion == null) return;

        // Mengambil inputMagnitude dari AvatarLocomotion
        inputMagnitude = thirdPersonLocomotion.Magnitude;
        isGrounded = thirdPersonLocomotion.isGrounded;

        // Mengatur parameter inputMagnitude di Animator
        targetAnimator.SetFloat(inputMagnitudeParam, inputMagnitude * Multiply);

        // Mengatur parameter isSprinting berdasarkan nilai inputMagnitude
        bool isCurrentlySprinting = inputMagnitude > 1.5f; // Sprint threshold
        targetAnimator.SetBool(isSprintingParam, isCurrentlySprinting);
        targetAnimator.SetBool(isGroundedParam, isGrounded);
    }

    void HandleJump()
    {
        if (targetAnimator == null || thirdPersonLocomotion == null) return;

        // Mengecek apakah tombol jump ditekan
        if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {
            isJumping = true;
            targetAnimator.Play(jumpStateName); // Memutar animasi loncat berdasarkan nama state
        }

        // Mengecek apakah karakter sudah menyentuh tanah lagi (menggunakan AvatarLocomotion)
        if (isJumping)
        {
            isJumping = false; // Reset status jumping ketika karakter menyentuh tanah
            isGrounded = true;
        }
    }
}