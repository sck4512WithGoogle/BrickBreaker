using UnityEngine;
using MJ.Data;
using UnityEngine.Events;


public class GameOverTrigger : MonoBehaviour
{
    [SerializeField] private UnityEvent onGameOver;
    [SerializeField] private Collider2D myCollider;



    private void OnTriggerEnter2D(Collider2D _Other)
    {
        if(_Other.CompareTag(Tags.BlockTag))
        {
            onGameOver.Invoke();
            myCollider.enabled = false;
        }
    }

    public void OnClickViewAds()
    {

    }

    public void OnClickExit()
    {
        Time.timeScale = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene(SceneNames.GameOverSceneName);
    }
}
