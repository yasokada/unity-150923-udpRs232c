using UnityEngine;
using System.Collections;
using UnityEngine.UI;

using System.Threading;

public class udpMonitorScript : MonoBehaviour {
	Thread monThr; // monitor Thread

	public Toggle ToggleComm;

	void Start () {
		monThr = new Thread (new ThreadStart (FuncMonData));
		monThr.Start ();
	}

	void Update () {	
	}

	void Monitor() {
		Debug.Log ("start monitor");
		while (ToggleComm.isOn) {
			Thread.Sleep(100);
		}
		Debug.Log ("end monitor");
	}

	private void FuncMonData() 
	{
		Debug.Log ("Start FuncMonData");
		while (true) {
			while(ToggleComm.isOn == false) {
				Thread.Sleep(100);
				continue;
			}
			Monitor();
		}
	}

}
