using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    public GameObject checkGameObject1;
    public GameObject checkGameObject2;
    public GameObject GameOb1;
    public GameObject GameOb2;

    public void Switching()
    {
        if (checkGameObject1.activeSelf)
        {
            GameOb1.SetActive(true);
        }
        else if (checkGameObject1.activeSelf && GameOb1.activeSelf)
        {
            GameOb1.SetActive(false);
        }
        else if (checkGameObject2)
        {
            GameOb2.SetActive(true);
        }
        else if (checkGameObject2.activeSelf && GameOb2.activeSelf)
        {
            GameOb2.SetActive(false);
        }
    }

}
