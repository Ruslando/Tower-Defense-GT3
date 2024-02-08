using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class FMODAudioEvent : MonoBehaviour
{
    private EventInstance eventInstance;
    private string playbackState;
    
    // FMOD Event reference
    [SerializeField] private EventReference fmodEventReference;
    [SerializeField] private bool isOneShot;
    
    private void Start()
    {
        InitializeAsEventInstance(fmodEventReference);
    }
    
    public void InitializeAsEventInstance(EventReference fmodEventReference)
    {
        // if already instantiated, release the old one
        if(eventInstance.isValid())
        {
            FMODSoundManager.instance.ReleaseEventInstance(this.eventInstance);
        }

        if(FMODSoundManager.instance != null)
        {
            eventInstance = FMODSoundManager.instance.CreateEventInstance(fmodEventReference);
        } 
    }

    public void SetParameter(string name, float value)
    {
        if(eventInstance.isValid())
        {
            eventInstance.setParameterByName(name, value);
        }
    }

    public void SetPitch(float pitch)
    {
        if(eventInstance.isValid())
        {
            eventInstance.setPitch(pitch);
        }
        
    }

    public void Play() 
    {
        if(isOneShot)
        {
            RuntimeManager.PlayOneShot(fmodEventReference);
        } else {
            PLAYBACK_STATE currentPlaybackState;
            eventInstance.getPlaybackState(out currentPlaybackState);

            if (currentPlaybackState != PLAYBACK_STATE.PLAYING)
            {
                eventInstance.start();

                PLAYBACK_STATE pbState;
                eventInstance.getPlaybackState(out pbState);
                playbackState = pbState.ToString();
            }
        }
    }

    public void Stop()
    {
        PLAYBACK_STATE currentPlaybackState;
        eventInstance.getPlaybackState(out currentPlaybackState);

        if (currentPlaybackState != PLAYBACK_STATE.STOPPED)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

            PLAYBACK_STATE pbState;
            eventInstance.getPlaybackState(out pbState);
            playbackState = pbState.ToString();
        }
    }

    public void Release()
    {
        if(this.eventInstance.isValid())
        {
            FMODSoundManager.instance.ReleaseEventInstance(this.eventInstance);
        }
    }
}


