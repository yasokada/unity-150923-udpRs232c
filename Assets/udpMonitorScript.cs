using UnityEngine;
using System.Collections;
using UnityEngine.UI;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

/*
 * v0.2 2015/09/04
 *   - refactor
 * v0.1 2015/09/04
 *   - fix UDP relay
 */ 

public class udpMonitorScript : MonoBehaviour {
 	Thread monThr = null; // monitor Thread
	static bool created = false;

	public Toggle ToggleComm;

	private string ipadr1;
	private string ipadr2;
	private int setPort;

	void Start () {
		if (!created) {
			created = true;
			DontDestroyOnLoad (this.gameObject);
			DontDestroyOnLoad(ToggleComm.gameObject.transform.parent.gameObject);
			ToggleComm.isOn = false; // false at first
			monThr = new Thread (new ThreadStart (FuncMonData));
			monThr.Start ();
		} else {
			Destroy(this.gameObject);
			Destroy(ToggleComm.gameObject.transform.parent.gameObject);
		}
	}

	void DebugPrintComm(string prefix, string fromIP, int fromPort, string toIP, int toPort)
	{
		string msg;
		msg = prefix + "from:" + fromIP + "(" + fromPort.ToString ()
			+ ") to " + toIP + "(" + toPort.ToString() + ")"; 
		Debug.Log (msg);
	}

	void Monitor() {
		UdpClient client = new UdpClient (setPort);
		client.Client.ReceiveTimeout = 300; // msec
		client.Client.Blocking = false;

		int portToReturn = 31415; // is set dummy value at first
		while (ToggleComm.isOn) {
			try {
				IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
				byte[] data = client.Receive(ref anyIP);
				string text = Encoding.ASCII.GetString(data);

				if (text.Length == 0) {
					Thread.Sleep(20);
					continue;
				}
				string fromIP = anyIP.Address.ToString();
				int fromPort = anyIP.Port;

				// send to the other
				if (fromIP.Equals(ipadr1)) {
					portToReturn = fromPort; // store the port used in the "else" clause
					client.Send(data, data.Length, ipadr2, setPort);
					DebugPrintComm("1 ", fromIP, fromPort, ipadr2, setPort);
				} else {
					client.Send(data, data.Length, ipadr1, portToReturn);
					DebugPrintComm("2 ", fromIP, fromPort, ipadr1, portToReturn);
				}
			}
			catch (Exception err) {

			}
			// without this sleep, on android, the app will freeze at Unity splash screen
			Thread.Sleep(200);
		}
		client.Close ();
	}

	private bool readSetting() {
		// TODO: return false if reading fails
		ipadr1 = SettingKeeperControl.str_ipadr1;
		ipadr2 = SettingKeeperControl.str_ipadr2;
		setPort = SettingKeeperControl.port;
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
			Debug.Log("read setting");
			readSetting();
			Debug.Log("monitor");
			Monitor();
		}
	}

}
