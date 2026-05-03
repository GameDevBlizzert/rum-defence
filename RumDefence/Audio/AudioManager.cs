using Microsoft.Xna.Framework;
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
    private Dictionary<string, Song> songs = new();
    private List<SoundEffect> footstepSounds = new();
    private List<SoundEffect> impactSounds = new();

    private float musicVolume = 0.5f;
    private float soundVolume = 1.0f;

    public float MusicVolume
    {
        get => musicVolume;
        set
        {
            musicVolume = MathHelper.Clamp(value, 0f, 1f);
            MediaPlayer.Volume = musicVolume;
        }
    }

    public float SoundVolume
    {
        get => soundVolume;
        set
        {
            soundVolume = MathHelper.Clamp(value, 0f, 1f);
            SoundEffect.MasterVolume = soundVolume;
        }
    }

    private Song currentSong;
    private string currentSongName;

    private bool isMusicActive = false;

    private bool isSuspended = false;

    private Random random = new Random();

    public void LoadContent()
    {
        var content = RumGame.Instance.Content;

        soundEffects["click"] = content.Load<SoundEffect>("Audio/click_004");
        soundEffects["confirmation"] = content.Load<SoundEffect>("Audio/confirmation_002");
        soundEffects["error"] = content.Load<SoundEffect>("Audio/error_008");
        soundEffects["switch"] = content.Load<SoundEffect>("Audio/switch_002");

        for (int i = 0; i < 5; i++)
            footstepSounds.Add(content.Load<SoundEffect>($"Audio/footstep_grass_00{i}"));

        for (int i = 0; i < 5; i++)
            impactSounds.Add(content.Load<SoundEffect>($"Audio/impactMining_00{i}"));

        songs["PineappleUnderTheSea"] = content.Load<Song>("Audio/PineappleUnderTheSea");
        songs["WhatCloudsAreMadeOf"] = content.Load<Song>("Audio/WhatCloudsAreMadeOf");
        songs["GentleBreeze"] = content.Load<Song>("Audio/GentleBreeze");

        MediaPlayer.Volume = musicVolume;
        SoundEffect.MasterVolume = soundVolume;
    }

    public void PlaySound(string soundName)
    {
        if (soundEffects.TryGetValue(soundName, out var sound))
            sound.Play();
        else
            System.Diagnostics.Debug.WriteLine($"Warning: Sound '{soundName}' not found");
    }

    public void PlayRandomFootstep()
    {
        if (footstepSounds.Count > 0)
            footstepSounds[random.Next(footstepSounds.Count)].Play();
        else
            System.Diagnostics.Debug.WriteLine("Warning: No footstep sounds loaded");
    }

    public void PlayRandomImpact()
    {
        if (impactSounds.Count > 0)
            impactSounds[random.Next(impactSounds.Count)].Play();
        else
            System.Diagnostics.Debug.WriteLine("Warning: No impact sounds loaded");
    }

    public void PlayBackgroundMusic(string songName = "PineappleUnderTheSea")
    {
        if (!songs.TryGetValue(songName, out var song))
        {
            System.Diagnostics.Debug.WriteLine($"Warning: Song '{songName}' not found");
            return;
        }

        // Always record intent, even if currently suspended.
        currentSongName = songName;
        currentSong = song;
        isMusicActive = true;

        // If the window is suspended, don't actually start the MediaPlayer;
        // SuspendAudio / ResumeAudio will handle it.
        if (isSuspended) return;

        // Same song already playing, nothing to do.
        if (MediaPlayer.State == MediaState.Playing && MediaPlayer.Queue.ActiveSong == song)
            return;

        MediaPlayer.Stop();
        MediaPlayer.Play(currentSong);
        MediaPlayer.IsRepeating = true;
    }

    public void StopBackgroundMusic()
    {
        isMusicActive = false;
        isSuspended = false;
        currentSong = null;
        currentSongName = null;
        MediaPlayer.Stop();
    }

    public void SuspendAudio()
    {
        if (isSuspended) return;   // already suspended
        isSuspended = true;

        if (isMusicActive && MediaPlayer.State == MediaState.Playing)
            MediaPlayer.Pause();
    }

    public void ResumeAudio()
    {
        if (!isSuspended) return;
        isSuspended = false;

        if (isMusicActive && currentSong != null)
        {
            if (MediaPlayer.State == MediaState.Paused)
                MediaPlayer.Resume();
            else
            {
                // MediaPlayer state was lost (some platforms stop on focus loss)
                MediaPlayer.Play(currentSong);
                MediaPlayer.IsRepeating = true;
            }
        }
    }

    public void PauseBackgroundMusic()
    {
        if (isMusicActive && MediaPlayer.State == MediaState.Playing)
            MediaPlayer.Pause();
    }

    public void ResumeBackgroundMusic()
    {
        if (isMusicActive && MediaPlayer.State == MediaState.Paused)
            MediaPlayer.Resume();
    }

    public bool IsBackgroundMusicPlaying => isMusicActive && !isSuspended;

    public void Update()
    {
        // Don't auto-restart while suspended or when music is intentionally off.
        if (!isMusicActive || isSuspended || currentSong == null) return;

        if (MediaPlayer.State == MediaState.Stopped)
        {
            MediaPlayer.Play(currentSong);
            MediaPlayer.IsRepeating = true;
        }
    }
}
