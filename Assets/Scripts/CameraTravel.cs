using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTravel : MonoBehaviour{
    public int speed = 20;
    // Update is called once per frame
    void Update(){
        this.transform.RotateAround(Vector3.zero, Vector3.up, speed*Time.deltaTime);
    }
}
