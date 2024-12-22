/*
 * Author: Rickman Roedavan
 * Created: 29 September 2024
 * Desc: This script handles avatar management, allowing players to switch between different characters.
 *       It manages loading and switching avatars, copying necessary components (e.g., Animator Controller),
 *       and ensuring a consistent experience when changing avatars.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ThirdPersonManager : MonoBehaviour
{
    [Header("Avatar Settings")]
    public GameObject AvatarParent;
    public GameObject AvatarReference;
    public bool RootMotion = false;

    [Header("Event Settings")]
    public UnityEvent StartEvent;
    GameObject CurrentAvatar;
    ThirdPersonAnimation thirdPersonAnimation;

    // Start is called before the first frame update
    void Start()
    {
        thirdPersonAnimation = GetComponent<ThirdPersonAnimation>();
        StartEvent?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangePrefabs(GameObject NewPrefabs)
    {
        if (NewPrefabs != null)
        {
            CurrentAvatar = GameObject.Instantiate(NewPrefabs);
        }
        CurrentAvatar.transform.parent = AvatarParent.transform;
        CurrentAvatar.transform.position = AvatarReference.transform.position;
        CurrentAvatar.transform.rotation = AvatarReference.transform.rotation;
        CurrentAvatar.GetComponent<Animator>().runtimeAnimatorController = AvatarReference.GetComponent<Animator>().runtimeAnimatorController;
        CurrentAvatar.GetComponent<Animator>().applyRootMotion = RootMotion;
        thirdPersonAnimation.targetAnimator = CurrentAvatar.GetComponent<Animator>();
        HideAllAvatar();
        CurrentAvatar.SetActive(true);
    }

    public void ChangeAvatar(GameObject NewAvatar)
    {
        if (NewAvatar != null)
        {
            CurrentAvatar = NewAvatar;
        }
        CurrentAvatar.transform.parent = AvatarParent.transform;
        CurrentAvatar.transform.position = AvatarReference.transform.position;
        CurrentAvatar.transform.rotation = AvatarReference.transform.rotation;
        CurrentAvatar.GetComponent<Animator>().runtimeAnimatorController = AvatarReference.GetComponent<Animator>().runtimeAnimatorController;
        CurrentAvatar.GetComponent<Animator>().applyRootMotion = RootMotion;
        thirdPersonAnimation.targetAnimator = CurrentAvatar.GetComponent<Animator>();
        HideAllAvatar();
        CurrentAvatar.SetActive(true);
    }

    public void HideAllAvatar()
    {
        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            // Mendapatkan child ke-i dari transform
            Transform child = transform.GetChild(i);

            // Mengecek apakah child tersebut memiliki komponen Animator
            Animator animator = child.GetComponent<Animator>();
            if (animator != null)
            {
                child.gameObject.SetActive(false);
            }
        }
    }
}
