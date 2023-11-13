using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Security.Cryptography;
using System.Text;
using UnityEngine.Networking;
using SimpleJSON;

/// <summary>
/// Set of tool used to send POST/GET request 
/// </summary>
public static class CPEClientTools
{

    internal static string _sessionID = "";
    internal static string _token = "";
    internal static string _requestToken = "";
    internal static string _requestTokenOne = "";
    internal static string _requestTokenTwo = "";
    internal static string _sessionCookie = "";
    private static string _CurrentSessionID;
    private static string _CurrentToken;

    /// <summary>
    /// Error code with its description
    /// </summary>
    public enum ErrorCode
    {
        ERROR_PASSWORD_MUST_AT_LEAST_6_CHARS = 9003,
        ERROR_BUSY = 100004,
        ERROR_CHECK_SIM_CARD_CAN_UNUSEABLE = 101004,
        ERROR_CHECK_SIM_CARD_PIN_LOCK = 101002,
        ERROR_CHECK_SIM_CARD_PUN_LOCK = 101003,
        ERROR_COMPRESS_LOG_FILE_FAILED = 103102,
        ERROR_CRADLE_CODING_FAILED = 118005,
        ERROR_CRADLE_GET_CRURRENT_CONNECTED_USER_IP_FAILED = 118001,
        ERROR_CRADLE_GET_CRURRENT_CONNECTED_USER_MAC_FAILED = 118002,
        ERROR_CRADLE_GET_WAN_INFORMATION_FAILED = 118004,
        ERROR_CRADLE_SET_MAC_FAILED = 118003,
        ERROR_CRADLE_UPDATE_PROFILE_FAILED = 118006,
        ERROR_DEFAULT = -1,
        ERROR_DEVICE_AT_EXECUTE_FAILED = 103001,
        ERROR_DEVICE_COMPRESS_LOG_FILE_FAILED = 103015,
        ERROR_DEVICE_GET_API_VERSION_FAILED = 103006,
        ERROR_DEVICE_GET_AUTORUN_VERSION_FAILED = 103005,
        ERROR_DEVICE_GET_LOG_INFORMATON_LEVEL_FAILED = 103014,
        ERROR_DEVICE_GET_PC_AISSST_INFORMATION_FAILED = 103012,
        ERROR_DEVICE_GET_PRODUCT_INFORMATON_FAILED = 103007,
        ERROR_DEVICE_NOT_SUPPORT_REMOTE_OPERATE = 103010,
        ERROR_DEVICE_PIN_MODIFFY_FAILED = 103003,
        ERROR_DEVICE_PIN_VALIDATE_FAILED = 103002,
        ERROR_DEVICE_PUK_DEAD_LOCK = 103011,
        ERROR_DEVICE_PUK_MODIFFY_FAILED = 103004,
        ERROR_DEVICE_RESTORE_FILE_DECRYPT_FAILED = 103016,
        ERROR_DEVICE_RESTORE_FILE_FAILED = 103018,
        ERROR_DEVICE_RESTORE_FILE_VERSION_MATCH_FAILED = 103017,
        ERROR_DEVICE_SET_LOG_INFORMATON_LEVEL_FAILED = 103013,
        ERROR_DEVICE_SET_TIME_FAILED = 103101,
        ERROR_DEVICE_SIM_CARD_BUSY = 103008,
        ERROR_DEVICE_SIM_LOCK_INPUT_ERROR = 103009,
        ERROR_DHCP_ERROR = 104001,
        ERROR_DIALUP_ADD_PRORILE_ERROR = 107724,
        ERROR_DIALUP_DIALUP_MANAGMENT_PARSE_ERROR = 107722,
        ERROR_DIALUP_GET_AUTO_APN_MATCH_ERROR = 107728,
        ERROR_DIALUP_GET_CONNECT_FILE_ERROR = 107720,
        ERROR_DIALUP_GET_PRORILE_LIST_ERROR = 107727,
        ERROR_DIALUP_MODIFY_PRORILE_ERROR = 107725,
        ERROR_DIALUP_SET_AUTO_APN_MATCH_ERROR = 107729,
        ERROR_DIALUP_SET_CONNECT_FILE_ERROR = 107721,
        ERROR_DIALUP_SET_DEFAULT_PRORILE_ERROR = 107726,
        ERROR_DISABLE_AUTO_PIN_FAILED = 101008,
        ERROR_DISABLE_PIN_FAILED = 101006,
        ERROR_ENABLE_AUTO_PIN_FAILED = 101009,
        ERROR_ENABLE_PIN_FAILED = 101005,
        ERROR_FIRST_SEND = 1,
        ERROR_FORMAT_ERROR = 100005,
        ERROR_GET_CONFIG_FILE_ERROR = 100008,
        ERROR_GET_CONNECT_STATUS_FAILED = 102004,
        ERROR_GET_NET_TYPE_FAILED = 102001,
        ERROR_GET_ROAM_STATUS_FAILED = 102003,
        ERROR_GET_SERVICE_STATUS_FAILED = 102002,
        ERROR_LANGUAGE_GET_FAILED = 109001,
        ERROR_LANGUAGE_SET_FAILED = 109002,
        ERROR_LOGIN_TOO_FREQUENTLY = 108003,
        ERROR_LOGIN_MODIFY_PASSWORD_FAILED = 108004,
        ERROR_LOGIN_NO_EXIST_USER = 108001,
        ERROR_LOGIN_PASSWORD_ERROR = 108002,
        ERROR_LOGIN_TOO_MANY_TIMES = 108007,
        ERROR_LOGIN_TOO_MANY_USERS_LOGINED = 108005,
        ERROR_LOGIN_USERNAME_OR_PASSWORD_ERROR = 108006,
        ERROR_NET_CURRENT_NET_MODE_NOT_SUPPORT = 112007,
        ERROR_NET_MEMORY_ALLOC_FAILED = 112009,
        ERROR_NET_NET_CONNECTED_ORDER_NOT_MATCH = 112006,
        ERROR_NET_REGISTER_NET_FAILED = 112005,
        ERROR_NET_SIM_CARD_NOT_READY_STATUS = 112008,
        ERROR_FIRMWARE_NOT_SUPPORTED = 100002,
        ERROR_NO_DEVICE = -2,
        ERROR_NO_RIGHT = 100003,
        ERROR_NO_SIM_CARD_OR_INVALID_SIM_CARD = 101001,
        ERROR_ONLINE_UPDATE_ALREADY_BOOTED = 110002,
        ERROR_ONLINE_UPDATE_CANCEL_DOWNLODING = 110007,
        ERROR_ONLINE_UPDATE_CONNECT_ERROR = 110009,
        ERROR_ONLINE_UPDATE_GET_DEVICE_INFORMATION_FAILED = 110003,
        ERROR_ONLINE_UPDATE_GET_LOCAL_GROUP_COMMPONENT_INFORMATION_FAILED = 110004,
        ERROR_ONLINE_UPDATE_INVALID_URL_LIST = 110021,
        ERROR_ONLINE_UPDATE_LOW_BATTERY = 110024,
        ERROR_ONLINE_UPDATE_NEED_RECONNECT_SERVER = 110006,
        ERROR_ONLINE_UPDATE_NOT_BOOT = 110023,
        ERROR_ONLINE_UPDATE_NOT_FIND_FILE_ON_SERVER = 110005,
        ERROR_ONLINE_UPDATE_NOT_SUPPORT_URL_LIST = 110022,
        ERROR_ONLINE_UPDATE_SAME_FILE_LIST = 110008,
        ERROR_ONLINE_UPDATE_SERVER_NOT_ACCESSED = 110001,
        ERROR_PARAMETER_ERROR = 100006,
        ERROR_PB_CALL_SYSTEM_FUCNTION_ERROR = 115003,
        ERROR_PB_LOCAL_TELEPHONE_FULL_ERROR = 115199,
        ERROR_PB_NULL_ARGUMENT_OR_ILLEGAL_ARGUMENT = 115001,
        ERROR_PB_OVERTIME = 115002,
        ERROR_PB_READ_FILE_ERROR = 115005,
        ERROR_PB_WRITE_FILE_ERROR = 115004,
        ERROR_SAFE_ERROR = 106001,
        ERROR_SAVE_CONFIG_FILE_ERROR = 100007,
        ERROR_SD_DIRECTORY_EXIST = 114002,
        ERROR_SD_FILE_EXIST = 114001,
        ERROR_SD_FILE_IS_UPLOADING = 114007,
        ERROR_SD_FILE_NAME_TOO_LONG = 114005,
        ERROR_SD_FILE_OR_DIRECTORY_NOT_EXIST = 114004,
        ERROR_SD_IS_OPERTED_BY_OTHER_USER = 114004,
        ERROR_SD_NO_RIGHT = 114006,
        ERROR_SET_NET_MODE_AND_BAND_FAILED = 112003,
        ERROR_SET_NET_MODE_AND_BAND_WHEN_DAILUP_FAILED = 112001,
        ERROR_SET_NET_SEARCH_MODE_FAILED = 112004,
        ERROR_SET_NET_SEARCH_MODE_WHEN_DAILUP_FAILED = 112002,
        ERROR_SMS_DELETE_SMS_FAILED = 113036,
        ERROR_SMS_LOCAL_SPACE_NOT_ENOUGH = 113053,
        ERROR_SMS_NULL_ARGUMENT_OR_ILLEGAL_ARGUMENT = 113017,
        ERROR_SMS_OVERTIME = 113018,
        ERROR_SMS_QUERY_SMS_INDEX_LIST_ERROR = 113020,
        ERROR_SMS_SAVE_CONFIG_FILE_FAILED = 113047,
        ERROR_SMS_SET_SMS_CENTER_NUMBER_FAILED = 113031,
        ERROR_SMS_TELEPHONE_NUMBER_TOO_LONG = 113054,
        ERROR_STK_CALL_SYSTEM_FUCNTION_ERROR = 116003,
        ERROR_STK_NULL_ARGUMENT_OR_ILLEGAL_ARGUMENT = 116001,
        ERROR_STK_OVERTIME = 116002,
        ERROR_STK_READ_FILE_ERROR = 116005,
        ERROR_STK_WRITE_FILE_ERROR = 116004,
        ERROR_UNKNOWN = 100001,
        ERROR_UNLOCK_PIN_FAILED = 101007,
        ERROR_USSD_AT_SEND_FAILED = 111018,
        ERROR_USSD_CODING_ERROR = 111017,
        ERROR_USSD_EMPTY_COMMAND = 111016,
        ERROR_USSD_ERROR = 111001,
        ERROR_USSD_FUCNTION_RETURN_ERROR = 111012,
        ERROR_USSD_IN_USSD_SESSION = 111013,
        ERROR_USSD_NET_NOT_SUPPORT_USSD = 111022,
        ERROR_USSD_NET_NO_RETURN = 111019,
        ERROR_USSD_NET_OVERTIME = 111020,
        ERROR_USSD_TOO_LONG_CONTENT = 111014,
        ERROR_USSD_XML_SPECIAL_CHARACTER_TRANSFER_FAILED = 111021,
        ERROR_WIFI_PBC_CONNECT_FAILED = 117003,
        ERROR_WIFI_STATION_CONNECT_AP_PASSWORD_ERROR = 117001,
        ERROR_WIFI_STATION_CONNECT_AP_WISPR_PASSWORD_ERROR = 117004,
        ERROR_WIFI_WEB_PASSWORD_OR_DHCP_OVERTIME_ERROR = 117002,
        ERROR_WRITE_ERROR = 100009,
        ERROR_THE_SD_CARD_IS_CURRENTLY_IN_USE = 114003,
        ERROR_VOICE_CALL_BUSY = 120001,
        ERROR_INVALID_TOKEN = 125001,
        ERROR_SESSION = 125002,
        ERROR_WRONG_SESSION_TOKEN = 125003
    }

    

