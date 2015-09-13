using UnityEngine;
using System.Collections;
using UnityEngine.UI;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using System.Collections.Generic; // for Dictionary

/* 
 * v0.5 2015/09/13
 *   - 
 * v0.4 2015/09/05
 *   - add delay feature
 * v0.3 2015/09/04
 *   - add UI > Label @ Setting Scene
 *   - rename Monitor() to DoRelay()
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
	private int delay_msec;

	private List<System.DateTime> list_comm_time;
	private List<string> list_comm_string;
	
	void Start () {
		if (!created) {
			created = true;
			DontDestroyOnLoad (this.gameObject);
			DontDestroyOnLoad(ToggleComm.gameObject.transform.parent.gameObject);
			ToggleComm.isOn = false; // false at first

			list_comm_time = new List<System.DateTime>();
			list_comm_string = new List<string>();

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

	void DoRelay() {
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

					list_comm_time.Add(System.DateTime.Now);
					list_comm_string.Add("tx," + text);

					client.Send(data, data.Length, ipadr2, setPort);
					DebugPrintComm("1 ", fromIP, fromPort, ipadr2, setPort);
				} else {
					// delay before relay 
					Thread.Sleep(delay_msec);

					list_comm_time.Add(System.DateTime.Now);
					list_comm_string.Add("rx," + text);

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
		delay_msec = SettingKeeperControl.delay_msec;
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
			DoRelay();
		}
	}

}
