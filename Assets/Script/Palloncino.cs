using UnityEngine;

// Gestisce il comportamento di un singolo palloncino
// Imposta il colore e il materiale
// Fa muovere il palloncino verso l'alto
// Gestisce il colpo del giocatore
// Gestisce suono ed effetto scoppio con il proprio colore
// Comunica al Manager se è stato colpito o mancato
// Distrugge il palloncino quando esce dallo schermo o viene colpito

public class Palloncino : MonoBehaviour
{
    public ColorePalloncino colore;

    [Header("Lista di materiali diversi per il palloncino")]
    public Material[] materiali;
    [Header("MeshRenderer del componente di cui si vuole cambiare colore")]
    public MeshRenderer meshRenderer;

    private float altezzaMax;
    private float velocita;

    private Manager manager;

    public bool initialized = false;
    private bool preso = false;

    public void Initialize(Manager _manager)
    {
        manager = _manager;
        velocita = manager.regole.velocita;
        altezzaMax = manager.regole.altezzaMax;
        colore = (ColorePalloncino)Random.Range(0, materiali.Length);
        meshRenderer.material = SelectMaterial(colore);
        initialized = true;
    }

    public void Initialize(Manager _manager, ColorePalloncino colorePalloncino)
    {
        manager = _manager;
        velocita = manager.regole.velocita;
        altezzaMax = manager.regole.altezzaMax;
        colore = colorePalloncino;
        meshRenderer.material = SelectMaterial(colore);
        initialized = true;
    }

    void Start()
    {
        if (initialized) return;

        colore = (ColorePalloncino)Random.Range(0, materiali.Length);
        meshRenderer.material = SelectMaterial(colore);
    }

    void Update()
    {
        if (manager.partitaFinita)
        {
            Destroy(gameObject);
            return;
        }

        transform.position += new Vector3(0, velocita * Time.deltaTime, 0);

        if (transform.position.y >= altezzaMax)
            Distruggi(EsitoPalloncino.MANCATO);
    }

    public void Colpisci()
    {
        if (preso) return;
        Distruggi(EsitoPalloncino.COLPITO);
    }

    void Distruggi(EsitoPalloncino esito)
    {
        manager.PalloncinoDistrutto(colore, esito, transform.position);
        Destroy(gameObject);
    }

    public Material SelectMaterial(ColorePalloncino tipo)
    {
        switch (tipo)
        {
            case ColorePalloncino.GIALLO: return materiali[0];
            case ColorePalloncino.BLU: return materiali[1];
            case ColorePalloncino.ROSSO: return materiali[2];
            case ColorePalloncino.VIOLA: return materiali[3];
            case ColorePalloncino.VERDE: return materiali[4];
            default: return materiali[0];
        }
    }
}

public enum ColorePalloncino
{
    GIALLO,
    BLU,
    ROSSO,
    VIOLA,
    VERDE
}

public enum EsitoPalloncino
{
    COLPITO,
    MANCATO,
}