    /// <summary>
    /// POST request method
    /// </summary>
    /// <param name="ip_address"></param>
    /// <param name="data"></param>
    /// <param name="api_type"></param>
    /// <returns></returns>
    

    public static IEnumerator POST_internal(string ip_address, string data, string api_type, System.Action<XmlDocument> callback)
    {
        XmlDocument doc = new XmlDocument();
        var wc = POST_WebClient();

        // Set the URI and add an uploadhandler to handle the data to be sent in the request
        wc.url = $"http://{ip_address}/{api_type}";
        wc.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(data));

        UnityWebRequestAsyncOperation webResponse = wc.SendWebRequest();

        // Wait until a response is received 
        yield return new WaitUntil(() => webResponse.isDone);

        if (webResponse.isDone) {

            HandleHeaders(wc);
            doc.LoadXml(wc.downloadHandler.text);

            if (wc.result != UnityWebRequest.Result.Success)
            {
                if (doc.OuterXml.ToString() == string.Empty) { Debug.Log("No response from router."); }
                else if (XMLTool.Beautify(doc).Contains("OK"))
                {


                    foreach (XmlNode node in doc.DocumentElement)
                    {
                        Debug.Log(node.Name + " : " + node.InnerText);
                    }
                }
                else if (doc.OuterXml.ToString().Contains("error"))
                {
                    Debug.Log("ERROR " + doc.SelectSingleNode("//error/code").InnerText.ToString() + " : " + ((ErrorCode)(int.Parse(doc.SelectSingleNode("//error/code").InnerText))).ToString());

                }
            }
            else {
                Debug.Log("INTERNAL POST operation sucessfully done.");
            }
            
        }

