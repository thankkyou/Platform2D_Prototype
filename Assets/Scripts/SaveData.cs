using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[System.Serializable]
public struct SaveData
{
    public static SaveData Instance;

    //map 
    public HashSet<string> sceneNames;

    //checkpoint
    public string checkpointSceneName;
    public Vector2 checkpointPos;

    public void Initialize()
    {
        if (!File.Exists(Application.persistentDataPath + "/save.checkpoint.data"))
        {
            BinaryWriter writer = new BinaryWriter(File.Create(Application.persistentDataPath + "/save.checkpoint.data"));
        }
        if(sceneNames == null)
        {
            sceneNames = new HashSet<string>();
        }
    }

    public void SaveCheckpoint()
    {
        using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + "/save.checkpoint.data")))
        {
            writer.Write(checkpointSceneName);
            writer.Write(checkpointPos.x);
            writer.Write(checkpointPos.y);
        }
    }

    public void LoadCheckpoint()
    {
        if(File.Exists(Application.persistentDataPath + "/save.checkpoint.data"))
        {
            using(BinaryReader reader = new BinaryReader(File.OpenRead(Application.persistentDataPath + "/save.checkpoint.data")))
            {
                checkpointSceneName = reader.ReadString();
                checkpointPos.x = reader.ReadSingle();
                checkpointPos.y = reader.ReadSingle();
            }
        }
    }
}
