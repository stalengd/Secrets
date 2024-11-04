using UnityEngine.SceneManagement;

namespace Anomalus.Infrastructure
{
    public static class Scenes
    {
        public static readonly int Main = SceneManager.GetSceneByName("Main").buildIndex; 
    }
}
