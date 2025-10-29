using System.Collections;
using System.Collections.Generic;
using Level;
using Scene;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Selectcontroller : MonoBehaviour
{
     private string sceneName = "Demo".ToString();
     public void changeScene()
     {
          SceneManager.LoadScene(sceneName);
     }
}
