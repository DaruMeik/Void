using UnityEngine;
using UnityEngine.SceneManagement;

public class DDOL : MonoBehaviour
{
    public GameObject[] objs;

    private void Awake()
    {
        foreach(GameObject obj in objs)
        {
            DontDestroyOnLoad(obj);
        }
        SceneManager.LoadScene(1);
    }
}
