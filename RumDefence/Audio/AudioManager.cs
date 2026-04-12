using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace RumDefence;

public class AudioManager
{
    private static AudioManager _instance;
    public static AudioManager Instance => _instance ??= new AudioManager();

    private Dictionary<string, SoundEffect> soundEffects = new();
    private Song backgroundMusic;
    private bool isMusicPlaying = false;

    public void LoadContent()
    {
        var content = RumGame.Instance.Content;

        // Load all sound effects
        soundEffects["click"] = content.Load<SoundEffect>("Audio/click_004");
        soundEffects["confirmation"] = content.Load<SoundEffect>("Audio/confirmation_002");
        soundEffects["error"] = content.Load<SoundEffect>("Audio/error_008");
        soundEffects["switch"] = content.Load<SoundEffect>("Audio/switch_002");

        // Load background music
        backgroundMusic = content.Load<Song>("Audio/PineappleUnderTheSea");
    }

    public void PlaySound(string soundName)
    {
        if (soundEffects.TryGetValue(soundName, out var sound))
        {
            sound.Play();
        }
        else
        {
            System.Diagnostics.Debug.WriteLine($"Warning: Sound '{soundName}' not found");
        }
    }

    public void PlayBackgroundMusic()
    {
        if (!isMusicPlaying && backgroundMusic != null)
        {
            MediaPlayer.Play(backgroundMusic);
            MediaPlayer.IsRepeating = true;
            isMusicPlaying = true;
        }
    }

    public void StopBackgroundMusic()
    {
        if (isMusicPlaying)
        {
            MediaPlayer.Stop();
            isMusicPlaying = false;
        }
    }

    public void PauseBackgroundMusic()
    {
        MediaPlayer.Pause();
    }

    public void ResumeBackgroundMusic()
    {
        MediaPlayer.Resume();
    }

    public bool IsBackgroundMusicPlaying => isMusicPlaying;
}
