using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO; 

[System.Serializable]
public class DadosJogo
{
    public int recordeGlobal;
}

public class JogoManager : MonoBehaviour
{
    public GameObject cartaPrefab;
    public Transform tabuleiro;
    public Sprite[] spritesCartas;
    
    [Header("UI")]
    public TextMeshProUGUI textoPontos;
    public TextMeshProUGUI textoRecorde;
    public GameObject painelVitoria;

    private int pontos;
    private int combo = 1;
    private int paresEncontrados;
    private int recorde;

    public bool podeClicar = true;
    private Carta primeiraCarta;
    private Carta segundaCarta;

    private string caminhoArquivo;

    void Awake()
    {
        caminhoArquivo = Application.persistentDataPath + "/dados_jogo.json";
    }

    void Start()
    {
        CarregarDados();
        GerarJogo();
    }

    void SalvarDados()
    {
        DadosJogo dados = new DadosJogo();
        dados.recordeGlobal = recorde;

        string json = JsonUtility.ToJson(dados, true); 
        File.WriteAllText(caminhoArquivo, json); 
        Debug.Log("Jogo Salvo em: " + caminhoArquivo);
    }
    public void ReiniciarRecorde()
    {
        recorde = 0;
        textoRecorde.text = "Recorde: 0";
    
        SalvarDados(); 
    
        Debug.Log("Recorde resetado com sucesso!");
    }

    void CarregarDados()
    {
        if (File.Exists(caminhoArquivo))
        {
            string json = File.ReadAllText(caminhoArquivo);
            DadosJogo dados = JsonUtility.FromJson<DadosJogo>(json);
            recorde = dados.recordeGlobal;
        }
        else
        {
            recorde = 0;
        }
        textoRecorde.text = "Recorde: " + recorde;
    }

    public void SairDoJogo()
{
    Debug.Log("Saindo do jogo...");

    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
}

    void GerarJogo()
    {
        foreach (Transform child in tabuleiro) Destroy(child.gameObject);
        paresEncontrados = 0;
        pontos = 0;
        combo = 1;
        AtualizarUI();

        List<int> ids = new List<int>();
        for (int i = 0; i < 5; i++) { ids.Add(i); ids.Add(i); }

        for (int i = 0; i < ids.Count; i++)
        {
            int temp = ids[i];
            int randomIndex = Random.Range(i, ids.Count);
            ids[i] = ids[randomIndex];
            ids[randomIndex] = temp;
        }

        foreach (int id in ids)
        {
            GameObject novaCarta = Instantiate(cartaPrefab, tabuleiro);
            novaCarta.GetComponent<Carta>().Setup(id, spritesCartas[id], this);
        }
    }

    public void CartaRevelada(Carta carta)
    {
        if (primeiraCarta == null) { primeiraCarta = carta; }
        else { segundaCarta = carta; StartCoroutine(VerificarPar()); }
    }

    IEnumerator VerificarPar()
    {
        podeClicar = false;
        if (primeiraCarta.idTipo == segundaCarta.idTipo)
        {
            pontos += 100 * combo;
            combo++;
            paresEncontrados++;
            primeiraCarta = null;
            segundaCarta = null;
            if (paresEncontrados == 5) Vitoria();
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            combo = 1; 
            primeiraCarta.Esconder();
            segundaCarta.Esconder();
            primeiraCarta = null;
            segundaCarta = null;
        }
        AtualizarUI();
        podeClicar = true;
    }

    void AtualizarUI() => textoPontos.text = "Pontos: " + pontos;

    void Vitoria()
    {
        painelVitoria.SetActive(true);
        if (pontos > recorde)
        {
            recorde = pontos;
            SalvarDados(); 
            textoRecorde.text = "Recorde: " + recorde;
        }
    }

    public void ReiniciarJogo() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
}