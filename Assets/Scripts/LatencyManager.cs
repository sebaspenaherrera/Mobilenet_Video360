using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System;
using System.Net;
using System.Net.Sockets;

public class LatencyManager : MonoBehaviour
{
    #region FIELDS
    // Private fields
    private double timeRequest = 0.00;
    private double timeResponse = 0.00;
    private double rtt = 0.00;
    private double rtt_ping = 0.00;
    private double expectedDelay = 1.00;
    //private string url = "https://storage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4";
    //private string url = "https://cdn.bitmovin.com/content/assets/playhouse-vr/mpds/105560.mpd";
    private string url;
    private IPAddress[] ipAddresses;
    private string hostIp = "";

    // Shared instances
    private static LatencyManager _sharedInstance;

    // Alias
    private GameManager gameManager;

    #endregion
    private void Awake()
    {
        // Instance the shared variable
        _sharedInstance = this;

        // Get the host configured in the Game Manager
        gameManager = GameManager.GetInstance();
        SetURL(gameManager.GetConfiguredURL()); 
        // Get the host IP address to esimate latency
        ResolveHost();
        GetValidIpHost();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Starts coroutine that PINGS the host each second
        StartCoroutine("PingHost");
        // Starts coroutine that sends HEAD requests to the host each second
        StartCoroutine("SendHeadRequest");

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region COROUTINE METHODS
    IEnumerator MPDrequest()
    {
        while (true)
        {
            timeRequest = Time.realtimeSinceStartupAsDouble;
            UnityWebRequest webRequest = CreateApiGetRequest(url);
            UnityWebRequestAsyncOperation webResponse = webRequest.SendWebRequest();
            while (!webResponse.isDone)
            {

            }

            string[] pages = url.Split(' ');
            int page = pages.Length - 1;

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
                    string data = webRequest.downloadHandler.text;
                    timeResponse = Time.realtimeSinceStartupAsDouble;
                    rtt = Mathf.Round((float)(timeResponse - timeRequest) * 100000.0f) / 100.0f;
                    //Debug.Log("RTT = " + rtt + " milliseconds" + data);
                    break;
            }

            yield return new WaitForSecondsRealtime(1f);
        }
    }

