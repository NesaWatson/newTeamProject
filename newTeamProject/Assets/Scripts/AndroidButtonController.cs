using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AndroidButtonController : MonoBehaviour
{
    public Button jumpButton;
    public Button attackButton;

    private void Start()
    {
        jumpButton.onClick.AddListener(Jump);
        attackButton.onClick.AddListener(Attack);
    }

    void Jump()
    {

    }

    void Attack()
    {
    }
}
