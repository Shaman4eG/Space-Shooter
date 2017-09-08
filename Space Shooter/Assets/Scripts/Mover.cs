using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour {

    public float speed;

    private Rigidbody rb;
    private Transform tf;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        tf = GetComponent<Transform>();

        rb.velocity = tf.forward * speed;
    }
}
