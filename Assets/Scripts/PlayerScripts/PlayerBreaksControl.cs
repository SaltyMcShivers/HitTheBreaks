using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerBreaksControl : MonoBehaviour
{
    public float defaultMoveSpeed;
    public float slowDownRate;
    public float speedUpRate;

    public float breaksDepletionTime;
    public float breaksRecoveryTime;
    public float breaksDisableTime;
    public float breakDepletionTransitionRatio;

    public Slider brakeMeter;
    public Image brakeBackground;
    public Image brakeFill;

    public Color backgroundDefault;
    public Color backgroundDisabled;
    public Color fillDefault;
    public Color fillDisabled;

    public float sprintMax;
    public float sprintRate;
    public float sprintRecoveryRate;

    public float minSwipeSpeed;

    public ParticleSystem rocketParticles;
    public ParticleSystem mainJetParticles;

    float currentSpeed;
    float breakIntegrity;
    float brokenBreaksStart;
    bool sprinting;

    bool gameOver = true;

    void Start()
    {
        brokenBreaksStart = -breaksDisableTime;
    }

    public void RestartPlayer()
    {
        currentSpeed = 0f;
        breakIntegrity = 1f;
        foreach (Transform t in transform)
        {
            if (t.parent == transform) t.gameObject.SetActive(true);
        }
        transform.position = new Vector3(0f, transform.position.y);
        brakeBackground.color = backgroundDefault;
        brakeFill.color = fillDefault;
        StartMainRocket();
        mainJetParticles.Stop();
        brokenBreaksStart = -breaksDisableTime;
        gameOver = false;
    }
	
	void Update () {
        if (gameOver) return;
        FindSpeed();
        if((Input.touchCount == 0 || Time.time - brokenBreaksStart <= breaksDisableTime) && mainJetParticles.isPlaying)
        {
            mainJetParticles.Stop();
        }
        if(Input.touchCount > 0 && Time.time - brokenBreaksStart > breaksDisableTime && !mainJetParticles.isPlaying)
        {
            mainJetParticles.Play();
        }
        if (Time.time - brokenBreaksStart <= breaksDisableTime)
        {
            brakeMeter.value = (Time.time - brokenBreaksStart) / breaksDisableTime;
        }
    }

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }

    public float GetBreakIntegrity()
    {
        return breakIntegrity;
    }

    void FindSpeed()
    {
        //if (Input.GetKey(KeyCode.Space))
        if(Input.touchCount > 0)
        {
            if (Time.time - brokenBreaksStart > breaksDisableTime)
            {
                if (currentSpeed == 0f && !sprinting)
                {
                    breakIntegrity -= 1f / breaksDepletionTime * Time.deltaTime;
                    brakeMeter.value = breakIntegrity;
                    if (breakIntegrity <= 0)
                    {
                        StartCoroutine(DisableBreaks());
                    }
                    return;
                }
                breakIntegrity -= 1f / breaksDepletionTime * Time.deltaTime * breakDepletionTransitionRatio;
                brakeMeter.value = breakIntegrity;
                if (breakIntegrity <= 0)
                {
                    StartCoroutine(DisableBreaks());
                }
                if (sprinting)
                {
                    currentSpeed += speedUpRate * sprintRate * Time.deltaTime;
                    if(currentSpeed > sprintMax)
                    {
                        sprinting = false;
                        currentSpeed = sprintMax;
                    }
                }
                else currentSpeed = Mathf.Max(currentSpeed - slowDownRate * Time.deltaTime, 0);

                //Check For Sprint
                /*
                if(Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow))
                {
                    StartCoroutine(SprintCoroutine());
                }
                */
                if (!sprinting && currentSpeed <= defaultMoveSpeed)
                {
                    foreach (Touch t in Input.touches)
                    {
                        if (t.phase == TouchPhase.Moved)
                        {
                            if (Mathf.Abs(t.deltaPosition.y) > minSwipeSpeed * Time.deltaTime && Mathf.Abs(t.deltaPosition.y) > Mathf.Abs(t.deltaPosition.x))
                            {
                                if (t.deltaPosition.y < 0)
                                {
                                    return;
                                }
                                else if (t.deltaPosition.y > 0)
                                {
                                    //Debug.Log("Sprint");
                                    sprinting = true;
                                    return;
                                }
                            }
                        }
                    }
                }

                return;
            }
        }
        breakIntegrity = Mathf.Min(1f, breakIntegrity + 1f / breaksRecoveryTime * Time.deltaTime);
        if (Time.time - brokenBreaksStart > breaksDisableTime)
        {
            brakeMeter.value = breakIntegrity;
        }
        if (sprinting)
        {
            currentSpeed += speedUpRate * sprintRate * Time.deltaTime;
            if (currentSpeed > sprintMax)
            {
                sprinting = false;
                currentSpeed = sprintMax;
            }
            return;
        }
        if(currentSpeed > defaultMoveSpeed)
        {
            currentSpeed -= sprintRecoveryRate * Time.deltaTime;
        }
        if (currentSpeed == defaultMoveSpeed) return;
        currentSpeed = Mathf.Min(currentSpeed + speedUpRate * Time.deltaTime, defaultMoveSpeed);
    }

    IEnumerator DisableBreaks()
    {
        brokenBreaksStart = Time.time;
        brakeBackground.color = backgroundDisabled;
        brakeFill.color = fillDisabled;
        yield return new WaitForSeconds(breaksDisableTime);
        breakIntegrity = 1f;
        brakeBackground.color = backgroundDefault;
        brakeFill.color = fillDefault;
    }

    public void StopMovement()
    {
        gameOver = true;
        StopMainRocket();
        mainJetParticles.Stop();
        StopAllCoroutines();
    }

    void StartMainRocket()
    {
        rocketParticles.Play();
    }

    void StopMainRocket()
    {
        rocketParticles.Stop();
    }

    public bool IsGameOver()
    {
        return gameOver;
    }
}
