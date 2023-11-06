using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RenderHeads.Media.AVProVideo;
using System.Net.NetworkInformation;
using System.Linq;


public class PlayerManager : MonoBehaviour
{

    // *************************** FIELDS *********************************************
    public TMPro.TMP_Text initTimetxt;

    // AVPRO Video MediaPlayerObjects
    private GameObject playerObject;
    private MediaPlayer mediaPlayer;
    private List<MediaPath> mediaPathList = new List<MediaPath>();

    // Runtime variables
    private double elapsedInitTime = 0.00;
    private bool mediaOpened = false;

    // Shared instance to share some resources along managers
    private static PlayerManager _sharedInstance;

    // Alias
    private GameManager gameManager;


    // ************************** UNITY METHODS *************************************
    private void Awake()
    {
        // Create the shared instance
        _sharedInstance = this;

        // Get the MediaPlayer Compoenent from the MediaPlayer Object
        playerObject = GameObject.FindGameObjectWithTag("VideoPlayer");
        mediaPlayer = playerObject.GetComponent<MediaPlayer>();
        mediaPlayer.AutoStart = false;
    }

    private void Start()
    {
        gameManager = GameManager.GetInstance();
    }

    // ************************* CLASS METHODS **************************************

    // Fetch the resources from this class through the shared instance
    public static PlayerManager GetInstance() {
        return _sharedInstance;
    } 

    // Add URL to the list of track to be displayed
    public void AddMediaTrack(string url)
    {
        MediaPath media = new MediaPath(url, MediaPathType.AbsolutePathOrURL);
        mediaPathList.Add(media);
    }

    // Get the list of Media tracks to be displayed
    List<MediaPath> GetMediaPathList()
    {
        return mediaPathList;
    }

    // Open the Media track identified with the index, also try to disable the caching
    void OpenMedia(int index) {
        Debug.Log("Trying to load: " + GetMediaPathList()[index]);
        mediaPlayer.OpenMedia(GetMediaPathList()[index], autoPlay: false);
        mediaPlayer.Cache.RemoveMediaFromCache(mediaPathList[index].GetResolvedFullPath());
    }

    // Close media track and remove the canche, just in case :P
    public bool CloseMediaPlayer()
    {
        mediaPlayer.Stop();
        mediaPlayer.Cache.RemoveMediaFromCache(mediaPlayer.MediaPath.GetResolvedFullPath());
        mediaPlayer.CloseMedia();

        if (!mediaPlayer.MediaOpened) mediaOpened = false;

        return mediaOpened;
    }

    // Force the player to play the track
    public void StartPlayback()
    {
        if (!mediaPlayer.Control.IsPlaying())
        {
            mediaPlayer.Play();
        }

    }


    // GETTERS METHODS
    public IMediaInfo GetPlayerInfo() {
        return mediaPlayer.Info;
    }

    public double GetBufferDuration() {
        return mediaPlayer.Control.GetBufferedTimes().Duration;
    }

    public string GetResolution() {
        return $"{mediaPlayer.Info.GetVideoWidth()}x{mediaPlayer.Info.GetVideoHeight()}";
    }

    void UpdatePlayerStats()
    {
        double mediaDuration = mediaPlayer.Info.GetDuration();
        int mediaDurationFrames = mediaPlayer.Info.GetDurationFrames();
        long estimatedBWUsed = mediaPlayer.Info.GetEstimatedTotalBandwidthUsed();
        int maxSeekFrame = mediaPlayer.Info.GetMaxFrameNumber();
        int duplicateFrames = mediaPlayer.Info.GetPlaybackQualityStats().DuplicateFrames;
        float perfectFrames = mediaPlayer.Info.GetPlaybackQualityStats().PerfectFramesT;
        int skippedFrames = mediaPlayer.Info.GetPlaybackQualityStats().SkippedFrames;
        int unityDroppedFrames = mediaPlayer.Info.GetPlaybackQualityStats().UnityDroppedFrames;
        string playerDescription = mediaPlayer.Info.GetPlayerDescription();
        float videoDisplayRate = mediaPlayer.Info.GetVideoDisplayRate();
        float videoFrameRate = mediaPlayer.Info.GetVideoFrameRate();
        int videoHeight = mediaPlayer.Info.GetVideoHeight();
        int videoWidth = mediaPlayer.Info.GetVideoWidth();
        bool hasAudio = mediaPlayer.Info.HasAudio();
        bool hasVideo = mediaPlayer.Info.HasVideo();
        bool isStalled = mediaPlayer.Info.IsPlaybackStalled();
        double bufferDuration = mediaPlayer.Control.GetBufferedTimes().Duration;

    }

    // STATE METHODS
    public bool HasMediaLoaded() {
        return mediaPlayer.MediaOpened;
    }

    public bool IsPlaying() {
        return mediaPlayer.Control.IsPlaying();
    }

    public bool IsBuffering() {
        return mediaPlayer.Control.IsBuffering();
    }

    public bool IsPaused() {
        return mediaPlayer.Control.IsPaused();
    }

    public bool IsSeeking() {
        return mediaPlayer.Control.IsSeeking();
    }

    public bool IsFinished() {
        return mediaPlayer.Control.IsFinished();
    }

    public bool IsStalled() {
        return mediaPlayer.Info.IsPlaybackStalled();
    }

    public void LoadMediaAndStart() {
        StartCoroutine("EstimateInitialTime");
    }

    // Function that must be run in a couroutine
    // This allows opening and playing the track. Also measures the period
    // throughout this function is called and the IsPlaying flag is switched
    // to true
    private IEnumerator EstimateInitialTime(){
        double initTime = Time.realtimeSinceStartupAsDouble;

        // Open the first video in the list of media sources and start playback
        OpenMedia(0);
        StartPlayback();
        // Wait until the video starts playbacking
        yield return new WaitWhile(() => !mediaPlayer.Control.IsPlaying());
        // Calculate the time elapsed since the video was requested
        elapsedInitTime = Time.realtimeSinceStartupAsDouble - initTime;
        gameManager.SetInitTimeAsync(elapsedInitTime);
        Debug.Log("Init time = " + elapsedInitTime);
    }
}
