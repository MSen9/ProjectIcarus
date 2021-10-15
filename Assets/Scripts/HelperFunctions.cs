using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperFunctions
{
    public static float AngleBetween(Vector3 pos1,Vector3 pos2)
    {
        float ANGLE_OFFSET = -90;
        return Mathf.Atan2(pos1.y - pos2.y, pos1.x - pos2.x) * Mathf.Rad2Deg + ANGLE_OFFSET;
    }

    static public float NormalizeAngle(float angle)
    {
        int CIRCLE_DEGREES = 360;
        while (angle > CIRCLE_DEGREES)
        {
            angle -= CIRCLE_DEGREES;
        }
        while (angle < 0)
        {
            angle += CIRCLE_DEGREES;
        }

        return angle;
    }

    //
    static public float RotateTowards(float currAngle,float rotateGoal, float rotateRate, float timePassed)
    {
        int HALF_CIRCLE_DEGREES = 180;
        rotateGoal = NormalizeAngle(rotateGoal);
        currAngle = NormalizeAngle(currAngle);
        float angleDist = currAngle - rotateGoal;
        //the left and right distance to the 0 degrees angle for the current and 
        if (angleDist > HALF_CIRCLE_DEGREES || (angleDist < 0 && angleDist >= -1 * HALF_CIRCLE_DEGREES))
        {
            currAngle += rotateRate * timePassed;
        }
        else if ((angleDist <= HALF_CIRCLE_DEGREES && angleDist > 0) || angleDist < -1 * HALF_CIRCLE_DEGREES)
        {
            currAngle -= rotateRate * timePassed;
        }

        return currAngle;
    }

    static public void GainHealth()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<HealthHandling>().health++;
        HealthPowerUpTracker healthPupTracker = GameObject.FindGameObjectWithTag("Canvas").GetComponent<HealthPowerUpTracker>();
        healthPupTracker.MakeUIObj(trackedTypes.health);
    }

    static public void GainPowerUp(PowerUpType pType)
    {
        HealthPowerUpTracker healthPupTracker = GameObject.FindGameObjectWithTag("Canvas").GetComponent<HealthPowerUpTracker>();
        PlayerShooting pShooting = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerShooting>();
        switch (pType)
        {
            case PowerUpType.fireRate:
                healthPupTracker.MakeUIObj(trackedTypes.fireRate);
                pShooting.fireRateBuffs++;
                break;
            case PowerUpType.shotSize:
                healthPupTracker.MakeUIObj(trackedTypes.shotSize);
                pShooting.shotSizeBuffs++;
                break;
            case PowerUpType.manaGen:
                healthPupTracker.MakeUIObj(trackedTypes.manaGen);
                pShooting.manaGenBuffs++;
                break;
            default:
                Debug.LogError("Error: No powerup type on powerUp");
                break;

        }

        pShooting.UpdateBuffs();
    }

    static public void LosePowerUp(PowerUpType pType)
    {
        HealthPowerUpTracker healthPupTracker = GameObject.FindGameObjectWithTag("Canvas").GetComponent<HealthPowerUpTracker>();
        PlayerShooting pShooting = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerShooting>();
        switch (pType)
        {
            case PowerUpType.fireRate:
                healthPupTracker.RemoveUIObj(trackedTypes.fireRate);
                pShooting.fireRateBuffs--;
                break;
            case PowerUpType.shotSize:
                healthPupTracker.RemoveUIObj(trackedTypes.shotSize);
                pShooting.shotSizeBuffs--;
                break;
            case PowerUpType.manaGen:
                healthPupTracker.RemoveUIObj(trackedTypes.manaGen);
                pShooting.manaGenBuffs--;
                break;
            default:
                Debug.LogError("Error: No powerup type on powerUp");
                break;

        }

        pShooting.UpdateBuffs();
    }
}
