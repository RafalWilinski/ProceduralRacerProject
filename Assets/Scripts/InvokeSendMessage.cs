using UnityEngine;
using System.Collections;

public class InvokeSendMessage : MonoBehaviour {

	public float interval;
	public string messageName;

	void Start () {
		InvokeRepeating("SendMsg", interval, interval);
	}

	private void SendMsg() {
		this.SendMessage(messageName);
	}
}
