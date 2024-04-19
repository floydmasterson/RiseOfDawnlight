using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] public Slider healthBar;
    [SerializeField] private Camera camera;
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    [SerializeField] private TextMeshProUGUI healthText;


    public void SetBar(int maxHealth)
    {
		healthBar.maxValue = maxHealth;
		healthBar.value = maxHealth;
		healthText.text = $"{maxHealth}/{maxHealth}";
	}
    public void UpdateHealth(int currrentHealth, int maxHealth)
    {
       
		healthText.text = $"{currrentHealth}/{maxHealth}";
        healthBar.value = currrentHealth;
    }
    // Update is called once per frame
    void Update()
    {
        transform.rotation = camera.transform.rotation;
        transform.position = target.position + offset;
    }
}
