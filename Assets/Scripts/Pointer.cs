using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : MonoBehaviour
{
    //game manager
    public GameManager gameManager;
    void Start()
    {
        //find game manager
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        gameManager.target = collision.gameObject;
    }
}
