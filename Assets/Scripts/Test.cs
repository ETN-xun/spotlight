using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using View;

public class Test : MonoBehaviour
{
    private void Start()
    {
        ViewManager.Instance.OpenView(ViewType.TestView);
    }
}
