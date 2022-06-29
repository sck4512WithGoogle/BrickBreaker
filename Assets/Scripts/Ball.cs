using UnityEngine;
using System.Collections;
using System;
using MJ.Data;

[DisallowMultipleComponent]
public sealed class Ball : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rigidBody;
    [SerializeField] private Collider2D myCollider;

    private static readonly float ShootForce = 7000f;
    private Transform myTransform;
    public bool IsMoving { get; set; } = false;
    

    private void Awake()
    {
        myTransform = transform;
        myCollider.enabled = false;
    }


    public void Shoot(Vector3 _Direction)
    {
        IsMoving = true;
        rigidBody.AddForce(_Direction * ShootForce);

        CoroutineExecuter.ExcuteAfterWaitTime(() => myCollider.enabled = true, 0.03f);
    }

    private void OnCollisionEnter2D(Collision2D _Other)
    {
        if(_Other.gameObject.CompareTag(Tags.BottomTag))
        {
            rigidBody.velocity = Vector2.zero;
            var ballPos = new Vector3(_Other.contacts[0].point.x, Constants.BottomY, myTransform.position.z);
            myTransform.position = ballPos; 
    
            GameManager.Instance.SetTotalBallPos(ballPos);

            StartCoroutine(MoveToTotalBallPos());
        }
        else if(IsMoving)
        {
            StartCoroutine(CheckVelocity());
        }
    }

    private IEnumerator CheckVelocity()
    {
        yield return null;
        var scalar = rigidBody.velocity.magnitude;

        var direction = rigidBody.velocity.normalized;

        if(Mathf.Abs(direction.x) >= 0.99f)
        {
            if(direction.x > 0)
            {
                direction.x -= 0.01f;
            }
            else
            {
                direction.x += 0.01f;
            }

            direction.y -= 0.01f;
            direction = direction.normalized;
          
            rigidBody.velocity = direction * scalar;
        }
    }

    private IEnumerator MoveToTotalBallPos()
    {
        var targetPos = GameManager.Instance.TotalBallPos;
        while (Vector3.Distance(myTransform.position, targetPos) > 0.12f)
        {
            myTransform.position = Vector3.MoveTowards(myTransform.position, targetPos, 4f);
            yield return null;
        }
        IsMoving = false;

        myCollider.enabled = false;
    }
}
