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
	
	void Start () {
		DontDestroyOnLoad (IF_ipadr1);
		DontDestroyOnLoad (IF_ipadr2);
		DontDestroyOnLoad (IF_port);
		loadOldSetting ();
	}

	public static void setIpadr1(string ipadr) // called from EndEdit
	{
		str_ipadr1 = ipadr;
	}
	public static void setIpadr2(string ipadr) // called from EndEdit
	{
		str_ipadr2 = ipadr;
	}

	void loadOldSetting()
	{
		IF_ipadr1.text = str_ipadr1;
		IF_ipadr2.text = str_ipadr2;
		IF_port.text = port.ToString ();
	}
	
	void Update () {

	}
}
