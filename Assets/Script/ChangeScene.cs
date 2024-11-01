using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void MoveTo(string parameter1)
    {
        SceneManager.LoadScene(parameter1);
    }
    public void Quit()
    {
        Application.Quit();
        UnityEditor.EditorApplication.ExitPlaymode();
    }
}
