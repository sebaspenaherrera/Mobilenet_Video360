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
    string ip_cpe = "192.168.8.1";
    string username = "admin";
    string password = "areyouready?1";
    string data = "";
    double expectedDelay = 1.00;
    int max_attempts = 3;

    // Shared instances
    private static CPEClient _sharedInstance;
    #endregion

    #region UNITY METHODS
    private void Awake()
    {
        _sharedInstance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        string stateConnection = "";
        //StartCoroutine(this.CheckConnection(ip_cpe, state => { stateConnection = state.ToString(); }));
        StartCoroutine(this.UserLogin(ip_cpe, username, password, state => { stateConnection = state.ToString(); }));
        Debug.Log($"LA CONEXION EN EL MAIN THREAD ES = {stateConnection}");
        //StartCoroutine(GetCPEData());
    }

    #endregion

    #region COROUTINES
    /*IEnumerator GetCPEData()
    {
        // Initialize the timer
        WaitForSecondsRealtime waitTime;

        while (true)
        {
            // Create a request and a response recipient
            double timeRequest = Time.realtimeSinceStartupAsDouble;
            yield return data = RequestCPEStats();

            // Extract the value of ping request
            double timeSpent = Time.realtimeSinceStartup - timeRequest;
            double waitTimeValue = expectedDelay - timeSpent;

            // Calculate the time to wait for new request
            if (waitTimeValue <= 1.00)
            {
                waitTime = new WaitForSecondsRealtime((float)waitTimeValue);
            }
            else if (waitTimeValue == 0.00)
            {
                waitTime = new WaitForSecondsRealtime(0.00f);
            }
            else
            {
                waitTime = new WaitForSecondsRealtime((float)(waitTimeValue + expectedDelay));
            }

            // Suspend the thread until the time passes
            yield return waitTime;
        }
    }*/

   /* IEnumerator isConnected(System.Action<bool> callback)
    {
        // Check the connection status with the CPE API
        
        if (HuaweiAPI.HuaweiAPI.MethodExample.loginState(ip_cpe) == true)
        {
            Debug.Log("(CPE Manager) --> Already logged in.");
            yield return true;
        }
        // If it is not connected, try to connect
        else
        {
            int x = 0;
            bool status = false;

            while (x < max_attempts)
            {

                //not logged in
                Debug.Log($"(CPE Manager) --> Not logged in, logging in... attempt {x + 1}");
                bool login = ConnectToCPE();
                if (login == false)
                {
                    Debug.Log("(CPE Manager) --> Failed to log in.");
                    status = false;
                    continue;
                }
                else
                {
                    // If was possible to log with the CPE API, break the loop
                    Debug.Log("(CPE Manager) --> Logged");
                    status = true;
                    x = max_attempts;
                    break;
                }
            }
            return status;
        }
    }
   */
    bool CallLoginState() {
        bool loginState = false;

        Debug.Log("(CPE Manager) --> Checking login state...");
        StartCoroutine(LoginStateAsync(result => {
            loginState = result;
        }));

        return loginState;
    }

    IEnumerator LoginStateAsync(System.Action<bool> callback) {
        bool loginState = HuaweiAPI.HuaweiAPI.MethodExample.loginState(ip_cpe);
        yield return null;
        callback.Invoke(loginState);
    }

    IEnumerator RequestCPEStats()
    {
        bool isConnected = false;
        // Check if the there is connection with the CPE API
        JSONNode jsonNode = new JSONObject();

        yield return isConnected = CallLoginState();

        if(isConnected)
        {
            // Request the signal, traffic and device info
            JSONNode jsonSignal = Get_SignalStats_internal();
            JSONNode jsonTraffic = Get_TrafficStats_internal();
            JSONNode jsonDeviceInfo = Get_DeviceStatus_internal();

            // Wrap each request with a json node
            jsonNode["signal"] = jsonSignal;
            jsonNode["traffic"] = jsonTraffic;
            jsonNode["info"] = jsonDeviceInfo;

            // Reset CPE stats
            JSONNode jsonResponse = ResetTrafficStats_internal();

            // Return the string version of the json object
            data = jsonNode.ToString();
        }
    }

    /*IEnumerator isConnected(System.Action<bool> callback)
    {
        // Check the connection status with the CPE API
        Debug.Log("(CPE Manager) --> Checking login state...");
        if (HuaweiAPI.HuaweiAPI.MethodExample.loginState(ip_cpe) == true)
        {
            Debug.Log("(CPE Manager) --> Already logged in.");
            yield return true;
        }
        // If it is not connected, try to connect
        else
        {
            int x = 0;
            bool status = false;

            while (x < max_attempts)
            {

                //not logged in
                Debug.Log($"(CPE Manager) --> Not logged in, logging in... attempt {x + 1}");
                bool login = ConnectToCPE();
                if (login == false)
                {
                    Debug.Log("(CPE Manager) --> Failed to log in.");
                    status = false;
                    continue;
                }
                else
                {
                    // If was possible to log with the CPE API, break the loop
                    Debug.Log("(CPE Manager) --> Logged");
                    status = true;
                    x = max_attempts;
                    break;
                }
            }
            return status;
        }
    }*/
    #endregion

    #region INTERNAL METHODS
    // Private methods




    bool ConnectToCPE() {
        return HuaweiAPI.HuaweiAPI.MethodExample.UserLogin(ip_cpe, username, password);
    }

    private JSONNode Get_SignalStats_internal()
    {
        XmlElement signal = HuaweiAPI.HuaweiAPI.Tools.GET(ip_cpe, "api/device/signal").DocumentElement;
        JSONNode jsonSignal = Xml_group.ConvertStatsXmlToJson(signal);

        return jsonSignal;
    }


    private JSONNode Get_DeviceStatus_internal()
    {
        XmlElement device_information = HuaweiAPI.HuaweiAPI.Tools.GET(ip_cpe, "api/device/information").DocumentElement;
        JSONNode json_device_information = Xml_group.ConvertXmlToJson(device_information);

        return json_device_information;
    }

    private JSONNode Get_TrafficStats_internal()
    {
        XmlElement traffic_stats = HuaweiAPI.HuaweiAPI.Tools.GET(ip_cpe, "api/monitoring/traffic-statistics").DocumentElement;
        JSONNode jsonTraffic = Xml_group.ConvertStatsXmlToJson(traffic_stats);

        return jsonTraffic;
    }

    private JSONNode ResetTrafficStats_internal() {
        var data = @"<request><ClearTraffic>1</ClearTraffic></request>";
        XmlElement response = HuaweiAPI.HuaweiAPI.Tools.POST(ip_cpe, data, "api/monitoring/clear-traffic").DocumentElement;
        JSONNode jsonResponse = Xml_group.ConvertXmlToJson(response);

        return jsonResponse;
    }

    /*IEnumerator CheckConnectionAsync(string ip_addr, System.Action<bool> callback) {
        XmlDocument checkLoginState;

        checkLoginState = this.CallAsyncGet(ip_addr, "api/user/state-login");
        
        yield return new WaitUntil(() => checkLoginState != null);

        if (checkLoginState.SelectSingleNode("//response/State").InnerText == "0")
        {
            Debug.Log("Already logged in");
            callback.Invoke(true);
        }
        else
        {
            Debug.Log("Not logged in");
            callback.Invoke(false);
        }
            
    }*/

    /*bool CheckConnection(string ip_addr) {
        bool state = false;
        StartCoroutine(CheckConnectionAsync(ip_addr, callback => { state = callback; }));

        return state;
    }*/
    #endregion
    /*
    #region PUBLIC METHODS
    public string ResetTrafficStats()
    {
        // Check if the there is connection with the CPE API
        if (isConnected())
        {
            JSONNode jsonResponse = ResetTrafficStats_internal();
            return jsonResponse.ToString();
        }
        return null;
    }

    public string GetSignalStats() {
        // Check if the there is connection with the CPE API
        if (isConnected()) {
            JSONNode jsonSignal = Get_SignalStats_internal();
            return jsonSignal.ToString();
        }
        return null;
    }

    public string GetTrafficStats()
    {
        // Check if the there is connection with the CPE API
        if (isConnected())
        {
            JSONNode jsonTraffic = Get_TrafficStats_internal();
            return jsonTraffic.ToString();
        }
        return null;
    }

    public string GetDeviceStatus()
    {
        // Check if the there is connection with the CPE API
        if (isConnected())
        {
            JSONNode json_device_information = Get_DeviceStatus_internal();
            return json_device_information.ToString();
        }
        return null;
    }

    public string RequestCPEStats() {
        // Check if the there is connection with the CPE API
        JSONNode jsonNode = new JSONObject();

        if (isConnected())
        {
            // Request the signal, traffic and device info
            JSONNode jsonSignal = Get_SignalStats_internal();
            JSONNode jsonTraffic = Get_TrafficStats_internal();
            JSONNode jsonDeviceInfo = Get_DeviceStatus_internal();

            // Wrap each request with a json node
            jsonNode["signal"] = jsonSignal;
            jsonNode["traffic"] = jsonTraffic;
            jsonNode["info"] = jsonDeviceInfo;

            // Reset CPE stats
            JSONNode jsonResponse = ResetTrafficStats_internal();

            // Return the string version of the json object
            return jsonNode.ToString();
        }
        return null;
    }

    public static CPEClient GetInstance() {
        return _sharedInstance;
    }

    public string GetStatsString() {
        Debug.Log($"(CPE Manager) --> Stats from CPE at {ip_cpe} = \n");
        return data;
    }
    #endregion*/
}