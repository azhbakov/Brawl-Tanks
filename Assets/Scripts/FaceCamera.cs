using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    void Update()
    {
		var t = Camera.main.transform.position;
		t.x = transform.position.x;
        transform.LookAt(t);
    }
}