    // Send a HTTP request to a host
    IEnumerator SendHeadRequest() {

        // Initialize the timer
        
        WaitForSecondsRealtime waitTime;

        while (true) {

            // Create a request and a response recipient
            timeRequest = Time.realtimeSinceStartupAsDouble;
            UnityWebRequest webRequest = CreateHeadRequest();
            UnityWebRequestAsyncOperation webResponse = webRequest.SendWebRequest();
            // Wait until a response is received 
            yield return new WaitUntil(() => webResponse.isDone);

            // Get the url first part
            string[] pages = url.Split(' ');
            int page = pages.Length - 1;

            if (webResponse.isDone)
            {
                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                        Debug.LogError(pages[page] + ": Connection error!");
                        rtt = -1;
                        break;
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                        rtt = -1;
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                        rtt = -1;
                        break;
                    case UnityWebRequest.Result.Success:
                        timeResponse = Time.realtimeSinceStartupAsDouble;
                        rtt = Mathf.Round((float)(timeResponse - timeRequest) * 100000.0f) / 100.0f;
                        Debug.Log("RTT by HEAD request = " + rtt + " milliseconds");
                        break;
                }
            }
            else {
                rtt = -1;
                Debug.Log($"Head Manager --> Error reaching the Web Server {pages[page]} with HEAD request");
            }

            // Extract the value of ping request
            double timeSpent = Time.realtimeSinceStartup - timeRequest;
            double waitTimeValue = expectedDelay - timeSpent;

            // Calculate the time to wait for new request
            if (waitTimeValue <= 1.00)
            {
                waitTime = new WaitForSecondsRealtime((float) waitTimeValue);
            }
            else if (waitTimeValue == 0.00)
            {
                waitTime = new WaitForSecondsRealtime(0.00f);
            }
            else
            {
                waitTime = new WaitForSecondsRealtime((float) (waitTimeValue + expectedDelay));
            }

            // Suspend the thread until the time passes
            yield return waitTime;
        }
        
    }

    // Ping a host
    IEnumerator PingHost()
    {
        // Initialize the timer
        float expectedDelay = 1.00f;
        WaitForSecondsRealtime waitTime;

        while (true)
        {
            // Reference time for this ping request
            float startTime = Time.realtimeSinceStartup;
            // Create a ping request to the host
            Ping ping = new Ping(hostIp);

            // Suspend the thread until the ping flag is true
            yield return new WaitUntil(() => ping.isDone);
            rtt_ping = ping.time;

            // Extract the value of ping request
            float timeSpent = Time.realtimeSinceStartup - startTime;
            float waitTimeValue = expectedDelay - timeSpent;
            if (ping.isDone)
            {


                if (waitTimeValue <= 1.00)
                {
                    rtt_ping = ping.time;
                    waitTime = new WaitForSecondsRealtime(waitTimeValue);
                }
                else if (waitTimeValue == 0.00)
                {
                    rtt_ping = ping.time;
                    waitTime = new WaitForSecondsRealtime(0.00f);
                }
                else
                {
                    rtt_ping = ping.time;
                    waitTime = new WaitForSecondsRealtime(waitTimeValue + expectedDelay);
                }
                Debug.Log($"Ping = {rtt_ping}");
            }
            else
            {
                rtt_ping = -1;
                Debug.Log($"PingManager --> El ping al host {hostIp} ha fallado");
                waitTime = new WaitForSecondsRealtime(waitTimeValue);
            }

            // Wait the time until next function call
            yield return waitTime;
        }
    }

    

    #endregion

    #region CLASS METHODS
    // Create a GET HTTP request
    public static UnityWebRequest CreateApiGetRequest(string url, string body = null)
    {
        return CreateHttpRequest(UnityWebRequest.kHttpVerbGET, url, body, "application/json");
    }

    // Create a POST HTTP request
    public static UnityWebRequest CreateApiPostRequest(string url, string body = null)
    {
        return CreateHttpRequest(UnityWebRequest.kHttpVerbPOST, url, body, "application/json");
    }

    public UnityWebRequest CreateHeadRequest() {
        return CreateHttpRequest(UnityWebRequest.kHttpVerbHEAD, url);
    }

    private static UnityWebRequest CreateHttpRequest(string method, string url, string body = null, string typeContent = null)
    {
        // Initialize web request
        var request = new UnityWebRequest();

        // Chose the format
        switch (method) {
            case UnityWebRequest.kHttpVerbHEAD:
                request = UnityWebRequest.Head(url);
                break;
            case UnityWebRequest.kHttpVerbGET:
                request = ConfigureCustomHttpRequest(method, url, body, typeContent);
                break;
            case UnityWebRequest.kHttpVerbPOST:
                request = ConfigureCustomHttpRequest(method, url, body, typeContent);
                break;
            default:
                request = null;
                break;
        }

        // Return the created request
        return request;
    }

    public static UnityWebRequest ConfigureCustomHttpRequest(string method, string url, string body = null, string typeContent = null) {

        var request = new UnityWebRequest();
        request.url = url;
        request.method = method;
        request.downloadHandler = new DownloadHandlerBuffer();
        request.uploadHandler = new UploadHandlerRaw(string.IsNullOrEmpty(body) ? null : Encoding.UTF8.GetBytes(body));
        request.SetRequestHeader("Accept", typeContent);
        request.SetRequestHeader("Content-Type", typeContent);
        request.timeout = 50;
        return request;
    }


    // Resolve host
    public void ResolveHost()
    {
        // Extract the host from the URL
        Uri uri = new Uri(url);
        string host = uri.Host;

        // Resolve the host to an IP address
        ipAddresses = Dns.GetHostAddresses(host);

        // Print the IP addresses
        foreach (IPAddress ipAddress in ipAddresses)
        {
            Debug.Log($"{host} IP address: {ipAddress}");
        }
    }

    
    // Determine the IP to ping
    void GetValidIpHost()
    {
        foreach (IPAddress ipAddress in ipAddresses)
        {
            if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
            {
                SetHostIP(ipAddress.ToString());
                break;
            }
        }
    }

    

    // GETTTERS AND SETTERS
    public void SetURL(string urlValue) {
        url = urlValue;
    }

    public void SetHostIP(string ipValue) {
        hostIp = ipValue;
    }

    public string GetURI() {
        return url;
    }

    public double GetRTT() {
        return rtt;
    }

    public double GetRTTPing() {
        return rtt_ping;
    }

    // Shared instance
    public static LatencyManager GetInstance() {
        return _sharedInstance;
    }

    #endregion
}
