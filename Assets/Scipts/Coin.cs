using System;
using TMPro;
using UnityEngine;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class Coin : MonoBehaviour
{
    public AudioClip coinClip;
    public int coinToGive = 1;
    public TextMeshProUGUI coinText;

    private void Start()
    {
        coinText = GameObject.FindWithTag("CoinText").GetComponent<TextMeshProUGUI>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Player player = collision.gameObject.GetComponent<Player>();
            player.coins += coinToGive;
            player.PlaySFX(coinClip, 0.4f);
            coinText.text = player.coins.ToString();
            Destroy(gameObject);
        }
    }
}
