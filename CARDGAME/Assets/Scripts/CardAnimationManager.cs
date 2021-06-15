using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CardAnimationManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Transform endPoint;

    void Start()
    {
        transform.DOMove(endPoint.position, 2f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
