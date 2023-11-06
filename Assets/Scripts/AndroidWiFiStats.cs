using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndroidWiFiStats : MonoBehaviour
{
    // Public fields ----------------------------------------------------------
    public TMPro.TMP_Text tx;
    public TMPro.TMP_Text rx;

    // Private fields ---------------------------------------------------------
    private double _txrate = 0;
    private double _rxrate = 0;
    private long _initialtx = 0;
    private long _initialrx = 0;
    private long _txbytes = 0;
    private long _rxbytes = 0;
    private float measInterval = 1.0f; // Interval for thrp calculations
    private AndroidJavaObject networkStatsHelper;

    // ****************************** METHODS *********************************
    private void Awake()
    {
        InitializeTrafficValues();
        Application.runInBackground = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        networkStatsHelper = new AndroidJavaObject("com.example.networkingbridge.NetworkingHelper", new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"));
        // Update the tx and rx bytes values and calculate rates
        InvokeRepeating("GetThrpStats", 1.0f, measInterval);
    }

    // Update is called once per frame
    void Update()
    {

    }

    struct trafficValues {
        public long txbytes;
        public long rxbytes;
        public long txpackets;
        public long rxpackets;
    }

    private trafficValues MapLongArrayToStruct(long[] array) {
        trafficValues _stats = new trafficValues();

        _stats.txbytes = array[0];
        _stats.rxbytes = array[1];
        _stats.txpackets = array[2];
        _stats.rxpackets = array[3];

        return _stats;
    }

    private trafficValues GetTrafficStatsFromAndroid()
    {
        trafficValues _trafficValues = new trafficValues();
        

        // If the runtime platform is Android, use Java-wrapped functions for Android
        if (Application.platform == RuntimePlatform.Android)
        {


            // Fetch txbytes and rxbytes values using the Android functions wrapped in the Java Library
            _trafficValues = MapLongArrayToStruct(networkStatsHelper.Call<long[]>("getWifiStats"));

        }
        else {
            _trafficValues.txbytes = -1;
            _trafficValues.rxbytes = -1;
        }

        return _trafficValues;

    }

    void GetThrpStats() {
        // Get the current values of tx and rx bytes in the interface, so it's possible to calculate metrics from this 
        trafficValues _trafficValues = GetTrafficStatsFromAndroid();

        // Calculate the difference in volumetric traffic in a period of time, which is the actual throughput in Kbps
        _txrate = ((_trafficValues.txbytes - _txbytes) * 8) / 10e3;
        _rxrate = ((_trafficValues.rxbytes - _rxbytes) * 8) / 10e3;

        // Update the last rx and tx bytes values
        _txbytes = _trafficValues.txbytes;
        _rxbytes = _trafficValues.rxbytes;

        // Update screen
        tx.text = _txrate.ToString();
        rx.text = _rxrate.ToString();
    }

    void InitializeTrafficValues() {

        // Get the first values of tx and rx bytes in the interface, so it's possible to calculate metrics from this 
        trafficValues _initialTrafficValues = GetTrafficStatsFromAndroid();
        _initialtx = _initialTrafficValues.txbytes;
        _initialrx = _initialTrafficValues.rxbytes;

        // Initialize the relative variables
        _txbytes = _initialtx;
        _rxbytes = _initialrx;
    }

    trafficValues GetTotalTraffic() {
        // Calculate the amount of bytes transferred from the initial values
        trafficValues _currentTraffic = new trafficValues();
        trafficValues _trafficValues = GetTrafficStatsFromAndroid();

        _currentTraffic.txbytes = _trafficValues.txbytes - _initialtx;
        _currentTraffic.rxbytes = _trafficValues.rxbytes - _initialrx;

        return _currentTraffic;
    }

    void ResetReference() {

        // Overwrite the tx and rx values with the last measured ones (This can be used to calculate stats for each session
        _initialtx = _txbytes;
        _initialrx = _rxbytes;
    }

    void CancelCalculations()
    {
        CancelInvoke("GetThrpStats");
    }

    void ForceCalculations()
    {
        InvokeRepeating("GetThrpStats", 0.0f, measInterval);
    }
}
