using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CharacterUISelector : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private CharacterData characterData; 

    private Image _image;
    private Button _button;
    private Vector3 _originalScale;
    private bool _isDeployed = false;

    void Awake()
    {
        _image = GetComponent<Image>();
        _button = GetComponent<Button>();
        _originalScale = transform.localScale;

        if (characterData != null)
        {
            _image.sprite = characterData.characterUISprite;
        }
    }

    void OnEnable()
    {
        DeploymentManager.OnCharacterSelected += HandleCharacterSelection;
        DeploymentManager.OnCharacterDeployed += HandleCharacterDeployment;
    }

    void OnDisable()
    {
        DeploymentManager.OnCharacterSelected -= HandleCharacterSelection;
        DeploymentManager.OnCharacterDeployed -= HandleCharacterDeployment;
    }
    
    // 实现IPointerClickHandler接口，使其可以被点击
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_isDeployed) return; // 如果已部署，不响应点击
        
        DeploymentManager.Instance.SelectCharacter(characterData);
    }

    // 处理角色被选择的事件
    private void HandleCharacterSelection(CharacterData selectedData)
    {
        if (_isDeployed) return;
        
        if (selectedData == characterData)
        {
            transform.localScale = _originalScale * 1.2f;
        }
        else
        {
            transform.localScale = _originalScale;
        }
    }

    // 处理角色被部署的事件
    private void HandleCharacterDeployment(CharacterData deployedData)
    {
        if (deployedData == characterData)
        {
            _isDeployed = true;
            _image.color = new Color(0.5f, 0.5f, 0.5f); 
            transform.localScale = _originalScale; 
            
        }
    }
}