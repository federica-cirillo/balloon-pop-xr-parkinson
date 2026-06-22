using System.Collections;
using UnityEngine;

// Genera palloncini a intervalli regolari di tempo
// Aspetta che partitaIniziata sia true prima di iniziare lo spawn
// Continua a spawnare finché la partita non è finita

public class PalloncinoSpawn : MonoBehaviour
{
    public Palloncino palloncino;
    private Manager manager;

    void Start()
    {
        manager = Manager.Instance;
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        // Aspetta che BoxMissione abbia finito l'animazione
        yield return new WaitUntil(() => manager.partitaIniziata);

        while (!manager.partitaFinita)
        {
            Spawn();
            yield return new WaitForSeconds(manager.regole.intervalloSpawn);
        }
    }

    public void Spawn()
    {
        Vector3 posizione = new Vector3(0, 0, 3);
        Palloncino palloncinoSpawnato = Instantiate(palloncino, posizione, Quaternion.identity);
        palloncinoSpawnato.Initialize(manager);

        Debug.Log("Ecco un nuovo palloncino");

        if (manager.regole.modalitaPunteggio == ModalitaPunteggio.TOTALI)
        {
            manager.AggiungiPalloncinoTarget();
        }

        if (manager.regole.modalitaPunteggio == ModalitaPunteggio.COLORE)
        {
            if (palloncinoSpawnato.colore == manager.regole.coloreDaColpire)
                manager.AggiungiPalloncinoTarget();
            else
                manager.AggiungiPalloncinoNoTarget();
        }
    }
}
