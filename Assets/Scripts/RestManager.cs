using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;

public class AppendedStats {
    KQI_group stats;
}

public class RestResponse {
    public string Response;
    public bool Status;
    public string Data;
}

public class RestManager : MonoBehaviour
{
    #region ATTRIBUTES
    // Private fields
    private string rest_URL = null;
    private string rest_host = null;
    private string statsJson = null;
    private Dictionary<string, string> appendedStats = new Dictionary<string, string>();
    private int sampleNumber = 0;
    private int mode = 0;
    private bool crowd = false;
    private bool cpe = false;
    private int id = 1;
    private int n_tries_crowd = 3;
    private RestResponse rest_response;
    private string response;

    // Alias
    private StatsManager stats;
    private GameManager gameManager;

    // Shared instance
    private static RestManager _sharedInstance;

    #endregion

    #region UNITY METHODS
    private void Awake()
    {
        // Get the instance of this class
        _sharedInstance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.GetInstance();
        stats = StatsManager.GetInstance();

        rest_URL = gameManager.GetRestURL();
        rest_host = GetRestHost(rest_URL);
        Debug.Log("host:" + rest_host);
        mode = gameManager.GetMode();
        crowd = gameManager.GetCrowdEnable();
    }

    #endregion

    #region CLASS METHODS
    public string GetCurrentStatsInJson() {
        // Get the current json stats object in string json format
        return JsonUtility.ToJson(stats.GetStatsObject());
    }

    public string GetSampleStatsJson() {
        statsJson = $"{{\"Service\": {GetCurrentStatsInJson()}, \"CPE\": {GetCPEStatsString()}}}";
        return statsJson;
    }

    public string GetCPEStatsString() {
        return stats.GetCPEStats();
    }

    public void AppendJson() {
        // If statsJson is null, initiate the string, else append the new json object
        if (statsJson is not null)
        {
            statsJson += $", \"{sampleNumber}\": {{\"Service\": {GetCurrentStatsInJson()}, \"CPE\": {GetCPEStatsString()}}}";
        }
        else {
            statsJson = $"\"{sampleNumber}\": {{\"Service\": {GetCurrentStatsInJson()}, \"CPE\": {GetCPEStatsString()}}}";
        }
        // Increase the sample counter
        sampleNumber++;
    }

    public string GetSessionStatsJson() {
        // Add the {} to the stats json object
        string statsAux = "{\"data\": {\n" + statsJson + "\n}}";

        // If the stats objects is not null, save a copy to be transferred to the
        // game object and reset the buffer string (ready for a new session)
        if (statsAux is not null) {
            ResetRestBuffer();
            return statsAux;
        }
        return null;
    }

    private void ResetRestBuffer() {
        // Force reset the buffer string
        statsJson = null;
        sampleNumber = 0;
    }

    public void SendStats(string jsonData, string timestamp = null) {
        // Create a thread to send stats
        StartCoroutine(SendStatsToRest(timestamp, jsonData, callback => { unlockNextIteration(callback); }));
    }

    public void SendHelloToRest(string id = null) {
        // Create a coroutine to request the REST server to start measuring crowd
        StartCoroutine(CheckCrowd(id: id, callback => { unlockPlayback(callback); }));
    }


    private string GetRestHost(string path) {
        // Extract the rest host from the path ("/" after the http://)
        int index = path.IndexOf('/', 7);

        string rest_host;
        if (index != -1)
        {
            rest_host = path.Substring(0, index);
        }
        else
        {
            rest_host = path;
        }

        return rest_host;
    }

    private void unlockPlayback(bool value) {
        gameManager.SetRestAck(value);
    }

    private void unlockNextIteration(bool value)
    {
        gameManager.SetNewSessionAck(value);
    }

    #endregion

