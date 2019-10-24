using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CSVLoad : MonoBehaviour {
    private string filePath = "CSV/CSVTest.csv";
    private string[] textList;
    private TextAsset CSVFile;
    public static  List<string[]> csvData = new List<string[]>();

	// Use this for initialization
	void Start () {
        
        CSVFile = Resources.Load("CSV/CSVTest") as TextAsset;
        StringReader render = new StringReader(CSVFile.text);
        while(render.Peek() > -1)
        {
            string line = render.ReadLine();
            csvData.Add(line.Split(','));
        }
	}
}
