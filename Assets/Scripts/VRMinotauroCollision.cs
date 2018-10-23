using UnityEngine;
using System.Collections;

public class VRMinotauroCollision : MonoBehaviour
{
    void OnCollisionEnter(Collision col)
    {
         Debug.Log("Collision detected");
        if (col.gameObject.tag == "minotauro")
        {
            Destroy(col.gameObject);
        }
    }
}