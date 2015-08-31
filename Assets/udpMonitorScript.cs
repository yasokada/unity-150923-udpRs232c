using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class udpMonitorScript : MonoBehaviour {

	public Toggle ToggleComm;
	private bool preToggle = false;

	void Start () {
	
	}
	
	void Update () {	
		if (preToggle != ToggleComm.isOn && ToggleComm.isOn) {
			preToggle = ToggleComm.isOn;

		}
	}
}
