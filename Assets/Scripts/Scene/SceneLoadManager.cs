using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scene
{
    public enum SceneType
    {
        EntryPoint,
        MainMenu,
        LevelSelect,
        Gameplay,
        TestScene
    }
    public class SceneLoadManager : MonoBehaviour
    {
        public static SceneLoadManager Instance { get; private set; }
        
        private AsyncOperation _asyncOperation;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        
        public void LoadScene(SceneType type, LoadSceneMode mode = LoadSceneMode.Single)
        {
            _asyncOperation = SceneManager.LoadSceneAsync(type.ToString(), mode);
            if (_asyncOperation is not null)
            {
                _asyncOperation.completed += OnSceneLoadEnd;
            }
        }

        private void OnSceneLoadEnd(AsyncOperation op)
        {
            _asyncOperation.completed -= OnSceneLoadEnd;
            
        }
    }
}