using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloseTheHint : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Hint;
    public GameObject Bg;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
        Bg.SetActive(false);
        Hint.SetActive(false);
        gameObject.SetActive(false);
    }
}
