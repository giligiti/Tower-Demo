using UnityEngine;
using UnityEngine.Rendering;

public class Main : MonoBehaviour
{
    [SerializeField] MonsterFactory bruteFactory;
    [SerializeField] EffectFactory effectFactory;
    [SerializeField] BulletFactory bulletFactory;
    public Vector3 dir;
    public bool isFire = false;
    GameObject eff;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject obj = bruteFactory.Create(Vector3.zero, Quaternion.identity);
        //eff = effectFactory.Create(Vector2.zero, Vector3.forward);
    }

    // Update is called once per frame
    void Update()
    {
        if (isFire)
        {
            isFire = false;
        }
    }


}
