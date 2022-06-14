using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class HealthBarController : MonoBehaviour
{
    public CharacterController controller;

    public Slider bar;
    public TMP_Text healthLabel;
    [Range(0, 100)]
    public int healthValue = 100;

    private int startingHealthValue;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();

        startingHealthValue = healthValue;
    }

    // Update is called once per frame
    void Update()
    {
        if (healthValue != startingHealthValue)
        {
            startingHealthValue = healthValue;
            bar.value = healthValue;
            OnValueChanged();
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            // Kill Code for testing
            TakeDamage(10);
        }
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("Damage");
        healthValue -= damage;
        if (healthValue < 0)
        {
            healthValue = 0;
        }
    }

    public void OnValueChanged()
    {
        healthLabel.text = bar.value.ToString();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("testing dmg");
        if(collision.gameObject.tag=="Enemy")
        {
            TakeDamage(10);
        }
    }
}