using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using SimpleJSON;


public class Xml_group : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Static functions that parses from xml document to jsonNode object (an actual JSON object with ToString() method
    public static JSONNode ConvertStatsXmlToJson(XmlNode node)
    {
        JSONNode jsonNode = new JSONObject();
        foreach (XmlNode childNode in node)
        {
            // Check if the xml node has several values separated by " " (space) and instead of being xml child nodes
            if (childNode.InnerText.Contains(" "))
            {
                string[] subnodes = childNode.InnerText.Split(" ");
                foreach(string subnode in subnodes) {
                    string[] keyvalue = subnode.Split(":");
                    jsonNode[$"{childNode.Name}_{keyvalue[0]}"] = keyvalue[1];
                }
            }
            // If the node is a simple node, only add a json node with its value
            else {
                jsonNode[childNode.Name] = childNode.InnerText;
            }
            
            
        }
        return jsonNode;
    }

    public static JSONNode ConvertXmlToJson(XmlNode node)
    {
        JSONNode jsonNode = new JSONObject();
        foreach (XmlNode childNode in node)
        {
            jsonNode[childNode.Name] = childNode.InnerText;
        }
        return jsonNode;
    }

}
