﻿using UnityEngine;
using System.Collections;

/*
 * score[0] = Frame 1 : 1
 * score[1] = Frame 1 : 2
 * 
 * score[2] = Frame 2 : 1
 * score[3] = Frame 2 : 2
 * 
 * score[4] = Frame 3 : 1
 * score[5] = Frame 3 : 2
 * 
 * score[6] = Frame 4 : 1
 * score[7] = Frame 4 : 2
 * 
 * score[8] = Frame 5 : 1
 * score[9] = Frame 5 : 2
 * 
 * score[10] = Frame 6 : 1
 * score[11] = Frame 6 : 2
 * 
 * score[12] = Frame 7 : 1
 * score[13] = Frame 7 : 2
 * 
 * score[14] = Frame 8 : 1
 * score[15] = Frame 8 : 2
 * 
 * score[16] = Frame 9 : 1
 * score[17] = Frame 9 : 2
 * 
 * score[18] = Frame 10 : 1
 * score[19] = Frame 10 : 2
 * score[20] = Frame 10 : 3
 * 
 */

public class ScoreController : MonoBehaviour {
	private static readonly int NUM_ROLLS = 21;
	private static readonly int NUM_PINS = 10;
	private static readonly int FARTHEST_POINT_Z = 9;
	private static readonly double ROTATION_THRESHOLD = 0.2;
	
	public GUIText scoreText;
	
	private int currentRoll;
	private int[] scoreBoard;
	private bool[] currentPinStates;
	private bool[] previousPinStates;
	private Transform[] pins;
	private int currentScore = 0;
	private bool levelBegan;

	void Start () {
		DontDestroyOnLoad(this);
		currentRoll = 0;
		levelBegan = false;
		scoreBoard = new int[NUM_ROLLS];
		ResetAllPins();
	}
	
	private void ResetAllPins() {
		ResetCurrentPins();
		ResetPreviousPins();
	}
	
	private void ResetCurrentPins() {
		currentPinStates = new bool[NUM_PINS];
		for(int i = 0; i < NUM_PINS; i++) {
			currentPinStates[i] = true;	
		}
	}
	
	private void ResetPreviousPins() {
		previousPinStates = new bool[NUM_PINS];
		for(int i = 0; i < NUM_PINS; i++) {
			previousPinStates[i] = true;
		}
	}
	
	public void SetPins(Transform[] pins) {
		this.pins = pins;
	}
	
	public void SetScoreTextObject(GUIText scoreText) {
		this.scoreText = scoreText;
	}
	
	public void beginLevel() {
		levelBegan = true;	
	}
	
	public void endLevel() {
		levelBegan = false;
		AddScoreToBoard();
		PrintPinStates();	
	}
	
	public void InitiatePins() {
		for(int i = 0; i < NUM_PINS; i++) {
			if(!previousPinStates[i]) {
				Destroy(pins[i].gameObject);
			}
		}
	}
	
	void Update () {
		if(levelBegan) {
			currentScore = 0;
			for(int i = 0; i < NUM_PINS; i++) {
				if(pins[i] != null) {
					Transform pin = pins[i];
					
					if(pinIsDown(pin.rotation.z, pin.rotation.x, pin.position.z)) {
						currentScore++;
						currentPinStates[i] = false;
					}
				}
			}
			SetScoreText();
		}
	}	

	private bool pinIsDown (double zRotation, double xRotation, double zPosition) {
		return zRotation > ROTATION_THRESHOLD || xRotation > ROTATION_THRESHOLD ||
				zRotation < -ROTATION_THRESHOLD ||
				xRotation < -ROTATION_THRESHOLD ||
				zPosition > FARTHEST_POINT_Z;
	}
	
	public void SetScoreText() {
		scoreText.text = "Total: " + GetTotalScore() +  " - Current Score: " + currentScore;
	}
	
	public void AddScoreToBoard() {
		scoreBoard[currentRoll] = currentScore;
		previousPinStates = currentPinStates;
		currentRoll++;
		CheckPinStates();
		
		if(currentRoll == NUM_ROLLS) {
			Debug.Log ("Total score for 21 rolls: " + GetTotalScore());
			ResetGame();
		}
	}
	
	public void CheckPinStates() {
		if(!(currentRoll > 18) && currentRoll % 2 == 0) {
			ResetAllPins();	
		} else {
			foreach(bool pinIsUp in previousPinStates) {
				if(pinIsUp)	{
					return;	
				}
			}
			ResetAllPins();
		}
	}
	
	public void ResetGame() {
		Destroy(this);
		Application.LoadLevel("MainMenu");
	}
	
	public int GetTotalScore() {
		int totalScore = 0;
		
		for(int i = 0; i < scoreBoard.Length; i++) {
			totalScore += scoreBoard[i];	
		}
		
		return totalScore;
	}
	
	public void PrintPinStates() {
		string pins = "Pins: ";
		for(int i = 0; i < NUM_PINS; i++) {
			if(currentPinStates[i]) {
				pins += (i + 1) + " ";	
			}
		}
		Debug.Log(pins + "are up.");
	}
}