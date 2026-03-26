using System;
using System.IO;
using UnityEngine;

[Serializable]
public class UserData
{
    public int gold;
    public int energy;

    public UserData()
    {
        gold = -1;     // TODO: JW: 초기값 추후 개선
        energy = -1;
    }
}

public class FileService
{
    public void SaveUserData(UserData userData)
    {
        string filePath = GetDataUserFilePath();
        
        string json = JsonUtility.ToJson(userData, true);
        File.WriteAllText(filePath, json);
    }

    public UserData LoadUserData()
    {
        string filePath = GetDataUserFilePath();
        
        if (!File.Exists(filePath))
        {
            return new UserData();
        }

        string json = File.ReadAllText(filePath);
        return JsonUtility.FromJson<UserData>(json);
    }
    
    string GetDataUserFilePath()
    {
        return Application.persistentDataPath + "/UserData.json";   // TODO: JW: 파일 경로를 다른 곳에 저장 요
    }
}
