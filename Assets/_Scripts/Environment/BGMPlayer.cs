using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    public  AudioClip   bgm;

    public float middleTime;
    
    private AudioSource audioSource1;
    private AudioSource audioSource2;

    private bool loaded;
    
    public void LoadSetting()
    {
        audioSource1 = GameManager.Instance.GetComponents<AudioSource>()[0];
        audioSource2 = GameManager.Instance.GetComponents<AudioSource>()[1];
        loaded       = true;
    }

    public void StartMusic(string prevClip)
    {
        if (!loaded)
            return;

        if (prevClip == bgm.name)
            return;

        AudioSource playingSource;
        AudioSource toPlaySource;
        
        
        if (audioSource1.isPlaying)
        {
            playingSource = audioSource1;
            toPlaySource  = audioSource2;
        }
        else
        {
            playingSource = audioSource2;
            toPlaySource  = audioSource1;
        }
        
        StartCoroutine(
            Lerp.LerpValueAfter(value => playingSource.volume = value,
                1,
                0f,
                1,
                Mathf.Lerp,
                null,
                () =>
                {
                    playingSource.Stop();
                    toPlaySource.clip   = bgm;
                    toPlaySource.time   = playingSource.isPlaying ? middleTime : 0;
                    toPlaySource.volume = 1;
                    toPlaySource.Play();
                    StartCoroutine(
                        Lerp.LerpValue(value => toPlaySource.volume = value,
                            0,
                            1f,
                            0.5f,
                            Mathf.Lerp
                        ));
                }));
    }
}
