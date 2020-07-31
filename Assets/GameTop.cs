using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Networking;

public class GameTop : MonoBehaviour
{
    [SerializeField] public string PlayerName = "Mystery Rustler";
    public int BoardSize = 3;
    public float SecondsPerClick = 6f;
    public float SecondsPerBoard = 20f;
    public int ConsecutiveWins = 0;
    public int GameScore = 0;
    [SerializeField] public AudioSource audioSource_SFX;
    [SerializeField] public AudioSource audioSource_Music;
    public int currentVolume = 1;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        SetSoundLevel(1);
    }

    public void ReportBoardWin(int newTotalScore) {
        ConsecutiveWins++;
        PlaySound("win");
        GameScore = newTotalScore;
        if (BoardSize < 6) {
            BoardSize++;
        }
    }

    public void ReportBoardLoss(int FinalScore) {
        PlaySound("lose");
        if (FinalScore > 0) {
            StartCoroutine(SendScoreToLeaderboard(FinalScore));
        }
        BoardSize = 3;
        ConsecutiveWins = 0;
        GameScore = 0;
    }

    IEnumerator SendScoreToLeaderboard(int Score) {
        string uri = "infamy.dev/highscore/add?id=969ece60-d2cd-11ea-838d-8f06b0af3ca9&name=" + PlayerName + "&score=" + Score;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                Debug.Log(pages[page] + ": Error: " + webRequest.error);
            }
            else
            {
                Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
            }
        }
    }

    public void PlaySound(string clipName)
    {
        AudioClip audioClip = GetAudioClipByName(clipName);
        if (audioClip != null) {
            audioSource_SFX.PlayOneShot(audioClip);
        } else {
            Debug.Log("null audio clip: " + clipName);
        }
    }
    public void StopSound(){
        audioSource_SFX.Stop();
    }
    public void PlayMusic(string songName) {
        AudioClip audioClip = GetSongClipByName(songName);
        if (audioClip != null) {
            audioSource_Music.Stop();
            audioSource_Music.clip = audioClip;
            audioSource_Music.Play();
        } else {
            Debug.Log("null audio clip: " + songName);
        }
    }

    AudioClip GetAudioClipByName(string clipName)
    {
        return (AudioClip)Resources.Load("Sounds/" + clipName);
    }

    AudioClip GetSongClipByName(string clipName)
    {
        return (AudioClip)Resources.Load("Songs/" + clipName);
    }

    public void SetSoundLevel(int volumeLevel) {
        currentVolume = volumeLevel;
        if (volumeLevel == 0) {
            audioSource_Music.volume = 0f;
            audioSource_SFX.volume = 0f;
        }
        if (volumeLevel == 1) {
            audioSource_Music.volume = 0.1f;
            audioSource_SFX.volume = 0.15f;
        }
        if (volumeLevel == 2) {
            audioSource_Music.volume = 0.2f;
            audioSource_SFX.volume = 0.225f;
        }
        if (volumeLevel == 3) {
            audioSource_Music.volume = 0.5f;
            audioSource_SFX.volume = 0.4f;
        }
    }
}
