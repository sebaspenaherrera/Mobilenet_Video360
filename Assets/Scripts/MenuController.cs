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
    public Canvas mainSection;
    public Canvas configSection;
    public Canvas cpeSection;

    // Default variables *************************************************************
    private static readonly List<string> media_url_list = new List<string> {
        "https://bitmovin-a.akamaihd.net/content/playhouse-vr/mpds/105560.mpd",
        "https://storage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4"
    };

    private const string url_rest_def = "http://127.0.0.1:8000/video360";
    private const int iterations_def = 60;
    private const int duration_def = 120;
    private const int mode_def = 0;  // 0 for Testbed (send stats per sesssion) and
                                     // 1 for Demo (send stats per second)
    private const int video_def = 0; // 0 for Playhouse-vr and 1 for Big Buck Bunny
    private const string videotag_def = "Playhouse-vr";
    private readonly string video_url_def = media_url_list[0];

    private const int cpe_enabled_def = 1;
    private const string cpe_url_def = "192.168.8.1";
    private const string cpe_username_def = "admin";
    private const string cpe_password_def = "areyouready?1";
    private const int cpe_interval_def = 1;
    private const int cpe_max_attempts_def = 3;

    

    // Objects in the UI config section **********************************************
    private CanvasGroup canvas_config;
    private TMPro.TextMeshProUGUI state_txt;
    private TMPro.TMP_InputField restURL_input;
    private TMPro.TMP_InputField iterations_input;
    private TMPro.TMP_InputField duration_input;
    private TMPro.TMP_Dropdown mode_dropdown;
    private TMPro.TMP_Dropdown video_dropdown;
    private Toggle cpe_toggle;

    // Objects in the UI main section ************************************************
    private CanvasGroup canvas_main;
    private TMPro.TMP_Text restURL_txt;
    private TMPro.TMP_Text iterations_txt;
    private TMPro.TMP_Text duration_txt;
    private TMPro.TMP_Text mode_txt;
    private TMPro.TMP_Text cpe_txt;
    private TMPro.TMP_Text video_txt;

    // Objects in the UI CPE section ************************************************
    private CanvasGroup canvas_cpe;
    private TMPro.TMP_InputField cpeURL_input;
    private TMPro.TMP_InputField username_input;
    private TMPro.TMP_InputField password_input;
    private TMPro.TMP_InputField interval_input;
    private TMPro.TMP_InputField attempts_input;
    private TMPro.TextMeshProUGUI stateCPE_txt;

    // Local variables ***************************************************************
    private string m_url = "";
    private int m_iterations = 0;
    private int m_duration = 0;
    private int m_mode = 0;
    private int m_cpe = 0;
    private int m_video = 0;
    private string m_videotag = "";

    private string m_cpe_url = "";
    private string m_username = "";
    private string m_password = "";
    private int m_interval = 0;
    private int m_attempts = 0;


    private void Awake()
    {
        // Set the application to work even in background
        Application.runInBackground = true;

        // Get the canvas group components for each section
        mainSection.TryGetComponent<CanvasGroup>(out canvas_main);
        configSection.TryGetComponent<CanvasGroup>(out canvas_config);
        cpeSection.TryGetComponent<CanvasGroup>(out canvas_cpe);

        // Get the children components from the main section
        restURL_txt = GameObject.FindGameObjectWithTag("restURLTxt").GetComponent<TMPro.TMP_Text>();
        iterations_txt = GameObject.FindGameObjectWithTag("iterationsTxt").GetComponent<TMPro.TMP_Text>();
        duration_txt = GameObject.FindGameObjectWithTag("durationTxt").GetComponent < TMPro.TMP_Text>();
        mode_txt = GameObject.FindGameObjectWithTag("modeTxt").GetComponent<TMPro.TMP_Text>();
        cpe_txt = GameObject.FindGameObjectWithTag("cpeTxt").GetComponent<TMPro.TMP_Text>();
        video_txt = GameObject.FindGameObjectWithTag("videoTxt").GetComponent<TMPro.TMP_Text>();

        // Get the children components from the config section
        restURL_input = GameObject.FindGameObjectWithTag("restURLInput").GetComponent<TMPro.TMP_InputField>();
        iterations_input = GameObject.FindGameObjectWithTag("iterationsInput").GetComponent<TMPro.TMP_InputField>();
        duration_input = GameObject.FindGameObjectWithTag("durationInput").GetComponent<TMPro.TMP_InputField>();
        mode_dropdown = GameObject.FindGameObjectWithTag("modeDropdown").GetComponent<TMPro.TMP_Dropdown>();
        video_dropdown = GameObject.FindGameObjectWithTag("videoDropdown").GetComponent<TMPro.TMP_Dropdown>();
        state_txt = GameObject.FindGameObjectWithTag("stateConfigTxt").GetComponent<TMPro.TextMeshProUGUI>();
        cpe_toggle = GameObject.FindGameObjectWithTag("cpeToggle").GetComponent<Toggle>();

        // Get the children components from the CPE section
        cpeURL_input = GameObject.FindGameObjectWithTag("cpeURLInput").GetComponent<TMPro.TMP_InputField>();
        username_input = GameObject.FindGameObjectWithTag("usernameInput").GetComponent<TMPro.TMP_InputField>();
        password_input = GameObject.FindGameObjectWithTag("passwordInput").GetComponent<TMPro.TMP_InputField>();
        interval_input = GameObject.FindGameObjectWithTag("intervalInput").GetComponent<TMPro.TMP_InputField>();
        attempts_input = GameObject.FindGameObjectWithTag("attemptsInput").GetComponent<TMPro.TMP_InputField>();
        stateCPE_txt = GameObject.FindGameObjectWithTag("stateCPETxt").GetComponent<TMPro.TextMeshProUGUI>();


        // Initialize variables
        //CONFIG
        m_url = PlayerPrefs.GetString("url_rest");
        m_iterations = PlayerPrefs.GetInt("iterations");
        m_duration = PlayerPrefs.GetInt("duration");
        m_mode = PlayerPrefs.GetInt("mode");
        m_cpe = PlayerPrefs.GetInt("cpe_enabled");
        m_video = PlayerPrefs.GetInt("video");
        m_videotag = PlayerPrefs.GetString("videotag");
        //CPE
        m_cpe_url = PlayerPrefs.GetString("url_cpe");
        m_username = PlayerPrefs.GetString("username");
        m_password = PlayerPrefs.GetString("password");
        m_interval = PlayerPrefs.GetInt("cpe_interval");
        m_attempts = PlayerPrefs.GetInt("max_attempts");
        
    }

    // Start is called before the first frame update
    void Start()
    {
        // Subscribe to the events
        // CONFIG
        restURL_input.onEndEdit.AddListener(delegate { ReadURL(restURL_input); });
        iterations_input.onEndEdit.AddListener(delegate { ReadIterations(iterations_input); });
        duration_input.onEndEdit.AddListener(delegate { ReadDuration(duration_input);});
        mode_dropdown.onValueChanged.AddListener(delegate { ReadMode(mode_dropdown); });
        cpe_toggle.onValueChanged.AddListener(delegate { ReadBooltoInt(cpe_toggle, out m_cpe); UpdateCPEState(); });
        video_dropdown.onValueChanged.AddListener(delegate { ReadDropdownValue(video_dropdown, out m_video); ReadDropdownTag(video_dropdown, out m_videotag); });
        // CPE
        cpeURL_input.onEndEdit.AddListener(delegate { ReadString(cpeURL_input, out m_cpe_url); });
        username_input.onEndEdit.AddListener(delegate { ReadString(username_input, out m_username); });
        password_input.onEndEdit.AddListener(delegate { ReadString(password_input, out m_password); });
        interval_input.onValueChanged.AddListener(delegate { ReadInt(interval_input, out m_interval); });
        attempts_input.onValueChanged.AddListener(delegate { ReadInt(attempts_input, out m_attempts); });

        // Overwrite the text fields in the main section
        restURL_txt.SetText(m_url);
        iterations_txt.SetText(m_iterations.ToString());
        duration_txt.SetText(m_duration.ToString());
        mode_txt.SetText(MapMode(m_mode));
        cpe_txt.SetText(MapCPEState(m_cpe));
        video_txt.SetText(media_url_list[m_video]);

        // Set the presaved configuration in the config section
        restURL_input.text = m_url;
        iterations_input.text = m_iterations.ToString();
        duration_input.text = m_duration.ToString();
        mode_dropdown.value = m_mode;
        cpe_toggle.isOn = m_cpe == 1 ? true : false;
        video_dropdown.value = m_video;

        // Set the presaved configuration in the cpe section
        cpeURL_input.text = m_cpe_url;
        username_input.text = m_username;
        password_input.text = m_password;
        interval_input.text = m_interval.ToString();
        attempts_input.text = m_attempts.ToString();
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    // FUNCTIONS

    public void ClickConfig()
    {
        // Disable the main section object and enable the config section one
        canvas_main.alpha = 0;
        canvas_main.interactable = false;
        canvas_config.alpha = 1;
        canvas_config.interactable = true;
        canvas_cpe.alpha = 0;
        canvas_cpe.interactable = false;
    }


    public void ClickBack()
    {
        // Disable the config section object and enable the main section one
        canvas_config.alpha = 0;
        canvas_config.interactable = false;
        canvas_cpe.alpha = 0;
        canvas_cpe.interactable = false;
        canvas_main.alpha = 1;
        canvas_main.interactable = true;
    }

    public void ClickConfigCPE() {
        // Disable the config and main sections object and enable the cpe section one
        canvas_config.alpha = 0;
        canvas_config.interactable = false;
        canvas_main.alpha = 0;
        canvas_main.interactable = false;
        canvas_cpe.alpha = 1;
        canvas_cpe.interactable = true;
    }

    public void ClickBackCpe()
    {
        // Disable the CPE section and enable the config one
        canvas_main.alpha = 0;
        canvas_main.interactable = false;
        canvas_cpe.alpha = 0;
        canvas_cpe.interactable = false;
        canvas_config.alpha = 1;
        canvas_config.interactable = true;
    }

    public void SaveCpeConfig() {
        // Save the values read from the fields
        // Read the current values
        ReadString(cpeURL_input, out m_cpe_url);
        ReadString(username_input, out m_username);
        ReadString(password_input, out m_password);
        ReadInt(interval_input, out m_interval);
        ReadInt(attempts_input, out m_attempts);

        // Overwrite the local variables and set the player preferences
        PlayerPrefs.SetString("url_cpe", m_cpe_url);
        PlayerPrefs.SetString("username", m_username);
        PlayerPrefs.SetString("password", m_password);
        PlayerPrefs.SetInt("cpe_interval", m_interval);
        PlayerPrefs.SetInt("max_attempts", m_attempts);

        // Set a warning message
        stateCPE_txt.text = $"{GetTime()}: New CPE configuration has been saved";
    }

    public void ClickResetCPE() {
        // Rewrite the session preferences with the default values
        PlayerPrefs.SetString("url_cpe", cpe_url_def);
        PlayerPrefs.SetString("username", cpe_username_def);
        PlayerPrefs.SetString("password", cpe_password_def);
        PlayerPrefs.SetInt("cpe_interval", cpe_interval_def);
        PlayerPrefs.SetInt("max_attempts", cpe_max_attempts_def);

        // Reset the local variables to the default values
        m_cpe_url = cpe_url_def;
        m_username = cpe_username_def;
        m_password = cpe_password_def;
        m_interval = cpe_interval_def;
        m_attempts = cpe_max_attempts_def;

        // Reset the values of the input fields and dropdown menu
        cpeURL_input.text = cpe_url_def;
        username_input.text = cpe_username_def;
        password_input.text = cpe_password_def;
        interval_input.text = cpe_interval_def.ToString();
        attempts_input.text = cpe_max_attempts_def.ToString();

        // Reset the text fields in the config section
        stateCPE_txt.text = $"{GetTime()}: Default CPE configuration has been loaded.\nPlease save the configuration!";
    }


    public void ClickSave()
    {

        // Read the new configuration values
        m_url = restURL_input.text;
        _ = int.TryParse(iterations_input.text, out m_iterations);
        _ = int.TryParse(duration_input.text, out m_duration);
        m_mode = mode_dropdown.value;
        ReadBooltoInt(cpe_toggle, out m_cpe);
        ReadDropdownValue(video_dropdown, out m_video);
        ReadDropdownTag(video_dropdown, out m_videotag);
        

        // Overwrite the local variables and set the player preferences
        PlayerPrefs.SetString("url_rest", m_url);
        PlayerPrefs.SetInt("iterations", m_iterations);
        PlayerPrefs.SetInt("duration", m_duration);
        PlayerPrefs.SetInt("mode", m_mode);
        PlayerPrefs.SetInt("cpe_enabled", m_cpe);
        PlayerPrefs.SetInt("video", m_video);
        PlayerPrefs.SetString("videotag", m_videotag);
        PlayerPrefs.SetString("url_video", media_url_list[m_video]);

        // Overwrite the text fields in the main section
        restURL_txt.SetText(m_url);
        iterations_txt.SetText(m_iterations.ToString());
        duration_txt.SetText(m_duration.ToString());
        mode_txt.SetText(MapMode(m_mode));
        cpe_txt.SetText(MapCPEState(m_cpe));
        video_txt.SetText(media_url_list[m_video]);

        // Set a warning message
        state_txt.text = $"{GetTime()}: New configuration has been saved";

    }


    public void ClickReset()
    {
        // Rewrite the session preferences with the default values
        PlayerPrefs.SetString("url_rest", url_rest_def);
        PlayerPrefs.SetInt("iterations", iterations_def);
        PlayerPrefs.SetInt("duration", duration_def);
        PlayerPrefs.SetInt("mode", mode_def);
        PlayerPrefs.SetInt("cpe_enabled", cpe_enabled_def);
        PlayerPrefs.SetInt("video", video_def);
        PlayerPrefs.SetString("videotag", videotag_def);
        PlayerPrefs.SetString("url_video", video_url_def);

        // Reset the local variables to the default values
        m_url = url_rest_def;
        m_iterations = iterations_def;
        m_duration = duration_def;
        m_mode = mode_def;
        m_cpe = cpe_enabled_def;
        m_video = video_def;
        m_videotag = videotag_def;

        // Reset the values of the input fields and dropdown menu
        restURL_input.text = url_rest_def;
        iterations_input.text = iterations_def.ToString();
        duration_input.text = duration_def.ToString();
        mode_dropdown.value = mode_def;
        cpe_toggle.isOn = cpe_enabled_def == 1 ? true : false;
        video_dropdown.value = video_def;

        // Overwrite the text fields in the main section
        restURL_txt.SetText(url_rest_def);
        iterations_txt.SetText(iterations_def.ToString());
        duration_txt.SetText(duration_def.ToString());
        mode_txt.SetText(MapMode(mode_def));
        cpe_txt.SetText(MapCPEState(cpe_enabled_def));
        video_txt.SetText(video_url_def);

        // Reset the text fields in the config section
        state_txt.text = $"{GetTime()}: Default configuration has been loaded.\nPlease save the configuration!";
    }

    public void ClickExit()
    {
        Application.Quit(0);
    }


    public void ClickStart()
    {
        SceneManager.LoadScene("Player");
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

    string MapCPEState(int value) {
        return value == 1 ? $"Enabled at: http://{m_cpe_url}" : "Disabled";
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

    void ReadString(TMPro.TMP_InputField text, out string variable)
    {
        variable = text.text;
    }

    void ReadInt(TMPro.TMP_InputField text, out int variable)
    {
        variable = int.Parse(text.text);
    }

    void ReadDouble(TMPro.TMP_InputField text, out double variable)
    {
        variable = double.Parse(text.text);
    }

    void ReadBooltoInt(Toggle toggle, out int variable)
    {
        variable = toggle.isOn ? 1 : 0;
    }

    void UpdateCPEState() {
        cpe_txt.text = MapCPEState(m_cpe);
        PlayerPrefs.SetInt("cpe_enabled", m_cpe);
    }

    void ReadDropdownTag(TMPro.TMP_Dropdown dropdown, out string variable) {
        variable = dropdown.options[dropdown.value].text;
    }

    void ReadDropdownValue(TMPro.TMP_Dropdown dropdown, out int variable) {
        variable = dropdown.value;
    }

    private string GetTime()
    {
        return DateTime.Now.ToString("HH:mm:ss");
    }
}