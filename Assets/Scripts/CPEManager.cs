using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HuaweiAPI;
using System.Xml;
using SimpleJSON;

public class CPEManager : MonoBehaviour
{
    string ip = "192.168.8.1";
    string username = "admin";
    string password = "areyouready?1";

    // Start is called before the first frame update
    void Start()
    {
        /*//check login state
        Debug.Log("Checking login state..");
        if (HuaweiAPI.HuaweiAPI.MethodExample.loginState(ip) == true)
        {
            Debug.Log("Already logged in.");
        }
        else
        {
            //not logged in
            Debug.Log("Not logged in, logging in..");
            var login = HuaweiAPI.HuaweiAPI.MethodExample.UserLogin(ip, username, password);
            if (login == false)
            {
                Debug.Log("Failed to log in.");
                return;
            }
            else Debug.Log("logged in maybe");
        }

        
        
        
        XmlElement device_information = HuaweiAPI.HuaweiAPI.Tools.GET(ip, "api/net/network").DocumentElement;

        
        
        //Debug.Log(jsonSignal.ToString());
        //Debug.Log(jsonTraffic.ToString());

        Debug.Log(device_information.OuterXml);
        
        JSONNode testjson = Xml_group.ConvertXmlToJson(device_information);
        Debug.Log(testjson.ToString());*/
        Debug.Log(resetTrafficStats());
        //Debug.Log(GetSignalStats());
        //Debug.Log(GetCPEStats());
    }

    // Update is called once per frame
    void Update()
    {

    }






    #region CLASS METHODS
    bool isConnected(int max_attempts = 3)
    {
        // Check the connection status with the CPE API
        Debug.Log("(CPE Manager) --> Checking login state...");
        if (HuaweiAPI.HuaweiAPI.MethodExample.loginState(ip) == true)
        {
            Debug.Log("(CPE Manager) --> Already logged in.");
            return true;
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


    bool ConnectToCPE() {
        return HuaweiAPI.HuaweiAPI.MethodExample.UserLogin(ip, username, password);
    }

    public string resetTrafficStats() {
        // Check if the there is connection with the CPE API
        if (isConnected())
        {
            var data = @"<request><ClearTraffic>1</ClearTraffic></request>";
            XmlElement response = HuaweiAPI.HuaweiAPI.Tools.POST(ip, data, "api/monitoring/clear-traffic").DocumentElement;
            JSONNode jsonResponse = Xml_group.ConvertXmlToJson(response);
            return jsonResponse.ToString();
        }
        return null;
    }

    public string GetSignalStats() {
        // Check if the there is connection with the CPE API
        if (isConnected()) {
            XmlElement signal = HuaweiAPI.HuaweiAPI.Tools.GET(ip, "api/device/signal").DocumentElement;
            JSONNode jsonSignal = Xml_group.ConvertStatsXmlToJson(signal);
            return jsonSignal.ToString();
        }
        return null;
    }

    public string GetTrafficStats()
    {
        // Check if the there is connection with the CPE API
        if (isConnected())
        {
            XmlElement traffic_stats = HuaweiAPI.HuaweiAPI.Tools.GET(ip, "api/monitoring/traffic-statistics").DocumentElement;
            JSONNode jsonTraffic = Xml_group.ConvertStatsXmlToJson(traffic_stats);
            return jsonTraffic.ToString();
        }
        return null;
    }

    public string GetDeviceStatus()
    {
        // Check if the there is connection with the CPE API
        if (isConnected())
        {
            XmlElement device_information = HuaweiAPI.HuaweiAPI.Tools.GET(ip, "api/device/information").DocumentElement;
            JSONNode json_device_information = Xml_group.ConvertXmlToJson(device_information);
            return json_device_information.ToString();
        }
        return null;
    }

    public string GetCPEStats() {
        // Check if the there is connection with the CPE API
        JSONNode jsonNode = new JSONObject();

        if (isConnected())
        {
            XmlElement signal = HuaweiAPI.HuaweiAPI.Tools.GET(ip, "api/device/signal").DocumentElement;
            JSONNode jsonSignal = Xml_group.ConvertStatsXmlToJson(signal);

            XmlElement traffic_stats = HuaweiAPI.HuaweiAPI.Tools.GET(ip, "api/monitoring/traffic-statistics").DocumentElement;
            JSONNode jsonTraffic = Xml_group.ConvertStatsXmlToJson(traffic_stats);

            XmlElement device_information = HuaweiAPI.HuaweiAPI.Tools.GET(ip, "api/device/information").DocumentElement;
            JSONNode json_device_information = Xml_group.ConvertXmlToJson(device_information);

            jsonNode["signal"] = jsonSignal;
            jsonNode["traffic"] = jsonTraffic;
            jsonNode["info"] = json_device_information;
            
            return jsonNode.ToString();
        }
        return null;
    }


    #endregion
}

