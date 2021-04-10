using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class swape : MonoBehaviour
{
    public GameObject current;
    public GameObject next;
    public Transform currentPlayer;
    public Transform nextPlayer;

    // Start is called before the first frame update

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab)) swapeCharactor();
    }
    void swapeCharactor()
    {
        nextPlayer.position = currentPlayer.position;
        nextPlayer.rotation = currentPlayer.rotation;
        current.SetActive(false);
        next.SetActive(true);
    }
}
