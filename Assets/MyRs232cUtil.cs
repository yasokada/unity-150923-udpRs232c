using UnityEngine;
using System.Collections;

using System.IO.Ports; // for RS-232C

/*
 * v0.1 2015/09/23
 *   - add Open() and Close()
 */

namespace NS_MyRs232cUtil
{
	public static class MyRs232cUtil {
		public static bool Open(string portname, out SerialPort sp) {
			sp = new SerialPort ();

			//      string[] portname = SerialPort.GetPortNames ();

			// portname : e.g. "COM3"
			sp.PortName = portname;
			sp.BaudRate = 9600;
			sp.DataBits = 8;
			sp.Parity = Parity.None;
			sp.StopBits = StopBits.One;
			sp.Handshake = Handshake.None;
			sp.Encoding = System.Text.Encoding.ASCII;

			try {
				sp.Open ();
				sp.DtrEnable = false;
				sp.RtsEnable = false;
			} catch(System.Exception e) {
				Debug.LogWarning(e.Message);
				return false;
			}
			return true;
		}

		public static void Close(ref SerialPort sp) {
			sp.Close ();
			sp.Dispose ();
		}
	}
}