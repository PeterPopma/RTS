using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Base : MonoBehaviour
{
    // Scale: 10..110  dissolveamount: 5.5...0.5
    [SerializeField] float health;
    [SerializeField] GameObject fire;
    Material fireMaterial;
    GameObject healthbar;
    [SerializeField] int playerNumber;
    float timeLeftShowHealth;
    float delayGameOver;
    LightFlicker lightFlicker;
    AudioSource soundExplosion;
    [SerializeField] private Transform vfxExplosion;

    public int PlayerNumber { get => playerNumber; set => playerNumber = value; }

    // Start is called before the first frame update
    void Start()
    {
        healthbar = Instantiate(Game.Instance.PfHealthbar);
        healthbar.transform.SetParent(Game.Instance.Canvas.transform, false);
        healthbar.name = "Health_" + gameObject.name;
        healthbar.transform.localScale = new Vector3(4,2,4);
        foreach (Transform child in healthbar.transform)
        {
            child.gameObject.SetActive(false);
        }

        fireMaterial = fire.GetComponent<Renderer>().material;
        health = 100;
        lightFlicker = fire.transform.Find("FireLight").GetComponent<LightFlicker>();
        soundExplosion = GameObject.Find("/Sound/Explosion").GetComponent<AudioSource>();
    }

    public void Damage(float amount)
    {
        health -= amount;

        timeLeftShowHealth = 1;

        if (health <= 0)
        {
            Game.Instance.SetGameState(GameState_.GameOver);
            Instantiate(vfxExplosion, transform.position, Quaternion.identity); 
            soundExplosion.Play();
            foreach (Transform child in healthbar.transform)
            {
                child.gameObject.SetActive(false);
            }
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Game.Instance.GameState != GameState_.Playing)
        {
            return;
        }
        if (timeLeftShowHealth > 0)
        {
            timeLeftShowHealth -= Time.deltaTime;
            foreach (Transform child in healthbar.transform)
            {
                child.gameObject.SetActive(true);
            }
        }
        else 
        { 
            foreach (Transform child in healthbar.transform)
            {
                child.gameObject.SetActive(false);
            } 
        }
        if (health < 100)
        {
            healthbar.GetComponent<Slider>().value = 100 - health;
            fire.SetActive(true);
            float scale = 10 + (100-health) * .5f;
            if (scale > 40)
            {
                scale = 40;
            }
            fire.transform.localScale = new Vector3(scale, scale, scale);
            fireMaterial.SetFloat("_DissolveAmount", 5.5f - ((100 - health) / 20));
        }
        else
        {
            fire.SetActive(false);
        }
        lightFlicker.MaxIntensity = (80 - health) * 50;

        // draw health bar
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0,0,-18));
        Vector2 canvasPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(Game.Instance.Canvas.GetComponent<RectTransform>(), screenPos, null, out canvasPos);

        if (screenPos.z > 0)
        {
            healthbar.SetActive(true);
            healthbar.GetComponent<RectTransform>().anchoredPosition = canvasPos;
        }
        else
        {
            healthbar.SetActive(false);
        }
    }
}
