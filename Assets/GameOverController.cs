using UnityEngine;
using MJ.Data;
using UnityEngine.Events;
using MJ.Data;

public class GameOverController : MonoBehaviour
{
    //[SerializeField] private UnityEvent onGameOver;
    [SerializeField] private Collider2D myCollider;

    private void OnTriggerEnter2D(Collider2D _Other)
    {
        if(_Other.CompareTag(Tags.BlockTag))
        {
            //onGameOver.Invoke();
            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneNames.GameOverSceneName);
            myCollider.enabled = false;
        }
    }
}
