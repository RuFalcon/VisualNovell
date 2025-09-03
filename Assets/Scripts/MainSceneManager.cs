using GamePush;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSceneManager : MonoBehaviour
{
    public void onClickLevel(int levelNumber)
    {
        SceneManager.LoadScene(levelNumber);
    }

    public async void ShowAchiviments()
    {
        await GP_Init.Ready;
        GP_Achievements.Open();
    }
}
