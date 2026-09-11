using UnityEngine;
using UnityEngine.SceneManagement;

namespace VRTraining.Infrastructure
{
    public sealed class SceneNavigation : MonoBehaviour
    {
        [SerializeField] private string lobbySceneName = "LobbyScene";
        [SerializeField] private string trainingSceneName = "Training";

        public void LoadLobby()
        {
            LoadScene(lobbySceneName);
        }

        public void LoadTraining()
        {
            LoadScene(trainingSceneName);
        }

        public void RestartCurrentScene()
        {
            var activeScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(activeScene.buildIndex);
        }

        private void LoadScene(string sceneName)
        {
            if (string.IsNullOrWhiteSpace(sceneName) ||
                !Application.CanStreamedLevelBeLoaded(sceneName))
            {
                Debug.LogError(
                    $"Scene '{sceneName}' is not available. Add it to Build Settings.",
                    this);
                return;
            }

            SceneManager.LoadScene(sceneName);
        }
    }
}
