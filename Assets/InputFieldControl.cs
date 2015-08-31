using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InputFieldControl : MonoBehaviour {

	public InputField IF_ipadr1;

	void Start () {
	
	}
	
	void Update () {
	
	}

	public void OnChangeIpadr1() {
		SettingKeeperControl.setIpadr1 (IF_ipadr1.text);
	}
}
