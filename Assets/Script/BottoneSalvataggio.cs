using UnityEngine;
using UnityEngine.UI;

// Gestisce il bottone di salvataggio a fine partita
// Al click: nasconde il panel corrente ed esporta i dati della sessione via email
// Collegato allo script SalvataggioDati che accumula i dati di tutti i livelli giocati

// Non è possibile usare OnClick dall'Inspector perché SalvataggioDati usa DontDestroyOnLoad e viene creato nella prima scena (non viene mantenuto il riferimento nelle scene successive)
// SalvataggioDati.Instance trova automaticamente l'istanza corretta tramite il Singleton
public class BottoneSalvataggio : MonoBehaviour
{
    public GameObject panel;
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            panel.SetActive(false);
            SalvataggioDati.Instance?.EsportaDati();
        });
    }
}