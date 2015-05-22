using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScriptAircraft : MonoBehaviour 
{
	private bool dragging = false;
	private bool imMoving = false;
	private bool imClickedOn = false;
	private bool imTravelling = false;
	private bool imLanding = false;
	private float startPosY = 0.0f;
	private int countDrag = 0;
	private int countMove = 0;
	private Vector3 mousePosition;
	private Vector3 mousePoint;
	private Vector3 pointCurrent;
	private Vector3 pointStore;
	private Vector3 targetWaypoint;
	private Vector3 moveDirection;
	private float moveSpeed = 3.0f;
	private Quaternion wayRotation;
	private float damping  = 6.0f;
	private List<float> posStoreX  = new List<float>();
	private List<float> posStoreZ = new List<float>();
	private List<GameObject> arrayPathMarker = new List<GameObject>(200);
	public GameObject objectPathMarker;
	
	private Transform imSelected;
	
	public GameObject objectAircraft;
	public Transform objectExplosion;
	
	private Vector3 setLandingZone1 = new Vector3(6, 0, 8);
	private Vector3 setLandingZone2 = new Vector3(10, 0, 3);
	private List<float> posRunwayX = new List<float>(){12.75f, 15.75f, 14.75f};
	private List<float> posRunwayZ = new List<float>(){8f, 8f, -0.5f};
	

	public ScriptSceneManager Scene_Manager;


	void Start () 
	{
		Time.timeScale = 1;
		dragging = false;
		startPosY = transform.position.y;
	}
	
	void Update () 
	{
		// check if not already following a trail
		if (!imMoving) 
		{
			RaycastHit rayHit;
			if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out rayHit)) 
			{
				mousePoint = rayHit.point;
				
				// select this object if it's being clicked on
				imClickedOn = false; // reset as raycast is set every frame , if mouseover this, stays flagged as true without this
				imSelected = rayHit.transform;
				if (imSelected == this.transform) 
					imClickedOn = true;
				
				if (Input.GetMouseButtonDown(0)) 
				{
					if (imClickedOn) 
					{
						OnTouchBegin(mousePoint);
					} // check if object has been clicked on
				}
				else if (Input.GetMouseButton(0)) 
				{
					if (dragging) 
					{
						OnTouchMove(mousePoint);
					} // check in-case mouse is down when object travelling is set to false with (dragging)
				}
				else if (Input.GetMouseButtonUp(0)) 
				{
					if (dragging) 
					{
						OnTouchEnd(mousePoint);
					} // check in-case mouse is released when object travelling is set to false with (dragging)
				}
			}
		}
	}
	
	void OnTouchBegin (Vector3 pointCurrent)
	{
		// destroy path markers and array
		for (var i = 0; i< arrayPathMarker.Count; i++) 
		{
		
			Destroy (arrayPathMarker[i]); // this destroys
			//arrayPathMarker[i]=null; // this sets to null (inst obj remains)
		}
		arrayPathMarker.Clear();
	
		//
		countDrag = 0;
		posStoreX.Clear();
		posStoreZ.Clear();
		AddSplinePoint(pointCurrent);
		dragging = true;
		imMoving = false;
	}
	
	void OnTouchMove (Vector3 pointCurrent) 
	{
		if ((dragging) && (countDrag < 200)) 
		{
			AddSplinePoint(pointCurrent);
			imTravelling = true;
		} 
		else 
		{
			dragging = false;
			imMoving = true;
		}
	}
	
	void OnTouchEnd (Vector3 pointCurrent) 
	{
		dragging = false;
		imMoving = true;
		if (imSelected) 
		{
			imSelected = null; 
			imClickedOn = false;
		} // resets state -> 'select this object if clicked on'
	}
	
	void AddSplinePoint (Vector3 pointStore) 
	{

		// store co-ordinates
		posStoreX.Add (pointStore.x);
		posStoreZ.Add (pointStore.z);
		// show path : Instantiate and load position into array as gameObject
		arrayPathMarker.Add ((GameObject)Instantiate(objectPathMarker, new Vector3(pointStore.x, 0.2f, pointStore.z), transform.rotation));
		// next position
		countDrag ++;

	}
	
	void FixedUpdate () 
	{
		
		// -- move gameObject --
		
		// check if following a trail and there is a trail marker to follow
		if ((imTravelling) && (posStoreX.Count>=countMove))
		{
			
			// --
			
			// move aircraft along path in time
			targetWaypoint = new Vector3(posStoreX[countMove], startPosY, posStoreZ[countMove]);
			moveDirection = targetWaypoint - transform.position;
			
			// check if the waypoint has been reached
			if(moveDirection.magnitude < 1) 
			{
				Destroy (arrayPathMarker[countMove]); // remove current path marker
				countMove++; // next waypoint position
				// still move forward (stops stuttery efect when moving)
				transform.position += moveDirection.normalized * moveSpeed * Time.deltaTime; // move gameObject over time
				wayRotation  = Quaternion.LookRotation(targetWaypoint - transform.position); // find rotation
				transform.rotation = Quaternion.Slerp(transform.rotation, wayRotation , Time.deltaTime * damping); // rotate gameObject over time
			} 
			else 
			{		
				transform.position += moveDirection.normalized * moveSpeed * Time.deltaTime; // move gameObject over time
				wayRotation  = Quaternion.LookRotation(targetWaypoint - transform.position); // find rotation
				transform.rotation = Quaternion.Slerp(transform.rotation, wayRotation , Time.deltaTime * damping); // rotate gameObject over time
			}
			
			if (countMove >= posStoreX.Count) 
				finishTravelling ();// stop at end of path
			
			// --
			
		} 
		else 
		{
			if (!imLanding) 
				countMove = 0; // stops the var resetting stopping the landing procedure
		}
		
		// -- aircraft flying normally
		if (!imTravelling)
			transform.position += (transform.forward * moveSpeed * Time.deltaTime);

		
		// -- Landing aircraft
		
		// Landing Procedure
		if (imLanding) 
		{
			//reset all other vars
			dragging = false; 
			imMoving = false; 
			imClickedOn = false; 
			imTravelling = false;
			// destroy path markers and array
			for (var p = 0; p< arrayPathMarker.Count; p++) 
			{
				Destroy (arrayPathMarker[p]); // this destroys
				arrayPathMarker[p]=null; // this sets to null (inst obj remains)
			}
			
			// follow runway waypoints
			targetWaypoint = new Vector3(posRunwayX[countMove], 1.125f, posRunwayZ[countMove]);
			moveDirection = targetWaypoint - transform.position;
			
			// check if the waypoint has been reached
			if(moveDirection.magnitude < 1) 
			{
				countMove++; // next waypoint position
			} 
			else 
			{		
				transform.position += moveDirection.normalized * Time.deltaTime; // move gameObject over time => (moveSpeed/2) * Time.deltaTime;
				wayRotation  = Quaternion.LookRotation(targetWaypoint - transform.position); // find rotation
				transform.rotation = Quaternion.Slerp(transform.rotation, wayRotation , Time.deltaTime * damping); // rotate gameObject over time
			}	
			
			// move this aircraft and clone another aircraft 
			if (countMove >= posRunwayX.Count) 
				relocateAircraft ();
		}
		
		// -- boundaries
		if (transform.position.x < -20f)
		{
			transform.position = new Vector3(transform.position.x + 40f, transform.position.y, transform.position.z);
		}
		if (transform.position.x > 20f) 
		{
			transform.position = new Vector3(transform.position.x - 40f, transform.position.y, transform.position.z);

		}
		if (transform.position.z < -15f) 
		{
			transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 30f);
		}
		if (transform.position.z > 15f) 
		{
			transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 30f);
		}
		
		// print out of current stored array
		if (Input.GetKeyDown("r")) {print ("posStoreX "+posStoreX+" : posStoreZ "+posStoreZ);}
	}
	
	void finishTravelling () 
	{
		countMove = 0;
		imMoving = false; 
		if (imSelected) 
		{
			imSelected = null; 
			imClickedOn = false;
		} // reset 'select this object if clicked on'
		
		// check for if a landing zone is in range
		if (((setLandingZone1 - transform.position).magnitude) < 4.0f) {imLanding = true; countMove = 0;}	
		if (((setLandingZone2 - transform.position).magnitude) < 4.0f) {imLanding = true; countMove = 0;}
		//
		
		imTravelling = false; 
	}
	
	void relocateAircraft () 
	{
		//reset all other vars
		dragging = false; 
		imMoving = false; 
		imClickedOn = false; 
		imTravelling = false;
		//
		countMove = 0; 
		imLanding = false;

		// add landed to count
		Scene_Manager.aircraftLanded += 1;

		// move aircraft
		transform.position = new Vector3((Random.Range(-6f,6f)), startPosY, -14f);
		transform.rotation = Quaternion.Euler(0f, (Random.Range(0,45f)), 0f);

		// create new aircraft
		Instantiate (objectAircraft, new Vector3(-18f, startPosY, (Random.Range(-10f,10f))), Quaternion.Euler(0f, (Random.Range(60f,120f)), 0f));
	}
	
	void OnTriggerEnter (Collider other) 
	{
		if ((other.tag == "Aircraft") && (!imLanding)) 
		{
			// empty path marker array
			for (var i = 0; i< arrayPathMarker.Count; i++)
			{
				Destroy (arrayPathMarker[i]); // this destroys
				arrayPathMarker[i]=null; // this sets to null (inst obj remains)
			}
			arrayPathMarker.Clear ();

			// add to count - lost
			Scene_Manager.aircraftLost += 1;

			//destroy Aircraft
			Destroy(gameObject);
			Instantiate (objectExplosion, transform.position, transform.rotation);
		}
	}
}
