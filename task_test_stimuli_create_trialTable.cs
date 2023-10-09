using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class task_test_stimuli_create_trialTable : MonoBehaviour
{

    public void create_trial_table()
    {

        TextAsset text_asset = (TextAsset)Resources.Load("Dirs_test_stimuli_position");
        List<string> DataSet = TextAssetToList(text_asset);

        int number_of_trials = 480;
        task_test_stimuli_position.reorganized_DataSet = new String[number_of_trials][];

        for (int i = 0; i < number_of_trials; i++)
        {
            string[] entries = DataSet[i].Split('\t');

            for (int j = 0; j < entries.Length; j++)
            {
                task_test_stimuli_position.reorganized_DataSet[i] = entries;
            }
        }
    }

    private List<string> TextAssetToList(TextAsset ta)
    {
        return new List<string>(ta.text.Split('\n'));
    }


}
