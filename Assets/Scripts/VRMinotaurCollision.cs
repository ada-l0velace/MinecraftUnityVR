using UnityEngine;
using System.Collections;

public class VRMinotaurCollision : MonoBehaviour
{
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "minotauro")
        {
            Destroy(col.gameObject);
        }
    }
}