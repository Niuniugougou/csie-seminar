﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LaserGenerator : MonoBehaviour {
    public GameObject LaserBall;
    public int BallForce = 2000;
    public float GeneratorBallTime = .1f;

    private float _timer = 0f;
    private List<GameObject> _laserBalls;
    
	// Use this for initialization
	void Start () {

    }

    // Update is called once per frame
    void Update () {
        _timer += Time.deltaTime;
        if (_timer >= GeneratorBallTime)
        {
            _timer = 0f;
            GameObject clone = (GameObject)Instantiate(LaserBall, this.transform.position, this.transform.rotation);
            clone.transform.localScale = new Vector3((float)20, (float)20, (float)20);
            clone.transform.SetParent(this.transform);
            foreach (Transform child in transform)
            {
                if (child.tag == "laserBall")
                {
                    Physics.IgnoreCollision(clone.GetComponent<SphereCollider>(), child.GetComponent<SphereCollider>());
                }
            }
            clone.GetComponent<Rigidbody>().AddForce(transform.forward * BallForce);
        }
	}
}
