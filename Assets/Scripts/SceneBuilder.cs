using System.Threading;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using NiceJson;

public class SceneBuilder : MonoBehaviour
{
    public GameObject original;
    public GameObject parent;
    private float jsonUpdateTime = 10.0f;
    private float jsonNextTime = 30.0f;
    private string URL = "http://127.0.0.1:8081/temps-attente/agences";

    IEnumerator getJson(){
        UnityWebRequest request = UnityWebRequest.Get(URL);
        yield return request.SendWebRequest();
    
        if(request.isNetworkError || request.isHttpError){
            Debug.LogError(request.error);
            yield break;
        }
        
        JsonArray json = (JsonArray) JsonNode.ParseJsonString(request.downloadHandler.text);

        foreach(JsonObject obj in json){
            var newSphere = Instantiate(original, original.transform.position, Quaternion.identity, parent.transform);

            Color color = new Color(UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f));

            newSphere.GetComponent<Renderer>().material.SetColor("_Color", color); 
            //newSphere.GetComponent<Rigidbody>().AddForce(new Vector3(UnityEngine.Random.Range(-500, 500), 0.0f, UnityEngine.Random.Range(-500, 500)));

            /*
            Vector3 scale = transform.localScale;
            scale = original.transform.localScale * (obj["realMaxWaitingTimeMs"]/700000 + 1);
            newSphere.transform.localScale = scale;
            newSphere.GetComponent<Rigidbody>().mass = scale.x > 1 ? scale.x * 5 : scale.x;
            */

            newSphere.name = obj["designation"];
            
            newSphere.GetComponent<MeshRenderer>().enabled = false;
            newSphere.GetComponent<SphereCollider>().enabled = false;
            newSphere.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;
        }
        original.SetActive(false);

        for(var i = 0; i < GameObject.FindGameObjectsWithTag("Sphere").Length; i++){
            StartCoroutine(makeSpheresAppear(i));
        }
    }

    IEnumerator updateJson() {
        UnityWebRequest request = UnityWebRequest.Get(URL);
        yield return request.SendWebRequest();
    
        if(request.isNetworkError || request.isHttpError){
            Debug.LogError(request.error);
            yield break;
        }

        JsonArray json = (JsonArray) JsonNode.ParseJsonString(request.downloadHandler.text);

        var i = 0;
        foreach(GameObject obj in GameObject.FindGameObjectsWithTag("Sphere")){          
            if(obj.name == "Sphere"){
                continue;
            }
            i++;
            
            Vector3 scale = obj.transform.localScale;
            scale = original.transform.localScale * (json[i-1]["realMaxWaitingTimeMs"]/700000 + 1);
            if(scale.x != obj.transform.localScale.x){
                obj.transform.localScale = scale;
                obj.GetComponent<Rigidbody>().mass = scale.x > 1 ? scale.x * 5 : scale.x;
            }
        }
        Debug.Log("Updated!");
    }

    IEnumerator makeSpheresAppear(int id){
        yield return new WaitForSeconds(id*0.2f);

        var objects = GameObject.FindGameObjectsWithTag("Sphere");
        if( objects[id].GetComponent<MeshRenderer>().enabled != true){
            objects[id].GetComponent<MeshRenderer>().enabled = true;
            objects[id].GetComponent<SphereCollider>().enabled = true;
            objects[id].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }
    }

    // Start is called before the first frame update
    void Start(){
        StartCoroutine(getJson());
    }

    // Update is called once per frame
    void FixedUpdate(){
        if(Time.time > jsonNextTime){
            jsonNextTime = Time.time + jsonUpdateTime;
            StartCoroutine(updateJson());
        }
        

        foreach(GameObject obj in GameObject.FindGameObjectsWithTag("Sphere")){
            var rb = obj.GetComponent<Rigidbody>();
            
            rb.velocity = new Vector3(
                (rb.velocity.x > 50) ? 50 : rb.velocity.x,
                (rb.velocity.y > 50) ? 50 : rb.velocity.y,
                (rb.velocity.z > 50) ? 50 : rb.velocity.z
            );

            rb.transform.Rotate(
                rb.velocity.x,
                rb.velocity.y,
                rb.velocity.z
            );
        }
    }
}
