using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data_Quest : MonoBehaviour
{
    public static Data_Quest instance = null;

    public TextAsset csvFile;

    public Dictionary<int, questData> data;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            data = new Dictionary<int, questData>();
            ReadQuestDataFromFile();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void ReadQuestDataFromFile()
    {
        string csvData = csvFile.text;
        string[] lines = csvData.Split('\n');
        string currId = "";
        foreach (string line in lines)
        {
            string[] values = line.Split(',');

            if (currId != values[0])
            {
                currId = values[0];
                questData qData = new questData();

                qData.title = values[1];
                qData.description = values[2];
                qData.type = (questType)int.Parse(values[3]);
                qData.target = new Dictionary<int, int>();
                qData.target.Add(int.Parse(values[4]), int.Parse(values[5]));
                qData.targetNPC = values[6];
                qData.prize = new Dictionary<int, int>();
                qData.prize.Add(int.Parse(values[7]), int.Parse(values[8]));

                //iData.describe = values[3];
                //iData.price = int.Parse(values[2]);
                //iData.name = values[1];
                //iData.sprite = Resources.Load<Sprite>("Images/Item" + String.Format("{0:D3}", int.Parse(values[0])));

                data.Add(int.Parse(values[0]), qData);
            }
        }


    }

    public enum questType
    {
        Collect = 0,
        Hunt = 1,
    }

    public struct questData
    {
        public string title;
        public string description;
        public questType type;
        public Dictionary<int, int> target;
        public string targetNPC;
        public Dictionary<int, int> prize;
    }

    

}
