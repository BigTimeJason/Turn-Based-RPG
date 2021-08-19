using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public static class CSVReader
{
    public static string[] GetStatsFromLevel(CharacterStatSheet characterStatSheet, int level)
    {
        string[] strArray = new string[] {};
        StreamReader streamReader = new StreamReader("Assets/Data/Characters/Stats_" + characterStatSheet +".csv");
        string line = null;

        line = streamReader.ReadLine();
        for (int i = 0; i <= level; i++)
        {
            line = streamReader.ReadLine();
        }
        strArray = line.Split(',');
        return strArray;
    }
}
