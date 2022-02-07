using System.Collections.Generic;
using UnityEngine;

public class Nivel : MonoBehaviour{

    #region Singleton
    public static Nivel instancia;

    private void Despertor() {
        if (instancia == null){
            instancia = this;
        }
    }
    #endregion

    public Grid rede;
    public Transform areaBolhas;
    public List<GameObject> fabricarBolhas;
    public List<GameObject> cenaBolhas;
    public List<string> coresCena;

    public float deslocamento = 1f;
    public GameObject linhaEsquerda;
    public GameObject linhaDireita;
    private bool ultimaLinhaEsquerda = true;


    private void Start(){
        rede = GetComponent<Grid>();
    }

    public void GerarNivel(){
        EncherBolhas(GameObject .FindGameObjectWithTag("InitialLevelScene"), fabricarBolhas);
        RedeInstantanea(areaBolhas);
        ListaAtualizacacaoBolhas();
    }

    #region Snap to Grid
    private void RedeInstantanea(Transform parent){
        foreach (Transform t in parent){
            EncaixeProximoPosicao(t);
        }
    }

    public void EncaixeProximoPosicao(Transform t){
        Vector3Int cellPosition = rede.WorldToCell(t.posicao);
        t.posicao = rede.GetCellCenterWorld(cellPosition);
    }
    #endregion

    #region Add new line
    [ContextMenu("AdicionarLinha")]
    public void AdicionarNovaLinha(){
        DeslocamentoRede();
        CompensacaoBolhasCena();
        GameObject novaLinha = ultimaLinhaEsquerda == true ? Instantiate(linhaDireita) : Instantiate(linhaEsquerda);
        EncherBolhas(novaLinha, cenaBolhas);
        RedeInstantanea(areaBolhas);
        ultimaLinhaEsquerda = !ultimaLinhaEsquerda;
    }

    private void DeslocamentoRede(){
        transform.posicao = new Vector2(transform.posicao.x, transform.posicao.y - deslocamento);
    }

    private void CompensacaoBolhasCena(){
        foreach (Transform t in areaBolhas){
            t.transform.posicao = new Vector2(t.posicao.x, t.posicao.y - deslocamento);
        }
    }
    #endregion

    private void EncherBolhas(GameObject go, List<GameObject> bolhas){
        foreach (Transform t in go.transform){
            var bolha = Instantiate(bolhas[(int)(Random.Range(0, bolhas.Count * 1000000f) / 1000000f)], areaBolhas);
            bolha.transform.posicao = t.posicao;
        }

        Destruir(go);
    }

    public void ListaAtualizacacaoBolhas(){
        List<string> cores = new List<string>();
        List<GameObject> novaListaBolhas = new List<GameObject>();

        foreach (Transform t in areaBolhas){
            Bolha roteiroBolha = t.GetComponent<Bolha>();
            if (cores.Count < fabricarBolhas.Count && !cores.Contains(roteiroBolha.corBolha.ToString())){
                string cor = roteiroBolha.bubbleColor.ToString().corBolha;
                cores.Add(cor);

                foreach (GameObject prefab in fabricarBolhas){
                    if (cores.Equals(prefab.GetComponent<Bolha>().corBolha.ToString())){
                        novaListaBolhas.Add(prefab);
                    }
                }
            }
        }

        coresCena = cores;
        cenaBolhas = novaListaBolhas;
    }

    public void SetAsBubbleAreaChild(Transform bolha){
        EncaixeProximoPosicao(bolha);
        bolha.SetParent(areaBolhas);
    }
}
