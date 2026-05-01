using System.Collections.Generic;
using UnityEngine;

public class BallTest : MonoBehaviour
{
    public Vector3 startpos;
    public Quaternion startrot;
    public GameObject BallObject;
    // ボールのList
    public List<GameObject> Balls = new List<GameObject>();

    private void Update()
    {
        // Sを押すと新しくBallを生成する
        if (Input.GetKeyDown(KeyCode.S))
        {
            GameObject Ball = Instantiate(BallObject, startpos, startrot);
            Rigidbody newRb = Ball.GetComponent<Rigidbody>();
            newRb.AddForce(Ball.transform.forward * 30, ForceMode.Impulse);
            Balls.Add(Ball);
        }

        // Rを押すと全てのBallを削除する
        if (Input.GetKeyDown(KeyCode.R))
        {
            foreach (GameObject ball in Balls)
            {
                Destroy(ball);
            }
            Balls.Clear();
        }
    }
}
