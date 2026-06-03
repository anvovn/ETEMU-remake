using UnityEngine;

public class EricRotate : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(10f * Time.deltaTime, 50f * Time.deltaTime, 0f);
    }
}
