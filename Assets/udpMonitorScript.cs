using UnityEngine;
using System.Collections;
using UnityEngine.UI;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
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
		UdpClient client = new UdpClient (port);
		client.Client.ReceiveTimeout = 300; // msec
		client.Client.Blocking = false;

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
				// send to the other
				if (fromIP.Equals(ipadr1)) {
					client.Send(data, data.Length, ipadr2, port);
				} else {
					client.Send(data, data.Length, ipadr1, port);
				}
			}
			catch (Exception err) {

			}
			// without this sleep, on android, the app will freeze at Unity splash screen
			Thread.Sleep(20);
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
			readSetting();
			Monitor();
		}
	}

}
