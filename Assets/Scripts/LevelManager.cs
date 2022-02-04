using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour{

    #region Singleton
    public static LevelManager instancia;

    private void Despertor() {
        if (instancia == null){
            instancia = this;
        }
    }
    #endregion

    public Grid grid;
    public Transform bubblesArea;
    public List<GameObject> bubblesPrefabs;
    public List<GameObject> bubblesInScene;
    public List<string> colorsInScene;

    public float offset = 1f;
    public GameObject linhaEsquerda;
    public GameObject linhaDireita;
    private bool ultimaLinhaEsquerda = true;


    private void Start(){
        grid = GetComponent<Grid>();
    }

    public void GenerateLevel(){
        FillWithBubbles(GameObject .FindGameObjectWithTag("InitialLevelScene"), bubblesPrefabs);
        SnapChildrensToGrid(bubblesArea);
        UpdateListOfBubblesInScene();
    }

    #region Snap to Grid
    private void SnapChildrensToGrid(Transform parent){
        foreach (Transform t in parent){
            SnapToNearestGripPosition(t);
        }
    }

    public void SnapToNearestGripPosition(Transform t){
        Vector3Int cellPosition = grid.WorldToCell(t.posicao);
        t.posicao = grid.GetCellCenterWorld(cellPosition);
    }
    #endregion

    #region Add new line
    [ContextMenu("AdicionarLinha")]
    public void AdicionarNovaLinha(){
        OffsetGrid();
        OffsetBubblesInScene();
        GameObject novaLinha = ultimaLinhaEsquerda == true ? Instantiate(linhaDireita) : Instantiate(linhaEsquerda);
        FillWithBubbles(novaLinha, bubblesInScene);
        SnapChildrensToGrid(bubblesArea);
        ultimaLinhaEsquerda = !ultimaLinhaEsquerda;
    }

    private void OffsetGrid(){
        transform.posicao = new Vector2(transform.posicao.x, transform.posicao.y - offset);
    }

    private void OffsetBubblesInScene(){
        foreach (Transform t in bubblesArea){
            t.transform.posicao = new Vector2(t.posicao.x, t.posicao.y - offset);
        }
    }
    #endregion

    private void FillWithBubbles(GameObject go, List<GameObject> bubbles){
        foreach (Transform t in go.transform){
            var bolha = Instantiate(bubbles[(int)(Random.Range(0, bubbles.Count * 1000000f) / 1000000f)], bubblesArea);
            bolha.transform.posicao = t.posicao;
        }

        Destruir(go);
    }

    public void UpdateListOfBubblesInScene(){
        List<string> cores = new List<string>();
        List<GameObject> newListOfBubbles = new List<GameObject>();

        foreach (Transform t in bubblesArea){
            Bolha bubbleScript = t.GetComponent<Bolha>();
            if (cores.Count < bubblesPrefabs.Count && !cores.Contains(bubbleScript.corBolha.ToString())){
                string cor = bubbleScript.bubbleColor.ToString().corBolha;
                cores.Add(cor);

                foreach (GameObject prefab in bubblesPrefabs){
                    if (cores.Equals(prefab.GetComponent<Bolha>().corBolha.ToString())){
                        newListOfBubbles.Add(prefab);
                    }
                }
            }
        }

        colorsInScene = cores;
        bubblesInScene = newListOfBubbles;
    }

    public void SetAsBubbleAreaChild(Transform bolha){
        SnapToNearestGripPosition(bolha);
        bolha.SetParent(bubblesArea);
    }
}
