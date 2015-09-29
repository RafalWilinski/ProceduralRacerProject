using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BackendManager : MonoBehaviour {

	public string BASE_URL = "http://projectrbackend-env.elasticbeanstalk.com";

	public enum RequestType {
		GetLeaderboard,
		PostScore
	}

	public void GetLeaderboards() {

	}

	public void PostScore() {

	}
}
