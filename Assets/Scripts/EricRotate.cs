using UnityEngine;

public class EricRotate : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0f, 50f * Time.deltaTime, 0f);
    }
}
