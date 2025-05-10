using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Managers
{
    public class GameRestart : MonoBehaviour
    {
        public void RestartGame()
        {
            Debug.Log("Restarting Game");
            SceneManager.LoadScene("Sketch room");
        }
    }
}