        callback.Invoke(doc);
    }


    public static IEnumerator POST(string ip_address, string data, string api_type, System.Action<XmlDocument> callback)
    {
        Debug.Log("Sending POST request to " + api_type + "..");

        //get session id token
        XmlDocument Sestoken = new XmlDocument();
        yield return GET_internal(ip_address, CPE_API_type.REQUEST_TOKEN, callback => { Sestoken = callback; });
        _CurrentSessionID = Sestoken.SelectSingleNode("//response/SesInfo").InnerText;
        _CurrentToken = Sestoken.SelectSingleNode("//response/TokInfo").InnerText;

        // Create a XML document for the response and create a UnityWebRequest Post
        XmlDocument doc = new XmlDocument();
        UnityWebRequest wc = Post_WebClient();


        // Set the URI and add an uploadhandler to handle the data to be sent in the request
        wc.url = $"http://{ip_address}/{api_type}";
        wc.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(data));

        UnityWebRequestAsyncOperation webResponse = wc.SendWebRequest();

        // Wait until a response is received 
        yield return new WaitUntil(() => webResponse.isDone);

        if (webResponse.isDone)
        {
            HandleHeaders(wc);
            doc.LoadXml(wc.downloadHandler.text);

            if (wc.result != UnityWebRequest.Result.Success)
            {
                
                if (doc.OuterXml.ToString() == string.Empty) { Debug.Log("No response from router."); }
                else if (XMLTool.Beautify(doc).Contains("OK"))
                {

                    foreach (XmlNode node in doc.DocumentElement)
                    {
                        Debug.Log(node.Name + " : " + node.InnerText);
                    }
                }
                else if (doc.OuterXml.ToString().Contains("error"))
                {
                    Debug.Log("ERROR " + doc.SelectSingleNode("//error/code").InnerText.ToString() + " : " + ((ErrorCode)(int.Parse(doc.SelectSingleNode("//error/code").InnerText))).ToString());

                }
            }
            else
            {
                Debug.Log("POST operation sucessfully done.");
            }

        }

        callback.Invoke(doc);
        
    }

    /// <summary>
    /// internal GET request method. This is for internal private usage
    /// </summary>
    /// <param name="ip_address"></param>
    /// <param name="api_type"></param>
    /// <returns></returns>
    ///

    public static IEnumerator GET_internal(string ip_address, string api_type, System.Action<XmlDocument> callback)
    {
        // Create a XML documento for the response
        XmlDocument doc = new XmlDocument();

        // Create a GET UnityWebRequest
        UnityWebRequest wc = GET_WebClient();
        // Set the URI and add an uploadhandler to handle the data to be sent in the request
        wc.url = $"http://{ip_address}/{api_type}";

        UnityWebRequestAsyncOperation webResponse = wc.SendWebRequest();

        // Wait until a response is received 
        yield return new WaitUntil(() => webResponse.isDone);

        if (webResponse.isDone)
        {
            HandleHeaders(wc);
            doc.LoadXml(wc.downloadHandler.text);
            
            if (wc.result != UnityWebRequest.Result.Success)
            {
                Debug.Log($"Response: Error from the server");
            }
            else {
                Debug.Log($"GET internal operation successfully done");
            }
        }

        callback.Invoke(doc);
    }


    /// <summary>
    /// GET request method
    /// </summary>
    /// <param name="ip_address"></param>
    /// <param name="api_type"></param>
    /// <returns></returns>
    ///

    public static IEnumerator GET(string ip_address, string api_type, System.Action<XmlDocument> callback)
    {
        Debug.Log("Sending GET request to " + api_type + "...");
        // Create a XML documento for the response
        XmlDocument doc = new XmlDocument();

        // Create a GET UnityWebRequest
        UnityWebRequest wc = GET_WebClient();
        // Set the URI and add an uploadhandler to handle the data to be sent in the request
        wc.url = $"http://{ip_address}/{api_type}";

        UnityWebRequestAsyncOperation webResponse = wc.SendWebRequest();

        // Wait until a response is received 
        yield return new WaitUntil(() => webResponse.isDone);

        if (webResponse.isDone)
        {
            HandleHeaders(wc);
            doc.LoadXml(wc.downloadHandler.text);

            if (wc.result != UnityWebRequest.Result.Success)
            {
                if (doc.OuterXml.ToString() == string.Empty) { Debug.Log("No response from router."); }
                else if (!XMLTool.Beautify(doc).Contains("error"))
                {
                    foreach (XmlNode node in doc.DocumentElement)
                    {
                        Debug.Log(node.Name + " : " + node.InnerText);

                    }
                }
                else if (doc.OuterXml.ToString().Contains("error"))
                {
                    Debug.Log("ERROR " + doc.SelectSingleNode("//error/code").InnerText.ToString() + " : " + ((ErrorCode)(int.Parse(doc.SelectSingleNode("//error/code").InnerText))).ToString());
                }
                else {
                    Debug.Log($"CPE Response = {doc.OuterXml}");
                }
            }
            else
            {
                Debug.Log($"CPE Response = GET Operation successfully done. \nResponse: {doc.OuterXml}");
            }
        }

        callback.Invoke(doc);

    }


    public static IEnumerator CheckConnection(this MonoBehaviour monoBehaviour, string ip_address, System.Action<bool> state)
    {
        // Declare a container for te callback returned by the coroutine
        XmlDocument api_response = new XmlDocument();
        // Call the coroutine async
        yield return GET(ip_address, CPE_API_type.CHECK_CONNECTION, callback => {api_response = callback;});
        // Check the connection state with the CPE
        if (api_response.SelectSingleNode("//response/State").InnerText == "0")
        {
            Debug.Log("Already logged in");
            state.Invoke(true);
        }
        else
        {
            Debug.Log("Not logged in");
            state.Invoke(false);
        }
    }

    private static IEnumerator Initialize(this MonoBehaviour monoBehaviour, string ip_address) {
        //If we dont have either a cookie or a session token, request it to the CPE using the API
        if (string.IsNullOrEmpty(CPEClientTools._sessionCookie) || string.IsNullOrEmpty(CPEClientTools._requestToken))
        {
            // Request a Session token to the CPE
            XmlDocument tokenResponse = new XmlDocument();
            yield return GET(ip_address, CPE_API_type.REQUEST_TOKEN, callback => { tokenResponse = callback; });
            // Extract the response info
            CPEClientTools._sessionID = tokenResponse.SelectSingleNode("//response/SesInfo").InnerText;
            CPEClientTools._token = tokenResponse.SelectSingleNode("//response/TokInfo").InnerText;
            // Save the token and cookie
            CPEClientTools._sessionCookie = CPEClientTools._sessionID;
            CPEClientTools._requestToken = CPEClientTools._token;
            Debug.Log($"_sessionCookie = {CPEClientTools._sessionCookie}\n_requestToken = {CPEClientTools._token}");
        }
    }

    public static IEnumerator UserLogin(this MonoBehaviour monoBehaviour, string ip_address, string username, string password, int maxAttempts, System.Action<bool> isLogged) {

        // Request the authentication token and session to the CPE
        yield return Initialize(monoBehaviour, ip_address);

        // Encode the session information to be sent
        string authinfo = CPEClientTools.SHA256andB64(username + CPEClientTools.SHA256andB64(password) + CPEClientTools._requestToken);
        string logininfo = string.Format("<?xml version=\"1.0\" encoding=\"UTF-8\"?><request><Username>{0}</Username><Password>{1}</Password><password_type>4</password_type>", username, authinfo);
        Debug.Log($"POST body = {logininfo}");

        // Send a post request to log in with the CPE (# of attempts = maxAttempts)
        XmlDocument login = new XmlDocument();
        int i = 0;
        while (i < maxAttempts) {
            yield return POST_internal(ip_address, logininfo, CPE_API_type.LOGIN, callback => { login = callback; });
            Debug.Log($"Login response = {login.OuterXml}");
            if (XMLTool.Beautify(login).Contains("OK")) {
                i = maxAttempts;
                break;
            }
            else{
                i++;
            }
        }

        // Check if the server has logged in successfully through an OK in the body

        if (XMLTool.Beautify(login).Contains("OK"))
        {
            Debug.Log("Login successful");
            isLogged.Invoke(true);
        }
        else
        {
            Debug.Log("Login failed");
            isLogged.Invoke(false);
        }
    }

    public static IEnumerator UserLogout(this MonoBehaviour monoBehaviour, string ip_address, string username, string password, System.Action<bool> hasLoggedOut) {

        // Encode the session information to be sent
        string data = "<?xml version:\"1.0\" encoding=\"UTF-8\"?><request><Logout>1</Logout></request>";

        XmlDocument logout = new XmlDocument();

        // Send a post request to log in with the CPE (# of attempts = maxAttempts)
        yield return POST_internal(ip_address, data, CPE_API_type.LOGOUT, callback => { logout = callback; });

        // Check if the server has confirmed logout through an OK in the body
        if (XMLTool.Beautify(logout).Contains("OK"))
        {
            Debug.Log("Logged out successfully");
            hasLoggedOut.Invoke(true);
        }
        else
        {
            Debug.Log("Logout failed");
            hasLoggedOut.Invoke(false);
        }
    }

    public static IEnumerator CPEMonitoring(this MonoBehaviour monoBehaviour, string ip_address, string username, string password, int maxAttempts, double iterTime, System.Action<bool> isMonitoring, System.Action<string> stats) {
        // Determine the initial time of this iteration
        double startTime = Time.realtimeSinceStartupAsDouble;

        // Check if the glass has previously logged in with the CPE
        bool isLogged = false;
        yield return CheckConnection(monoBehaviour, ip_address, callback => { isLogged = callback; });

        // If the HMD is not logged in with the CPE, attempt to connect using the API with a max number of attempts
        if (!isLogged)
        {
            yield return UserLogin(monoBehaviour, ip_address, username, password, maxAttempts, callback => { isLogged = callback; });
        }

            
        double spentTime = Time.realtimeSinceStartupAsDouble - startTime;
        Debug.Log("Time spent in CPE login = " + spentTime);
        startTime = Time.realtimeSinceStartupAsDouble;

        // If the attempt of Login was successful, request metrics, else jump this sample
        if (isLogged)
        {
            isMonitoring.Invoke(true);
            // Get signal stats
            JSONNode signalStats = null;
            yield return GetSignalJSON_internal(ip_address, callback => { signalStats = callback; });

            // Get device info
            JSONNode deviceInfo = null;
            yield return GetDeviceInfoJSON_internal(ip_address, callback => { deviceInfo = callback; });

            // Get traffic stats
            JSONNode trafficStats = null;
            yield return GetTrafficJSON_internal(ip_address, callback => { trafficStats = callback; });

            // Reset stats
            bool hasReset = false;
            yield return ResetStats_internal(ip_address, callback => { hasReset = callback; });

            // Build a JSON Node with that stats as subnodes
            // Wrap each request with a json node
            JSONNode jsonNode = new JSONObject();
            jsonNode["signal"] = signalStats;
            jsonNode["traffic"] = trafficStats;
            jsonNode["info"] = deviceInfo;

            stats.Invoke(jsonNode.ToString());
            Debug.Log("Stats gathered = \n" + jsonNode.ToString());
        }
        else
        {
            stats.Invoke("");
            isMonitoring.Invoke(false);
        }

        // Estimate the time spent in an iteration and the time to wait
        spentTime = Time.realtimeSinceStartupAsDouble - startTime;
        double waitTimeValue = iterTime - spentTime;
        WaitForSecondsRealtime waitTime;

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
            waitTime = new WaitForSecondsRealtime((float)(waitTimeValue + iterTime));
        }

        // Suspend the thread until the time passes
        yield return waitTime;
        Debug.Log("Time spent in CPE stats query = " + spentTime);
    }

    public static IEnumerator GetSignalJSON_internal(string ip_address, System.Action<JSONNode> stats) {
        // Get signal stats
        XmlDocument signal = new XmlDocument();
        yield return GET_internal(ip_address, CPE_API_type.SIGNAL_STATS, callback => { signal = callback; });
        JSONNode signalStats = Xml_group.ConvertStatsXmlDocumentToJson(signal);
        stats.Invoke(signalStats);
    }

    public static IEnumerator GetDeviceInfoJSON_internal(string ip_address, System.Action<JSONNode> stats) {
        // Get device info
        XmlDocument device = new XmlDocument();
        yield return GET_internal(ip_address, CPE_API_type.DEVICE_INFO, callback => { device = callback; });
        JSONNode deviceInfo = Xml_group.ConvertXmlDocumentToJson(device);
        stats.Invoke(deviceInfo);
    }

    public static IEnumerator GetTrafficJSON_internal(string ip_address, System.Action<JSONNode> stats)
    {
        // Get device info
        XmlDocument traffic = new XmlDocument();
        yield return GET_internal(ip_address, CPE_API_type.TRAFFIC_STATS, callback => { traffic = callback; });
        JSONNode trafficStats = Xml_group.ConvertXmlDocumentToJson(traffic);
        stats.Invoke(trafficStats);
    }

    public static IEnumerator ResetStats_internal(string ip_address, System.Action<bool> isReset) {
        // Reset stats
        //string data = @"<request><ClearTraffic>1</ClearTraffic></request>";
        string data = "<?xml version:\"1.0\" encoding=\"UTF-8\"?><request><ClearTraffic>1</ClearTraffic></request>";
        XmlDocument response = new XmlDocument();
        yield return POST(ip_address, data, CPE_API_type.RESET_STATS, callback => { response = callback; });

        Debug.Log(XMLTool.Beautify(response));
        // Check the response
        if (XMLTool.Beautify(response).Contains("OK"))
        {
            Debug.Log("Stats resetted at the CPE");
            isReset.Invoke(true);
        }
        else
        {
            Debug.Log("Stats reset has been failed");
            isReset.Invoke(false);
        }
    }



    /// <summary>
    /// WebClient for GET and POST request
    /// </summary>
    /// <returns></returns>
    private static UnityWebRequest GET_WebClient() {
        UnityWebRequest wc = new UnityWebRequest();

        wc.method = UnityWebRequest.kHttpVerbGET;
        wc.downloadHandler = new DownloadHandlerBuffer();
        wc.SetRequestHeader("Cookie", _sessionCookie);
        wc.SetRequestHeader("__RequestVerificationToken", _requestToken);
        wc.SetRequestHeader("Accept", "*/*");
        wc.SetRequestHeader("User-Agent", "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.9.2.12) Gecko/20101026 Firefox/3.6.12");

        return wc;
    }

    private static UnityWebRequest Post_WebClient()
    {
        UnityWebRequest wc = new UnityWebRequest();

        wc.method = UnityWebRequest.kHttpVerbPOST;
        wc.downloadHandler = new DownloadHandlerBuffer();
        wc.SetRequestHeader("Cookie", _CurrentSessionID);
        wc.SetRequestHeader("__RequestVerificationToken", _CurrentToken);
        wc.SetRequestHeader("Accept", "*/*");
        wc.SetRequestHeader("User-Agent", "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.9.2.12) Gecko/20101026 Firefox/3.6.12");

        return wc;
    }

    private static UnityWebRequest POST_WebClient()
    {
        UnityWebRequest wc = new UnityWebRequest();

        wc.method = UnityWebRequest.kHttpVerbPOST;
        wc.downloadHandler = new DownloadHandlerBuffer();
        wc.SetRequestHeader("Cookie", _sessionCookie);
        wc.SetRequestHeader("__RequestVerificationToken", _requestToken);
        wc.SetRequestHeader("Accept", "*/*");
        wc.SetRequestHeader("User-Agent", "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.9.2.12) Gecko/20101026 Firefox/3.6.12");

        return wc;
    }

    /// <summary>
    /// Headers for GET and POST request
    /// </summary>
    /// <param name="wc"></param>
    private static void HandleHeaders(UnityWebRequest wc)
    {
        // Get a dictionary of response headers from the UnityWebRequest
        Dictionary<string, string> responserHeaders = wc.GetResponseHeaders();


        // If the key does not exist or is empty, update the value

        if (responserHeaders.ContainsKey("__RequestVerificationTokenOne"))
        {
            _requestTokenOne = !string.IsNullOrEmpty(responserHeaders["__RequestVerificationTokenOne"]) ? responserHeaders["__RequestVerificationTokenOne"] : _requestTokenOne;
        }
        if (responserHeaders.ContainsKey("__RequestVerificationTokenTwo"))
        {
            _requestTokenTwo = !string.IsNullOrEmpty(responserHeaders["__RequestVerificationTokenTwo"]) ? responserHeaders["__RequestVerificationTokenTwo"] : _requestTokenTwo;
        }
        if (responserHeaders.ContainsKey("__RequestVerificationToken"))
        {
            _requestToken = !string.IsNullOrEmpty(responserHeaders["__RequestVerificationToken"]) ? responserHeaders["__RequestVerificationToken"] : _requestToken;
        }
        if (responserHeaders.ContainsKey("Set-Cookie"))
        {
            _sessionCookie = !string.IsNullOrEmpty(responserHeaders["Set-Cookie"]) ? responserHeaders["Set-Cookie"] : _sessionCookie;
        }
    }

    /// <summary>
    /// This method is used to encode password and to encode product of user + encoded password to sha256
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    internal static string SHA256andB64(string text)
    {
        var hashBytes = System.Text.Encoding.UTF8.GetBytes(SHA256(text));
        return System.Convert.ToBase64String(hashBytes);
    }

    /// <summary>
    /// Method to encode password to sha256
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    internal static string SHA256(string text)
    {
        StringBuilder Sb = new StringBuilder();

        using (SHA256 hash = SHA256Managed.Create())
        {
            Encoding enc = Encoding.UTF8;
            byte[] result = hash.ComputeHash(enc.GetBytes(text));

            foreach (byte b in result)
                Sb.Append(b.ToString("x2"));
        }
        return Sb.ToString();
    }


    /// <summary>
    /// This method make the response body xml readable
    /// </summary>
    /// <param name="doc"></param>
    /// <returns></returns>
    
}

