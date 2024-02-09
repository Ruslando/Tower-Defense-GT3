using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Music")]
    [SerializeField] private FMODAudioEvent mainMenuMusic;
    [SerializeField] private FMODAudioEvent stageMusic;
    [SerializeField] private FMODAudioEvent endGameMusic;

    [Header("Effects")]
    [SerializeField] private FMODAudioEvent startGameEffect;
    [SerializeField] private FMODAudioEvent countDownEffect;
    [SerializeField] private FMODAudioEvent overtakeEffect;
    [SerializeField] private FMODAudioEvent damageEffect;

    private void Start()
    {
        mainMenuMusic.Play();
    }

    private void OnEnable()
    {
        LevelManager.OnRestartGame += HandleRestartGame;
        LevelManager.OnStartGame += HandleStartGame;
        Kart.OnLastLapCompleted += HandleLastLap;
        Kart.OnAllLapsCompleted += HandleEndGame;
        Kart.OnOvertake += HandleOvertake;
        Kart.OnDebuffApplied += HandleDamage;
    }

    private void HandleRestartGame()
    {
        mainMenuMusic.Stop();
        endGameMusic.Stop();

        stageMusic.SetParameter("LastLap", 0f);
        stageMusic.Stop();

        startGameEffect.Play();
        countDownEffect.Play();
    }

    private void HandleStartGame()
    {
        stageMusic.Play();
    }

    private void HandleLastLap(Kart kart)
    {
        stageMusic.SetParameter("LastLap", 1f);
    }

    private void HandleEndGame(Kart kart)
    {
        stageMusic.Stop();
        endGameMusic.Play();
    }

    private void HandleOvertake(Kart kart)
    {
        overtakeEffect.Play();
    }

    private void HandleDamage(Kart kart, KartDebuffType type)
    {
        damageEffect.Play();
    }
}
