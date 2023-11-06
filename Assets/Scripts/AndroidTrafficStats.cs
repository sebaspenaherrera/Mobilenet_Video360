using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndroidTrafficStats : MonoBehaviour
{
    // ********************************* FIELDS ************************************
    // Public fields ----------------------------------------------------------
    public TMPro.TMP_Text tx;
    public TMPro.TMP_Text rx;

    // Private fields ---------------------------------------------------------
    private double _txrate = 0;
    private double _rxrate = 0;
    private long _txpacketrate = 0;
    private long _rxpacketrate = 0;
    private long _initialtx = 0;
    private long _initialrx = 0;
    private long _initialtxpacket = 0;
    private long _initialrxpacket = 0;
    private long _txbytes = 0;
    private long _rxbytes = 0;
    private long _txpackets = 0;
    private long _rxpackets = 0;
    private float measInterval = 1.0f; // Interval for thrp calculations

    // Shared instance
    private static AndroidTrafficStats _sharedInstance;

    // ****************************** UNITY METHODS *********************************
    private void Awake()
    {
        _sharedInstance = this;
        InitializeTrafficValues();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Update the tx and rx bytes values and calculate rates
        InvokeRepeating("GetTrafficStats", 1.0f, measInterval);
    }


    // ****************************** INTERNAL METHODS *********************************
    struct trafficValues {
        public long txbytes;
        public long rxbytes;
        public long txpackets;
        public long rxpackets;
    }

    private trafficValues MapLongArrayToStruct(long[] array)
    {
        // Map each value in the array to a variable. The order is defined by the JAVA Wrapper
        // library that handles the values from the Android S.O.
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

            // Import Java classes to the context of Unity
            //AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            //AndroidJavaObject unityActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject androidTrafficStats = new AndroidJavaObject("com.example.networkstatsandroid.NetworkStatsAndroid");

            // Fetch txbytes and rxbytes values using the Android functions wrapped in the Java Library
            _trafficValues = MapLongArrayToStruct(androidTrafficStats.CallStatic<long[]>("getStats"));
        }
        else {
            _trafficValues.txbytes = -1;
            _trafficValues.rxbytes = -1;
            _trafficValues.txpackets = -1;
            _trafficValues.rxpackets = -1;
        }

        return _trafficValues;

    }


    void GetTrafficStats() {
        // Get the current values of tx and rx bytes and packets in the interface, so it's possible to calculate metrics from this 
        trafficValues _trafficValues = GetTrafficStatsFromAndroid();

        // Calculate the difference in volumetric traffic in a period of time, which is the actual throughput in Kbps
        // The packet rate is in 10e3 scale (kilo packets per second)
        _txrate = ((_trafficValues.txbytes - _txbytes) * 8) / 10e3;
        _rxrate = ((_trafficValues.rxbytes - _rxbytes) * 8) / 10e3;
        _txpacketrate = ((_trafficValues.txpackets - _txpackets));
        _rxpacketrate = ((_trafficValues.rxpackets - _rxpackets));

        // Update the last rx and tx values
        _txbytes = _trafficValues.txbytes;
        _rxbytes = _trafficValues.rxbytes;
        _txpackets = _trafficValues.txpackets;
        _rxpackets = _trafficValues.rxpackets;

        // Update screen
        tx.text = _txrate.ToString();
        rx.text = _rxrate.ToString();

        // TODO implement the statsManager to handle all the KQIs in a single entity
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

    void GetPacketStats()
    {
        // Get the current values of tx and rx packets in the interface, so it's possible to calculate metrics from this 
        trafficValues _trafficValues = GetTrafficStatsFromAndroid();

        // Calculate the difference in volumetric traffic in a period of time, which is the actual throughput in 10e3 packets
        _txpacketrate = ((_trafficValues.txpackets - _txpackets));
        _rxpacketrate = ((_trafficValues.rxpackets - _rxpackets));

        // Update the last rx and tx packet values
        _txpackets = _trafficValues.txpackets;
        _rxpackets = _trafficValues.rxpackets;

        // Update screen
        tx.text = _txrate.ToString();
        rx.text = _rxrate.ToString();

    }

    void InitializeTrafficValues() {

        // Get the first values of tx and rx bytes in the interface, so it's possible to calculate metrics from this 
        trafficValues _initialTrafficValues = GetTrafficStatsFromAndroid();
        _initialtx = _initialTrafficValues.txbytes;
        _initialrx = _initialTrafficValues.rxbytes;
        _initialtxpacket = _initialTrafficValues.txpackets;
        _initialrxpacket = _initialTrafficValues.rxpackets;

        // Initialize the relative variables
        _txbytes = _initialtx;
        _rxbytes = _initialrx;
        _txpackets = _initialtxpacket;
        _rxpackets = _initialrxpacket;
    }

    trafficValues GetTotalTraffic() {
        // Calculate the amount of bytes transferred from the initial values
        trafficValues _currentTraffic = new trafficValues();
        trafficValues _trafficValues = GetTrafficStatsFromAndroid();

        _currentTraffic.txbytes = _trafficValues.txbytes - _initialtx;
        _currentTraffic.rxbytes = _trafficValues.rxbytes - _initialrx;
        _currentTraffic.txpackets = _trafficValues.txpackets - _initialtxpacket;
        _currentTraffic.rxpackets = _trafficValues.rxpackets - _initialrxpacket;

        return _currentTraffic;
    }

    void ResetReference() {

        // Overwrite the tx and rx values with the last measured ones (This can be used to calculate stats for each session
        _initialtx = _txbytes;
        _initialrx = _rxbytes;
        _initialtxpacket = _txpackets;
        _initialrxpacket = _rxpackets;
    }

    void CancelTrafficCalculations()
    {
        CancelInvoke();
    }

    void ForceTrafficCalculations()
    {
        InvokeRepeating("GetTrafficStats", 0.0f, measInterval);
    }

    void ForceOnlyThrpCalculations()
    {
        InvokeRepeating("GetThrpStats", 0.0f, measInterval);
    }

    // ****************************** GETTER METHODS *********************************
    public double GetTxRate() {
        return _txrate;
    }

    public double GetRxRate() {
        return _rxrate;
    }

    public long GetTxPacketRate() {
        return _txpacketrate;
    }

    public long GetRxPacketRate() {
        return _rxpacketrate;
    }

    public long GetTxBytes() {
        return _txbytes;
    }

    public long GetRxBytes() {
        return _rxbytes;
    }

    public long GetTxPackets() {
        return _txpackets;
    }

    public long GetRxPackets() {
        return _rxpackets;
    }

    // ****************************** STATIC METHODS *********************************
    public static AndroidTrafficStats GetInstance() {
        return _sharedInstance;
    }
}
