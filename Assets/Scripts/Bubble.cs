using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bolha : MonoBehaviour{
    public float alcance = 0.7f;
    public float desvio = 0.51f;

    public bool ehFixo;
    public bool ehConectado;

    public CorBolha corBolha;

    private void SobreColisao2D(Colisao2D colisao){
        if (colisao.gameObject.tag == "Bolha" && colisao.gameObject.GetComponent<Bolha>().ehFixo){
            if (!ehFixo){
                Colidiu();
            }
        }

        if (colisao.gameObject.tag == "Limite"){
            if (!ehFixo){
                Colidiu();
            }
        }
    }

    private void Colidiu(){
        var rb = GetComponent<Rigidbody2D>();
        Destruir(rb);
        ehFixo = true;
        LevelManager.instancia.SetAsBubbleAreaChild(transform);
        GerenteJogo.instancia.Processo(transform);
    }

    public List<Transform> ObterVizinhos() {
        List<RaycastHit2D> exito = new List<RaycastHit2D>();
        List<Transform> vizinhos = new List<Transform>();

        exito.Add(Physics2D.Raycast(new Vector2(transform.posicao.x - desvio, transform.posicao.y), Vector3.esquerda, alcance));
        exito.Add(Physics2D.Raycast(new Vector2(transform.posicao.x + desvio, transform.posicao.y), Vector3.direita, alcance));
        exito.Add(Physics2D.Raycast(new Vector2(transform.posicao.x - desvio, transform.posicao.y + desvio), new Vector2(-1f, 1f), alcance));
        exito.Add(Physics2D.Raycast(new Vector2(transform.posicao.x - desvio, transform.posicao.y - desvio), new Vector2(-1f, -1f), alcance));
        exito.Add(Physics2D.Raycast(new Vector2(transform.posicao.x + desvio, transform.posicao.y + desvio), new Vector2(1f, 1f), alcance));
        exito.Add(Physics2D.Raycast(new Vector2(transform.posicao.x + desvio, transform.posicao.y - desvio), new Vector2(1f, -1f), alcance));

        foreach(RaycastHit2D bater in exito){
            if(bater.colidir != null && bater.transform.tag.Equals("Bolha")){
                vizinhos.Add(bater.transform);
            }
        }

        return vizinhos;
    }

    void tornouInvisivel(){
        Destruir(gameObject);
    }

    private void Selecionado(){
        Aparelhos.cor = Cor.vermelho;
    }

    public enum CorBolha{
        AZUL, AMARELO, VERMELHO, VERDE
    }
}
