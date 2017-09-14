using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGScroller : MonoBehaviour {

    public float scrollSpeed;
    public float tileSizeZ;

    private Transform tf;
    private Vector3 startPosition;

	// Use this for initialization
	void Start () {
        tf = GetComponent<Transform>();
        startPosition = tf.position;
	}
	
	// Update is called once per frame
	void Update () {
        float newPosition = Mathf.Repeat(Time.time * scrollSpeed, tileSizeZ);
        tf.position = startPosition + Vector3.forward * newPosition;
	}
}
