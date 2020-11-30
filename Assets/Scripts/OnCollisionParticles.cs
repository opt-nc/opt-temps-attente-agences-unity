using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnCollisionParticles : MonoBehaviour
{
    public GameObject particles;
    private List<GameObject> emitters;
    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.tag == "Sphere"){
            Material thisMat = this.GetComponent<Renderer>().material;
            Material otherMat = other.gameObject.GetComponent<Renderer>().material;

            Light thisLight = this.GetComponent<Light>();
            thisLight.intensity = this.transform.localScale.x > 3 ? this.transform.localScale.x*2 : 6;
            thisLight.range = this.transform.localScale.x < 3 ? 15 : this.transform.localScale.x*2;
            thisLight.color = thisMat.color;
            
            Light otherLight = other.gameObject.GetComponent<Light>(); 
            otherLight.intensity = other.gameObject.transform.localScale.x*2;
            otherLight.color = otherMat.color;
        }
    }
}
