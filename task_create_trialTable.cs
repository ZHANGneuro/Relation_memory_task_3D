using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class task_create_trialTable : MonoBehaviour
{
    public void create_trial_table()
    {

        TextAsset text_asset = (TextAsset)Resources.Load("Dirs_sub_" + task_grid_learning.subject_ID.ToString() + "_sess" +
                           task_grid_learning.subject_session.ToString());
        List<string> DataSet = TextAssetToList(text_asset);

        int number_of_trials = 120;
        task_grid_learning.reorganized_DataSet = new String[number_of_trials][];

        for (int i = 0; i < number_of_trials; i++)
        {
            string[] entries = DataSet[i].Split('\t');

            for (int j = 0; j < entries.Length; j++)
            {
                task_grid_learning.reorganized_DataSet[i] = entries;
            }
        }

        TextAsset text_asset_record = (TextAsset)Resources.Load("sub" + task_grid_learning.subject_ID.ToString() + "_sess" +
                   task_grid_learning.subject_session.ToString() + "_millisecondRecord");
        List<string> DataSet_record = TextAssetToList(text_asset_record);


        int number_of_record = DataSet_record.Count - 1;
        task_grid_learning.reorganized_records = new String[number_of_record][];

        for (int i = 0; i < number_of_record; i++)
        {
            string[] entries = DataSet_record[i].Split('\t');
            for (int j = 0; j < entries.Length; j++)
            {
                task_grid_learning.reorganized_records[i] = entries;
            }
        }

    }

    private List<string> TextAssetToList(TextAsset ta)
    {
        return new List<string>(ta.text.Split('\n'));
    }


}
