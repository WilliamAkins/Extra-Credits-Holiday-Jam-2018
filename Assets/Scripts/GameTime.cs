using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTime : MonoBehaviour
{
    [Header("Set how many in-game minuites = 1 real second")]
    public int gameSpeedInMins = 15;

    [Header("Start time as a 24-hour clock")]
    [Range(0, 24)]
    public float startTimeInHours = 7;

    float dayLengthInMins = 0;

    //total seconds in 1 day
    private const int SECS_IN_DAY = 24 * 3600;

    //the time that each time state begins
    private const int START_SUNRISE = 5 * 3600;
    private const int START_DAY = 7 * 3600;
    private const int START_SUNSET = 17 * 3600;
    private const int START_NIGHT = 19 * 3600;

    //References to various objects
    private ManageMainInfo manageMainInfo;
    private GameObject dayAndNight;
    private Light sun;
    private Light moon;

    private float initSunIntensity = 0;
    private float initMoonIntensity = 0;

    //stores the current state of the day night cycle
    public enum TimeState { Sunrise, Day, Sunset, Night }
    private TimeState currentTimeState = TimeState.Day;

    //the number of days that have passed
    private byte day = 1;
    //the current time of day in the game, in seconds
    private float time = 0;
    //the total time the player entered, converted to seconds
    private float dayLengthInSecs = 0;
    //says whether the once the time state has changed, has it updated anything that needed updating
    private bool stateUpdated = false;
    //This is the speed at which state transitions will occur with effects such as fading out lights etc
    private float sunInterpolationSpeed = 0.0f;
    private float moonInterpolationSpeed = 0.0f;

    //----------[ START PUBLIC FUNCTIONS ]----------
    public int getTime()
    {
        return (int)time;
    }

    public byte getDay()
    {
        return day;
    }

    public float getInGameSecsPerRealSecs()
    {
        return (SECS_IN_DAY / dayLengthInSecs) * Time.deltaTime;
    }

    public TimeState getTimeState()
    {
        //returns the current time state
        return currentTimeState;
    }
    //----------[ END PUBLIC FUNCTIONS ]----------

    private void setTimeState(TimeState timeState)
    {
        //Updates the current time state
        currentTimeState = timeState;
    }

    private void setSunInterpolationSpeed()
    {
        switch (currentTimeState)
        {
            case TimeState.Sunrise:
                sunInterpolationSpeed = initSunIntensity / (1 / ((START_DAY - START_SUNRISE) / getInGameSecsPerRealSecs()));
                break;
            case TimeState.Sunset:
                sunInterpolationSpeed = initSunIntensity / (1 / ((START_NIGHT - START_SUNSET) / getInGameSecsPerRealSecs()));
                break;
            default:
                sunInterpolationSpeed = 0.0f;
                break;
        }
    }

    private void setMoonInterpolationSpeed()
    {
        switch (currentTimeState)
        {
            case TimeState.Sunrise:
                moonInterpolationSpeed = initMoonIntensity / (1 / ((START_DAY - START_SUNRISE) / getInGameSecsPerRealSecs()));
                break;
            case TimeState.Sunset:
                moonInterpolationSpeed = initMoonIntensity / (1 / ((START_NIGHT - START_SUNSET) / getInGameSecsPerRealSecs()));
                break;
            default:
                moonInterpolationSpeed = 0.0f;
                break;
        }
    }

    private void updateSunrise()
    {
        sun.intensity += sunInterpolationSpeed;
        moon.intensity -= moonInterpolationSpeed;

        //check if the state should update to the next one
        if (time >= START_DAY)
        {
            setTimeState(TimeState.Day);

            //sun.intensity = initSunIntensity;
            //moon.intensity = 0;
        }
    }

    private void updateDay()
    {
        //update any changes to the system 1 time per state change
        if (!stateUpdated)
        {
            stateUpdated = true;
        }

        //check if the state should update to the next one
        if (time >= START_SUNSET)
        {
            stateUpdated = false;
            setTimeState(TimeState.Sunset);
        }
    }

    private void updateSunset()
    {
        sun.intensity -= sunInterpolationSpeed;
        moon.intensity += moonInterpolationSpeed;

        //check if the state should update to the next one
        if (time >= START_NIGHT)
        {
            setTimeState(TimeState.Night);

            //sun.intensity = 0;
            //moon.intensity = initMoonIntensity;
        }
    }

    private void updateNight()
    {
        //update any changes to the system 1 time per state change
        if (!stateUpdated)
        {
            stateUpdated = true;
        }

        //check if the state should update to the next one
        if (time >= START_SUNRISE && time < START_DAY)
        {
            stateUpdated = false;
            setTimeState(TimeState.Sunrise);
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        manageMainInfo = GameObject.Find("EventManager").GetComponent<ManageMainInfo>();

        //Get a reference to sun object
        dayAndNight = GameObject.Find("DayAndNight");
        sun = dayAndNight.transform.Find("Sun").GetComponent<Light>();
        moon = dayAndNight.transform.Find("Moon").GetComponent<Light>();

        //get the day length in mins
        dayLengthInMins = 24.0f / gameSpeedInMins;

        //convert the minutes entered into seconds
        dayLengthInSecs = dayLengthInMins * 60.0f;

        //setup the initial intensity of the sun and moon
        initSunIntensity = sun.intensity;
        initMoonIntensity = moon.intensity;

        sun.intensity = initSunIntensity;
        moon.intensity = 0;

        //Sets the current time to the entered time and moves the day and night system to the correct starting position
        time = startTimeInHours * 3600.0f;
        dayAndNight.transform.RotateAround(Vector3.zero, Vector3.left, 15.0f * startTimeInHours);
    }

    // Update is called once per frame
    private void Update()
    {
        if (manageMainInfo.getGamePaused() == false)
        {
            //rotates the light around a pivot; in degrees per second
            dayAndNight.transform.RotateAround(Vector3.zero, Vector3.left, (360.0f / dayLengthInSecs) * Time.deltaTime);

            //updates the state transition interpolation speed each frame
            //setInterpolationSpeed();

            //Stores the current game time in seconds
            time += getInGameSecsPerRealSecs();

            //Debug.Log("TIme = " + time);

            if (time >= SECS_IN_DAY)
            {
                day++;
                time = 0;
            }

            //updates the current state of the day and night system
            switch (currentTimeState)
            {
                case TimeState.Sunrise:
                    updateSunrise();
                    break;
                case TimeState.Day:
                    updateDay();
                    break;
                case TimeState.Sunset:
                    updateSunset();
                    break;
                case TimeState.Night:
                    updateNight();
                    break;
                default:
                    break;
            }
        }
    }
}
