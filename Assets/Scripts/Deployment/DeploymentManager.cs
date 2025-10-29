using UnityEngine;
using System;
using System.Collections.Generic;

public class DeploymentManager : MonoBehaviour
{
    public static DeploymentManager Instance { get; private set; }

    public static event Action<CharacterData> OnCharacterSelected;
    public static event Action<CharacterData> OnCharacterDeployed;
    public static event System.Action OnDeploymentComplete;

    [SerializeField] private int totalCharactersToDeploy = 3;

    private CharacterData _selectedCharacter;
    private HashSet<CharacterData> _deployedCharacters = new HashSet<CharacterData>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void SelectCharacter(CharacterData characterData)
    {
        if (_deployedCharacters.Contains(characterData)) return;
        _selectedCharacter = characterData;
        OnCharacterSelected?.Invoke(_selectedCharacter);
    }
    
    public void AttemptDeploy(Vector2Int coord)
    {
        if (_selectedCharacter == null)
        {
            Debug.Log("请先选择一个角色再进行部署！");
            return;
        }

        if (!IsTileValidForDeployment(coord))
        {
            Debug.Log("这个位置不能部署！");
            return;
        }


        bool success = GridManager.Instance.PlaceUnit(coord, _selectedCharacter.characterPrefab);

        if (success)
        {
            _deployedCharacters.Add(_selectedCharacter);
            OnCharacterDeployed?.Invoke(_selectedCharacter);
            _selectedCharacter = null;
            OnCharacterSelected?.Invoke(null);

            if (_deployedCharacters.Count >= totalCharactersToDeploy)
            {
                OnDeploymentComplete?.Invoke();
                Debug.Log("所有角色部署完毕！进入游戏主逻辑。");
                FindObjectOfType<MapInputController>().enabled = false; 
            }
        }
        else
        {
             Debug.LogError($"GridManager.PlaceUnit在坐标 {coord} 部署失败！");
        }
    }

    private bool IsTileValidForDeployment(Vector2Int coord)
    {
        var cell = GridManager.Instance.GetCell(coord);

        if (cell == null) return false;
        if (cell.CurrentUnit != null) return false;
        if (cell.DestructibleObject != null || cell.ObjectOnCell != null) return false;
        if (!cell.isDeployableZone) return false;

        // --- 在这里添加你自己的其他特定部署规则 ---
        // 例如，你可以在 GridCell 中添加一个 isDeployableZone 的布尔值
        // if (!cell.isDeployableZone) return false;
        
        return true;
    }
}