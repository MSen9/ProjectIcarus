using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperFunctions
{
    //retruns the angle between two positions in degrees
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

    static public void GainClear()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<BulletClears>().clears++;
        HealthPowerUpTracker healthPupTracker = GameObject.FindGameObjectWithTag("Canvas").GetComponent<HealthPowerUpTracker>();
        healthPupTracker.MakeUIObj(trackedTypes.clears);
    }
    static public void GainPowerUp(PowerUpType pType, int pupCount = 1)
    {
        if(pupCount == 0)
        {
            return;
        }
        HealthPowerUpTracker healthPupTracker = GameObject.FindGameObjectWithTag("Canvas").GetComponent<HealthPowerUpTracker>();
        PlayerShooting pShooting = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerShooting>();
        switch (pType)
        {
            case PowerUpType.fireRate:
                healthPupTracker.MakeUIObj(trackedTypes.fireRate, pupCount);
                pShooting.fireRateBuffs += pupCount;
                break;
            case PowerUpType.shotSize:
                healthPupTracker.MakeUIObj(trackedTypes.shotSize, pupCount);
                pShooting.shotSizeBuffs += pupCount;
                break;
            case PowerUpType.manaGen:
                healthPupTracker.MakeUIObj(trackedTypes.manaGen, pupCount);
                pShooting.manaGenBuffs += pupCount;
                break;
            case PowerUpType.shotPen:
                healthPupTracker.MakeUIObj(trackedTypes.shotPen, pupCount);
                pShooting.shotPenBuffs += pupCount;
                break;
            case PowerUpType.shotSpread:
                healthPupTracker.MakeUIObj(trackedTypes.shotSpread, pupCount);
                pShooting.shotSpreadBuffs += pupCount;
                break;
            case PowerUpType.shotExplode:
                healthPupTracker.MakeUIObj(trackedTypes.shotExplode, pupCount);
                pShooting.shotExplodeBuffs += pupCount;
                break;
            case PowerUpType.shotSplit:
                healthPupTracker.MakeUIObj(trackedTypes.shotSplit, pupCount);
                pShooting.shotSplitBuffs += pupCount;
                break;
            case PowerUpType.OMEGA:
                healthPupTracker.MakeUIObj(trackedTypes.fireRate, pupCount);
                pShooting.fireRateBuffs += pupCount;
                healthPupTracker.MakeUIObj(trackedTypes.shotSize, pupCount);
                pShooting.shotSizeBuffs += pupCount;
                healthPupTracker.MakeUIObj(trackedTypes.manaGen, pupCount);
                pShooting.manaGenBuffs += pupCount;
                healthPupTracker.MakeUIObj(trackedTypes.shotPen, pupCount);
                pShooting.shotPenBuffs += pupCount;
                int randomMegaType = Random.Range((int)PowerUpType.shotSpread,(int)PowerUpType.shotSplit + 1);
                GainPowerUp((PowerUpType)randomMegaType, pupCount);
                /*
                healthPupTracker.MakeUIObj(trackedTypes.shotSpread, pupCount);
                pShooting.shotSpreadBuffs += pupCount;
                healthPupTracker.MakeUIObj(trackedTypes.shotExplode, pupCount);
                pShooting.shotExplodeBuffs += pupCount;
                healthPupTracker.MakeUIObj(trackedTypes.shotSplit, pupCount);
                pShooting.shotSplitBuffs += pupCount;
                */
                break;
            default:
                Debug.LogError("Error: No powerup type on powerUp");
                break;

        }

        pShooting.UpdateBuffs();
    }

    static public bool LosePowerUp(PowerUpType pType)
    {
        HealthPowerUpTracker healthPupTracker = GameObject.FindGameObjectWithTag("Canvas").GetComponent<HealthPowerUpTracker>();
        PlayerShooting pShooting = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerShooting>();
        switch (pType)
        {
            case PowerUpType.fireRate:
                if(pShooting.fireRateBuffs <= 0)
                {
                    return false;
                }
                healthPupTracker.RemoveUIObj(trackedTypes.fireRate);
                pShooting.fireRateBuffs--;
                break;
            case PowerUpType.shotSize:
                if (pShooting.shotSizeBuffs <= 0)
                {
                    return false;
                }
                healthPupTracker.RemoveUIObj(trackedTypes.shotSize);
                pShooting.shotSizeBuffs--;
                break;
            case PowerUpType.manaGen:
                if (pShooting.manaGenBuffs <= 0)
                {
                    return false;
                }
                healthPupTracker.RemoveUIObj(trackedTypes.manaGen);
                pShooting.manaGenBuffs--;
                break;
            case PowerUpType.shotPen:
                if (pShooting.shotPenBuffs <= 0)
                {
                    return false;
                }
                healthPupTracker.RemoveUIObj(trackedTypes.shotPen);
                pShooting.shotPenBuffs--;
                break;
            default:
                Debug.LogError("Error: No powerup type on powerUp");
                break;

        }
        
        pShooting.UpdateBuffs();
        return true;
    }

    public static List<Vector3> GetEquilateralShapePointPositions(int pointCount, float pointCenterDist, float angleOffset)
    {
        List<Vector3> pointPositions = new List<Vector3>();
        angleOffset = angleOffset * Mathf.Deg2Rad;
        float equWallAngle = 180 * ((pointCount - 2) / (float)pointCount);
        Vector3 pointPos;
        Vector3 lastEndPointPos = Vector3.zero;
        for (int i = 0; i < pointCount; i++)
        {
            if (i == 0)
            {
                //firstPointGen
                pointPos = new Vector3(Mathf.Cos(angleOffset), Mathf.Sin(angleOffset)) * pointCenterDist;

            }
            else
            {
                pointPos = lastEndPointPos;

            }
            float angleBetween = AngleBetween(Vector3.zero, pointPos);
            float endAngle = (angleBetween + (90 - equWallAngle)) * Mathf.Deg2Rad;
            lastEndPointPos = new Vector3(Mathf.Cos(endAngle), Mathf.Sin(endAngle)) * pointCenterDist;

            pointPositions.Add(pointPos);
        }

        return pointPositions;
    }
    
}
