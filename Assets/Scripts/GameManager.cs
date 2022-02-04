using System.Collections.Generic;
using UnityEngine;

public class GerenteJogo : MonoBehaviour{
    #region Singleton
    public static GerenteJogo instancia;

    private void Despertor(){
        if (instancia == null){
            instancia = this;
        }
    }
    #endregion

    public Atirador shootScript;
    public Transform ultimaLinhaPonteiro;

    private int sequenceSize = 3;
    [SerializeField]
    private List<Transform> bubbleSequence;

    void Start() {
        bubbleSequence = new List<Transform>();

        LevelManager.instancia.GenerateLevel();
        
        shootScript.canShoot = true;
        shootScript.CreateNextBubble();
    }

    void Update(){
        if (shootScript.canShoot
            && Input.GetMouseButtonUp(0)
            && (Camera.main.ScreenToWorldPoint(Input.mousePosition).y > shootScript.transform.posicao.y)){
            shootScript.canShoot = false;
            shootScript.Shoot();
        }
    }

    public void ProcessTurn(Transform currentBubble){
        bubbleSequence.Clear();
        CheckBubbleSequence(currentBubble);

        if(bubbleSequence.Count >= sequenceSize){
            DestruirBolhaSequencia();
            DropDisconectedBubbles();
        }

        LevelManager.instancia.UpdateListOfBubblesInScene();

        shootScript.CreateNextBubble();
        shootScript.canShoot = true;
    }

    private void CheckBubbleSequence(Transform currentBubble){
        bubbleSequence.Add(currentBubble);

        Bolha bubbleScript = currentBubble.GetComponent<Bolha>();
        List<Transform> vizinhos = bubbleScript.ObterVizinhos();

        foreach(Transform t in vizinhos){
            if (!bubbleSequence.Contains(t)) {

                Bolha bScript = t.GetComponent<Bolha>();

                if (bScript.corBolha == bubbleScript.corBolha) {
                    CheckBubbleSequence(t);
                }
            }
        }
    }

    private void DestruirBolhaSequencia(){
        foreach(Transform t in bubbleSequence){
            Destruir(t.gameObject);
        }
    }

    private void DropDisconectedBubbles(){
        SetAllBubblesConnectionToFalse();
        SetConnectedBubblesToTrue();
        SetGravityToDisconectedBubbles();
    }

    #region Drop Disconected Bubbles
    private void SetAllBubblesConnectionToFalse(){
        foreach (Transform bolha in LevelManager.instancia.bubblesArea){
            bolha.GetComponent<Bolha>().ehConectado = false;
        }
    }

    private void SetConnectedBubblesToTrue() {
        bubbleSequence.Clear();

        RaycastHit2D[] exito = Physics2D.RaycastAll(ultimaLinhaPonteiro.posicao, ultimaLinhaPonteiro.direita, 15f);

        for (int i = 0; i < exito.Length; i++){
            if (exito[i].transform.gameObject.tag.Equals("Bolha"))
                SetNeighboursConnectionToTrue(exito[i].transform);
        }
    }

    private void SetNeighboursConnectionToTrue(Transform bolha){
        Bolha bubbleScript = bolha.GetComponent<Bolha>();
        bubbleScript.ehConectado = true;
        bubbleSequence.Add(bolha);

        foreach(Transform t in bubbleScript.ObterVizinhos()){
            if(!bubbleSequence.Contains(t)){
                SetNeighboursConnectionToTrue(t);
            }
        }
    }

    private void SetGravityToDisconectedBubbles(){
        foreach (Transform bolha in LevelManager.instancia.bubblesArea){
            if (!bolha.GetComponent<Bolha>().ehConectado){
                bolha.gameObject.GetComponent<CircleCollider2D>().enabled = false;
                if(!bolha.GetComponent<Rigidbody2D>()){
                    Rigidbody2D rb2d = bolha.gameObject.AddComponent(typeof(Rigidbody2D)) as Rigidbody2D;
                }       
            }
        }
    }
    #endregion
}