using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class instructions : MonoBehaviour
{
    public float time;
    private float timer = 0;
    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
        this.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > time || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.M))
        {
            this.gameObject.SetActive(false);
        }
    }
}
