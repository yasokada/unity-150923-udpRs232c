using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InputFieldControl : MonoBehaviour {

	public InputField IF_ipadr1;
	public InputField IF_ipadr2;

	void Start () {
	
	}
	
	void Update () {
	
	}
	
	public void OnEndEditIpadr1() {
		SettingKeeperControl.setIpadr1 (IF_ipadr1.text);
	}
	public void OnEndEditIpadr2() {
		SettingKeeperControl.setIpadr2 (IF_ipadr2.text);
	}
}
