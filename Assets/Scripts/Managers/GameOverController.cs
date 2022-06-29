using UnityEngine;
using MJ.Data;
using UnityEngine.Events;


public class GameOverController : MonoBehaviour
{
    //[SerializeField] private UnityEvent onGameOver;
    [SerializeField] private Collider2D myCollider;
    [SerializeField] private GameObject resurrectionPopupUI;


    private void OnTriggerEnter2D(Collider2D _Other)
    {
        if(_Other.CompareTag(Tags.BlockTag))
        {
            //onGameOver.Invoke();
            //UnityEngine.SceneManagement.SceneManager.LoadScene(SceneNames.GameOverSceneName);
            resurrectionPopupUI.SetActive(true);
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
