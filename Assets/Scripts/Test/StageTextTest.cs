using UnityEngine;
using TMPro;

public class StageTextTest : MonoBehaviour
{
    [Header("测试按钮")]
    [SerializeField] private KeyCode deploymentKey = KeyCode.Alpha1;
    [SerializeField] private KeyCode enemyTurnKey = KeyCode.Alpha2;
    [SerializeField] private KeyCode playerTurnKey = KeyCode.Alpha3;
    
    private GameManager gameManager;
    private TextMeshProUGUI stageText;
    
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        
        // 查找StageText组件
        GameObject stageTextObj = GameObject.Find("FightView/Background/StageText");
        if (stageTextObj != null)
        {
            stageText = stageTextObj.GetComponent<TextMeshProUGUI>();
        }
        
        Debug.Log("StageTextTest initialized. Press 1/2/3 to test different stages.");
        Debug.Log("1 - Deployment Stage, 2 - Enemy Turn, 3 - Player Turn");
    }
    
    void Update()
    {
        if (Input.GetKeyDown(deploymentKey))
        {
            TestDeploymentStage();
        }
        else if (Input.GetKeyDown(enemyTurnKey))
        {
            TestEnemyTurn();
        }
        else if (Input.GetKeyDown(playerTurnKey))
        {
            TestPlayerTurn();
        }
    }
    
    void TestDeploymentStage()
    {
        Debug.Log("Testing Deployment Stage");
        if (stageText != null)
        {
            stageText.text = "部署阶段";
            stageText.color = Color.white;
        }
    }
    
    void TestEnemyTurn()
    {
        Debug.Log("Testing Enemy Turn");
        if (stageText != null)
        {
            stageText.text = "敌人回合";
            stageText.color = Color.red;
        }
    }
    
    void TestPlayerTurn()
    {
        Debug.Log("Testing Player Turn");
        if (stageText != null)
        {
            stageText.text = "玩家回合";
            stageText.color = Color.white;
        }
    }
}