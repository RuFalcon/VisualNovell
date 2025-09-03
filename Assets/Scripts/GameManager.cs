using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static bool soundOff = false;


    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
}
