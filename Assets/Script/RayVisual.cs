using UnityEngine;
using Oculus.Interaction;

// Aggiorna due LineRenderer ogni frame per simulare un effetto laser azzurro
// - lineRendererCore: linea sottile azzurro chiaro (il nucleo del laser)
// - lineRendererGlow: linea spessa azzurro semitrasparente (il bagliore intorno)
// Va aggiunto sullo stesso GameObject che ha i LineRenderer (HandRayInteractor)

public class RayVisual : MonoBehaviour
{
    [Header("Riferimenti")]
    public RayInteractor rayInteractor;

    [Header("Laser Core (bianco)")]
    public LineRenderer lineRendererCore;

    [Header("Laser Glow (azzurro semitrasparente)")]
    public LineRenderer lineRendererGlow;

    void Awake()
    {
        lineRendererCore.positionCount = 2;
        lineRendererGlow.positionCount = 2;

        lineRendererCore.enabled = false;
        lineRendererGlow.enabled = false;
    }

    void Update()
    {
        if (rayInteractor == null || !rayInteractor.isActiveAndEnabled)
        {
            lineRendererCore.enabled = false;
            lineRendererGlow.enabled = false;
            return;
        }

        Vector3 origine = rayInteractor.Origin;
        Vector3 destinazione;

        if (rayInteractor.CollisionInfo.HasValue)
        {
            destinazione = rayInteractor.CollisionInfo.Value.Point;
        }
        else
        {
            destinazione = origine + rayInteractor.Forward * rayInteractor.MaxRayLength;
        }

        lineRendererCore.enabled = true;
        lineRendererCore.SetPosition(0, origine);
        lineRendererCore.SetPosition(1, destinazione);

        lineRendererGlow.enabled = true;
        lineRendererGlow.SetPosition(0, origine);
        lineRendererGlow.SetPosition(1, destinazione);
    }
}

