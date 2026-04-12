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
    private Song currentSong;
    private bool isMusicPlaying = false;

    public void LoadContent()
    {
        var content = RumGame.Instance.Content;

        // Load all sound effects
        soundEffects["click"] = content.Load<SoundEffect>("Audio/click_004");
        soundEffects["confirmation"] = content.Load<SoundEffect>("Audio/confirmation_002");
        soundEffects["error"] = content.Load<SoundEffect>("Audio/error_008");
        soundEffects["switch"] = content.Load<SoundEffect>("Audio/switch_002");

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

