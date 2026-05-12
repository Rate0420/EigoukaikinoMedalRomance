using UnityEngine;

public class Croon : MonoBehaviour
{
    public float rps = 0.1f;
    float timer;
    [SerializeField] bool zRotation;
    void Update()
    {
        timer += Time.deltaTime;
        if (zRotation)
        {
            transform.Rotate(0, 0, rps * Time.deltaTime * 360);
        }
        else
        {
            transform.Rotate(0, rps * Time.deltaTime * 360, 0);
        }
        // 5•b‰ñ“]A2•b’â~A‹t‰ñ“]A2•b’â~‚ğŒJ‚è•Ô‚·
        //if (timer < 5)
        //{
        //    transform.Rotate(0, rps * Time.deltaTime * 360, 0);
        //}
        //else if (timer < 7)
        //{
        //    // ’â~
        //}
        //else if (timer < 12)
        //{
        //    transform.Rotate(0, -rps * Time.deltaTime * 360, 0);
        //}
        //else if (timer < 14)
        //{
        //    // ’â~
        //}
        //else
        //{
        //    timer = 0;
        //}
    }
}
