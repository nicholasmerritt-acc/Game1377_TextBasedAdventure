using NUnit.Framework;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Day1ExamCode : MonoBehaviour
{
    
    [SerializeField] private GameObject dummyPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        IncrementalSum();
        IndexingIntoAnArray();
        RandomElementFromArray();
    }
  
    private void IncrementalSum()
    {
        int n = 0;
        for (int i = 0; i < 5; i++)
        {
            n = n + i;
        }
        Debug.Log(n); // 1st time: n = 0+0=0, 2nd Time: n =  0+1=1, 3rd Time: n = 1+2=3, 4th Time: n = 3+3=6, 5th Time: n = 6+4=10
    }

    private void IndexingIntoAnArray()
    {
        string[] games = { "Mario", "Final Fantasy", "Tetris", "GTA", "Minecraft" };
        Debug.Log(games[3]);
    }

    [SerializeField] private string[] GameStartShouts = { "Let's-a go!", "Here we go!", "It's-a me, Mario!", "I'm Chris Pratt :(" };
    private void RandomElementFromArray()
    {
        int randomIndex = Random.Range(0, GameStartShouts.Length);
        Debug.Log(GameStartShouts[randomIndex]);
    }

    int CalculateAttackRoll(int baseProficiency, int attackBonus) 
    {
        int randomRoll = Random.Range(0, 20);
        return baseProficiency +attackBonus + randomRoll;
    }

    //5. Input: Assume you have a GameObject called DummyPrefab and you are writing your code in the Update method.
    //    Write code that would create one of these prefabs at a random position where x is between -5f and 5f, y is 0f, 
    //    and z = between - 4f and 4f whenever you press the S key.

    // Update is called once per frame
    void Update()
    {
        Debug.Log(Time.deltaTime);
        if(Input.GetKeyDown(KeyCode.S))
        {
            float randomX = Random.Range(-5f, 5f);
            float y = 0f;
            float randomZ = Random.Range(-4f, 4f);
            Instantiate(dummyPrefab, new Vector3(randomX, y, randomZ), Quaternion.identity);
        }
    }
      

    private void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
        }
    }

    //4. Collisions: Write a method commonly used in Unity for checking collisions that would exist in a script
    //    (you only need to write the method). In the method, it should check if the object collided with has a Rigidbody component.
    //    If it does, destroy the object this script would be attached to. Otherwise, destroy the other object that collided with this one.
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Rigidbody>() != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Destroy(collision.gameObject);
        }
    }
}
