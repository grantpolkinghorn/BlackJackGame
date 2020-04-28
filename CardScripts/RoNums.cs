using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouletteTile : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject prefab;
    private int point;
    public int Point { get { return this.point; } }
    public GameObject Prefab { get { return this.prefab; } }

    public Num(GameObject prefab)
    {
        this.prefab = prefab;
        string name = prefab.name;
        int point;
                // other remaining possible cards, 0 - 36
        return point = Convert.ToInt16(name);
        this.point = point;
    }
}
