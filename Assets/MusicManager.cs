using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [Header("Music")]
    [SerializeField] private EventReference mainMenuMusic;
    private FMODAudioEvent mainMenuMusicInstance;
    [SerializeField] private EventReference stageMusic;
    private FMODAudioEvent stageMusicInstance;
    [SerializeField] private EventReference endGameMusic;
    private FMODAudioEvent endGameMusicInstance;

    [Header("Effects")]
    [SerializeField] private EventReference startGameEffect;
    private FMODAudioEvent startGameEffectInstance;
    [SerializeField] private EventReference countDownEffect;
    private FMODAudioEvent countDownEffectInstance;

    [Header("Voices")]
    [SerializeField] private EventReference overtakeEffect;
    private FMODAudioEvent overtakeEffectInstance;
    [SerializeField] private EventReference damageEffect;
    private FMODAudioEvent damageEffectInstance;

    private void Start()
    {
        InstantiateReferences();
        mainMenuMusicInstance.Play();
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

    private void InstantiateReferences()
    {
        mainMenuMusicInstance = new FMODAudioEvent(mainMenuMusic);
        stageMusicInstance = new FMODAudioEvent(stageMusic);
        endGameMusicInstance = new FMODAudioEvent(endGameMusic);

        startGameEffectInstance = new FMODAudioEvent(startGameEffect, true);
        countDownEffectInstance = new FMODAudioEvent(countDownEffect, true);
        overtakeEffectInstance = new FMODAudioEvent(overtakeEffect, true);
        damageEffectInstance = new FMODAudioEvent(damageEffect, true);
    }

    private void HandleRestartGame()
    {
        mainMenuMusicInstance.Stop();
        endGameMusicInstance.Stop();
        stageMusicInstance.SetParameter("LastLap", 0f);
        stageMusicInstance.Stop();
        startGameEffectInstance.Play();
        countDownEffectInstance.Play();
    }
    private void HandleStartGame()
    {
        stageMusicInstance.Play();
    }
    private void HandleLastLap(Kart kart)
    {
        stageMusicInstance.SetParameter("LastLap", 1f);
    }
    private void HandleEndGame(Kart kart)
    {
        stageMusicInstance.Stop();
        endGameMusicInstance.Play();
    }
    private void HandleOvertake(Kart kart)
    {
        overtakeEffectInstance.Play();
    }
    private void HandleDamage(Kart kart, KartDebuffType type)
    {
        damageEffectInstance.Play();
    }
}
