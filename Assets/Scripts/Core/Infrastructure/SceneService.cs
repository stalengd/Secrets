using UnityEngine.SceneManagement;

namespace Secrets.Infrastructure
{
    public sealed class SceneService
    {
        public void LoadMain()
        {
            Load(Scenes.Main);
        }

        private void Load(int sceneIndex)
        {
            SceneManager.LoadScene(sceneIndex);
        }
    }
}
