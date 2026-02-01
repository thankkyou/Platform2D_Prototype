using System;
using Unity.VisualScripting;
using UnityEngine;

public class MaDa : Enemy
{
    [SerializeField] private float chaseDistance;
    [SerializeField] private float stunDuration;
    float timer;

    protected override void Start()
    {
        
        base.Start();
        ChangeState(EnemyStates.MaDa_Idle);
    }

    protected override void Update()
    {
        base.Update();
        if (!PlayerController.Instance.pState.alive)
        {
            ChangeState(EnemyStates.MaDa_Idle);
        }
        
    }

    protected override void UpdateEnemyStates()
    {
        float _dist = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);
        switch (GetCurrentEnemyState)
        {
            case EnemyStates.MaDa_Idle:
                if (_dist < chaseDistance)
                {
                    rb.linearVelocity = new Vector2(0, 0);
                    ChangeState(EnemyStates.MaDa_Chase);
                }
                break;

            case EnemyStates.MaDa_Chase:
                rb.MovePosition(Vector2.MoveTowards(transform.position, PlayerController.Instance.transform.position, Time.deltaTime * speed));
                FlipMaDa();
                if(_dist > chaseDistance)
                {
                    ChangeState(EnemyStates.MaDa_Idle);
                }
                break;
            
            case EnemyStates.MaDa_Stunned:
                timer += Time.deltaTime;

                if (timer > stunDuration)
                {
                    ChangeState(EnemyStates.MaDa_Idle);
                    timer = 0;
                }
                break;
            
            case EnemyStates.MaDa_Death:
                Death(1);
                break;
        }
    }

    public override void EnemyHit(float _damageDone)
    {
        base.EnemyHit(_damageDone);

        if (health > 0)
        {
            ChangeState(EnemyStates.MaDa_Stunned);
        }
        else
        {
            ChangeState(EnemyStates.MaDa_Death);
        }
    }

    protected override void Death(float _destroyTime)
    {
        rb.gravityScale = 12;
        base.Death(_destroyTime);
    }

    protected override void ChangeCurrentAnimation()
    {
        anim.SetBool("Idle", GetCurrentEnemyState == EnemyStates.MaDa_Idle);
        anim.SetBool("Chase", GetCurrentEnemyState == EnemyStates.MaDa_Chase);
        anim.SetBool("Stunned", GetCurrentEnemyState == EnemyStates.MaDa_Stunned);

        if (GetCurrentEnemyState == EnemyStates.MaDa_Death)
        {
            anim.SetTrigger("Death");
        }
    }

    void FlipMaDa()
    {
        sr.flipX = PlayerController.Instance.transform.position.x < transform.position.x;
    }

}
