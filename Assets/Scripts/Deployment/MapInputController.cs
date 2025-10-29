using UnityEngine;

public class MapInputController : MonoBehaviour
{
    private Camera _mainCamera;

    void Start()
    {
        _mainCamera = Camera.main;
    }

    void Update()
    {
        // 只在鼠标左键点击时响应
        if (Input.GetMouseButtonDown(0))
        {
            // 将屏幕点击位置转换为世界坐标
            Vector3 worldPos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            
            // 使用你的 GridManager 将世界坐标转换为 GridCell
            GridCell targetCell = GridManager.Instance.WorldToCell(worldPos);

            // 如果点击到了一个有效的格子上
            if (targetCell != null)
            {
                // 通知 DeploymentManager 尝试在该坐标部署
                DeploymentManager.Instance.AttemptDeploy(targetCell.Coordinate);
            }
        }
    }
}