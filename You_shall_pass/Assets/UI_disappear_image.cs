using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_disappear_image : MonoBehaviour
{
    private float intensity;
    private Color color;
    public float duration;

    // Start is called before the first frame update
    void Start()
    {
        intensity = 1;
        color = GetComponent<Image>().color;
    }

    // Update is called once per frame
    void Update()
    {
        intensity -= Time.deltaTime / duration;

        color = new Color(color.r, color.g, color.b, (int) Mathf.Floor(intensity * 255));
    }
}
