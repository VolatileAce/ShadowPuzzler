using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Net : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        Destroy(gameObject, 3);
	}
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Shadow")
        {
            return;
        }
        if (other.tag == "Wall" || other.tag == "Player")
        {
            Destroy(gameObject);
        }
    }
}
