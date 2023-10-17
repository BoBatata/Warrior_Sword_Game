using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] private Image lifeImage;
    [SerializeField] private TextMeshProUGUI scoreText;
    private int playerLifes;
    private float playerScore;

    private void Awake()
    {
        #region Singleton

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        #endregion
    }

    private void Update()
    {
        playerLifes = PlayerBehavior.instance.GetPlayerHealth();
        HealthDamage(playerLifes);
    }

    private void HealthDamage(int playerCurrentLife)
    {
        if (playerCurrentLife == 3)
        {
            lifeImage.fillAmount = 1f;
        }
        else if (playerCurrentLife == 2)
        {
            lifeImage.fillAmount = 0.65f;
        }
        else if (playerCurrentLife == 1)
        {
            lifeImage.fillAmount = 0.33f;
        }
        else
        {
            lifeImage.fillAmount = 0;
        }
    }

    public void CurrentScore(int updateScore)
    {
        scoreText.text = "Score: " + updateScore;
    }
}
