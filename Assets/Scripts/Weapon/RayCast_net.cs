using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Assets.Script;

public class RayCast_net : NetworkBehaviour
{
	public int gunDamage = 1;                                           // Set the number of hitpoints that this gun will take away from shot objects with a health script
    public float fireRate = 0.25f;                                      // Number in seconds which controls how often the player can fire
    public float weaponRange = 50f;                                     // Distance in Unity units over which the player can fire
    public float hitForce = 100f;                                       // Amount of force which will be added to objects with a rigidbody shot by the player
    public Transform gunEnd;                                            // Holds a reference to the gun end object, marking the muzzle location of the gun

    private Camera fpsCam;                                              // Holds a reference to the first person camera
    private WaitForSeconds shotDuration = new WaitForSeconds(0.07f);    // WaitForSeconds object used by our ShotEffect coroutine, determines time laser line will remain visible
    private AudioSource gunAudio;                                       // Reference to the audio source which will play our shooting sound effect
    private LineRenderer laserLine;                                     // Reference to the LineRenderer component which will display our laserline
    private float nextFire;   


    void Start()
    {
        // Get and store a reference to our LineRenderer component
        laserLine = GetComponent<LineRenderer>();

        // Get and store a reference to our AudioSource component
        gunAudio = GetComponent<AudioSource>();

        // Get and store a reference to our Camera by searching this GameObject and its children
        fpsCam = GetComponentInChildren<Camera>();
    }


    void Update()
    {
         // Check if the player has pressed the fire button and if enough time has elapsed since they last fired
        if (Input.GetKey(Player.settings.fire) && Time.time > nextFire && !Game.game.pause)
        {
        	// Update the time when our player can fire next
    		nextFire = Time.time + fireRate;
			PresShoot(fpsCam.transform.forward, fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f)), gunEnd.position);
			
        }
    }
	
	void PresShoot(Vector3 camForward, Vector3 rayOrigin, Vector3 end)
	{
		if (!isLocalPlayer)
		{
			return;
		}
		Shoot(camForward, rayOrigin, end);
	}
	
	[Client]
	void Shoot(Vector3 camForward, Vector3 rayOrigin, Vector3 end)
    {
    	// Start our ShotEffect coroutine to turn our laser line on and off
        StartCoroutine(ShotEffect());

        // Create a vector at the center of our camera's viewport
        //Vector3 rayOrigin = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));

        // Declare a raycast hit to store information about what our raycast has hit
        RaycastHit hit;
        
        // Set the start position for our visual effect for our laser to the position of gunEnd
        laserLine.SetPosition(0, end);

        // Check if our raycast has hit anything
        if (Physics.Raycast(rayOrigin, camForward, out hit, weaponRange))
        {
            // Set the end position for our laser line 
            laserLine.SetPosition(1, hit.point);

			if (!isServer) 
			{
				Game.map.Shoot(hit.transform.gameObject);
				CmdHit(hit.transform.gameObject.name);
			}
			else
			{
				RpcHit(hit.transform.gameObject.name);
			}

            //// Check if the object we hit has a rigidbody attached
            //if (hit.rigidbody != null)
            //{
            //    // Add force to the rigidbody we hit, in the direction from which it was hit
            //    hit.rigidbody.AddForce(-hit.normal * hitForce);
            //}
            
        }
        else
        {
            // If we did not hit anything, set the end of the line to a position directly in front of the camera at the distance of weaponRange
            laserLine.SetPosition(1, rayOrigin + (camForward * weaponRange));
        }
	}
    
    [Command]
    public void CmdHit(string name)
    {
		Game.map.Shoot(GameObject.Find(name));
	}
	
	[ClientRpc]
    public void RpcHit(string name)
    {
		Game.map.Shoot(GameObject.Find(name));
	}
    
    private IEnumerator ShotEffect()
    {
        // Play the shooting sound effect
        gunAudio.Play();

        // Turn on our line renderer
        laserLine.enabled = true;

        //Wait for .07 seconds
        yield return shotDuration;

        // Deactivate our line renderer after waiting
        laserLine.enabled = false;
    }
}
