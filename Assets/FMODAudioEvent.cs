using FMOD.Studio;
using FMODUnity;

public class FMODAudioEvent
{
    private EventInstance eventInstance;
    private EventReference fmodEventReference;
    private bool isOneShot;

    public FMODAudioEvent(EventReference fmodEventReference, bool isOneShot = false)
    {
        this.fmodEventReference = fmodEventReference;
        this.isOneShot = isOneShot;
        InitializeAsEventInstance();
    }
    
    public void InitializeAsEventInstance()
    {
        // if already instantiated, release the old one
        if(eventInstance.isValid())
        {
            FMODSoundManager.instance.ReleaseEventInstance(this.eventInstance);
        }

        if(FMODSoundManager.instance != null && fmodEventReference.IsNull == false)
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


