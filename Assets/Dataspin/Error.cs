using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;

public class Error {

	private DataspinRequestMethod method;
	private int code;
	private string message;

	private const string JsonKey_ErrorCode = "error_code";
	private const string JsonKey_Message = "message";

	public int Code {
		get { 
			return code; 
		}
	}

	public string Message {
		get { 
			return message; 
		}
	}

	public DataspinRequestMethod RequestMethod {
		get { 
			return method; 
		}
	}

	public Error (string json, DataspinRequestMethod _method) {
		method = _method;
		try {
			InitializeErrorFromJson(json);
		}
		catch {
			message = json;
		}
	}

	public Error (string msg) {
		message = msg;
	}

	public Error (int e, string msg) {
		code = e;
		message = msg;
	}

	private void InitializeErrorFromJson(string json) {
		var dict = Json.Deserialize(json) as Dictionary<string,object>;

		code = (dict.ContainsKey (JsonKey_ErrorCode)) ? (int)(long)dict[JsonKey_ErrorCode] : -1;
		message = (dict.ContainsKey (JsonKey_Message)) ? (string)dict[JsonKey_Message] : null;

	}

	public override string ToString() {
		return string.Format("[DataspinError: ErrorCode={0}, Message={1}, Method={2}]", code, message, method.ToString());
	}
}
