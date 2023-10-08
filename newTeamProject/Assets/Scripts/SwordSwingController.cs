using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSwingController : MonoBehaviour
{
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Keypad0)) 
        {
            animator.SetTrigger("Swing");
        }
    }
}
