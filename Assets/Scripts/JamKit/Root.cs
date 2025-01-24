using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JamKit
{
    public class Root : MonoBehaviour
    {
        [SerializeField] private JamKit _jamKit;
        [SerializeField] private Camera _camera;

        private IScene _currentScene;
        private bool _isSceneLoading;
        [SerializeField] private string _currentSceneName = "Game";

        private IEnumerator Start()
        {
            yield return _jamKit.Run(ChangeSceneCoroutine("", _currentSceneName));
        }

        private void Update()
        {
            if (_isSceneLoading)
            {
                return;
            }

            string nextSceneName = _currentScene.Tick();
            if (nextSceneName != _currentSceneName)
            {
                _jamKit.Run(ChangeSceneCoroutine(_currentSceneName, nextSceneName));
            }
        }

        private IEnumerator ChangeSceneCoroutine(string oldSceneName, string newSceneName)
        {
            _isSceneLoading = true;

            if (oldSceneName != "")
            {
                yield return _currentScene.Exit();
                yield return SceneManager.UnloadSceneAsync(oldSceneName);
            }

            _currentSceneName = newSceneName;
            yield return SceneManager.LoadSceneAsync(newSceneName, new LoadSceneParameters(LoadSceneMode.Additive));
            _currentScene = FindObjectOfType<SceneRoot>();
            _currentScene.Init(_jamKit, _camera);
            _isSceneLoading = false;
        }

    }
}