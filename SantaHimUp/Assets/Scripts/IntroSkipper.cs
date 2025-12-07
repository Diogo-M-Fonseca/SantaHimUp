using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroSkipper : MonoBehaviour
{

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
