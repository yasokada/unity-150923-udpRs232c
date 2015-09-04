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
 *   - 
 * v0.1 2015/09/04
 *   - fix UDP relay
 */ 

public class udpMonitorScript : MonoBehaviour {
 	Thread monThr = null; // monitor Thread
	static bool created = false;

	public Toggle ToggleComm;

	private string ipadr1;
	private string ipadr2;
	private int port;

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
	}

	void Monitor() {
		UdpClient client = new UdpClient (port);
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
				string fromPort = anyIP.Port.ToString();

				// send to the other
				if (fromIP.Equals(ipadr1)) {
					portToReturn = Convert.ToInt32 (fromPort); // store the port 
					client.Send(data, data.Length, ipadr2, port);
					DebugPrintComm("1 ", fromIP, portToReturn, ipadr2, port);
				} else {
					int toPort = Convert.ToInt32(portToReturn);
					client.Send(data, data.Length, ipadr1, toPort);

					Debug.Log("2 from: " + fromIP + "(" + "..." 
					          + ") to " + ipadr1 + "(" + portToReturn + ")");
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
			Debug.Log("read setting");
			readSetting();
			Debug.Log("monitor");
			Monitor();
		}
	}

}
