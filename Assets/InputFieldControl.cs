using UnityEngine;
using System.Collections;
using UnityEngine.UI;

using System; // for Convert.ToInt16()

public class InputFieldControl : MonoBehaviour {

	public InputField IF_ipadr1;
	public InputField IF_ipadr2;
	public InputField IF_port;
	public InputField IF_delay;

	public void OnEndEditIpadr1() {
		SettingKeeperControl.setIpadr1 (IF_ipadr1.text);
	}
	public void OnEndEditIpadr2() {
		SettingKeeperControl.setIpadr2 (IF_ipadr2.text);
	}
	public void OnEndEditPort() {
		if (IF_port.text.Length == 0) {
			return;
		}
		int port = Convert.ToInt16 (IF_port.text);
		SettingKeeperControl.setPort (port);
	}
	public void OnEndEditDelay() {
		if (IF_delay.text.Length == 0) {
			return;
		}
		int delay = Convert.ToInt16 (IF_delay.text);
		SettingKeeperControl.setDelay (delay);
	}
}
