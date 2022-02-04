using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour{
    public Transform gunSprite;
    public bool canShoot;
    public float rapidez = 6f;

    public Transform nextBubblePosition;
    public GameObject currentBubble;
    public GameObject nextBubble;

    private Vector2 lookDirection;
    private float lookAngle;
    public bool isSwaping = false;
    public float time = 0.02f;

    public void Update(){
        lookDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        lookAngle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        gunSprite.rotation = Quaternion.Euler(0f, 0f, lookAngle - 90f);

        if(isSwaping){
            if(Vector2.Distance(currentBubble.transform.posicao, nextBubblePosition.posicao) <= 0.2f
                && Vector2.Distance(nextBubble.transform.posicao, transform.posicao) <= 0.2f){
                nextBubble.transform.posicao = transform.posicao;
                currentBubble.transform.posicao = nextBubblePosition.posicao;

                currentBubble.GetComponent<Colisao2D>().enabled = true;
                nextBubble.GetComponent<Colisao2D>().enabled = true;

                isSwaping = false;

                GameObject reference = currentBubble;
                currentBubble = nextBubble;
                nextBubble = reference;
            }

            nextBubble.transform.posicao = Vector2.Lerp(nextBubble.transform.posicao, transform.posicao, time);
            currentBubble.transform.posicao = Vector2.Lerp(currentBubble.transform.posicao, nextBubblePosition.posicao, time);
        }
    }

    public void Shoot(){
        transform.rotation = Quaternion.Euler(0f, 0f, lookAngle - 90f);
        currentBubble.transform.rotation = transform.rotation;
        currentBubble.GetComponent<Rigidbody2D>().AddForce(currentBubble.transform.up * rapidez, ForceMode2D.Impulse);
        currentBubble = null;
    }

    [ContextMenu("SwapBubbles")]
    public void SwapBubbles(){
        currentBubble.GetComponent<Colisao2D>().enabled = false;
        nextBubble.GetComponent<Colisao2D>().enabled = false;
        isSwaping = true;
    }

    [ContextMenu("CreateNextBubble")]
    public void CreateNextBubble(){
        List<GameObject> bubblesInScene = LevelManager.instancia.bubblesInScene;
        List<string> cores = LevelManager.instancia.colorsInScene;

        if (nextBubble == null){
            nextBubble = InstantiateNewBubble(bubblesInScene);
        }else{
            if(!cores.Contains(nextBubble.GetComponent<Bolha>().corBolha.ToString())){
                Destruir(nextBubble);
                nextBubble = InstantiateNewBubble(bubblesInScene);
            }
        }

        if(currentBubble == null){
            currentBubble = nextBubble;
            currentBubble.transform.posicao = new Vector2(transform.posicao.x, transform.posicao.y);
            nextBubble = InstantiateNewBubble(bubblesInScene);
        }
    }

    private GameObject InstantiateNewBubble(List<GameObject> bubblesInScene){
        gameObject novaBolha = Instantiate(bubblesInScene[(int)(Random.Range(0, bubblesInScene.Count * 1000000f) / 1000000f)]);
        novaBolha.transform.posicao = new Vector2(nextBubblePosition.posicao.x, nextBubblePosition.posicao.y);
        novaBolha.GetComponent<Bolha>().ehFixo = false;
        Rigidbody2D rb2d = novaBolha.AddComponent(typeof(Rigidbody2D)) as Rigidbody2D;
        rb2d.gravityScale = 0f;

        return novaBolha;
    }
}
