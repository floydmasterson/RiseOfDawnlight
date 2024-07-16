using System.Collections.Generic;
using UnityEngine;

public class ArcRenderer : MonoBehaviour
{
	public GameObject arrowPrefab;
	public GameObject dotPrefab;
	public int poolSize;
	private List<GameObject> dotPool = new List<GameObject>();
	private GameObject arrowInstance;

	public float spacing = 50;
	public float arrowAngleAdjusment = 0;
	public int dotsToSkip = 1;
	private Vector3 arrowDirection;
	public bool onEnmey;
	public GameObject obsitcalRayObject;
	[SerializeField] private float obsticalRayDistance;

	void Start()
	{
		arrowInstance = Instantiate(arrowPrefab, transform);
		arrowInstance.transform.localPosition = Vector3.zero;
		obsitcalRayObject = arrowInstance;
		InitializeDotPool(poolSize);
	}
	void Update()
	{
		Vector3 mousePos = Input.mousePosition;

		mousePos.z = 0;

		Vector3 startPos = transform.position;
		Vector3 midPoint = CalculateMidPoint(startPos, mousePos);

		UpdateArc(startPos, midPoint, mousePos);
		PositionAndRotateArrow(mousePos);

		int Obstruction = LayerMask.NameToLayer("cardTarget");
		RaycastHit2D hit = Physics2D.Raycast(obsitcalRayObject.transform.position, Vector2.right, obsticalRayDistance);
		Debug.DrawRay(obsitcalRayObject.transform.position, Vector2.right * hit.distance);
		if(hit.collider != null && hit.collider.gameObject.layer == Obstruction)
		{
			onEnmey = true;
			Debug.Log($"trigger enter on {hit.collider.name}");
		}
		else if( onEnmey == true)
		{
			onEnmey = false;
			Debug.Log($"trigger exit");
		}
	}
	
	private void UpdateArc(Vector3 start, Vector3 mid, Vector3 end)
	{
		int numberOfDots = Mathf.CeilToInt(Vector3.Distance(start, end) / spacing);

		for (int i = 0; i < numberOfDots && i < dotPool.Count; i++)
		{
			float t = i / (float)numberOfDots;
			t = Mathf.Clamp(t, 0f, 1f);

			Vector3 position = QuadraticBezierPoint(start, mid, end, t);

			if (i != numberOfDots - dotsToSkip)
			{
				dotPool[i].transform.position = position;
				dotPool[i].SetActive(true);
			}
			if (i == numberOfDots - (dotsToSkip + 1) && i - dotsToSkip + 1 >= 0)
			{
				arrowDirection = dotPool[i].transform.position;
			}

		}
		//Decactivate unused Dots
		for (int i = numberOfDots - dotsToSkip; i < dotPool.Count; i++)
		{
			if (i > 0)
			{
				dotPool[i].SetActive(false);
			}
		}
	}

	private Vector3 QuadraticBezierPoint(Vector3 start, Vector3 control, Vector3 end, float t)
	{
		float u = 1 - t;
		float tt = t * t;
		float uu = u * u;

		Vector3 point = uu * start;
		point += 2 * u * t * control;
		point += tt * end;
		return point;
	}

	private void PositionAndRotateArrow(Vector3 position)
	{
		arrowInstance.transform.position = position;
		Vector3 direction = arrowDirection - position;
		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		angle += arrowAngleAdjusment;
		arrowInstance.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}


	private Vector3 CalculateMidPoint(Vector3 start, Vector3 end)
	{
		Vector3 midPoint = (start + end) / 2;
		float arcHeight = Vector3.Distance(start, end) / 3f;
		midPoint.y += arcHeight;
		return midPoint;
	}

	private void InitializeDotPool(int count)
	{
		for (int i = 0; i < count; i++)
		{
			GameObject dot = Instantiate(dotPrefab, Vector3.zero, Quaternion.identity, transform);
			dot.SetActive(false);
			dotPool.Add(dot);
		}
	} 

	private void OnTriggerEnter2D(Collider2D other)
	{
		onEnmey = true;
		Debug.Log($"trigger enter on {other.name}");
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		
	}
		

}
