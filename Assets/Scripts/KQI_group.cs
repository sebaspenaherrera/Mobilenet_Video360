using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is a skeleton for all the metrics measured by this client
public class KQI_group
{
    // THE KQIs are organized by categories. If new KQIs are added, please
    // don't make a mess!!

    // TIMES
    public string timestamp = "";
    public double initTime = 0.00;
    public double stallTime = 0.00;
    public double overallStallTime = 0.00;
    public double bufferTime = 0.00;

    // NETWORK
    public double rtt_ping = 0.00;
    public double rtt = 0.00;
    public double estimatedBWExoplayer = 0.00;
    public double tx_rate = 0.00;
    public double rx_rate = 0.00;
    public long tx_packetRate = 0;
    public long rx_packetRate = 0;

    // RESOLUTION
    public int width = 0;
    public int height = 0;
    public string resolution = "0x0";
    public int res_switches = 0;
    public int res_profile = 0;

    // FRAME RATE
    public double displayed_frameRate = 0.00;
    public double encoded_frameRate = 0.00;
    public double screen_frameRate = 0.00;
    public double min_screen_frameRate = 1000.0;
    public double max_screen_frameRate = 0.00;

    // FRAME INFO
    public int duplicatedFrames = 0;
    public float perfectFrames = 0;
    public int skippedFrames = 0;
    public int unityDroppedFrames = 0;

    // MEDIA INFO
    public string playerDescription = "";
    public int maxFrameNumber = 0;
    public int durationFrames = 0;
    public double durationMedia = 0.00;

    // PLAYER STATE
    public bool hasAudio = false;
    public bool hasVideo = false;
    public bool isStalled = false;
    public bool isPlaying = false;
    public bool isBuffering = false;

    // DEFAULT CONSTRUCTOR
    public KQI_group() {

    }
}
