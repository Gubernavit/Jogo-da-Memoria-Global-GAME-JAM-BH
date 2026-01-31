using UnityEngine;
using UnityEngine.UI;

public class Carta : MonoBehaviour
{
    public int idTipo; 
    public Image imagemFrente;
    public GameObject verso;
    private JogoManager manager;

    public void Setup(int id, Sprite spriteFrente, JogoManager m)
    {
        idTipo = id;
        imagemFrente.sprite = spriteFrente;
        manager = m;
        Esconder();
    }

    public void AoClicar()
    {
        
        if (verso.activeSelf && manager.podeClicar)
        {
            Revelar();
            manager.CartaRevelada(this);
        }
    }

    public void Revelar() => verso.SetActive(false);
    public void Esconder() => verso.SetActive(true);
}