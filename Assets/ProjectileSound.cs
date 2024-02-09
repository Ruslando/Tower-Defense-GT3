using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using JetBrains.Annotations;
using UnityEngine;

public class ProjectileSound : MonoBehaviour
{
    [SerializeField] private EventReference projectileSound;
    private FMODAudioEvent projectileSoundInstance;
    
    // Start is called before the first frame update
    void Start()
    {
        if(!LevelManager.Instance.IsGameOver())
        {
            InstantiateReference();
        }
    }

    private void InstantiateReference()
    {
        projectileSoundInstance = new FMODAudioEvent(projectileSound);
    }

    public void Play()
    {
        projectileSoundInstance.Play();
    }

    public void Stop()
    {
        projectileSoundInstance.Stop();
        projectileSoundInstance.Release();
    }

    public void SetParameter(string parameterName, float value)
    {
        projectileSoundInstance.SetParameter(parameterName, value);
    }

    private void OnDestroy()
    {
        projectileSoundInstance.Stop();
        projectileSoundInstance.Release();
    }
}
