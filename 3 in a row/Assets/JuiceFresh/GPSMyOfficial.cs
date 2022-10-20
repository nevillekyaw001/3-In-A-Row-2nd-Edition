using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SceneManagement;

public class GPSMyOfficial : MonoBehaviour
{
    //public static GPSMyOfficial instance;
    bool connectedGPS;

    public int savedlevel;
    public bool LoadClicked = false;

    public GameObject WarningPanel;
    

    
    
    private void Awake()
    {
        //if (instance == null)
        //{
        //    instance = this;
        //}
        //else
        //{
        //    Destroy(gameObject);
        //}
        //DontDestroyOnLoad(this);

        //savedlevel = PlayerPrefs.GetInt("OpenLevel");

        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();

        
    }

    // Start is called before the first frame update
    void Start()
    {
        LoginintoGPS();
    }

    // Update is called once per frame
    void Update()
    {
        //if(SceneManager.GetActiveScene().buildIndex == 1)
        //{
        //    Debug.Log(PlayerPrefs.GetInt("OpenLevel"));
        //}
        
    }

    void LoginintoGPS()
    {
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);

    }

    void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            connectedGPS = true;
            //debugtext.text = ("GPS Login = " + connectedGPS);
        }
        else
            connectedGPS = false;
    }

    public void PlusButton()
    {
        WarningPanel.SetActive(true);
    }
    public void CloseButton()
    {
        WarningPanel.SetActive(false);
    }


}
