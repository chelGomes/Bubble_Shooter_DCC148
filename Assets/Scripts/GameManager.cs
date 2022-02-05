﻿using System.Collections.Generic;
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

    public Atirador atirarRoteiro;
    public Transform ultimaLinhaPonteiro;

    private int tamanho = 3;
    [SerializeField]
    private List<Transform> sequenciaBolha;

    void Start() {
        sequenciaBolha = new List<Transform>();

        LevelManager.instancia.GenerateLevel();
        
        atirarRoteiro.possoAtirar = true;
        atirarRoteiro.CriarProximaBolha();
    }

    void Update(){
        if (atirarRoteiro.possoAtirar
            && Input.GetMouseButtonUp(0)
            && (Camera.main.ScreenToWorldPoint(Input.posicaoMouse).y > atirarRoteiro.transform.posicao.y)){
            atirarRoteiro.possoAtirar = false;
            atirarRoteiro.Roteiro();
        }
    }

    public void ProcessTurn(Transform currentBubble){
        sequenciaBolha.Clear();
        BolhaVerficacao(currentBubble);

        if(sequenciaBolha.Count >= tamanho){
            DestruirBolhaSequencia();
            DropDisconectedBubbles();
        }

        LevelManager.instancia.UpdateListOfBubblesInScene();

        atirarRoteiro.CriarProximaBolha();
        atirarRoteiro.possoAtirar = true;
    }

    private void BolhaVerficacao(Transform currentBubble){
        sequenciaBolha.Add(currentBubble);

        Bolha bubbleScript = currentBubble.GetComponent<Bolha>();
        List<Transform> vizinhos = bubbleScript.ObterVizinhos();

        foreach(Transform t in vizinhos){
            if (!sequenciaBolha.Contains(t)) {

                Bolha bScript = t.GetComponent<Bolha>();

                if (bScript.corBolha == bubbleScript.corBolha) {
                    BolhaVerficacao(t);
                }
            }
        }
    }

    private void DestruirBolhaSequencia(){
        foreach(Transform t in sequenciaBolha){
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
        sequenciaBolha.Clear();

        RaycastHit2D[] exito = Physics2D.RaycastAll(ultimaLinhaPonteiro.posicao, ultimaLinhaPonteiro.direita, 15f);

        for (int i = 0; i < exito.Length; i++){
            if (exito[i].transform.gameObject.tag.Equals("Bolha"))
                SetNeighboursConnectionToTrue(exito[i].transform);
        }
    }

    private void SetNeighboursConnectionToTrue(Transform bolha){
        Bolha bubbleScript = bolha.GetComponent<Bolha>();
        bubbleScript.ehConectado = true;
        sequenciaBolha.Add(bolha);

        foreach(Transform t in bubbleScript.ObterVizinhos()){
            if(!sequenciaBolha.Contains(t)){
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