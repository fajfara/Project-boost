using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
	Rigidbody _rigidbody;
	AudioSource _audioSource;
	[SerializeField] float rcsThrust = 100f;
	[SerializeField] float mainThrust = 100f;
	[SerializeField] float levelLoadDelay = 2f;
	
	// Audio clips
	[SerializeField] AudioClip mainEngine;
	[SerializeField] AudioClip deadSound;
	[SerializeField] AudioClip successSound;
	
	// Particles
	
	[SerializeField] ParticleSystem mainEngineParticles;
	[SerializeField] ParticleSystem successParticles;
	[SerializeField] ParticleSystem deathParticles;
	
	

	enum State
	{
		Alive,
		Dying,
		Transcending
	};

	State state = State.Alive;
	// Use this for initialization
	void Start ()
	{
		_rigidbody = GetComponent<Rigidbody>();
		_audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		
		if (state == State.Alive)
		{
			RespondToThrustInput();
			RespondToRotate();
		}
		
	}

	void OnCollisionEnter(Collision collision)
	{
		if (state != State.Alive)
		{
			return;
		}
		switch (collision.gameObject.tag)
		{
			case "Friendly":
				
				break;
			case "Finish":
				StartSuccessSequence();
				break;
			default:
				StartDeathSequence();
				break;
		}
	}

	private void StartDeathSequence()
	{
		state = State.Dying;
		_audioSource.Stop();
		_audioSource.PlayOneShot(deadSound);
		deathParticles.Play();
		Invoke("LoadFirstLevel", levelLoadDelay); 
	}

	private void StartSuccessSequence()
	{
		state = State.Transcending;
		_audioSource.Stop();
		_audioSource.PlayOneShot(successSound);
		successParticles.Play();
		Invoke("LoadNextLevel", levelLoadDelay); 
	}

	private void LoadNextLevel()
	{
		SceneManager.LoadScene(1); // TODO allow for more than 2 levels
	}
	
	private void LoadFirstLevel()
	{
		SceneManager.LoadScene(0);
	}

	private void RespondToThrustInput()
	{
		if (Input.GetKey(KeyCode.Space))
		{
			ApplyThrust();
		}
		else
		{
			_audioSource.Stop();
			mainEngineParticles.Stop();
		}
	}

	private void ApplyThrust()
	{
		_rigidbody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
		if (!_audioSource.isPlaying)
		{
			_audioSource.PlayOneShot(mainEngine);
		}
		mainEngineParticles.Play();
	}

	private void RespondToRotate()
	{
		_rigidbody.freezeRotation = true; // Take manual control of rotation
		if (Input.GetKey(KeyCode.A))
		{
			transform.Rotate(Vector3.forward * (rcsThrust * Time.deltaTime));
		}
		else if (Input.GetKey(KeyCode.D))
		{
			transform.Rotate(-Vector3.forward * (rcsThrust * Time.deltaTime));
		}
		_rigidbody.freezeRotation = false; //  Resume physics control of rotation
	}

	
}
