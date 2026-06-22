using System;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public static Manager Instance;

    public static event Action<int> OnPuntiCambiati;
    public static event Action OnErroriCambiati;
    public static event Action OnPartitaFinita;

    [Header("Regole del livello")]
    public Regole regole;

    [Header("Stato partita")]
    public int punti = 0;
    public int erroriColore = 0;
    public int erroriMancato = 0;
    public int totalePallonciniTarget = 0;
    public int PallonciniNoTarget = 0;
    public float timer = 0;
    public bool partitaFinita = false;
    public bool partitaIniziata = false; 

    [Header("Componenti audio")]
    public AudioSource audioSource;
    public AudioClip suonoColpito;

    [Header("Effetti visivi")]
    public GameObject effettoScoppio;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        timer = regole.tempoMax;
        SalvataggioDati.Instance?.ImpostaManager(this);
    }

    void Update()
    {
        if (!partitaIniziata) return;
        if (partitaFinita) return;

        timer -= Time.deltaTime;

        if (timer <= 0)
            FinePartita();
    }
    public void IniziaPartita()
    {
        partitaIniziata = true;
    }

    public void PalloncinoDistrutto(ColorePalloncino colore, EsitoPalloncino esito, Vector3 posizione)
    {
        if (partitaFinita) return;

        if (esito == EsitoPalloncino.COLPITO)
        {
            PlayScoppio(posizione);

            if (regole.modalitaPunteggio == ModalitaPunteggio.TOTALI)
            {
                punti++;
                OnPuntiCambiati?.Invoke(punti);
            }
            else if (regole.modalitaPunteggio == ModalitaPunteggio.COLORE)
            {
                if (colore == regole.coloreDaColpire)
                {
                    punti++;
                    OnPuntiCambiati?.Invoke(punti);
                }
                else
                {
                    erroriColore++;
                    OnErroriCambiati?.Invoke();
                }
            }
        }
        else if (esito == EsitoPalloncino.MANCATO)
        {
            if (regole.modalitaPunteggio == ModalitaPunteggio.TOTALI)
            {
                erroriMancato++;
                OnErroriCambiati?.Invoke();
            }
            else if (regole.modalitaPunteggio == ModalitaPunteggio.COLORE)
            {
                if (colore == regole.coloreDaColpire)
                {
                    erroriMancato++;
                    OnErroriCambiati?.Invoke();
                }
            }
        }

        Debug.Log($"Palloncino {colore}, Stato: {esito}");
    }

    public void FinePartita()
    {
        partitaFinita = true;
        OnPartitaFinita?.Invoke();
        Debug.Log("Partita finita");
    }

    public void AggiungiPalloncinoTarget()
    {
        totalePallonciniTarget++;
    }

    public void AggiungiPalloncinoNoTarget()
    {
        PallonciniNoTarget++;
    }

    public void PlayScoppio(Vector3 posizione)
    {
        audioSource.PlayOneShot(suonoColpito);
        Instantiate(effettoScoppio, posizione, Quaternion.identity);
    }
}