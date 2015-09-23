﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using System.Collections.Generic; // for Dictionary

using System.IO.Ports; // for RS-232C
using NS_MyRs232cUtil;

// TODO: bug > thread is still running after finishing player mode

/* 
 * v0.6 2015/09/23
 *   - add RS-232C connection (9600 8N1)
 * * ----------- UdpMonitor ==> udpRs232c ------------
 * v0.5 2015/09/13
 *   - add data export command (export,88555bd)
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


public class udpRs232cScript : MonoBehaviour {
	// data export command
	//     88555bd: hash of v0.1 commit @ github
	const string kExportCommand = "export,88555bd";

	Thread monThr = null; // monitor Thread
	static bool created = false;

	public Toggle ToggleComm;
	public Text T_commStatus;
	private string s_commStatus = "";

	private string ipadr1;
	private string ipadr2;
	private int setPort;
	private int delay_msec;

	private SerialPort mySP;
	private string acc232rcvd = "";

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
	
	private void exportData(ref UdpClient client, ref IPEndPoint anyIP) 
	{
		byte[] data;
		string text;

		int idx = 0;

		text = "SOT"; // start of table
		text = text + System.Environment.NewLine;
		data = System.Text.Encoding.ASCII.GetBytes(text);
		client.Send(data, data.Length, anyIP);

		foreach (var commtime in list_comm_time) {
			text = commtime.ToString("yyyy/MM/dd HH:mm:ss");
			text = text + ",";
			text = text + list_comm_string[idx];

			// below comment out because text already includes <CR><LF>
	//		text = text + System.Environment.NewLine;

			data = System.Text.Encoding.ASCII.GetBytes (text);
			client.Send (data, data.Length, anyIP);

			idx++;
		}

		text = "EOT"; // start of table
		text = text + System.Environment.NewLine;
		data = System.Text.Encoding.ASCII.GetBytes(text);
		client.Send(data, data.Length, anyIP);

	}

	enum returnType {
		Continue,
		FallThrough,
	};

	private returnType handleUdp(ref UdpClient client, out string udpString, out int retPort) {
		try {
			IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
			byte[] data = client.Receive(ref anyIP);
			string text = Encoding.ASCII.GetString(data);

			string fromIP = anyIP.Address.ToString();
			int fromPort = anyIP.Port;
			retPort = fromPort;
			
			if (text.Length == 0) {
				udpString = "";
				return returnType.Continue; // continue;
			}

			if (text.Contains(kExportCommand)) {
				exportData(ref client, ref anyIP);
				udpString = "";
				return returnType.Continue; // continue;
			}

			// get incoming text
			if (fromIP.Equals(ipadr1)) {
				list_comm_time.Add(System.DateTime.Now);
				list_comm_string.Add("tx," + text);

				// echo back for test
//				client.Send(data, data.Length, ipadr1, fromPort); // TODO: remove

				DebugPrintComm("1 ", fromIP, fromPort, ipadr2, setPort);

				udpString = text;
				return returnType.FallThrough; // TODO: cotinue???
//			} else {
//				// delay before relay 
//				Thread.Sleep(delay_msec);
//
//				list_comm_time.Add(System.DateTime.Now);
//				list_comm_string.Add("rx," + text);
//
//				client.Send(data, data.Length, ipadr1, portToReturn);
//				DebugPrintComm("2 ", fromIP, fromPort, ipadr1, portToReturn);
			}
		}
		catch (Exception err) {
		}
		udpString = "";
		retPort = 0;
		return returnType.FallThrough;
	}

	private returnType handleRs232c(ref SerialPort mySP, ref UdpClient client, string fromUdp, int fromPort){
		try {
			if (fromUdp.Length > 0) {
				mySP.Write (fromUdp);
			}
		}
		catch (System.Exception) {
		}

		byte rcv;
		char tmp;
		bool hasRcvd = false;
		string text;
		
		try {
			rcv = (byte)mySP.ReadByte();
			if (rcv != 255) {
				hasRcvd = true;
				
				tmp = (char)rcv;
//				if (tmp != 0x0d && tmp != 0x0a) { // not CRLF
//					accRcvd = accRcvd + tmp.ToString();
//				}
//				if (tmp == 0x0d) { // CR
//					mySP.WriteLine(accRcvd);
//					rcvdCRLF = true;
//				}
				text = tmp.ToString();
				byte [] data = System.Text.Encoding.ASCII.GetBytes(text);
				client.Send(data, data.Length, ipadr1, fromPort);
			}
		} catch (System.Exception) {
		}

		return returnType.Continue;
	}


	bool DoRelay() {
		UdpClient client = new UdpClient (setPort);
		client.Client.ReceiveTimeout = 300; // msec
		client.Client.Blocking = false;

		bool open232c = MyRs232cUtil.Open (ipadr2, out mySP);
		mySP.ReadTimeout = 1;

		s_commStatus = ipadr2 + " open";
		// TODO: uncomment after debug // HACKME: for TDD (using GameObject to ON/OFF)
//		if (open232c == false) {
//			s_commStatus = ipadr2 + " open fail";
//			return false;
//		}
		mySP.Write(">");

		int portToReturn = 31415; // is set dummy value at first
		string udpString = "";
		while (ToggleComm.isOn) {
			int tmpPort = 0;
			returnType res1 = handleUdp(ref client, out udpString, out tmpPort);
			if (tmpPort > 0) {
				portToReturn = tmpPort;
			}
			returnType res2 = handleRs232c(ref mySP, ref client, udpString, portToReturn);
			if (res1.Equals(returnType.Continue) 
			    && res2.Equals(returnType.Continue)) {
				Thread.Sleep(20);
				continue;
			}

			if (res1 == returnType.Continue) {
				Thread.Sleep(20);
				continue;
			}

			// without this sleep, on android, the app will freeze at Unity splash screen
			Thread.Sleep(200);
		}
		client.Close ();
		MyRs232cUtil.Close (ref mySP);

		return true;
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
			bool resRelay = DoRelay();
			if (resRelay == false) {
				break; // COM port open fail etc.
			}
		}
	}

	void Update() {
		T_commStatus.text = s_commStatus;
	}

}
