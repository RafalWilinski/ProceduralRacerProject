using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BackendRequestObject {

	public string url;

	private Dictionary<string,object> internalFlags;

	public WWW request;

	public BackendRequestObject(string url) {

	}
}
