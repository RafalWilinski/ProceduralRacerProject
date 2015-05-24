using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class EventSystemDebugger : MonoBehaviour {
	public Text label;

	void Update () {
		if(!EventSystem.current.IsPointerOverGameObject()) {
			label.text = "Current Mouseover: null";
		}
		else {
			PerformRaycast();
			/*
			if(EventSystem.current.currentSelectedGameObject != null)
				label.text = "Current Mouseover: "+EventSystem.current.currentSelectedGameObject.name;
			else 
				label.text = "Blocked!";
			*/
		}
	}

	void PerformRaycast () {
         PointerEventData cursor = new PointerEventData(EventSystem.current);                            // This section prepares a list for all objects hit with the raycast
         cursor.position = Input.mousePosition;
         List<RaycastResult> objectsHit = new List<RaycastResult> ();
         EventSystem.current.RaycastAll(cursor, objectsHit);
         int count = objectsHit.Count;
         int x = 0;
 	
 		if(objectsHit.Count > 0 && objectsHit[x].gameObject != null)
 			label.text = "Current Mouseover: "+objectsHit[x].gameObject;
    }
}
