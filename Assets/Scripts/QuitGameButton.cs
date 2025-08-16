using UnityEngine;

public class QuitGameButton : MonoBehaviour
{
    /*
    * Author: Jaasper Lee Zong Hng
    * Date: 3/08/2025
    * Description: Button script for quitting the game
    */

    public void QuitGame() /// Quit the game
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // For testing in the editor
#endif
    }
}