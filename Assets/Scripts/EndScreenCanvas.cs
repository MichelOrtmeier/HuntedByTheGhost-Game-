using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreenCanvas : MonoBehaviour
{
    private void OnEnable()
    {
        
    }

    public void OnRestartButtonClick()
    {
        SceneManager.LoadScene(0);
    }
}
