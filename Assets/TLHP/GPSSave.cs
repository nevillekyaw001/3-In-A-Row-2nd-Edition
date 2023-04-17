using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using System;
using System.Data;

public class GPSSave : MonoBehaviour
{
    bool connectedGPS;
    public GPSTesting GPSTEST;
    

    [SerializeField] InputField datatocloud;
    [SerializeField] Text debugtext;

    private void Awake()
    {
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
            debugtext.text = ("GPS Login = " + connectedGPS);
        }
        else
            connectedGPS = false;
    }

    //cloud save
    bool issaving = false;
    string SAVE_NAME = "savegames";
    public void OpenSaveToCloud(bool saving)
    {
        if (Social.localUser.authenticated)
        {
            issaving = saving;
            ((PlayGamesPlatform)Social.Active).SavedGame.OpenWithAutomaticConflictResolution(SAVE_NAME, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, SaveGameOpen);
        }
    }

    private void SaveGameOpen(SavedGameRequestStatus status, ISavedGameMetadata meta)
    {
        if(status == SavedGameRequestStatus.Success)
        {
            if (issaving) //If issaving bool is true, we are saving data to cloud
            {
                byte[] data = System.Text.ASCIIEncoding.ASCII.GetBytes(GetDataToStoreInCloud());

                SavedGameMetadataUpdate update = new SavedGameMetadataUpdate.Builder().Build();

                ((PlayGamesPlatform)Social.Active).SavedGame.CommitUpdate(meta, update, data, SaveUpdate);
            }
            else //If issaving bool is false, we are opening our saved data from cloud
            {
                ((PlayGamesPlatform)Social.Active).SavedGame.ReadBinaryData(meta, ReadDatafromCloud);
            }
        }
        
    }

    private void ReadDatafromCloud(SavedGameRequestStatus status, byte[] data)
    {
        if(status == SavedGameRequestStatus.Success)
        {
            string savedata = System.Text.ASCIIEncoding.ASCII.GetString(data);

            LoadDataFromCloudToOurGame(savedata);
        }
    }

    public void LoadDataFromCloudToOurGame(string savedata)
    {
        string[] data = savedata.Split("|");
        debugtext.text = data[0];

        //GPSTEST.level = int.Parse(cloudStringArr);
    }

    public string GetDataToStoreInCloud() //we are setting the value that we are going to store the data in cloud
    {
        string Data = "";
        //dataToSave += PlayerPrefs.GetInt("openLevel").ToString();

        //data[0]
        Data += datatocloud.text;
        Data += "|";


        return Data;
    }

    private void SaveUpdate(SavedGameRequestStatus status, ISavedGameMetadata meta) 
    {
        //use this debug whether the game data is uploaded to cloud
        debugtext.text = "successfully added data to the cloud";

        if(status == SavedGameRequestStatus.Success)
        {
            Debug.Log("The game is Saved");
        }
        else
        {
            Debug.Log("Failed to save");
        }
    }
}
