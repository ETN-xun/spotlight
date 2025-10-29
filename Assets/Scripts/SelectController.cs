using System.Collections;
using System.Collections.Generic;
using Level;
using Scene;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectController : MonoBehaviour
{
    public void ChangeScene()
    {
        SceneManager.LoadScene("Demo");
    }
}
