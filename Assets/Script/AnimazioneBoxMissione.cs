using System.Collections;
using UnityEngine;

// Script su BOX_MISSIONE
// Mostra la BOX_MISSIONE al centro per 10 secondi poi la fa scorrere verso l'alto
// Solo quando l'animazione è finita avvisa il Manager di iniziare

public class AnimazioneBoxMissione : MonoBehaviour
{
    [Header("Riferimenti")]
    public Manager manager;

    [Header("Box Punteggio (disattivata finché la box missione non è in alto)")]
    public GameObject boxPunteggio;

    [Header("Posizioni (coordinate locali RectTransform)")]
    public float posizioneCentroY = 0f;
    public float posizioneAltoY = 400f;

    [Header("Tempi")]
    public float tempoAttesa = 10f;
    public float tempoAnimazione = 1.5f;

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        // Posiziona al centro all'avvio
        Vector2 pos = rectTransform.anchoredPosition;
        pos.y = posizioneCentroY;
        rectTransform.anchoredPosition = pos;

        // BoxTimer disattivata all'inizio
        if (boxPunteggio != null)
            boxPunteggio.SetActive(false);

        StartCoroutine(AnimazioneBox());
    }

    private IEnumerator AnimazioneBox()
    {
        // Aspetta al centro
        yield return new WaitForSeconds(tempoAttesa);

        // Anima verso l'alto
        float tempo = 0f;
        Vector2 posizioneInizio = rectTransform.anchoredPosition; // posizione attuale
        Vector2 posizioneFine = new Vector2(posizioneInizio.x, posizioneAltoY); // posizione finale

        while (tempo < tempoAnimazione)
        {
            tempo += Time.deltaTime; // incrementa tempo
            float t = Mathf.SmoothStep(0f, 1f, tempo / tempoAnimazione); //  SmoothStep per un movimento fluido
            rectTransform.anchoredPosition = Vector2.Lerp(posizioneInizio, posizioneFine, t); // aggiorna posizione
            yield return null;
        }

        rectTransform.anchoredPosition = posizioneFine;

        // Box arrivata in alto: attiva BoxTimer e avvisa il Manager
        if (boxPunteggio != null)
            boxPunteggio.SetActive(true);

        manager.IniziaPartita();
    }

}