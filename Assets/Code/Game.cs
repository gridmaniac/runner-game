using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    [SerializeField]
    Player player;

    [SerializeField]
    Transform g;

    [SerializeField]
    Transform f;

    [SerializeField]
    Transform o;

    public GameObject[] groundPrefabs;
    List<GameObject> grounds;

    public GameObject[] foregroundPrefabs;
    List<GameObject> foregrounds;

    public GameObject[] obstaclePrefabs;
    List<GameObject> obstacles;

    public GameObject[] coinPrefabs;

    float gW = 8.0f;
    float gX = -10.0f;
    int gNum = 4;

    float fW = 20.0f;
    int fNum = 3;

    int oNum = 3;
    float oOffset = 0.0f;
    float oDMin = 8.0f;
    float oDMax = 15.0f;

    float baseSpeed = 8.0f;
    float maxSpeed = 13.0f;

    float speed;
    float speedScoreRange = 1000.0f;

    AudioSource audioSource;

    public AudioSource[] music;
    int musicLevel;
    float musicVolume = 0.2f;

    public SpriteRenderer[] backgrounds;
    int backgroundLevel;

    int[] obstacleLevels = { 3, 5 };
    int obstacleLevel;

    [SerializeField]
    Text scoreText;

    [SerializeField]
    GameObject restartUI;

    [SerializeField]
    Text recordText;

    [SerializeField]
    GameObject tutorialUI;

    float score;
    float[] scoreLevels = { 500.0f };
    float scoreCounter;

    bool isStarted = false;

    // Start is called before the first frame update
    void Start()
    {
        player.OnCoinCollected += CoinCollected;
        player.OnHurted += GameOver;

        audioSource = GetComponent<AudioSource>();

        if (!PlayerPrefs.HasKey("Record"))
            PlayerPrefs.SetInt("Record", 0);

        recordText.text = PlayerPrefs.GetInt("Record").ToString();
        restartUI.SetActive(true);
    }

    public void StartGame()
    {
        player.gameObject.SetActive(true);

        speed = baseSpeed;
        backgroundLevel = 0;
        musicLevel = 0;
        obstacleLevel = 0;
        score = 0.0f;
        scoreCounter = 0.0f;

        music[musicLevel].Play();
        ResetBackground();

        player.isHuring = false;

        foreach (var x in music)
            x.volume = musicVolume;

        if (grounds != null)
            foreach (var x in grounds)
                Destroy(x.gameObject);

        if (foregrounds != null)
            foreach (var x in foregrounds)
                Destroy(x.gameObject);

        if (obstacles != null)
            foreach (var x in obstacles)
                Destroy(x.gameObject);

        grounds = new List<GameObject>();

        for (int i = 0; i < gNum; i++)
        {
            var go = new GameObject();
            go.transform.position = new Vector2(gX + gW * i, 0.0f);
            go.transform.SetParent(g);

            var ground = Instantiate(groundPrefabs[Random.Range(0, groundPrefabs.Length)]);
            ground.transform.SetParent(go.transform);
            ground.transform.localPosition = Vector2.zero;

            grounds.Add(go);
        }

        foregrounds = new List<GameObject>();

        for (int i = 0; i < fNum; i++)
        {
            var go = new GameObject();
            go.transform.position = new Vector2(i * fW, 0.0f);
            go.transform.SetParent(f);

            var foreground = Instantiate(foregroundPrefabs[Random.Range(0, foregroundPrefabs.Length)]);
            foreground.transform.SetParent(go.transform);
            foreground.transform.localPosition = Vector2.zero;

            foregrounds.Add(go);
        }

        obstacles = new List<GameObject>();

        for (int i = 0; i < oNum; i++)
        {
            oOffset = i == 0 ? 15.0f : obstacles[i - 1].transform.position.x;

            var go = new GameObject();
            go.transform.position = new Vector2(oOffset + Random.Range(oDMin, oDMax), 0.0f);
            go.transform.SetParent(o);

            var obstacle = Instantiate(obstaclePrefabs[Random.Range(0, obstacleLevels[obstacleLevel])]);
            obstacle.transform.SetParent(go.transform);
            obstacle.transform.localPosition = Vector2.zero;

            obstacles.Add(go);
        }

        restartUI.SetActive(false);
        isStarted = true;

        tutorialUI.SetActive(false);
        tutorialUI.SetActive(true);
    }

    void CoinCollected()
    {
        score += 100.0f;
    }

    void GameOver()
    {
        foreach (var x in music)
            x.Stop();

        isStarted = false;

        int record = PlayerPrefs.GetInt("Record");
        
        if (score > record)
            PlayerPrefs.SetInt("Record", (int)score);

        recordText.text = PlayerPrefs.GetInt("Record").ToString();
        restartUI.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isStarted)
            return;

        for (int i = 0; i < grounds.Count; i++)
        {
            grounds[i].transform.Translate(Vector2.left * speed * Time.deltaTime);

            if (grounds[i].transform.position.x < gX - gW)
            {
                int index = i > 0 ? i - 1 : grounds.Count - 1;
                grounds[i].transform.position = grounds[index].transform.position + new Vector3(gW - 0.2f, 0.0f);

                Destroy(grounds[i].transform.GetChild(0).gameObject);

                var ground = Instantiate(groundPrefabs[Random.Range(0, groundPrefabs.Length)]);
                ground.transform.SetParent(grounds[i].transform);
                ground.transform.localPosition = Vector2.zero;
            }
        }

        for (int i = 0; i < foregrounds.Count; i++)
        {
            foregrounds[i].transform.Translate(Vector2.left * speed * 0.05f * Time.deltaTime);

            if (foregrounds[i].transform.position.x < -fW)
            {
                int index = i > 0 ? i - 1 : foregrounds.Count - 1;
                foregrounds[i].transform.position = foregrounds[index].transform.position + new Vector3(fW, 0.0f);

                Destroy(foregrounds[i].transform.GetChild(0).gameObject);

                var foreground = Instantiate(foregroundPrefabs[Random.Range(0, foregroundPrefabs.Length)]);
                foreground.transform.SetParent(foregrounds[i].transform);
                foreground.transform.localPosition = Vector2.zero;
            }
        }

        for (int i = 0; i < obstacles.Count; i++)
        {
            obstacles[i].transform.Translate(Vector2.left * speed * Time.deltaTime);

            if (obstacles[i].transform.position.x < -15.0f)
            {
                int index = i > 0 ? i - 1 : obstacles.Count - 1;
                obstacles[i].transform.position = obstacles[index].transform.position + new Vector3(Random.Range(oDMin, oDMax), 0.0f);

                if (obstacles[i].transform.childCount > 0)
                    Destroy(obstacles[i].transform.GetChild(0).gameObject);

                GameObject prefab = null;
                if (Random.value < 0.2f)
                    prefab = coinPrefabs[Random.Range(0, coinPrefabs.Length)];
                else
                    prefab = obstaclePrefabs[Random.Range(0, obstacleLevels[obstacleLevel])];

                var obstacle = Instantiate(prefab);
                obstacle.transform.SetParent(obstacles[i].transform);
                obstacle.transform.localPosition = Vector2.zero;
            }
        }

        AdjustByScore();
        BlendBackgrounds();
        BlendMusic();
    }

    void BlendMusic()
    {
        if (musicLevel > 0)
        {
            music[musicLevel - 1].volume = Mathf.Lerp(music[musicLevel - 1].volume, 0.0f, Time.deltaTime * 0.2f);
            if (music[musicLevel - 1].volume < musicVolume * 0.5f)
                music[musicLevel - 1].Stop();
            else if (!music[musicLevel - 1].isPlaying)
                music[musicLevel - 1].Play();

            if (music[musicLevel - 1].volume >= musicVolume * 0.5f)
                return;

        }

        music[musicLevel].volume = Mathf.Lerp(music[musicLevel].volume, musicVolume, Time.deltaTime * 0.2f);
        if (music[musicLevel].volume < musicVolume * 0.5f)
            music[musicLevel].Stop();
        else if (!music[musicLevel].isPlaying)
            music[musicLevel].Play();
    }

    void BlendBackgrounds()
    {
        if (backgroundLevel > 0)
        {
            var o = backgrounds[backgroundLevel - 1].color;
            backgrounds[backgroundLevel - 1].color = new Color(o.r, o.g, o.b, Mathf.Lerp(o.a, 0.0f, Time.deltaTime * 0.2f));
        }

        var n = backgrounds[backgroundLevel].color;
        backgrounds[backgroundLevel].color = new Color(n.r, n.g, n.b, Mathf.Lerp(n.a, 1.0f, Time.deltaTime * 0.2f));
    }

    void ResetBackground()
    {
        var c = Color.white;

        for (int i = 0; i < backgrounds.Length; i++)
        {
            if (i == backgroundLevel)
                backgrounds[i].color = new Color(c.r, c.g, c.b, 1.0f);
            else
                backgrounds[i].color = new Color(c.r, c.g, c.b, 0.0f);
        }
    }

    void AdjustByScore()
    {
        speed = baseSpeed + (maxSpeed - baseSpeed) * (Mathf.Clamp(score, 0.0f, speedScoreRange) / speedScoreRange);

        for (int i = 0; i < scoreLevels.Length; i++)
        {
            if (score > scoreLevels[i])
            {
                backgroundLevel = i + 1;
                obstacleLevel = i + 1;

                if (scoreCounter < scoreLevels[i])
                {
                    scoreCounter = score;
                    audioSource.Play();
                }
            }
        }

        if (score > speedScoreRange)
            musicLevel = 1;
    }

    void FixedUpdate()
    {
        if (!isStarted)
            return;

        score += speed * 0.05f;
        scoreText.text = "Score: " + ((int)(score)).ToString();
    }
}
