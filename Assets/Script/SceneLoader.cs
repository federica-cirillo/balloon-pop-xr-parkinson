using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void CaricaScena(string nomeScena)
    {
        SceneManager.LoadScene(nomeScena);
    }

}