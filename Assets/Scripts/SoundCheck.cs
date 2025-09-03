using GamePush;
using UnityEngine;
using UnityEngine.UI;

public class SoundCheck : MonoBehaviour
{
    [SerializeField] private Sprite _soundOnImage;
    [SerializeField] private Sprite _soundOffImage;
    [SerializeField] private Image _soundImage;

    async void Update()
    {
        await GP_Init.Ready;
        CheckSound();
    }

    public void ManageSound()
    {
        GameManager.soundOff = !GameManager.soundOff;
        AudioListener.pause = GameManager.soundOff;
        GP_Player.Set("middleages_soundoff", GameManager.soundOff);
        GP_Player.Sync();
        if (GameManager.soundOff)
        {
            SoundManager.instance.StopMusic();

        }
        else
        {
            SoundManager.instance.MusicPlay();
        }
    }

    private void CheckSound()
    {
        AudioListener.pause = GameManager.soundOff;
        if (GameManager.soundOff)
        {
            _soundImage.sprite = _soundOffImage;
            //SoundManager.instance.StopMusic();

        }
        else
        {
            _soundImage.sprite = _soundOnImage;
            //SoundManager.instance.PlayMusic();
        }
    }
}
