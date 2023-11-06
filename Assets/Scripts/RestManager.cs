using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AppendedStats {
    KQI_group stats;
}

public class RestManager : MonoBehaviour
{
    #region ATTRIBUTES
    // Private fields
    private string rest_URL = null;
    private string statsJson = null;
    private Dictionary<string, string> appendedStats = new Dictionary<string, string>();
    private int sampleNumber = 0;

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
    }

    #endregion

    #region CLASS METHODS
    public string GetCurrentStatsInJson() {
        // Get the current json stats object in string json format
        return JsonUtility.ToJson(stats.GetStatsObject());
    }

    public void AppendJson() {
        // If statsJson is null, initiate the string, else append the new json object
        if (statsJson is not null)
        {
            statsJson += $", \"{sampleNumber}\": {GetCurrentStatsInJson()}";
        }
        else {
            statsJson = $"\"{sampleNumber}\": {GetCurrentStatsInJson()}";
        }
        // Increase the sample counter
        sampleNumber++;
    }

    public string GetSessionStatsJson() {
        // Add the {} to the stats json object
        string statsAux = "{\n" + statsJson + "\n}";

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
    }

    public void SendStats(string jsonData) {
        // Create a thread to send stats
        StartCoroutine(SendStatsToRest(jsonData));
    }

    #endregion

    #region COROUTINES
    private IEnumerator SendStatsToRest(string jsonData) {

        UnityWebRequest webRequest = LatencyManager.CreateApiPostRequest(rest_URL, jsonData);
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
                    Debug.LogError(pages[page] + ": Connection error!");
                    break;
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log($"REST Manager --> Stats sucessfully sent to the Rest server {pages[page]}");
                    break;
            }
        }
        else
        {
            Debug.Log($"REST Manager --> Error reaching the Web Server {pages[page]} with HEAD request");
        }
    }
    #endregion

    #region STATIC METHODS
    public static RestManager GetInstance() {
        return _sharedInstance;
    }
    #endregion
}
