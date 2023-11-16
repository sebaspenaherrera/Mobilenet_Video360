using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    #region ATTRIBUTES
    // UI Elements
    [Header("RESOLUTION")]
    public TMPro.TMP_Text resolution_txt;
    public TMPro.TMP_Text resSwitches_txt;
    public TMPro.TMP_Text resProfile_txt;

    [Header("FRAME RATE")]
    public TMPro.TMP_Text displayRate_txt;
    public TMPro.TMP_Text screenRate_txt;

    [Header("NETWORK")]
    public TMPro.TMP_Text rtt_txt;
    public TMPro.TMP_Text rttPing_txt;
    public TMPro.TMP_Text txRate_txt;
    public TMPro.TMP_Text rxRate_txt;
    public TMPro.TMP_Text cpeStatus_txt;

    [Header("TIMES")]
    public TMPro.TMP_Text initTime_txt;
    public TMPro.TMP_Text stallTime_txt;
    public TMPro.TMP_Text bufferTime_txt;

    [Header("CONTROL")]
    public TMPro.TMP_Text iteration_txt;
    public TMPro.TMP_Text elapsedTime_txt;

    // Alias
    private StatsManager stats;
    private GameManager gameManager;
    private CPEClient cpe;

    // Shared instance
    private static UIManager _sharedInstance;

    #endregion

    #region UNITY METHODS

    // Tasks to be done after initializing this game object
    private void Awake()
    {
        _sharedInstance = this;
        
    }

    // Start is called before the first frame update
    void Start()
    {
        stats = StatsManager.GetInstance();
        gameManager = GameManager.GetInstance();
        cpe = CPEClient.GetInstance();
        InvokeRepeating("UpdateText", 0.0f, 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion

    #region STATIC METHODS
    public static UIManager GetInstance() {
        return _sharedInstance;
    }
    #endregion

    #region CLASS METHODS
    public void InitializeUI() {
        // Reset stats values
        stats.ResetStats();

        // Update text fields with values in the stats manager
        UpdateText();
        
    }

    void UpdateText() {
        // Write the values onto the text fields

        // RESOLUTION
        resolution_txt.text = "Resolution = " + stats.GetStatsObject().resolution;
        resSwitches_txt.text = "Res. switches = " + stats.GetStatsObject().res_switches.ToString();
        resProfile_txt.text = "Res. profile = " + stats.GetStatsObject().res_profile.ToString();

        // FRAME RATE
        displayRate_txt.text = "Display rate = " + stats.GetStatsObject().displayed_frameRate.ToString("0.000");
        screenRate_txt.text = "Screen rate = " + stats.GetStatsObject().screen_frameRate.ToString("0.000");

        // NETWORK
        rtt_txt.text = "Latency = " + stats.GetStatsObject().rtt.ToString("0.000");
        rttPing_txt.text = "Ping = " + stats.GetStatsObject().rtt_ping.ToString("0.000");
        txRate_txt.text = "Tx Rate = " + stats.GetStatsObject().tx_rate.ToString("0.000");
        rxRate_txt.text = "Rx Rate = " + stats.GetStatsObject().rx_rate.ToString("0.000");
        cpeStatus_txt.text = "IsMonitoringCPE = " + cpe.IsMonitoringCPE();

        // TIMES
        initTime_txt.text = "Init time = " + stats.GetStatsObject().initTime.ToString("0.000");
        stallTime_txt.text = "Stall time = " + stats.GetStatsObject().overallStallTime.ToString("0.000");
        bufferTime_txt.text = "Buffer time = " + stats.GetStatsObject().bufferTime.ToString("0.000");

        // CONTROL
        iteration_txt.text = "Iteration = " + gameManager.GetIterationNumber().ToString();
        elapsedTime_txt.text = "Elapsed time = " + gameManager.GetElapsedTime().ToString();
    }
    #endregion
}
