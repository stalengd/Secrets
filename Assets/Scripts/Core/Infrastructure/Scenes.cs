using UnityEngine.SceneManagement;

namespace Secrets.Infrastructure
{
    public static class Scenes
    {
        public static readonly int Main = SceneManager.GetSceneByName("Main").buildIndex; 
    }
}
