using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateCard : MonoBehaviour
{
    //
    [SerializeField]
    private GameObject cardImage;
    [SerializeField]
    private Transform cardParent;

    // Start is called before the first frame update
    void Start()
    {
        /*
         GameObject cardObj= Instantiate(cardImage);
        cardObj.transform.SetParent(cardParent,false);        
         */
        //別解
        Instantiate(cardImage, cardParent);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
