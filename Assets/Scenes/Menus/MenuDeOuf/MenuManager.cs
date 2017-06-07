using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour {

	public GameObject mainMenu, weaponsMenu,graphismeMenu,controlsMenu,MultiMenu, /*background,*/StartMenu;

	private Animator mainMenuAnim,weaponsMenuAnim,graphismeMenuAnim,/*backgroundAnim,*/MultiMenuAnim,StartMenuAnim;
	private Animator controlsMenuAnim;
	private bool isMainMenuOnLeft,isWeaponMenuActivate,isGraphismeMenuActivate,isControlsMenuActivate,isMultiMenuActivate,isStartMenuLeft;


	// Use this for initialization
	public void Start () {
		Time.timeScale = 1.0f;
		mainMenuAnim = mainMenu.GetComponent<Animator>();
		weaponsMenuAnim = weaponsMenu.GetComponent<Animator>();
		graphismeMenuAnim = graphismeMenu.GetComponent<Animator>();
		MultiMenuAnim = MultiMenu.GetComponent<Animator>();
		controlsMenuAnim = controlsMenu.GetComponent<Animator>();
		//backgroundAnim = background.GetComponent<Animator>();
		StartMenuAnim = StartMenu.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// fonction des boutons du menu

	public void OnWeaponsButtonClick(){
		//print("OnWeaponsButtonClick");
		if (isGraphismeMenuActivate) {
			OnGraphismeButtonClick ();
		}
		if (isControlsMenuActivate) {
			OnControlsButtonClick ();
		}
		if(isMainMenuOnLeft==false){
			mainMenuAnim.SetBool("isMovingLeft",true);
			weaponsMenuAnim.SetBool("isMovingIn",true);
			//backgroundAnim.SetBool("isTurning",true);
			isMainMenuOnLeft =true;
			isWeaponMenuActivate = true;
		}else{
			mainMenuAnim.SetBool("isMovingLeft",false);
			weaponsMenuAnim.SetBool("isMovingIn",false);
			//backgroundAnim.SetBool("isTurning",false);
			isMainMenuOnLeft =false;
			isWeaponMenuActivate = false;

		}
	}
	public void OnGraphismeButtonClick(){
		//print ("OnGraphismeButtonClick");
		if (isWeaponMenuActivate) {
			OnWeaponsButtonClick ();
		}
		if (isControlsMenuActivate) {
			OnControlsButtonClick ();
		}
		if (isMainMenuOnLeft == false) {
			mainMenuAnim.SetBool ("isMovingLeft", true);
			graphismeMenuAnim.SetBool ("isMovingIn", true);
			//backgroundAnim.SetBool ("isTurning", true);
			isMainMenuOnLeft = true;
			isGraphismeMenuActivate = true;
		} else {
			mainMenuAnim.SetBool ("isMovingLeft", false);
			graphismeMenuAnim.SetBool ("isMovingIn", false);
			//backgroundAnim.SetBool ("isTurning", false);
			isMainMenuOnLeft = false;
			isGraphismeMenuActivate = false;
		}
	}
	public void OnControlsButtonClick(){
		//print ("OnGraphismeButtonClick");
		if (isWeaponMenuActivate) {
			OnWeaponsButtonClick ();
		}
		if (isGraphismeMenuActivate) {
			OnGraphismeButtonClick ();
		}
		if (isMainMenuOnLeft == false) {
			mainMenuAnim.SetBool ("isMovingLeft", true);
			controlsMenuAnim.SetBool ("isMovingIn", true);
			//backgroundAnim.SetBool ("isTurning", true);
			isMainMenuOnLeft = true;
			isControlsMenuActivate = true;
		} else {
			mainMenuAnim.SetBool ("isMovingLeft", false);
			controlsMenuAnim.SetBool ("isMovingIn", false);
			//backgroundAnim.SetBool ("isTurning", false);
			isMainMenuOnLeft = false;
			isControlsMenuActivate = false;
		}
	}
	public void SetTrueisMainMenuOnLeft()
	{
		isMainMenuOnLeft = false;
	}
	public void Start2 () {
		mainMenuAnim = mainMenu.GetComponent<Animator>();
		weaponsMenuAnim = weaponsMenu.GetComponent<Animator>();
		graphismeMenuAnim = graphismeMenu.GetComponent<Animator>();
		controlsMenuAnim = controlsMenu.GetComponent<Animator>();
		//backgroundAnim = background.GetComponent<Animator>();
	}
	public void OnMultiClick(){
		//print ("OnMultiClick");
		if (isStartMenuLeft == false) {
			StartMenuAnim.SetBool ("isMovingLeft", true);
			MultiMenuAnim.SetBool ("isMovingIn", true);
			//backgroundAnim.SetBool ("isTurning", true);
			isStartMenuLeft = true;
			isMultiMenuActivate = true;
		} else {
			StartMenuAnim.SetBool ("isMovingLeft", false);
			MultiMenuAnim.SetBool ("isMovingIn", false);
			//backgroundAnim.SetBool ("isTurning", false);
			isStartMenuLeft = false;
			isMultiMenuActivate = false;
		}
	}
}