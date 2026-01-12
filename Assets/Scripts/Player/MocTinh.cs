using UnityEngine;

public class MocTinh : Enemy
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb.gravityScale = 12f;
    }
    protected override void Awake()
    {
        base.Awake(); 
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override void EnemyHit(float _damageDone)
    {
        base.EnemyHit(_damageDone);
    }
}
