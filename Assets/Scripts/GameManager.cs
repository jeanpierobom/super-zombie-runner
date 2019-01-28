using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    private TimeManager timeManager;
    private bool gameStarted;

    public GameObject playerPrefab;
    private GameObject player;

    private GameObject floor;
    private Spawner spawner;

    void Awake()
    {
        floor = GameObject.Find("Foreground");
        spawner = GameObject.Find("Spawner").GetComponent<Spawner>();
        timeManager = GetComponent<TimeManager>();
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
    }

    void ResetGame()
    {
        spawner.active = true;
        player = GameObjectUtil.Instantiate(playerPrefab, new Vector3(0, (Screen.height / PixelPerfectCamera.pixelsToUnit) / 2 + 100, 0));

        // Add the callback
        var playerDestroyScript = player.GetComponent<DestroyOffscreen>();
        playerDestroyScript.DestroyCallback += OnPlayerKilled;

        gameStarted = true;
    }
}
