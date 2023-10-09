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

public class task_grid_learning : MonoBehaviour
{

	public static string subject_ID = "9";
	public static string subject_session = "5";

	public Camera MainCamera;
	public GameObject A_point;
	public Texture2D mouseCursor;

	static float reference_coordinate_x = 100.0f;
	static float reference_coordinate_z = 100.0f;
	static float reference_coordinate_y = 21.8f;

	// mouselook
	public float mouseSensitivity = 150.0f; //100.0f

	private float rotY; // rotation around the up/y axis
	private float rotX; // rotation around the right/x axis

	public static Vector3 subject_position = new Vector3(reference_coordinate_x, reference_coordinate_y, reference_coordinate_z);

	public static string[][] reorganized_DataSet;
	public static string[][] reorganized_records;

	Camera m_MainCamera;

	GameObject canvasobject;
	GameObject canvas_fixation;
	GameObject textObject;

	bool exp_information_input = false;
	bool trial_running = false;
	bool shown_warning = false;

	Vector3 screenPos_2d;
	Vector3 screenPos_3d;

	Color32 backgroundColor = new Color32(192, 192, 192, 0);
	Color fontColor = Color.black;

	CursorMode cursorMode = CursorMode.ForceSoftware;// CursorMode.Auto;

	UnityEngine.UI.Image InstructionScreen;

	int num_of_trial;
	int counter = 0;
	string object_id_clicked;
	float distance_camera_and_character = 10.0f;

	void Start()
	{
		Screen.SetResolution(1024, 768, true, 60);
		m_MainCamera = Instantiate(MainCamera, subject_position, Quaternion.Euler(0, 0, 0)); //
		m_MainCamera.enabled = true;

		Application.targetFrameRate = 10000;
		UnityEngine.Cursor.SetCursor(mouseCursor, new Vector2(mouseCursor.width / 2, mouseCursor.width / 2), cursorMode);
		UnityEngine.Cursor.lockState = CursorLockMode.None;

		task_create_trialTable create_trial_table = MainCamera.GetComponent<task_create_trialTable>();
		create_trial_table.create_trial_table();
		num_of_trial = reorganized_DataSet.Length;

		exp_information_input = true;
	}
	void OnGUI()
	{
		if (exp_information_input)
		{
			GUI.Window(0, new Rect(Screen.width / 2 - 500 / 2, Screen.height / 5, 500, 500), ShowGUI, " ");
		}
	}
	void ShowGUI(int windowID)
	{
		GUI.skin.textField.fontSize = 40;
		var Gui_subNum = GUI.skin.GetStyle("Label");
		Gui_subNum.alignment = TextAnchor.MiddleCenter;
		Gui_subNum.fontSize = 40;
		GUI.Label(new Rect(25, 100, 120, 50), "被试号", Gui_subNum);
		subject_ID = GUI.TextField(new Rect(150, 100, 200, 50), subject_ID, 5);
		var Gui_session = GUI.skin.GetStyle("Label");
		Gui_session.alignment = TextAnchor.MiddleCenter;
		Gui_session.fontSize = 40;
		GUI.Label(new Rect(25, 250, 120, 50), "Sess", Gui_session);
		subject_session = GUI.TextField(new Rect(150, 250, 280, 50), subject_session, 12);
		GUIStyle myButtonStyle = new GUIStyle(GUI.skin.button);
		myButtonStyle.fontSize = 40;
		if (shown_warning)
		{
			var give_warning = GUI.skin.GetStyle("Label");
			give_warning.alignment = TextAnchor.MiddleCenter;
			give_warning.fontSize = 30;
			GUI.Label(new Rect((500 - 350) / 2, 320, 350, 50), "请输入session，数字1-5", give_warning);
		}
		if (GUI.Button(new Rect((500 - 150) / 2, 400, 150, 50), "OK", myButtonStyle))
		{
			if (subject_session == "")
			{
				shown_warning = true;
			}
			else
			{
				exp_information_input = false;
				// show_fixation();
				getMessageScreen();
				UnityEngine.Cursor.visible = true;
				UnityEngine.Cursor.lockState = CursorLockMode.Locked;
				trial_running = true;
				// Invoke("show_fixation", 0f);
			}
		}
	}




	void Update()
	{

		if (trial_running & (counter < reorganized_records.Length - 1))
		{
			counter = counter + 1;


			// float mouseX = Input.GetAxis("Mouse X");
			// float mouseY = -Input.GetAxis("Mouse Y");
			// rotX += mouseY * mouseSensitivity * Time.deltaTime;
			// rotY += mouseX * mouseSensitivity * Time.deltaTime;

			// UnityEngine.Debug.Log((rotX).ToString() + "-" + (rotY).ToString() + "-" + (rotX % 360).ToString() + "-" + (rotY % 360).ToString());

			rotX = float.Parse(reorganized_records[counter][2]);
			rotY = float.Parse(reorganized_records[counter][3]);
			m_MainCamera.transform.localRotation = Quaternion.Euler(rotX, rotY, 0.0f);

			screenPos_2d = Input.mousePosition;
			Ray ray = m_MainCamera.ScreenPointToRay(screenPos_2d);
			screenPos_3d = ray.GetPoint(distance_camera_and_character);

			if (counter % 10 == 0)
			{
				GameObject cur_point = Instantiate(A_point, screenPos_3d, Quaternion.Euler(0, 0, 0));
				cur_point.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

				// Color customColor = new Color(0.4f, 0.9f, 0.7f, 1.0f);

				cur_point.GetComponent<Renderer>().material.color = new Color(0, 0, 0);
			}



			showInstruction("trial: " + reorganized_records[counter][0], 50, -50, 50);
		}
	}



	void showInstruction(string inputText, int inputXposition, int inputYposition, int inputFontSize)
	{
		var text = textObject.GetComponent<Text>();
		text.horizontalOverflow = HorizontalWrapMode.Overflow;
		text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
		text.color = fontColor;
		text.text = inputText;
		RectTransform text_size = text.GetComponent<RectTransform>();
		text_size.sizeDelta = new Vector2(1024, 768);
		text_size.anchoredPosition = new Vector3(inputXposition, inputYposition, 0);
		text.fontSize = inputFontSize;
	}

	void getMessageScreen()
	{
		canvasobject = new GameObject("Canvas");
		Canvas cur_canvas = canvasobject.AddComponent<Canvas>();
		cur_canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		InstructionScreen = canvasobject.AddComponent<UnityEngine.UI.Image>();
		InstructionScreen.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
		InstructionScreen.rectTransform.anchoredPosition = Vector3.zero;
		InstructionScreen.color = backgroundColor;
		textObject = new GameObject("Text");
		textObject.transform.SetParent(InstructionScreen.transform);
		textObject.AddComponent<Text>();
	}

}
