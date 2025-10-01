using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using View;

public class Test : MonoBehaviour
{
    private void Start()
    {
        // ViewManager.Instance.OpenView(ViewType.TestView);
        StartCoroutine(PlaceUnitsWithDelay());
    }

    private IEnumerator PlaceUnitsWithDelay()
    {
        yield return new WaitForSeconds(0.25f);
        GridManager.Instance.PlaceUnit(new Vector2Int(2, 1), Resources.Load<Unit>("Unit/TestUnit"));
        GridManager.Instance.PlaceUnit(new Vector2Int(3, 1), Resources.Load<Unit>("Unit/TestUnit"));
        GridManager.Instance.PlaceUnit(new Vector2Int(4, 1), Resources.Load<Unit>("Unit/TestUnit"));
    }
}
