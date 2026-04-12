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
    private Song currentSong;
    private bool isMusicPlaying = false;
    private Random random = new Random();

    public void LoadContent()
    {
        var content = RumGame.Instance.Content;

        // Load all sound effects
        soundEffects["click"] = content.Load<SoundEffect>("Audio/click_004");
        soundEffects["confirmation"] = content.Load<SoundEffect>("Audio/confirmation_002");
        soundEffects["error"] = content.Load<SoundEffect>("Audio/error_008");
        soundEffects["switch"] = content.Load<SoundEffect>("Audio/switch_002");

        // Load footstep sounds
        for (int i = 0; i < 5; i++)
        {
            footstepSounds.Add(content.Load<SoundEffect>($"Audio/footstep_grass_00{i}"));
        }

        // Load impact mining sounds
        for (int i = 0; i < 5; i++)
        {
            impactSounds.Add(content.Load<SoundEffect>($"Audio/impactMining_00{i}"));
        }

        // Load all songs
        songs["PineappleUnderTheSea"] = content.Load<Song>("Audio/PineappleUnderTheSea");
        songs["WhatCloudsAreMadeOf"] = content.Load<Song>("Audio/WhatCloudsAreMadeOf");
        songs["GentleBreeze"] = content.Load<Song>("Audio/GentleBreeze");
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

    public void PlayRandomFootstep()
    {
        if (footstepSounds.Count > 0)
        {
            int randomIndex = random.Next(footstepSounds.Count);
            footstepSounds[randomIndex].Play();
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("Warning: No footstep sounds loaded");
        }
    }

    public void PlayRandomImpact()
    {
        if (impactSounds.Count > 0)
        {
            int randomIndex = random.Next(impactSounds.Count);
            impactSounds[randomIndex].Play();
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("Warning: No impact sounds loaded");
        }
    }

    public void PlayBackgroundMusic(string songName = "PineappleUnderTheSea")
    {
        if (!songs.TryGetValue(songName, out var song))
        {
            System.Diagnostics.Debug.WriteLine($"Warning: Song '{songName}' not found");
            return;
        }

        // If the same song is already playing, don't restart it
        if (currentSong == song && isMusicPlaying)
        {
            return;
        }

        // Stop current music if switching to a different song
        if (isMusicPlaying)
        {
            MediaPlayer.Stop();
        }

        currentSong = song;
        MediaPlayer.Play(currentSong);
        MediaPlayer.IsRepeating = true;
        isMusicPlaying = true;
    }

    public void StopBackgroundMusic()
    {
        if (isMusicPlaying)
        {
            MediaPlayer.Stop();
            isMusicPlaying = false;
            currentSong = null;
        }
    }

    public void PauseBackgroundMusic()
    {
        if (isMusicPlaying)
        {
            MediaPlayer.Pause();
        }
    }

    public void ResumeBackgroundMusic()
    {
        if (isMusicPlaying)
        {
            MediaPlayer.Resume();
        }
    }

    public bool IsBackgroundMusicPlaying => isMusicPlaying;
}
