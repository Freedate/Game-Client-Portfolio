using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Data_Item : MonoBehaviour
{
    public static Data_Item instance = null;

    public TextAsset csvFile;

    public Dictionary<int, ItemData> data;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            data = new Dictionary<int, ItemData>();
            ReadItemDataFromFile();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void ReadItemDataFromFile()
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
                ItemData iData = new ItemData();
                iData.describe = values[3];
                iData.price = int.Parse(values[2]);
                iData.name = values[1];
                iData.sprite = Resources.Load<Sprite>("Images/Item" + String.Format("{0:D3}", int.Parse(values[0])));

                data.Add(int.Parse(values[0]), iData);
            }
        }
    }

    public struct ItemData
    {
        public string name;
        public int price;
        public string describe;
        public Sprite sprite;
    }

}
