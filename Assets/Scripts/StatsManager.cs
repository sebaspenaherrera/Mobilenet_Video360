using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StatsManager : MonoBehaviour
{
    //FIELDS
    private KQI_group stats;

    // Shared instance
    private static StatsManager _sharedInstance;

    // Alias
    private PlayerManager player;
    private AndroidTrafficStats androidStats;
    private GameManager gameManager;
    private LatencyManager latencyManager;


    void Awake()
    {
        _sharedInstance = this;
        stats = new KQI_group();
    }

    void Start()
    {
        // After initialized, pass the reference to the companion objects
        gameManager = GameManager.GetInstance();
        androidStats = AndroidTrafficStats.GetInstance();
        player = PlayerManager.GetInstance();
        latencyManager = LatencyManager.GetInstance();
    }

    public static StatsManager GetInstance() {
        return _sharedInstance;
    }

    public KQI_group GetStatsObject() {
        return stats;
    }

    public void ResetStats() {
        stats = new KQI_group();
    }

    public void UpdateStats()
    {
        // TIMES
        stats.timestamp = gameManager.GetUnixTimestamp();
        stats.initTime = gameManager.GetInitTime();
        stats.stallTime = gameManager.GetStallTime(gameManager.CountStallPerSec());
        stats.overallStallTime = gameManager.GetStallTime(gameManager.GetStallCounter());
        stats.bufferTime = player.GetBufferDuration();

        // NETWORK
        stats.rtt_ping = latencyManager.GetRTTPing();
        stats.rtt = latencyManager.GetRTT();
        stats.estimatedBWExoplayer = player.GetPlayerInfo().GetEstimatedTotalBandwidthUsed();
        stats.tx_rate = androidStats.GetTxRate();
        stats.rx_rate = androidStats.GetRxRate();
        stats.tx_packetRate = androidStats.GetTxPacketRate();
        stats.rx_packetRate = androidStats.GetRxPacketRate();

        // RESOLUTION
        stats.width = player.GetPlayerInfo().GetVideoWidth();
        stats.height = player.GetPlayerInfo().GetVideoHeight();
        stats.resolution = player.GetResolution();
        stats.res_switches = gameManager.GetResolutionSwitchesCounter();
        stats.res_profile = gameManager.GetResolutionProfile();

        // FRAME RATE
        stats.displayed_frameRate = player.GetPlayerInfo().GetVideoDisplayRate();
        stats.encoded_frameRate = player.GetPlayerInfo().GetVideoFrameRate();
        stats.screen_frameRate = gameManager.GetAverageScreenFrameRate();
        stats.max_screen_frameRate = gameManager.GetMaxScreenFrameRate();
        stats.min_screen_frameRate = gameManager.GetMinScreenFrameRate();

        // FRAME INFO
        stats.duplicatedFrames = player.GetPlayerInfo().GetPlaybackQualityStats().DuplicateFrames;
        stats.perfectFrames = player.GetPlayerInfo().GetPlaybackQualityStats().PerfectFramesT;
        stats.skippedFrames = player.GetPlayerInfo().GetPlaybackQualityStats().SkippedFrames;
        stats.unityDroppedFrames = player.GetPlayerInfo().GetPlaybackQualityStats().UnityDroppedFrames;

        // MEDIA INFO
        stats.playerDescription = player.GetPlayerInfo().GetPlayerDescription();
        stats.maxFrameNumber = player.GetPlayerInfo().GetMaxFrameNumber();
        stats.durationFrames = player.GetPlayerInfo().GetDurationFrames();
        stats.durationMedia = player.GetPlayerInfo().GetDuration();

        // PLAYER STATE
        stats.hasAudio = player.GetPlayerInfo().HasAudio();
        stats.hasVideo = player.GetPlayerInfo().HasVideo();
        stats.isStalled = player.IsStalled();
        stats.isPlaying = player.IsPlaying();
        stats.isBuffering = player.IsBuffering();
    }

}
