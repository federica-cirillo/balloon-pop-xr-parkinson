using TMPro;
using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Manager manager;

    [Header("Testi")]
    public TextMeshProUGUI testoMissione;
    public TextMeshProUGUI testoTimer;
    public TextMeshProUGUI testoColpiti;
    public TextMeshProUGUI testoReport;
    public TextMeshProUGUI testoOperazione;

    [Header("Panel")]
    public GameObject PanelPartitaInCorso;
    public GameObject PanelPartitaFinita;

    [Header("Box Operazione")]
    public GameObject BoxOperazione;

    [Header("Colori palloncini")]
    public Material[] materialiPalloncino;

    private ColorePalloncino colore;
    private Coroutine coroutineOperazioni;

    void Start()
    {
        PanelPartitaFinita.SetActive(false);
        BoxOperazione.SetActive(false);
        testoTimer.text = FormatTimer(manager.regole.tempoMax);
        testoColpiti.text = "0";

        AggiornaMissione();

        Manager.OnPuntiCambiati += AggiornaPunti;
        Manager.OnPartitaFinita += AggiornaUIFinale;
        Manager.OnPartitaFinita += StopOperazioni;

        // Avvia la coroutine che aspetta partitaIniziata prima di attivare BoxOperazione
        if (manager.regole.taskCognitivo && manager.regole.operazioni != null && manager.regole.operazioni.Length > 0)
            StartCoroutine(AttivaCognitivoAlInizio());
    }

    // Aspetta che la partita inizi (BoxMissione salita) poi attiva BoxOperazione e avvia le operazioni
    private IEnumerator AttivaCognitivoAlInizio()
    {
        yield return new WaitUntil(() => manager.partitaIniziata);

        BoxOperazione.SetActive(true);
        coroutineOperazioni = StartCoroutine(MostraOperazioni(manager.regole.operazioni));
    }

    private string FormatTimer(float t)
    {
        int minuti = (int)(t / 60);
        int secondi = (int)(t % 60);
        return $"{minuti:00}:{secondi:00}";
    }

    void Update()
    {
        if (!manager.partitaFinita)
            testoTimer.text = FormatTimer(Mathf.Max(0, manager.timer));
    }

    void OnDestroy()
    {
        Manager.OnPuntiCambiati -= AggiornaPunti;
        Manager.OnPartitaFinita -= AggiornaUIFinale;
        Manager.OnPartitaFinita -= StopOperazioni;
    }

    private void AggiornaPunti(int nuoviPunti)
    {
        testoColpiti.text = "" + nuoviPunti;
    }

    private void AggiornaMissione()
    {
        if (manager.regole.modalitaPunteggio == ModalitaPunteggio.TOTALI)
        {
            testoMissione.text = "COLPISCI I PALLONCINI";
            if (manager.regole.taskCognitivo)
                testoMissione.text += " E RISOLVI LE OPERAZIONI";
        }
        else if (manager.regole.modalitaPunteggio == ModalitaPunteggio.COLORE)
        {
            colore = manager.regole.coloreDaColpire;
            testoMissione.text = $"COLPISCI I PALLONCINI DI COLORE <b><color=#{ColoreDaMateriale(colore)}>{colore}</color></b>";
        }
    }

    void AggiornaUIFinale()
    {
        PanelPartitaInCorso.SetActive(false);
        PanelPartitaFinita.SetActive(true);

        int mancati = manager.totalePallonciniTarget - manager.punti;

        if (manager.regole.modalitaPunteggio == ModalitaPunteggio.TOTALI)
            testoReport.text = "PALLONCINI COLPITI: " + manager.punti + "\nPALLONCINI MANCATI: " + mancati;

        if (manager.regole.modalitaPunteggio == ModalitaPunteggio.COLORE)
            testoReport.text = $"COLPITI DI COLORE {colore}: {manager.punti}\nMANCATI DI COLORE {colore}: {mancati}\nCOLPITI DI COLORE NON {colore}: {manager.erroriColore}";
    }

    private IEnumerator MostraOperazioni(string[] operazioni)
    {
        float timerOperazione = manager.regole.tempoMax / operazioni.Length;

        for (int i = 0; i < operazioni.Length; i++)
        {
            testoOperazione.text = "" + operazioni[i];
            yield return new WaitForSeconds(timerOperazione);
        }

        testoOperazione.text = "";
    }

    private void StopOperazioni()
    {
        if (coroutineOperazioni != null)
        {
            StopCoroutine(coroutineOperazioni);
            testoOperazione.text = "";
        }
    }

    private string ColoreDaMateriale(ColorePalloncino colore)
    {
        Color c = materialiPalloncino[(int)colore].color;
        return ColorUtility.ToHtmlStringRGB(c);
    }
}








