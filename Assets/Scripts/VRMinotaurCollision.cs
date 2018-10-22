using UnityEngine;
using System.Collections;

public class VRMinotaurCollision : MonoBehaviour
{
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name == "minotaur_hr-wind")
        {
            Destroy(col.gameObject);
        }
    }
}