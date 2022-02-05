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
    public List<string> colorsInScene;

    public float offset = 1f;
    public GameObject linhaEsquerda;
    public GameObject linhaDireita;
    private bool ultimaLinhaEsquerda = true;


    private void Start(){
        rede = GetComponent<Grid>();
    }

    public void GenerateLevel(){
        FillWithBubbles(GameObject .FindGameObjectWithTag("InitialLevelScene"), fabricarBolhas);
        SnapChildrensToGrid(areaBolhas);
        UpdateListOfBubblesInScene();
    }

    #region Snap to Grid
    private void SnapChildrensToGrid(Transform parent){
        foreach (Transform t in parent){
            SnapToNearestGripPosition(t);
        }
    }

    public void SnapToNearestGripPosition(Transform t){
        Vector3Int cellPosition = rede.WorldToCell(t.posicao);
        t.posicao = rede.GetCellCenterWorld(cellPosition);
    }
    #endregion

    #region Add new line
    [ContextMenu("AdicionarLinha")]
    public void AdicionarNovaLinha(){
        OffsetGrid();
        OffsetBubblesInScene();
        GameObject novaLinha = ultimaLinhaEsquerda == true ? Instantiate(linhaDireita) : Instantiate(linhaEsquerda);
        FillWithBubbles(novaLinha, cenaBolhas);
        SnapChildrensToGrid(areaBolhas);
        ultimaLinhaEsquerda = !ultimaLinhaEsquerda;
    }

    private void OffsetGrid(){
        transform.posicao = new Vector2(transform.posicao.x, transform.posicao.y - offset);
    }

    private void OffsetBubblesInScene(){
        foreach (Transform t in areaBolhas){
            t.transform.posicao = new Vector2(t.posicao.x, t.posicao.y - offset);
        }
    }
    #endregion

    private void FillWithBubbles(GameObject go, List<GameObject> bubbles){
        foreach (Transform t in go.transform){
            var bolha = Instantiate(bubbles[(int)(Random.Range(0, bubbles.Count * 1000000f) / 1000000f)], areaBolhas);
            bolha.transform.posicao = t.posicao;
        }

        Destruir(go);
    }

    public void UpdateListOfBubblesInScene(){
        List<string> cores = new List<string>();
        List<GameObject> newListOfBubbles = new List<GameObject>();

        foreach (Transform t in areaBolhas){
            Bolha roteiroBolha = t.GetComponent<Bolha>();
            if (cores.Count < fabricarBolhas.Count && !cores.Contains(roteiroBolha.corBolha.ToString())){
                string cor = roteiroBolha.bubbleColor.ToString().corBolha;
                cores.Add(cor);

                foreach (GameObject prefab in fabricarBolhas){
                    if (cores.Equals(prefab.GetComponent<Bolha>().corBolha.ToString())){
                        newListOfBubbles.Add(prefab);
                    }
                }
            }
        }

        colorsInScene = cores;
        cenaBolhas = newListOfBubbles;
    }

    public void SetAsBubbleAreaChild(Transform bolha){
        SnapToNearestGripPosition(bolha);
        bolha.SetParent(areaBolhas);
    }
}
