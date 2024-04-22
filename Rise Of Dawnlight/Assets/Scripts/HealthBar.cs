using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

	public enum StatusEffects
	{
		Bleed,
		Cursed,
		Burn,
		Dazed,
		Knocked_Down,
		Poisoned,
	}

	[SerializeField] public Slider healthBar;
	[SerializeField] private Camera targetCamera;
	[SerializeField] private Transform target;
	[SerializeField] private Vector3 offset;
	[SerializeField] private TextMeshProUGUI healthText;
	[SerializeField] private GameObject[] statusEffects;


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
		transform.rotation = targetCamera.transform.rotation;
		transform.position = target.position + offset;
	}

	public void ToggleStatus(StatusEffects effectNumber, bool state)
	{
		statusEffects[(int)effectNumber].SetActive(state);
	}

}
