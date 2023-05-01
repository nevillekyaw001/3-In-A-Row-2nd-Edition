using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine.SceneManagement;
using TMPro;

public class CountDownTimer: MonoBehaviour
 {
    public static CountDownTimer instance;
    public Button AddGemButton;
    public bool ButtonIsClicked = false;
    public TMP_Text countDown;
    float ElapsedTime;
    float NextTimeToAppear = 32f;

    string TempStr;

    public bool LoadClicked = false;

    public GameObject WarningPanel;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (ButtonIsClicked)
        {
            AddGemButton.interactable = false;
            NextTimeToAppear -= Time.deltaTime;
            countDown.SetText(NextTimeToAppear.ToString());
        }
        else
        {
            AddGemButton.interactable = true;
        }

        if (NextTimeToAppear <= 0)
        {
            ButtonIsClicked = false;
            NextTimeToAppear = 32f;
        }

        
    }
    //cloud save
    bool issaving = false;
    string SAVE_NAME = "savegames";
    public void OpenSave(bool saving)
    {
        if (Social.localUser.authenticated)
        {
            issaving = saving;
            ((PlayGamesPlatform)Social.Active).SavedGame.OpenWithAutomaticConflictResolution(SAVE_NAME, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, SaveGameOpen);
        }
    }

    private void SaveGameOpen(SavedGameRequestStatus status, ISavedGameMetadata meta)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            if (issaving) //If issaving bool is true, we are saving data to cloud
            {
                byte[] data = System.Text.ASCIIEncoding.ASCII.GetBytes(GetDataToStoreInCloud());

                SavedGameMetadataUpdate update = new SavedGameMetadataUpdate.Builder().Build();

                ((PlayGamesPlatform)Social.Active).SavedGame.CommitUpdate(meta, update, data, saveupdate);
            }
            else //If issaving bool is false, we are opening our saved data from cloud
            {
                ((PlayGamesPlatform)Social.Active).SavedGame.ReadBinaryData(meta, ReadDatafromCloud);
            }
        }

    }

    private void ReadDatafromCloud(SavedGameRequestStatus status, byte[] data)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            string savedata = System.Text.ASCIIEncoding.ASCII.GetString(data);

            LoadDataFromCloudToOurGame(savedata);
        }
    }

    public void LoadDataFromCloudToOurGame(string savedata) //Loading the data
    {
        string[] data = savedata.Split("|");
        TempStr = data[0];
        PlayerPrefs.SetInt("OpenLevel", int.Parse(TempStr));

        //GPSTEST.level = int.Parse(cloudStringArr);
    }

    public string GetDataToStoreInCloud() //we are setting the value that we are going to store the data in cloud , Saving data
    {
        string Data = "";
        //dataToSave += PlayerPrefs.GetInt("openLevel").ToString();

        //data[0]
        //if(PlayerPrefs.GetInt("OpenLevel") )
        //{

        //}
        Data += PlayerPrefs.GetInt("OpenLevel").ToString();
        Data += "|";


        return Data;
    }

    private void saveupdate(SavedGameRequestStatus status, ISavedGameMetadata meta)
    {
        //use this debug whether the game data is uploaded to cloud
        //debugtext.text = "successfully added data to the cloud";

        if (status == SavedGameRequestStatus.Success)
        {
            Debug.Log("The game is Saved");
        }
        else
        {
            Debug.Log("Failed to save");
        }
    }
    public void Agree()
    {
        OpenSave(false);
        LoadClicked = true;
        StartCoroutine(Wait());
        

    }
    public void PlusButton()
    {
        WarningPanel.SetActive(true);
    }
    public void CloseButton()
    {
        WarningPanel.SetActive(false);
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(5f);
        WarningPanel.SetActive(false);
        LoadClicked = false;
        SceneManager.LoadScene(0); 
    }
}
