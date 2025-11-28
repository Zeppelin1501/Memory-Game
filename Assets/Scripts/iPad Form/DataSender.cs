using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class DataSender : MonoBehaviour
{
    public TMP_InputField inputField;

    private void Start()
    {
        if (CheckAndSendOfflineData())
        {
            Debug.Log("Offline data sent successfully.");
        }

        CheckNetworkStatus();
    }

    public void SendData()
    {
        StartCoroutine(DelaySendData());
    }

    private void CheckNetworkStatus()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            Debug.Log("Device is online.");
        }
        else
        {
            Debug.Log("Device is offline.");
        }
    }

    private IEnumerator DelaySendData()
    {
        yield return new WaitForSeconds(0.1f); // Wait for 1 second

        string currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        // Combine the current time with input data
        string inputData = currentTime + "," + inputField.text;

        // Split the combined data into an array
        string[] data = inputData.Split(',');

        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            Debug.Log("Device is online. Sending data to server...");
             StartCoroutine(PostData(data));
           // SaveDataLocally(data);
        }
        else
        {
            Debug.Log("Device is offline. Data cannot be sent.");
            SaveDataLocally(data);
        }
    }

    IEnumerator PostData(string[] data)
    {
        string url = "https://creativetechnologyllc.com/dct_data.php";

        // Create a WWWForm
        WWWForm form = new WWWForm();

        // Add fields to the form
        form.AddField("ID", data[0]);
        form.AddField("Name", data[1]);
        form.AddField("Email", data[2]);
        form.AddField("PhoneNumber", data[3]);

        // Send the form to the server
        UnityWebRequest www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Data sent successfully");
        }
    }

    private void SaveDataLocally(string[] data)
    {
        // Create a data object to be serialized
        DataObject dataObject = new DataObject
        {
            ID = data[0],
            Name = data[1],
            Email = data[2],
            PhoneNumber = data[3]
        };

        // Define the file path for saving
        string filePath = Path.Combine(Application.persistentDataPath, "offline_data.json");

        List<DataObject> dataList = new List<DataObject>();

        // Check if the file exists
        if (File.Exists(filePath))
        {
            // Read existing data from the file
            string jsonData = File.ReadAllText(filePath);
            dataList = JsonUtility.FromJson<DataListWrapper>(jsonData).dataList;
        }

        // Add the new data object to the list
        dataList.Add(dataObject);

        // Create a wrapper object to serialize the list
        DataListWrapper wrapper = new DataListWrapper
        {
            dataList = dataList
        };

        // Serialize the wrapper object to JSON
        string newJsonData = JsonUtility.ToJson(wrapper, true);

        // Save the JSON string to the file
        File.WriteAllText(filePath, newJsonData);

        Debug.Log("Data saved locally to " + filePath);
    }

    private bool CheckAndSendOfflineData()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "offline_data.json");

        if (File.Exists(filePath))
        {
            // Read existing data from the file
            string jsonData = File.ReadAllText(filePath);
            DataListWrapper wrapper = JsonUtility.FromJson<DataListWrapper>(jsonData);

            // Check network reachability
            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                Debug.Log("Device is online. Sending offline data to server...");

                foreach (var dataObject in wrapper.dataList)
                {
                    string[] data = new string[] { dataObject.ID, dataObject.Name, dataObject.Email, dataObject.PhoneNumber };
                    StartCoroutine(PostData(data));
                }

                // Delete the file after sending all data
                File.Delete(filePath);
                Debug.Log("Offline data file deleted.");
                return true;
            }
            else
            {
                Debug.Log("Device is offline. Cannot send offline data.");
                return false;
            }
        }
        else
        {
            Debug.Log("No offline data to send.");
            return false;
        }
    }

    [Serializable]
    private class DataObject
    {
        public string ID;
        public string Name;
        public string Email;
        public string PhoneNumber;
    }

    [Serializable]
    private class DataListWrapper
    {
        public List<DataObject> dataList = new List<DataObject>();
    }
}