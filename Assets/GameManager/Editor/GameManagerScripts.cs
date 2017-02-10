using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public class GameManagerScripts : EditorWindow {
	private const string kTemplatePath = "GameManager/Templates";

	private static string GetAbsoluteCustomTemplatePath() {
		return Path.Combine(Application.dataPath, kTemplatePath);
	}

	private static string GetTemplate(string nameWithoutExtension) {
		string path = Path.Combine(GetAbsoluteCustomTemplatePath(), nameWithoutExtension + ".cs.txt");
		if (File.Exists(path))
			return File.ReadAllText(path);
		return null;
	}

	private static string TargetDir() {
		return Path.Combine("Assets/Game/Controllers/", Selection.activeObject.name);
	}

	private static void CreateScript(string filename, string data) {
		if (!Directory.Exists(TargetDir())) {
			Directory.CreateDirectory(TargetDir());
		}
		var writer = new StreamWriter(Path.Combine(TargetDir(), filename + ".cs"));
		writer.Write(data);
		writer.Close();
		writer.Dispose();
		AssetDatabase.Refresh();
	}

	[MenuItem("Component/GameManager/Create State", false, 0)]
	private static void CreateState() {
		ScriptPrescription prescription = new ScriptPrescription();
		prescription.m_ClassName = Selection.activeObject.name + "State";
		prescription.m_Template = GetTemplate("StateTemplate");
		string script = new NewScriptGenerator(prescription).ToString();
		CreateScript(prescription.m_ClassName, script);
	}

	[MenuItem("Component/GameManager/Create Controller", false, 0)]
	private static void CreateController() {
		ScriptPrescription prescription = new ScriptPrescription();
		prescription.m_ClassName = Selection.activeObject.name + "Controller";
		prescription.m_Template = GetTemplate("ControllerTemplate");
		string script = new NewScriptGenerator(prescription).ToString();
		CreateScript(prescription.m_ClassName, script);
	}
}