public static class XMLTool
{
    /// <summary>
    /// Make response body xml readable
    /// </summary>
    /// <param name="doc"></param>
    /// <returns></returns>
    internal static string Beautify(this XmlDocument doc)
    {
        StringBuilder sb = new StringBuilder();
        XmlWriterSettings settings = new XmlWriterSettings
        {
            Indent = true,
            IndentChars = "  ",
            NewLineChars = "\r\n",
            NewLineHandling = NewLineHandling.Replace
        };
        using (XmlWriter writer = XmlWriter.Create(sb, settings))
        {
            doc.Save(writer);
        }
        return sb.ToString();
    }
}


public static class CPE_API_type {
    /// <summary>
    /// Summarize the APIs to be used in this client
    /// </summary>
    /// <param name="doc"></param>
    /// <returns></returns>
    public static readonly string CHECK_CONNECTION = "api/user/state-login";
    public static readonly string REQUEST_TOKEN = "api/webserver/SesTokInfo";
    public static readonly string LOGIN = "api/user/login";
    public static readonly string AUTHLOGIN = "/api/user/authentication_login";
    public static readonly string LOGOUT = "api/user/logout";
    public static readonly string SIGNAL_STATS = "api/device/signal";
    public static readonly string TRAFFIC_STATS = "api/monitoring/traffic-statistics";
    public static readonly string DEVICE_INFO = "api/device/information";
    public static readonly string RESET_STATS = "api/monitoring/clear-traffic";
}

    
