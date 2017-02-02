using UnityEngine;
using System.Collections;

public class numberPop : MonoBehaviour {


    public float duration;
    private float expiration;

    public void updateText(string s)
    {
        GetComponent<TextMesh>().text = s;
    }

    public void updateColor(Color c)
    {
        GetComponent<TextMesh>().color = c;
    }

    public void setTextPos(Vector3 v)
    {
       // transform.position = v;
    }
    void Start()
    {
        GetComponent<Renderer>().sortingLayerName = "Ammo";
        GetComponent<Renderer>().sortingOrder = 1;
        expiration = 0;
    }
    void Update()
    {
        transform.Translate(new Vector3(Time.deltaTime * 0.5f, Time.deltaTime*2f, 0));
        expiration += Time.deltaTime;
        if (expiration > duration)
            Destroy(gameObject);
    }
}
