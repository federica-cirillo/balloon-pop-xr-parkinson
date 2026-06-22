using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SalvataggioDati : MonoBehaviour
{
    public static SalvataggioDati Instance;

    [SerializeField] private Manager manager;

    // Credenziali email 
    private const string MITTENTE = "PallonciniParkinson@gmail.com";
    private const string APP_PASSWORD = "eztvryqyqatiovxs"; // con spazi rimossi
    private const string DESTINATARIO = "parkinsonapp.dati@proton.me";

    private class DatiSessione
    {
        public string livello;
        public string modalita;
        public float durataLivello;
        // TOTALI
        public int pallonciniColpiti;
        public int pallonciniTotaliGenerati;
        public float percentualeColpiti;
        // COLORE
        public string coloreTarget;
        public int targetColpiti;
        public int targetTotaliGenerati;
        public float percentualeTargetColpiti;
        public int colpitiColoreErrato;
        public int noTargetTotaliGenerati;
        public float percentualeErroreColore;
    }

    private List<DatiSessione> sessioniAccumulate = new List<DatiSessione>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        Manager.OnPartitaFinita += AccumulaDati;
    }

    void OnDisable()
    {
        Manager.OnPartitaFinita -= AccumulaDati;
    }

    public void ImpostaManager(Manager nuovoManager)
    {
        manager = nuovoManager;
    }

    private void AccumulaDati()
    {
        if (manager == null)
        {
            Debug.LogWarning("SalvataggioDati: Manager non assegnato.");
            return;
        }

        float durata = manager.regole.tempoMax - Mathf.Max(0, manager.timer);

        DatiSessione s = new DatiSessione
        {
            livello = SceneManager.GetActiveScene().name,
            durataLivello = (float)Math.Round(durata, 1)
        };

        if (manager.regole.modalitaPunteggio == ModalitaPunteggio.TOTALI)
        {
            int totGen = manager.totalePallonciniTarget;
            int colpiti = manager.punti;
            float perc = totGen > 0 ? (float)colpiti / totGen * 100f : 0f;

            s.modalita = "TOTALI";
            s.pallonciniColpiti = colpiti;
            s.pallonciniTotaliGenerati = totGen;
            s.percentualeColpiti = (float)Math.Round(perc, 1);
        }
        else
        {
            int targetGen = manager.totalePallonciniTarget;
            int targetCol = manager.punti;
            float percT = targetGen > 0 ? (float)targetCol / targetGen * 100f : 0f;
            int noTargetGen = manager.PallonciniNoTarget;
            int errCol = manager.erroriColore;
            float percE = noTargetGen > 0 ? (float)errCol / noTargetGen * 100f : 0f;

            s.modalita = "COLORE";
            s.coloreTarget = manager.regole.coloreDaColpire.ToString();
            s.targetColpiti = targetCol;
            s.targetTotaliGenerati = targetGen;
            s.percentualeTargetColpiti = (float)Math.Round(percT, 1);
            s.colpitiColoreErrato = errCol;
            s.noTargetTotaliGenerati = noTargetGen;
            s.percentualeErroreColore = (float)Math.Round(percE, 1);
        }

        sessioniAccumulate.Add(s);
        Debug.Log($"Sessione accumulata in memoria ({sessioniAccumulate.Count} totali)");
    }

    // Chiamato dal bottone "Fine"
    public void EsportaDati()
    {
        if (sessioniAccumulate.Count == 0)
        {
            Debug.LogWarning("Nessuna sessione da esportare.");
            return;
        }

        DateTime oraEsportazione = DateTime.Now;
        string nomeFile = $"PARTITA_{oraEsportazione:dd-MM-yyyy_HH-mm-ss}.json";

        // Salva il file localmente
        string cartella;
#if UNITY_ANDROID && !UNITY_EDITOR
        cartella = "/sdcard/Documents/ParkinsonApp/";
#else
        cartella = Application.persistentDataPath + "/";
#endif
        if (!Directory.Exists(cartella))
            Directory.CreateDirectory(cartella);

        string percorso = Path.Combine(cartella, nomeFile);
        ScriviFile(percorso, oraEsportazione);

        Debug.Log($"File salvato in: {percorso}");

        // Invia il file via email
        StartCoroutine(InviaEmail(percorso, nomeFile, oraEsportazione));

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{percorso.Replace("/", "\\")}\"");
#endif
    }

    private IEnumerator InviaEmail(string percorsoFile, string nomeFile, DateTime ora)
    {
        // Piccola attesa per assicurarsi che il file sia stato scritto
        yield return new WaitForSeconds(0.5f);

        try
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(MITTENTE);
            mail.To.Add(DESTINATARIO);
            mail.Subject = $"ParkinsonApp - Dati partita {ora:dd/MM/yyyy HH:mm}";
            mail.Body = $"In allegato i dati della sessione del {ora:dd/MM/yyyy} alle {ora:HH:mm}.";

            // Allega il file JSON
            Attachment allegato = new Attachment(percorsoFile);
            mail.Attachments.Add(allegato);

            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.Credentials = new NetworkCredential(MITTENTE, APP_PASSWORD);
            smtp.EnableSsl = true;

            smtp.Send(mail);
            mail.Dispose();

            Debug.Log("Email inviata con successo!");
        }
        catch (Exception e)
        {
            Debug.LogError($"Errore invio email: {e.Message}");
        }
    }

    private void ScriviFile(string percorso, DateTime oraEsportazione)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("{");
        sb.AppendLine($"    \"Data\": \"{oraEsportazione:dd/MM/yyyy}\",");
        sb.AppendLine($"    \"Ora\": \"{oraEsportazione:HH:mm:ss}\",");
        sb.AppendLine($"    \"ID Utente\": \"\",");
        sb.AppendLine($"    \"Eta\": \"\",");
        sb.AppendLine($"    \"Mano Dominante\": \"\",");
        sb.AppendLine("     \"Dati Partita\": [");

        for (int i = 0; i < sessioniAccumulate.Count; i++)
        {
            string blocco = CostruisciBloccoSessione(sessioniAccumulate[i]);
            string[] righe = blocco.Split('\n');
            foreach (string riga in righe)
                if (!string.IsNullOrWhiteSpace(riga))
                    sb.AppendLine("        " + riga.Trim());

            if (i < sessioniAccumulate.Count - 1)
                sb.AppendLine("        ,");
        }

        sb.AppendLine("    ]");
        sb.AppendLine("}");

        File.WriteAllText(percorso, sb.ToString());
    }

    private string CostruisciBloccoSessione(DatiSessione s)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("{");
        sb.AppendLine($"    \"Livello\": \"{s.livello}\",");
        sb.AppendLine($"    \"Palloncini da colpire\": \"{s.modalita}\",");
        sb.AppendLine($"    \"Durata (sec)\": {s.durataLivello},");

        if (s.modalita == "TOTALI")
        {
            sb.AppendLine($"    \"Palloncini Colpiti\": {s.pallonciniColpiti},");
            sb.AppendLine($"    \"Totale Palloncini Generati\": {s.pallonciniTotaliGenerati},");
            sb.AppendLine($"    \"Percentuale Palloncini Colpiti\": {s.percentualeColpiti}");
        }
        else
        {
            sb.AppendLine($"    \"Colore da Colpire\": \"{s.coloreTarget}\",");
            sb.AppendLine($"    \"Palloncini {s.coloreTarget} Colpiti\": {s.targetColpiti},");
            sb.AppendLine($"    \"Totale Palloncini {s.coloreTarget} Generati\": {s.targetTotaliGenerati},");
            sb.AppendLine($"    \"Percentuale Palloncini {s.coloreTarget} Colpiti\": {s.percentualeTargetColpiti},");
            sb.AppendLine($"    \"Palloncini NO {s.coloreTarget} Colpiti\": {s.colpitiColoreErrato},");
            sb.AppendLine($"    \"Totale Palloncini NO {s.coloreTarget} Generati\": {s.noTargetTotaliGenerati},");
            sb.AppendLine($"    \"Percentuale Palloncini NO {s.coloreTarget} Colpiti\": {s.percentualeErroreColore}");
        }

        sb.Append("}");
        return sb.ToString();
    }
}