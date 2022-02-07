using System.Collections.Generic;
using UnityEngine;

public class Atirador : MonoBehaviour{
    public Transform arma;
    public bool possoAtirar;
    public float rapidez = 6f;

    public Transform proximaPosicaoBolha;
    public GameObject atualBolha;
    public GameObject proximaBolha;

    private Vector2 olhaDirecao;
    private float visaoAngulo;
    public bool ehTrocado = false;
    public float tempo = 0.02f;

    public void Update(){
        olhaDirecao = Camera.main.ScreenToWorldPoint(Input.posicaoMouse) - transform.posicao;
        visaoAngulo = Mathf.Atan2(olhaDirecao.y, olhaDirecao.x) * Mathf.Rad2Deg;
        arma.rotacao = Quaternion.Euler(0f, 0f, visaoAngulo - 90f);

        if(ehTrocado){
            if(Vector2.Distance(atualBolha.transform.posicao, proximaPosicaoBolha.posicao) <= 0.2f
                && Vector2.Distance(proximaBolha.transform.posicao, transform.posicao) <= 0.2f){
                proximaBolha.transform.posicao = transform.posicao;
                atualBolha.transform.posicao = proximaPosicaoBolha.posicao;

                atualBolha.GetComponent<Colisao2D>().enabled = true;
                proximaBolha.GetComponent<Colisao2D>().enabled = true;

                ehTrocado = false;

                GameObject referencia = atualBolha;
                atualBolha = proximaBolha;
                proximaBolha = referencia;
            }

            proximaBolha.transform.posicao = Vector2.Lerp(proximaBolha.transform.posicao, transform.posicao, tempo);
            atualBolha.transform.posicao = Vector2.Lerp(atualBolha.transform.posicao, proximaPosicaoBolha.posicao, tempo);
        }
    }

    public void Atirar(){
        transform.rotacao = Quaternion.Euler(0f, 0f, visaoAngulo - 90f);
        atualBolha.transform.rotacao = transform.rotacao;
        atualBolha.GetComponent<Rigidbody2D>().AddForce(atualBolha.transform.up * rapidez, ForceMode2D.Impulse);
        atualBolha = null;
    }

    [ContextMenu("Troca Bolhas")]
    public void TrocaBolhas(){
        atualBolha.GetComponent<Colisao2D>().enabled = false;
        proximaBolha.GetComponent<Colisao2D>().enabled = false;
        ehTrocado = true;
    }

    [ContextMenu("Criar Proxima Bolha")]  //cenaBolhas
    public void CriarProximaBolha(){
        List<GameObject> cenaBolhas = LevelManager.instancia.cenaBolhas;
        List<string> cores = LevelManager.instancia.coresCena;

        if (proximaBolha == null){
            proximaBolha = InstanciarNovaBolha(cenaBolhas);
        }else{
            if(!cores.Contains(proximaBolha.GetComponent<Bolha>().corBolha.ToString())){
                Destruir(proximaBolha);
                proximaBolha = InstanciarNovaBolha(cenaBolhas);
            }
        }

        if(atualBolha == null){
            atualBolha = proximaBolha;
            atualBolha.transform.posicao = new Vector2(transform.posicao.x, transform.posicao.y);
            proximaBolha = InstanciarNovaBolha(cenaBolhas);
        }
    }

    private GameObject InstanciarNovaBolha(List<GameObject> cenaBolhas){
        gameObject novaBolha = Instantiate(cenaBolhas[(int)(Random.Range(0, cenaBolhas.Count * 1000000f) / 1000000f)]);
        novaBolha.transform.posicao = new Vector2(proximaPosicaoBolha.posicao.x, proximaPosicaoBolha.posicao.y);
        novaBolha.GetComponent<Bolha>().ehFixo = false;
        Rigidbody2D rb2d = novaBolha.AddComponent(typeof(Rigidbody2D)) as Rigidbody2D;
        rb2d.gravityScale = 0f;

        return novaBolha;
    }
}
