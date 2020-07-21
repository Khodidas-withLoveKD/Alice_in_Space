using UnityEngine;
using UnityEngine.SceneManagement;

/* Difficulty level
 * Increase difficulty -> increase thrust decrease Rotation
 * Low difficulty: Thrust = 700; Rotation = 450
 * Medium: Thrust = ; Rotation = ; 
 * High: Thrust = ; Rotation = ;
 */
public class rocket_engineering : MonoBehaviour
{
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float upThrust = 10f;
    [SerializeField] float levelLoadDelay = 2f;

    [SerializeField] AudioClip engineSound;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip levelCompleteSound;

    [SerializeField] ParticleSystem engineParticles;
    [SerializeField] ParticleSystem deathParticles;
    [SerializeField] ParticleSystem levelCompleteParticles;

    Rigidbody rigidbody;
    AudioSource audioSource;
    int totalLevels;

    enum State {Alive, Dying, Transcending};
    State state = State.Alive;      //default state

    // Start is called before the first frame update
    void Start()
    {
        totalLevels = SceneManager.sceneCountInBuildSettings;
        rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive)    //To stop the update method from being called multiple times
            return;

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                //print("Let's Begin");
                break;

            case "Finish":
                startLevelCompleteSequence();
                break;

            default:
                startDeathSequence();
                break;
        }
    }

    private void startDeathSequence()
    {
        state = State.Dying;
        audioSource.Stop();
        deathParticles.Play();
        //print("Below deathParticles.Particls");
        audioSource.PlayOneShot(deathSound);
        Invoke("LoadSameLevel", levelLoadDelay);
    }

    private void startLevelCompleteSequence()
    {
        state = State.Transcending;
        audioSource.Stop();
        levelCompleteParticles.Play();
        //print("Below levelComplete.Particls");
        audioSource.PlayOneShot(levelCompleteSound);        
        Invoke("LoadNextLevel", levelLoadDelay);
    }

    private void LoadSameLevel()
    {
        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentLevel);
    }

    private void LoadNextLevel()
    {
        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        //print("Total Levels = "+totalLevels);
        int nextLevel = (currentLevel + 1) % totalLevels;
        SceneManager.LoadScene(nextLevel);
    }

    private void RespondToThrustInput()
    {
        float thrustInThisFrame = upThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust(thrustInThisFrame);
        }
        else
        {
            audioSource.Stop();
            engineParticles.Stop();
        }
    }

    private void ApplyThrust(float thrustInThisFrame)
    {
        rigidbody.AddRelativeForce(Vector3.up * thrustInThisFrame);
        if (!audioSource.isPlaying)
            audioSource.PlayOneShot(engineSound);

        if (!engineParticles.isPlaying)
        {
            engineParticles.Play();
            //print("Inside engineParticles.Play()");
        }
    }

    private void RespondToRotateInput()
    {
        float rotationInThisFrame = rcsThrust * Time.deltaTime;

        rigidbody.freezeRotation = true;   //Rotate manually off

        if (Input.GetKey(KeyCode.A))
            transform.Rotate(Vector3.forward * rotationInThisFrame);
        else if (Input.GetKey(KeyCode.D))
            transform.Rotate(-Vector3.forward * rotationInThisFrame);

        rigidbody.freezeRotation = false;  //Set manual rotation on
    }

    
}
