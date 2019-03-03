﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GameManager : MonoBehaviour {

    private TimeManager timeManager;
    private bool gameStarted;

    public GameObject playerPrefab;
    private GameObject player;

    private GameObject floor;
    private Spawner spawner;

    public Text continueText;
    private float blinkTime = 0f;
    private bool blink;

    public Text scoreText;
    private float timeElapsed = 0f;
    private float bestTime = 0f;
    private bool beatBestTime;

    private AudioSource audioBestScore;
    private AudioSource audioGameOver;


    void Awake()
    {
        floor = GameObject.Find("Foreground");
        spawner = GameObject.Find("Spawner").GetComponent<Spawner>();
        timeManager = GetComponent<TimeManager>();
        audioBestScore = GameObject.Find("AudioBestScore").GetComponent<AudioSource>();
        audioGameOver = GameObject.Find("AudioGameOver").GetComponent<AudioSource>();

    }

    // Use this for initialization
    void Start () {
        var floorHeight = floor.transform.localScale.x;
        var pos = floor.transform.position;
        pos.x = 0;
        pos.y = -((Screen.height / PixelPerfectCamera.pixelsToUnit) / 2) + (floorHeight / 2);
        floor.transform.position = pos;

        spawner.active = false;

        Time.timeScale = 0;

        continueText.text = "PRESS ANY BUTTON TO START";

        bestTime = PlayerPrefs.GetFloat("BestTime");
    }
	
	// Update is called once per frame
	void Update () {
        if (!gameStarted && Time.timeScale == 0)
        {
            if (Input.anyKeyDown)
            {
                timeManager.ManipulateTime(1, 1);
                ResetGame();
            }
        }

        continueText.text = "PRESS ANY BUTTON TO START";

        if (!gameStarted)
        {
            blinkTime++;
            if (blinkTime % 40 == 0)
            {
                blink = !blink;
            }

            continueText.canvasRenderer.SetAlpha(blink ? 0 : 1);
            var textColor = beatBestTime ? "#FF0" : "#FFF";
            scoreText.text = "TIME: " + FormatTime(timeElapsed) + "\n<color=" + textColor + ">BEST: " + FormatTime(bestTime) + "</color>";
        } else
        {
            timeElapsed += Time.deltaTime;
            scoreText.text = "TIME: " + FormatTime(timeElapsed);
        }
    }

    void OnPlayerKilled()
    {
        spawner.active = false;

        // Remove the callback to prevent memory leaks
        var playerDestroyScript = player.GetComponent<DestroyOffscreen>();
        playerDestroyScript.DestroyCallback -= OnPlayerKilled;

        player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        timeManager.ManipulateTime(0, 5.5f);

        gameStarted = false;

        continueText.text = "PRESS ANY BUTTON TO RESTART";

        if (timeElapsed > bestTime)
        {
            bestTime = timeElapsed;
            PlayerPrefs.SetFloat("BestTime", bestTime);
            beatBestTime = true;
            audioBestScore.Play();
        } else
        {
            audioGameOver.Play();
        }
    }

    void ResetGame()
    {
        spawner.active = true;
        player = GameObjectUtil.Instantiate(playerPrefab, new Vector3(0, (Screen.height / PixelPerfectCamera.pixelsToUnit) / 2 + 100, 0));

        // Add the callback
        var playerDestroyScript = player.GetComponent<DestroyOffscreen>();
        playerDestroyScript.DestroyCallback += OnPlayerKilled;

        gameStarted = true;

        continueText.canvasRenderer.SetAlpha(0);

        timeElapsed = 0f;
        beatBestTime = false;
    }

    string FormatTime(float value)
    {
        TimeSpan t = TimeSpan.FromSeconds(value);
        return string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
    }
}
