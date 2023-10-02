using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class MenuController : MonoBehaviour
{
    // --------------------------------- ATTRIBUTES ----------------------------------
    // Objects with the sections in the UI *******************************************
    public GameObject mainSection;
    public GameObject configSection;

    // Default variables *************************************************************
    private const string url_rest_def = "http://192.168.196.3:6000/data/vr";
    private const int iterations_def = 60;
    private const int duration_def = 120;
    private const int mode_def = 1;  // 1 for Testbed (send stats per sesssion) and
                                       // 0 for Demo (send stats per second)

    // Objects in the UI config section **********************************************
    private CanvasGroup canvas_config;
    private TMPro.TextMeshProUGUI state_txt;
    private TMPro.TMP_InputField restURL_input;
    private TMPro.TMP_InputField iterations_input;
    private TMPro.TMP_InputField duration_input;
    private TMPro.TMP_Dropdown mode_dropdown;

    // Objects in the UI main section ************************************************
    private CanvasGroup canvas_main;
    private TMPro.TMP_Text restURL_txt;
    private TMPro.TMP_Text iterations_txt;
    private TMPro.TMP_Text duration_txt;
    private TMPro.TMP_Text mode_txt;

    // Local variables ***************************************************************
    private string m_url = "";
    private int m_iterations = 0;
    private int m_duration = 0;
    private int m_mode = 1;


    private void Awake()
    {
        // Get the canvas group components for each section
        mainSection.TryGetComponent<CanvasGroup>(out canvas_main);
        configSection.TryGetComponent<CanvasGroup>(out canvas_config);

        // Get the children components for the main section
        restURL_txt = GameObject.FindGameObjectWithTag("restURLTxt").GetComponent<TMPro.TMP_Text>();
        iterations_txt = GameObject.FindGameObjectWithTag("iterationsTxt").GetComponent<TMPro.TMP_Text>();
        duration_txt = GameObject.FindGameObjectWithTag("durationTxt").GetComponent < TMPro.TMP_Text>();
        mode_txt = GameObject.FindGameObjectWithTag("modeTxt").GetComponent<TMPro.TMP_Text>();

        // Get the children components for the config section
        restURL_input = GameObject.FindGameObjectWithTag("restURLInput").GetComponent<TMPro.TMP_InputField>();
        iterations_input = GameObject.FindGameObjectWithTag("iterationsInput").GetComponent<TMPro.TMP_InputField>();
        duration_input = GameObject.FindGameObjectWithTag("durationInput").GetComponent<TMPro.TMP_InputField>();
        mode_dropdown = GameObject.FindGameObjectWithTag("modeDropdown").GetComponent<TMPro.TMP_Dropdown>();
        state_txt = GameObject.FindGameObjectWithTag("stateConfigTxt").GetComponent<TMPro.TextMeshProUGUI>();

        // Initialize variables
        m_url = PlayerPrefs.GetString("url_rest");
        m_iterations = PlayerPrefs.GetInt("iterations");
        m_duration = PlayerPrefs.GetInt("duration");
        m_mode = PlayerPrefs.GetInt("mode");

        
    }

    // Start is called before the first frame update
    void Start()
    {
        // Subscribe to the events
        restURL_input.onEndEdit.AddListener(delegate { ReadURL(restURL_input); });
        iterations_input.onEndEdit.AddListener(delegate { ReadIterations(iterations_input); });
        duration_input.onEndEdit.AddListener(delegate { ReadDuration(duration_input);});
        mode_dropdown.onValueChanged.AddListener(delegate { ReadMode(mode_dropdown); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // FUNCTIONS

    public void ClickConfig()
    {
        // Disable the main section object and enable the config section one
        //mainSection.SetActive(false);
        //configSection.SetActive(true);
        canvas_main.alpha = 0;
        canvas_main.interactable = false;
        canvas_config.alpha = 1;
        canvas_config.interactable = true;
        duration_txt.text = "Test";
        Debug.Log(duration_txt.text);
    }

    public void ClickBack()
    {
        // Disable the config section object and enable the main section one
        //configSection.SetActive(false);
        //mainSection.SetActive(true);
        canvas_config.alpha = 0;
        canvas_config.interactable = false;
        canvas_main.alpha = 1;
        canvas_main.interactable = true;
    }

    public void ClickExit()
    {
        Application.Quit(0);
    }

    public void ClickStart()
    {
        // TODO
    }

    public void ClickSet()
    {

        // Read the new configuration values
        m_url = restURL_input.text;
        _ = int.TryParse(iterations_input.text, out m_iterations);
        _ = int.TryParse(duration_input.text, out m_duration);
        m_mode = mode_dropdown.value;

        // Overwrite the local variables and set the player preferences
        PlayerPrefs.SetString("url_rest", m_url);
        PlayerPrefs.SetInt("iterations", m_iterations);
        PlayerPrefs.SetInt("duration", m_duration);
        PlayerPrefs.SetInt("mode", m_mode);

        // Set a warning message
        state_txt.text = $"{GetTime()}: New configuration has been set Dropdown value:{m_mode}";

        // Overwrite the text fields in the main section
        restURL_txt.SetText(m_url);
        iterations_txt.SetText(m_iterations.ToString());
        duration_txt.SetText(m_duration.ToString());
        mode_txt.SetText(MapMode(m_mode));

    }

    public void ClickReset()
    {
        // Rewrite the session preferences with the default values
        PlayerPrefs.SetString("url_rest", url_rest_def);
        PlayerPrefs.SetInt("iterations", iterations_def);
        PlayerPrefs.SetInt("duration", duration_def);
        PlayerPrefs.SetInt("mode", mode_def);

        // Reset the text fields in the config section
        state_txt.text = "Default configuration has been reinstated";
        //restURL_input.text = url_rest_def;
        iterations_input.text = iterations_def.ToString();
        duration_input.text = duration_def.ToString();
        //mode_dropdown.value = mode_def;
        restURL_input.interactable = true;
        restURL_input.textComponent.text = "Hola";

        // Overwrite the text fields in the main section
        restURL_txt.SetText(url_rest_def);
        iterations_txt.SetText(iterations_def.ToString());
        duration_txt.SetText(duration_def.ToString());
        mode_txt.SetText(MapMode(mode_def));
    }

    public string MapMode(int value)
    {
        if (value == 0)
        {
            return "Testbed";
        }
        else if (value == 1)
        {
            return "Demo";
        }
        else return "Error";
    }

    private void ReadURL(TMPro.TMP_InputField text)
    {
        m_url = text.text;
    }

    private void ReadIterations(TMPro.TMP_InputField text)
    {
        m_iterations = int.Parse(text.text);
    }

    private void ReadDuration(TMPro.TMP_InputField text)
    {
        m_duration = int.Parse(text.text);
    }

    private void ReadMode(TMPro.TMP_Dropdown drop)
    {
        m_duration = drop.value;
    }

    private string GetTime()
    {
        return DateTime.Now.ToString("HH:mm:ss");
    }
}