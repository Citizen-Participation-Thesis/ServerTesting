using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ItemSpawner : MonoBehaviour
{
    private String loadedUrl = "default";
    private int activeBundle;
    private int offset = 0;
    public InitialResponse savedData;
    public UnityEngine.Object instance;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(GetRequest("http://192.168.1.2:8000/rest/GetProject/?format=json"));
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (loadedUrl.Equals(savedData.bundle))
            {
                SpawnInstance();
            }
            else
            {
                StartCoroutine(GetBundle(savedData.bundle));   
            }
        }
    }

    public void FetchData()
    {
        StartCoroutine(GetRequest("http://192.168.1.2:8000/rest/GetProject/?format=json"));
    }

    public void PlaceAssets()
    {
        if (loadedUrl.Equals(savedData.bundle))
        {
            SpawnInstance();
        }
        else
        {
            StartCoroutine(GetBundle(savedData.bundle));   
        }
    }


    void SpawnInstance()
    {
        AssetBundle[] bundles = Resources.FindObjectsOfTypeAll<AssetBundle>();
        foreach (AssetBundle bundle in bundles)
        {
            if (activeBundle == bundle.GetInstanceID())
            {
                if(instance != null){Destroy(instance);}

                instance = null;

                var newObj = bundle.LoadAsset(savedData.model_files[0].hidden_name);
                Vector3 spawnPosition = new Vector3(0, 1, 4);
                Quaternion spawnRotation =  Quaternion.Euler(-90, 180, 0); 
                instance = Instantiate(newObj,  spawnPosition, spawnRotation);
                offset += 1;
                if(bundle != null){ bundle.Unload(false); }
                return;
            }
        }
        Debug.Log("No Match!!!!!");
    }

    IEnumerator GetBundle(String url)
    {
        Debug.Log("Fetching from: "+url);
        using (UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle(url))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                // Get downloaded asset bundle
                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(uwr);
                activeBundle = bundle.GetInstanceID();
                SpawnInstance();
                loadedUrl = url;
            }
        }
    }
    
    void ReadData(String data)
    {
        var wrappedData = JsonUtility.FromJson<WrappedResponse>("{\"projects\":" + data + "}");
        savedData = wrappedData.projects.First();
    }

    IEnumerator GetRequest(string uri)
    {
        
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {  
            //webRequest.SetRequestHeader("cookie", cookie);
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    Debug.LogError("Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError("Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError("HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(":\nReceived: " + webRequest.downloadHandler.text);
                    ReadData(webRequest.downloadHandler.text);
                    break;
            }
        }
    }
}

[Serializable]
public class WrappedResponse
{
    public InitialResponse[] projects;
}

[Serializable]
public class InitialResponse
{
    public String title, bundle, description, status, qr;
    public ModelFile[] model_files;
}

[Serializable]
public class ModelFile
{
    public String hidden_name, presentable_name;
    public bool placeable;
    public Prefix[] prefixes;
    public SwappableGroup[] group;
}

[Serializable]
public class Prefix
{
    public String prefix;
    public SerializableMaterial[] materials;
}

[Serializable]
public class SerializableMaterial
{
    public String hex_color;
}

[Serializable]
public class SwappableGroup
{
    public String name;
}
