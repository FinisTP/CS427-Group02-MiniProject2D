using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SaveProgress(RecordTracker recordTracker)
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/GriefOfTheEvergreen.grief";
        FileStream stream = new FileStream(path, FileMode.Create);

        Record data = new Record(recordTracker);

        binaryFormatter.Serialize(stream, data);
        stream.Close();
    }

    public static Record LoadProgress()
    {
        string path = Application.persistentDataPath + "/GriefOfTheEvergreen.grief";
        Debug.Log(path);
        if (File.Exists(path))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            Record data = binaryFormatter.Deserialize(stream) as Record;

            stream.Close();

            return data;
        } else
        {
            Debug.LogError("Save file not found!");
            return null;
        }
    }

}

