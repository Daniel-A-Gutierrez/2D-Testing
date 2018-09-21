using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
	GameManager gameManager;
	float fadeInTime;
	Animator anim ;
	void Start()
	{
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
		anim = GameObject.FindGameObjectWithTag("GameManager").transform.Find("Canvas").transform.Find("Panel").GetComponent<Animator>();
		fadeInTime = anim.GetCurrentAnimatorStateInfo(0).length;
	}
	public void Continue()
	{
		
		StartCoroutine("DelayedContinue");
	}

	public void NewGame()
	{
		
		StartCoroutine("OpenTextField");
	}

	public void Options()
	{

	}

	public void Begin()
	{
		string t = transform.Find("InputField").gameObject.GetComponent<InputField>().text;
		gameManager.SetPath ( t.Trim());
		StartCoroutine("DelayedNewGame");
	}

	IEnumerator DelayedContinue()
	{
		anim.Play("FadeToBlack");
		yield return new WaitForSeconds(fadeInTime);
		gameManager.GetComponent<GameManager>().Continue("madeup");
	}

	IEnumerator DelayedNewGame()
	{
		anim.Play("FadeToBlack");
		yield return new WaitForSeconds(fadeInTime);
		gameManager.GetComponent<GameManager>().NewGame();
	}

	IEnumerator OpenTextField()
	{
		anim.Play("FadeToBlack");
		yield return new WaitForSeconds(fadeInTime);
		transform.Find("InputField").gameObject.SetActive(true);
		transform.Find("New Game").gameObject.SetActive(false);
		transform.Find("Continue").gameObject.SetActive(false);
		transform.Find("Start").gameObject.SetActive(true);
		anim.Play("FadeIn");
	}
	
}
