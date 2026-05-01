using UnityEngine;

public class CroonHole : MonoBehaviour
{
    //　クルーンの穴にボールが入ったときに、穴が占有されているかどうかを管理するスクリプト

    public int holeIndex; // この穴のインデックス
    public Bounder bounder; // Bounderスクリプトの参照

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger entered: " + other.gameObject.name + "Tag name:" + other.tag);
        
        if (other.CompareTag("Ball"))
        {
            Debug.Log("Ball entered hole " + holeIndex);
            // ボールがこの穴に入ったとき、BounderスクリプトのholeOccupied配列の対応するインデックスをtrueにする
            bounder.holeOccupied[holeIndex] = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            Debug.Log("Ball exited hole " + holeIndex);
            // ボールがこの穴から出たとき、BounderスクリプトのholeOccupied配列の対応するインデックスをfalseにする
            bounder.holeOccupied[holeIndex] = false;
        }
    }
}
