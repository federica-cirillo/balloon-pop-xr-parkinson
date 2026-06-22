using UnityEngine;

// Contiene tutte le regole configurabili del livello (da Inspector)
// La partita finisce sempre a TEMPO
// Definisce come vengono assegnati i punti (TOTALI o COLORE)
// Gli errori sono sempre attivi:
//   TOTALI -> ogni palloncino mancato è errore
//   COLORE -> colpire colore sbagliato O mancare il colore target sono entrambi errori

[CreateAssetMenu(menuName = "Regole")]
public class Regole : ScriptableObject
{
    [Header("Come sono assegnati i punti")]
    public ModalitaPunteggio modalitaPunteggio;

    [Header("Colore da colpire (valido solo in modalità COLORE)")]
    public ColorePalloncino coloreDaColpire;

    [Header("Durata della partita (secondi)")]
    public float tempoMax = 0;

    [Header("Task cognitivo")]
    public bool taskCognitivo = false;
    public string[] operazioni;

    [Header("Regole palloncino")]
    public float intervalloSpawn = 6;
    public float velocita = 0.35f;
    public float altezzaMax = 3f;
}

public enum ModalitaPunteggio
{
    TOTALI, // ogni palloncino colpito vale un punto
    COLORE  // solo i palloncini del colore target valgono un punto
}


