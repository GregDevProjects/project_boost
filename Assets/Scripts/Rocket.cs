using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust = 150f;
    [SerializeField] float mainThrust = 150f;
    [SerializeField] float levelLoadDelay = 2f;
    [SerializeField] AudioClip death;
    [SerializeField] AudioClip startLevel;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem deathParticles;

    Rigidbody rigidBody;
    AudioSource audioSourceSuccessDeath;
    AudioSource audioSourceThrust;

    [SerializeField]
    bool collisionsDisabled = false;

    enum State {Alive, Dying, Transcending};
    State state = State.Alive;
    // Start is called before the first frame update
    void Start()
    {
        //'generics'
        //gets a refrence to something attached to the game object
        rigidBody = GetComponent<Rigidbody>();
        AudioSource[] audioSources = GetComponents<AudioSource>();

        audioSourceSuccessDeath = audioSources[0];
        audioSourceThrust = audioSources[1];
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Dying)
        {
            audioSourceThrust.Stop();
            return;
        }
        ProcessInput();
    }

    private void ProcessInput()
    {
        Rotate();
        Thrust();
        if (Debug.isDebugBuild)
        {
            DebugKeys();
        }
        
    }

    private void DebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextScene();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            collisionsDisabled = !collisionsDisabled;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state !=State.Alive || collisionsDisabled)
        {
            return;
        }
        switch(collision.gameObject.tag)
        {
            case "Friendly":
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
        mainEngineParticles.Stop();
        deathParticles.Play();
        audioSourceSuccessDeath.Stop();
        audioSourceSuccessDeath.PlayOneShot(death);
        Invoke("Death", 2f); //parameterize time
        state = State.Dying;
    }

    private void StartFinish()
    {
        successParticles.Play();
        Invoke("LoadNextScene", levelLoadDelay);
        state = State.Transcending;
        audioSourceSuccessDeath.Stop();
        audioSourceSuccessDeath.PlayOneShot(startLevel);
    }

    private void Death()
    {
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        //int nextSceneIndex = SceneManager.
        print( currentSceneIndex);
        print(SceneManager.sceneCountInBuildSettings);
        if (SceneManager.sceneCountInBuildSettings <= currentSceneIndex + 1)
        {
            SceneManager.LoadScene(0);
            return;
        }
        SceneManager.LoadScene(currentSceneIndex+1);
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
            ApplyTrust();
        }
        else
        {
            mainEngineParticles.Stop();
            audioSourceThrust.Stop();
        }
    }

    private void ApplyTrust()
    {
        mainEngineParticles.Play();
        float thrustThisFrame = mainThrust * Time.deltaTime;
        rigidBody.AddRelativeForce(Vector3.up * thrustThisFrame);
        if (!audioSourceThrust.isPlaying)
        {
            audioSourceThrust.Play();
        }
    }
}