    #region COROUTINES
    private IEnumerator SendHello(string id, System.Action<bool> response) {
        // Declare a webRequest object
        UnityWebRequest webRequest;

        webRequest = LatencyManager.CreateApiGetRequest(url: $"{rest_host}/crowdcell/initiate_monitoring?id={id}&enable_crowd={crowd}&enable_cpe={cpe}");
        Debug.Log($"(REST Manager) --> Sending HELLO to: {webRequest.url}");

        // Send the webrequest
        UnityWebRequestAsyncOperation webResponse = webRequest.SendWebRequest();

        // Wait until a response is received 
        yield return new WaitUntil(() => webResponse.isDone);

        // Get the url first part
        string[] pages = webRequest.url.Split(' ');
        int page = pages.Length - 1;

        if (webResponse.isDone)
        {
            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    Debug.LogError("(REST Manager)-- >" + pages[page] + ": Connection error!");
                    rest_response = new RestResponse();
                    rest_response.Status = false;
                    break;
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError("(REST Manager)-- >" + pages[page] + ": Error: " + webRequest.error);
                    rest_response = new RestResponse();
                    rest_response.Status = false;
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError("(REST Manager)-- >" + pages[page] + ": HTTP Error: " + webRequest.error
                        + "Data: " + webRequest.downloadHandler.text);
                    rest_response = new RestResponse();
                    rest_response.Status = false;
                    break;
                case UnityWebRequest.Result.Success:
                    // Save the response
                    string response_txt = webRequest.downloadHandler.text;

                    // Check the response
                    rest_response = JsonUtility.FromJson<RestResponse>(response_txt);  
                    break;
            }
        }

        Debug.Log("Respuesta json: " + rest_response.Status);
        if (rest_response.Status)
        {
            // Return the coroutine response to a variable
            response.Invoke(true);
        }
        else {
            response.Invoke(false);
        }
    }

    private IEnumerator CheckCrowd(string id, System.Action<bool> response) {
        int i = 1;
        bool status = false;

        while (i <= this.n_tries_crowd) {
            yield return SendHello(id: id, callback => { status = callback; });

            if (status)
            {
                Debug.LogAssertion($"(REST Manager) --> Crowdcell monitoring notified.");
                break;
            }
            else {
                Debug.LogError($"(REST Manager) --> Crowdcell not reachable. Tries left: {this.n_tries_crowd - i}");
                i++;
            }
            
        }

        if (i > 3) {
            Debug.LogError($"(REST Manager) --> Crowdcell not reachable. Continuing with experiment without crowd monitoring");
            status = true;
        }

        response.Invoke(status);
    }


    private IEnumerator SendStatsToRest(string timestamp, string jsonData, System.Action<bool>response) {
        // Declare a webRequest object
        UnityWebRequest webRequest;

        // Target an endpoint depending if the mode is Testbed or Demo
        if(mode == 0) webRequest = LatencyManager.CreateApiPostRequest($"{rest_URL}/testbed/{timestamp}", jsonData);
        else webRequest = LatencyManager.CreateApiPostRequest($"{rest_URL}/demo/", jsonData);
        Debug.Log($"(REST Manager) --> Trying to send stats to: {webRequest.url} \nStats: {jsonData}");

        //UnityWebRequest webRequest = LatencyManager.CreateApiPostRequest($"{rest_URL}/{timestamp}", jsonData);
        UnityWebRequestAsyncOperation webResponse = webRequest.SendWebRequest();

        // Wait until a response is received 
        yield return new WaitUntil(() => webResponse.isDone);

        // Get the url first part
        string[] pages = rest_URL.Split(' ');
        int page = pages.Length - 1;

        if (webResponse.isDone)
        {
            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    Debug.LogError("(REST Manager)-- >" + pages[page] + ": Connection error!");
                    break;
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError("(REST Manager)-- >" + pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError("(REST Manager)-- >" + pages[page] + ": HTTP Error: " + webRequest.error
                        + "Data: " + webRequest.downloadHandler.text);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log($"(REST Manager) --> Stats sucessfully sent to the Rest server {pages[page]}");
                    break;
            }
        }
        else
        {
            Debug.Log($"(REST Manager) --> Error reaching the REST Server {pages[page]}. Request failed");
        }

        // Send the ACK to unblock next iteration
        response.Invoke(true);
    }
    #endregion

    #region STATIC METHODS
    public static RestManager GetInstance() {
        return _sharedInstance;
    }
    #endregion
}
