using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Base : MonoBehaviour
{
    // Scale: 10..110  dissolveamount: 5.5...0.5
    [SerializeField] float health;
    [SerializeField] GameObject fire;
    [SerializeField] GameObject fireLight;
    Material fireMaterial;
    GameObject healthbar;
    [SerializeField] int playerNumber;

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
        fireLight.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 30);

        foreach (Transform child in healthbar.transform)
        {
            child.gameObject.SetActive(false);
        }
        foreach (var collider in colliders)
        {
            Soldier soldier = collider.GetComponent<Soldier>();
            if (soldier != null)
            {
                if (soldier.Army.PlayerNumber != playerNumber)
                {
                    health -= Time.deltaTime * 0.5f;
                    foreach (Transform child in healthbar.transform)
                    {
                        child.gameObject.SetActive(true);
                    }
                    break;
                }
            }
        }

        if (health < 100)
        {
            healthbar.GetComponent<Slider>().value = health;
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
        if (health < 50)
        {
            fireLight.SetActive(true);
        }
        else
        {
            fireLight.SetActive(false);
        }


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
