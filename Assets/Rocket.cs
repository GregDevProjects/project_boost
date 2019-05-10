using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust = 150f;
    [SerializeField] float mainThrust = 150f;
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip death;
    [SerializeField] AudioClip startLevel;

    Rigidbody rigidBody;
    AudioSource audioSource;

    enum State {Alive, Dying, Transcending};
    State state = State.Alive;
    // Start is called before the first frame update
    void Start()
    {
        //'generics'
        //gets a refrence to something attached to the game object
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Dying)
        {
           // audioSource.Stop();
            return;
        }
        ProcessInput();
    }

    private void ProcessInput()
    {
        Rotate();
        Thrust();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state !=State.Alive)
        {
            return;
        }
        switch(collision.gameObject.tag)
        {
            case "Friendly":
                //do nothing
                print("okay");
                break;
            case "Finish":
                StartFinish();
                break;
            default:
                StartDeath();
                break;
        }

    }

    private void StartDeath()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(death);
        Invoke("Death", 2f); //parameterize time
        state = State.Dying;
    }

    private void StartFinish()
    {
        Invoke("LoadNextScene", 1f); //parameterize time
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(startLevel);
    }

    private void Death()
    {
        
        SceneManager.LoadScene(0);
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(1);
    }

    private void Rotate()
    {
        //take manual control of rotation
        rigidBody.freezeRotation = true;
        
        float rotationThisFrame = rcsThrust * Time.deltaTime;
        
        if (Input.GetKey(KeyCode.A))
        {
            //if there is a longer frame time, we need to rotate further 
            //so rotate * deltatime
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.back * rotationThisFrame);

        }

        rigidBody.freezeRotation = false;
    }

    private void Thrust()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            float thrustThisFrame = mainThrust * Time.deltaTime;
            rigidBody.AddRelativeForce(Vector3.up * thrustThisFrame);
            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(mainEngine);
            } else
            {
                //https://support.unity3d.com/hc/en-us/articles/206116386-How-do-I-play-multiple-Audio-Sources-from-one-GameObject-
            }
        } 
    }
}
