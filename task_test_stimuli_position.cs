
using System;
using System.IO;
using System.Globalization;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.UIElements;
using UnityStandardAssets.Characters.FirstPerson;
using System.Diagnostics;
using System.Threading;
using TMPro;
using System.Runtime.InteropServices;

public class task_test_stimuli_position : MonoBehaviour
{

	public static string subject_ID = "1";
	public static string subject_session = "1";

	public Camera MainCamera;
	public Camera temp_camera;
	public GameObject character_object_c;
	public GameObject character_object_i;

	static float reference_coordinate_x = 100.0f;
	static float reference_coordinate_z = 100.0f;
	static float reference_coordinate_y = 21.8f;

	// mouselook
	public float mouseSensitivity = 50.0f; //100.0f
	public float clampAngle = 85.0f; // 80.0f
	
	public static Vector3 subject_position = new Vector3(reference_coordinate_x, reference_coordinate_y, reference_coordinate_z);
	public static Vector3 hidden_position = new Vector3(0, 0, 0);

	public static string[][] reorganized_DataSet;
	
	List<int> trial_order = new List<int>();

	Camera m_MainCamera;
	Camera temp_camera_copy;

	Canvas canvas;
	Canvas canvas2;

	Vector3 character_worldPos;
	Vector3 screenPos_target;
	Vector3 screenPos_lookingat;
	Vector3 character_originPos;

	List<GameObject> list_object_presented;
	List<string> list_object_name;
	List<int> list_angles;

	int trial_ith = 0;

	float distance_camera_and_character = 10.0f;

	void Start()
	{
		Screen.SetResolution(1024, 768, true, 60);
		m_MainCamera = Instantiate(MainCamera, subject_position, Quaternion.Euler(0, 0, 0));
		temp_camera_copy = Instantiate(temp_camera, hidden_position, Quaternion.Euler(0, 0, 0));
		m_MainCamera.enabled = true;
		Application.targetFrameRate = 60;
		Invoke("go_next", 0f);
	}

	void go_next()
	{
		task_test_stimuli_create_trialTable create_trial_table = temp_camera_copy.GetComponent<task_test_stimuli_create_trialTable>();
		create_trial_table.create_trial_table();



		Invoke("draw_character", 0.0f);
	}

	void draw_character()
	{
		list_object_name = new List<string>();
		list_angles = new List<int>();
		list_object_presented = new List<GameObject>();

		List<float> list_3Dsphere_x = new List<float>();
        List<float> list_3Dsphere_y = new List<float>();
        List<float> list_3Dsphere_z = new List<float>();

		(character_originPos, list_3Dsphere_x, list_3Dsphere_y, list_3Dsphere_z) = new task_test_stimuli_position_utility().get_loc_1st_char1(distance_camera_and_character, reference_coordinate_x, reference_coordinate_y, reference_coordinate_z);

		string[] cur_trial_info = reorganized_DataSet[trial_ith];
	
		list_object_name.Add("1");
		list_object_name.Add("1");
		list_object_name.Add("1");
		list_object_name.Add("1");

		list_angles.Add(Int32.Parse(cur_trial_info[4]));
		list_angles.Add(Int32.Parse(cur_trial_info[5]));
		list_angles.Add(Int32.Parse(cur_trial_info[6]));

		// place character
		m_MainCamera.enabled = false;
		m_MainCamera.transform.position = hidden_position;
		temp_camera_copy.transform.position = subject_position;

		for (int ith_character = 0; ith_character < list_object_name.Count; ith_character++)
		{
			if (ith_character == 0)
			{
				GameObject cur_object = showObject(character_originPos);
				list_object_presented.Add(cur_object);
			}
			else
			{
				calculate_new_char_position(list_angles[ith_character - 1], list_object_presented[ith_character - 1].transform.position);
				GameObject cur_object = showObject(character_worldPos);
				list_object_presented.Add(cur_object);
			}
		}


		// for (int ith_character = 0; ith_character < list_3Dsphere_x.Count; ith_character++)
		// {
		// 	Vector3 character_originPos = new Vector3(list_3Dsphere_x[ith_character], list_3Dsphere_y[ith_character], list_3Dsphere_z[ith_character]);
		// 	GameObject cur_object = showObject(character_originPos);

		// }
		



		Invoke("use_main", 0.0f);
	}

	void use_main(){
		temp_camera_copy.enabled = false;
		temp_camera_copy.transform.position = hidden_position;

		m_MainCamera.transform.position = subject_position;
		m_MainCamera.transform.forward = (character_originPos - subject_position).normalized;
		m_MainCamera.enabled = true;

		if (trial_ith<120){
			trial_ith = trial_ith + 1;
			draw_character();
		}
	}


	void Update()
	{

	}
	void calculate_new_char_position(int cur_angle, Vector3 object_Pos)
	{
		temp_camera_copy.transform.forward = (object_Pos - subject_position).normalized;
		screenPos_lookingat = temp_camera_copy.WorldToScreenPoint(object_Pos);

		float distance_among_characters = 300.0f;
		double iter_x = Math.Cos((Math.PI / 180) * cur_angle) * distance_among_characters;
		double iter_y = Math.Sin((Math.PI / 180) * cur_angle) * distance_among_characters;

		screenPos_target = new Vector3(screenPos_lookingat[0] + (float)iter_x, screenPos_lookingat[1] + (float)iter_y, 0f);

		Ray ray = temp_camera_copy.ScreenPointToRay(screenPos_target);
		character_worldPos = ray.GetPoint(distance_camera_and_character);

		List<float> temp_list = new List<float>();
		for (int ith_placed_object = 0; ith_placed_object < list_object_presented.Count; ith_placed_object++)
		{
			float mag_dif = Vector3.Distance(list_object_presented[ith_placed_object].transform.position, character_worldPos);
			temp_list.Add(mag_dif);
		}
			
	}

	GameObject showObject(Vector3 stimuli_position)
	{
		GameObject cur_object = Instantiate(character_object_c, hidden_position, Quaternion.Euler(0, 0, 0));
		cur_object.name = "1";

		cur_object.transform.position = stimuli_position;
		cur_object.transform.localScale = new Vector3(0.015f, 0.015f, 0.015f);
		cur_object.transform.forward = (stimuli_position - subject_position).normalized;
		UnityEngine.UI.Image IMAGE_component;

		cur_object.AddComponent<Canvas>();
		IMAGE_component = cur_object.AddComponent<UnityEngine.UI.Image>();

		// pick character
		string cur_path = "sub_" + subject_ID + "_sess_" + subject_session + "_order_1_stimulus_1";
		Texture2D char_image = (Texture2D)Resources.Load(cur_path);
		Rect rect = new Rect(0f, 0f, 128f, 128f);
		Sprite icon = Sprite.Create(char_image, rect, new Vector2(0.8f, 0.8f));
		IMAGE_component.sprite = icon;
		var sprite_width = IMAGE_component.sprite.rect.width;

		// add collider
		BoxCollider cur_boxCollider = cur_object.GetComponent<BoxCollider>();
		cur_boxCollider.center = hidden_position;
		cur_boxCollider.size = new Vector3(sprite_width, sprite_width, sprite_width);
		return cur_object;
	}



}
