using UnityEngine;
using System.Collections;
using UnityEngine.UI;

using System.Threading;

public class udpMonitorScript : MonoBehaviour {
	Thread monThr; // monitor Thread

	public Toggle ToggleComm;

	private string ipadr1;
	private string ipadr2;
	private int port;

	void Start () {
		ToggleComm.isOn = false; // false at first

		monThr = new Thread (new ThreadStart (FuncMonData));
		monThr.Start ();
	}
	
	void Monitor() {
		Debug.Log ("start monitor");
		while (ToggleComm.isOn) {
			Thread.Sleep(100);
		}
		Debug.Log ("end monitor");
	}

	private bool readSetting() {
		// TODO: return false if reading fails
		ipadr1 = SettingKeeperControl.str_ipadr1;
		ipadr2 = SettingKeeperControl.str_ipadr2;
		port = SettingKeeperControl.port;
		return true;
	}

	private void FuncMonData() 
	{
		Debug.Log ("Start FuncMonData");
		while (true) {
			while(ToggleComm.isOn == false) {
				Thread.Sleep(100);
				continue;
			}
			readSetting();
			Monitor();
		}
	}

}
