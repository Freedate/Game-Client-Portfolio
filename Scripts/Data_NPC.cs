using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class Data_NPC : MonoBehaviour
{
    public static Data_NPC instance = null;

    public TextAsset csvFile;

    public Dictionary<string, NPCData> data;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            data = new Dictionary<string, NPCData>();
            ReadNPCDataFromFile();

        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ReadNPCDataFromFile()
    {
        string csvData = csvFile.text;
        string[] lines = csvData.Split('\n');
        string currId = "";
        foreach(string line in lines)
        {
            string[] values = line.Split(',');
            
            if (currId != values[0])
            {
                currId = values[0];
                NPCData nData = new NPCData();
                nData.scriptData = new Dictionary<int, List<string>>();
                nData.name = values[1];

                data.Add(values[0], nData);
                
                continue;
            }

            //List<string> scriptsData = new List<string>();
            List<string> scriptsData = values[2].Split(';').ToList();
            
            data.Last().Value.scriptData.Add(int.Parse(values[1]), scriptsData);

        }
        
    }

    public struct NPCData
    {
        public string name;
        public Dictionary<int, List<string>> scriptData;
    }
}
