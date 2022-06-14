using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PlayerHealth : MonoBehaviour
{
    public float health;
    public Slider bar;
    public TMP_Text healthLabel;


    // Start is called before the first frame update
    void Start()
    {
        bar.value = health;
        healthLabel.text = bar.value.ToString();

    }

    // Update is called once per frame
    void Update()
    {
     
    }

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("testing dmg");
        if (collision.gameObject.tag == "Enemy")
        {
            bar.value = bar.value - 10f;
        }
    }
}
