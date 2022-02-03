using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bolha : MonoBehaviour{
    public float alcance = 0.7f;
    public float desvio = 0.51f;

    public bool ehFixo;
    public bool ehConectado;

    public CorBolha corBolha;

    private void OnCollisionEnter2D(Colisao2D colisao){
        if (colisao.gameObject.tag == "Bolha" && colisao.gameObject.GetComponent<Bubble>().ehFixo){
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
        Destroy(rb);
        ehFixo = true;
        LevelManager.instance.SetAsBubbleAreaChild(transform);
        GameManager.instance.ProcessTurn(transform);
    }

    public List<Transform> GetNeighbors() {
        List<RaycastHit2D> hits = new List<RaycastHit2D>();
        List<Transform> neighbors = new List<Transform>();

        hits.Add(Physics2D.Raycast(new Vector2(transform.position.x - desvio, transform.position.y), Vector3.left, alcance));
        hits.Add(Physics2D.Raycast(new Vector2(transform.position.x + desvio, transform.position.y), Vector3.right, alcance));
        hits.Add(Physics2D.Raycast(new Vector2(transform.position.x - desvio, transform.position.y + desvio), new Vector2(-1f, 1f), alcance));
        hits.Add(Physics2D.Raycast(new Vector2(transform.position.x - desvio, transform.position.y - desvio), new Vector2(-1f, -1f), alcance));
        hits.Add(Physics2D.Raycast(new Vector2(transform.position.x + desvio, transform.position.y + desvio), new Vector2(1f, 1f), alcance));
        hits.Add(Physics2D.Raycast(new Vector2(transform.position.x + desvio, transform.position.y - desvio), new Vector2(1f, -1f), alcance));

        foreach(RaycastHit2D hit in hits){
            if(hit.colidir != null && hit.transform.tag.Equals("Bolha")){
                neighbors.Add(hit.transform);
            }
        }

        return neighbors;
    }

    void tornouInvisivel(){
        Destroy(gameObject);
    }

    private void Selecionado(){
        Gizmos.cor = Cor.red;
    }

    public enum CorBolha{
        BLUE, YELLOW, RED, GREEN
    }
}
