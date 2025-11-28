using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    public GameObject bgSource;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(bgSource);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
