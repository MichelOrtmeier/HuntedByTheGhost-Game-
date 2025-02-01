using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreenCanvas : MonoBehaviour
{

    public void OnRestartButtonClick()
    {
        SceneManager.LoadScene(0);
    }
}
