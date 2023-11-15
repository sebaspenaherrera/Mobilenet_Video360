using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HuaweiAPI;
using System.Xml;
using SimpleJSON;
using System.Threading;

public class CPEClient : MonoBehaviour
{
    #region FIELDS
    // Private fields
    /*string ip_cpe = "192.168.8.1";
    string username = "admin";
    string password = "areyouready?1";
    string statsJSON = "";
    double expectedDelay = 1.00;
    int max_attempts = 3;*/
    string ip_cpe = "";
    string username = "";
    string password = "";
    string statsJSON = "";
    double expectedDelay;
    int max_attempts;

    //State fields
    private bool isLoggedCPE = false;
    private bool isMonitoring = false;
    private bool isCPEenabled = false;

    //Alias
    private StatsManager stats;

    // Shared instances
    private static CPEClient _sharedInstance;
    #endregion

    #region UNITY METHODS
    private void Awake()
    {
        // Set the shared instance for external access
        _sharedInstance = this;
        // Set the statsManager instance alias for stats writing
        stats = StatsManager.GetInstance();
    }

    private void Start()
    {
        if (isCPEenabled)
        {
            // Start the CPE stats client only if the CPE option has been set by the GameManager
            StartCoroutine(Run_CPEMonitoring());
        }
        
    }


    #endregion

    #region CLASS METHODS
    public IEnumerator Run_CPEMonitoring() {
        // Start with authentication
        yield return this.UserLogin(ip_cpe, username, password, max_attempts, state => { isLoggedCPE = state; });

        if (isLoggedCPE) {
            while (true) {
                yield return this.CPEMonitoring(ip_cpe, username, password, max_attempts, expectedDelay, callback => { isMonitoring = callback; }, callback2 => { statsJSON = callback2; });
            }
            
        }
    }

    public bool IsConnectedWithCPE() {
        return isLoggedCPE;
    }

    public bool IsMonitoringCPE() {
        return isMonitoring;
    }

    public string GetCPEStats() {
        return statsJSON;
    }

    public void SetEnableCPE(bool enable) {
        isCPEenabled = enable;
    }

    public void SetCPEFields(string ip, string username, string password, int interval, int max_attempts) {
        this.ip_cpe = ip;
        this.username = username;
        this.password = password;
        this.expectedDelay = interval;
        this.max_attempts = max_attempts;
    }

    public static CPEClient GetInstance() {
        return _sharedInstance;
    }

    #endregion
}