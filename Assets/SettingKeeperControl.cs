using UnityEngine;
using System.Collections;
using UnityEngine.UI;

using System; // for Convert.ToInt16()

public class SettingKeeperControl : MonoBehaviour {

	public InputField IF_ipadr1;
	public InputField IF_ipadr2;
	public InputField IF_port;
	public InputField IF_delay;

	public static string str_ipadr1 = "";
	public static string str_ipadr2 = "";
	public static int port = 0;
	public static int delay_msec = 0;
	
	void Start () {
		DontDestroyOnLoad (IF_ipadr1);
		DontDestroyOnLoad (IF_ipadr2);
		DontDestroyOnLoad (IF_port);
		DontDestroyOnLoad (IF_delay);
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
	public static void setPort(int port_) {
		port = port_;
	}
	public static void setDelay(int delay_) {
		delay_msec = delay_;
	}

	void loadOldSetting()
	{
		IF_ipadr1.text = str_ipadr1;
		IF_ipadr2.text = str_ipadr2;
		IF_port.text = port.ToString ();
		IF_delay.text = delay_msec.ToString ();
	}	
}
