using UnityEngine;
using System.Collections;
using UnityEngine.UI;

using System; // for Convert.ToInt16()

public class SettingKeeperControl : MonoBehaviour {

	public InputField IF_ipadr1;
	public InputField IF_ipadr2;
	public InputField IF_port;

	public static string str_ipadr1 = "";
	public static string str_ipadr2 = "";
	public static int port = 0;

	bool justStarted = true;

	void Start () {
		DontDestroyOnLoad (this); // to access public static variables from other scenes
		loadOldSetting ();
	}

	public static void setIpadr1(string ipadr)
	{
		str_ipadr1 = ipadr;
	}

	void loadOldSetting()
	{
		IF_ipadr1.text = str_ipadr1;
		IF_ipadr2.text = str_ipadr2;
		IF_port.text = port.ToString ();
	}

	string getIpadr(string txt) 
	{
		if (txt.Length == 0) {
			return "";
		}
		return txt;
	}
	int getPort(string txt) {
		if (txt.Length == 0) {
			return 0;
		}
		return Convert.ToInt16 (txt);
	}

	void Update () {
//		// TODO:
//		if (IF_ipadr1.text.Length != 0) {
//			str_ipadr1 = getIpadr (IF_ipadr1.text);
//		}
//		if (IF_ipadr2.text.Length != 0) {
//			str_ipadr2 = getIpadr (IF_ipadr2.text);
//		}
//		if (IF_port.text.Length != 0) {
//			port = getPort (IF_port.text);
//		}
	}
}
