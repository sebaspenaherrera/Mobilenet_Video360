using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    // ******************************** FIELDS **************************************
    // Initial parameters (Should be loaded from the Player Prefs variables)
    private int mode = 0;
    private int maxIteration = 4;
    private int duration = 10;
    private string media_URL = "https://storage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4";
    //"https://cdn.bitmovin.com/content/assets/playhouse-vr/mpds/105560.mpd"
    private string rest_URL = "127.0.0.1:8000/Video360";
    private bool isCPEenabled = true;

    // Shared instance for GameManager resources access
    private static GameManager _sharedInstance;

    // Runtime variables

    private int counterIter = 0;
    private int counterTime = 0;
    private int counterCycles = 0;
    private int counterStall = 0;
    private int counterStallPerSec = 0;
    private int resSW = 0;
    private int resProfile = 0;
    private int videoHeight = 0;
    private double initTime = 0.00;
    private double counterFPSHMD = 0.00;
    private double avgFPSHMD = 0.00;
    private double maxFPSHMD = 0.00;
    private double minFPSHMD = 1000.00;
    private double currentFPS = 0.00;

    private bool mediaOpened = false;
    private bool firstSecond = false;

    private string experimentTimestamp = "";
    // Alias
    private PlayerManager player;
    private StatsManager stats;
    private AndroidTrafficStats androidStats;
    private RestManager rest;
    private CPEClient cpe;

    // ****************************** UNITY METHODS **********************************
    // Instance the shared instances 
    private void Awake()
    {
        _sharedInstance = this;
    }

    // Add a media track in the queue
    void Start()
    {
        // Enable all the alias
        stats = StatsManager.GetInstance();
        cpe = CPEClient.GetInstance();
        androidStats = AndroidTrafficStats.GetInstance();
        rest = RestManager.GetInstance();
        player = PlayerManager.GetInstance();
        player.AddMediaTrack(media_URL);

        // Activate/Deactivate the CPE client
        cpe.SetEnableCPE(isCPEenabled);
    }

    private void Update()
    {
        currentFPS = 1 / Time.unscaledDeltaTime;
    }

    // Control the workflow (Iteration --> Load Media + Play for n seconds + Stop and Close media)
    void FixedUpdate()
    {
        InstanceExperiment();
    }


    // ****************************** CLASS METHODS **********************************
    void InstanceExperiment()
    {
        // IF CPE has been enable wait until the authentication is done
        if(cpe.IsConnectedWithCPE() || !isCPEenabled) {
            // EXPERIMENT CODE
            // Control the number of iterations of the experiment (Starts from 0)
            if (counterIter < maxIteration)
            {
                // SESSION ITERATION 
                // Check the duration of a playback (Playback time < Duration = Active session)
                if (counterTime <= duration)
                {
                    // Check if the iteration has initiated the playback
                    if (!mediaOpened)
                    {
                        experimentTimestamp = GetUnixTimestamp();
                        player.LoadMediaAndStart();
                        mediaOpened = true;


                    }

                    // HERE goes all the tasks that can be made each frame update (1/nFrames)s
                    // Estimate stats per second
                    if (IsANewSecond())
                    {
                        InterSecondTasks();
                    }
                    // If an entire second has passed, average the instantaneous frame rates, reset the cycles counter
                    // and increase the time counter in one second. 
                    else
                    {
                        // Only take reference after passed the 1st second of each iteration
                        if (!firstSecond)
                        {
                            videoHeight = player.GetPlayerInfo().GetVideoHeight();
                            Debug.Log("Initial resolution = " + videoHeight);
                            firstSecond = true;
                        }
                        CountResolutionSwitches();
                        AverageFPS();
                        stats.UpdateStats();
                        // HERE goes the REST CODE. If DEMO mode is set send stats everysecond, else every session
                        HandleRestStats();
                        counterCycles = 0;
                        counterTime++;
                    }
                }
                else
                {
                    mediaOpened = player.CloseMediaPlayer();
                    // Try to send session stats if testbed mode is set, else ignore this line
                    SendSessionStats();
                    // Set the parameters for a new iteration
                    counterIter++;
                    counterTime = 0;
                    resProfile = 0;
                    resSW = 0;
                    firstSecond = false;
                    player.GetPlayerInfo().GetPlaybackQualityStats().Reset();
                    stats.ResetStats();

                }
                //

            }
            else
            {
                Application.Quit(0);
            }
        }
        
    }

    bool IsANewSecond()
    {
        // 50 cycles are equivalent to a second
        if ((counterCycles < (int)(1 / Time.fixedDeltaTime)) && (counterCycles >= 0))
        {
            return true;
        }
        // This is an 1-second update (Do everything needed to be done after 1 second period
        else
        {
            return false;
            
        }
    }

    void InterSecondTasks() {
        // If a fraction of second has passed, count stalled frames and estimate the instantaneous frame 
        CountStalls();
        CountFPS();
        counterCycles++;
    }

    void CountFPS()
    {
        // If it is a portion of second, sum the instantenous frame rate and determine maximum and minimum values
        // Each currentFPS values is calculated in Update() method but only request in each FixedUpdate() call
        counterFPSHMD += currentFPS;
        if (currentFPS > maxFPSHMD) { maxFPSHMD = currentFPS; }
        if (currentFPS < minFPSHMD) { minFPSHMD = currentFPS; }
    }

    
    void AverageFPS() {
        // If a cycle of 1/currentFPS has finished (1 second), calculate the average FPS value per a-second period
        avgFPSHMD = counterFPSHMD / (1 / Time.fixedDeltaTime);
        counterFPSHMD = 0;
    }


    void CountStalls()
    {
        if (player.IsStalled())
        {
            counterStall++;
        }
    }


    void CountResolutionSwitches() {
        // Get the current video height resolution and compare if any has occured
        int currentVideoHeight = player.GetPlayerInfo().GetVideoHeight();

        if (currentVideoHeight != videoHeight) {
            resSW++;

            // If the current resolution is higher, increase the profile, otherwise decrease
            if (currentVideoHeight > videoHeight)
            {
                resProfile++;
            }
            else {
                resProfile--;
            }

            // Overwrite the value for next call
            videoHeight = currentVideoHeight;
        } 
    }

    void HandleRestStats() {
        // Use the algorithm for Testbed or Demo mode
        // (Testbed = append stats, Demo = send stats per second, if possible)
        switch (mode)
        {
            // Case 0 - Testbed
            case 0:
                rest.AppendJson();
                break;
            case 1:
                string statsJson = rest.GetCurrentStatsInJson();
                //Todo SendStats()
                break;
        }
    }

    void SendSessionStats() {
        // If testbed mode is set, send stats to the rest server, else do nothing
        switch (mode)
        {
            case 0:
                Debug.Log("Session stats = " + rest.GetSessionStatsJson());
                //Todo SendStatsJson()
                break;
            case 1:
                break;
        }                
    }

    // *********************************** GETTER METHODS ***********************************************

    public static GameManager GetInstance() {
        return _sharedInstance;
    }

    public double GetInitTime() {
        return initTime;
    }

    public int GetStallCounter() {
        return counterStall;
    }

    public double GetStallTime(int counter)
    {
        return counter * Time.fixedDeltaTime;
    }

    public int CountStallPerSec()
    {
        int lastSecStall = counterStall - counterStallPerSec;
        counterStallPerSec = counterStall;

        return lastSecStall;
    }

    public int GetResolutionSwitchesCounter() {
        return resSW;
    }

    public int GetResolutionProfile() {
        return resProfile;
    }

    public double GetAverageScreenFrameRate() {
        return avgFPSHMD;
    }

    public double GetMaxScreenFrameRate() {
        return maxFPSHMD;
    }

    public double GetMinScreenFrameRate() {
        return minFPSHMD;
    }

    public string GetUnixTimestamp() {
        // Get the offset from current time in UTC time
        DateTimeOffset dto = new DateTimeOffset(DateTime.UtcNow);

        // Get the unix timestamp in seconds, and add the milliseconds
        return dto.ToUnixTimeMilliseconds().ToString();
    }

    public int GetIterationNumber() {
        return counterIter;
    }

    public int GetElapsedTime() {
        return counterTime;
    }

    public string GetConfiguredURL() {
        return media_URL;
    }

    public string GetRestURL() {
        return rest_URL;
    }

    // ******************************* SETTER METHODS *****************************
    public void SetInitTimeAsync(double value) {
        initTime = value;
    }

}
