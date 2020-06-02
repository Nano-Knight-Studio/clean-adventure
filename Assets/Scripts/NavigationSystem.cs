using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationSystem : MonoBehaviour
{
    private MapPoint[] allPoints;
    public List<MapPoint> passedPoints = new List<MapPoint>();
    public List<MapPoint> verifiedPoints = new List<MapPoint>();
    public List<MapPoint> currentPath = new List<MapPoint>();

    void Start()
    {
        allPoints = FindObjectsOfType<MapPoint>();
    }

    public MapPoint GetNextPoint (MapPoint currentPoint, MapPoint targetPoint)
    {
        passedPoints.Clear();
        verifiedPoints.Clear();
        currentPath = GetNextPoints(currentPoint, targetPoint);
        if (currentPath != null)
        {
            // if (currentPath.Contains(currentPoint))
            // {
            //     currentPath.Remove(currentPoint);
            // }
            return currentPath[currentPath.Count -2];
        }
        else
        {
            return null;
        }
    }

    public List<MapPoint> GetNextPoints (MapPoint currentPoint, MapPoint targetPoint)
	{
		//Registering point in passedPoints
		if (!passedPoints.Contains(currentPoint))
		{
			passedPoints.Add(currentPoint);
		}
		
		//Arrived
		if (currentPoint == targetPoint)
		{
			verifiedPoints.Add(currentPoint);
			return verifiedPoints;
		}
		
		//There's only one option left, so it's an end.
		if (currentPoint.availablePoints.Length == 1)
		{
			//It's not the first point
			if (currentPoint != passedPoints[0])
			{
				return null;
			}
		}
		
		//Next iterations
		foreach (MapPoint p in currentPoint.availablePoints)
		{
			if (!passedPoints.Contains(p)) //Next point has not been passed yet. And it's unlocked
			{
				//If all next iterations return null,
				//This path is wrong,
				//So returns null.
				int nullIterationCount = 0;
				int necessaryIterations = 0;
				foreach (MapPoint i in currentPoint.availablePoints)
				{
					//Counting necessary iterations
					if (!passedPoints.Contains(i))
					{
						necessaryIterations++;
					}
					//The iteration should not be executed in a point that has been passed already
					if (!passedPoints.Contains(i))
					{
						if (GetNextPoints(i, targetPoint) == null) //Next iteration returned null
						{
							nullIterationCount++;
						}
					}
				}

				if (nullIterationCount == necessaryIterations) //Every next iteration returned null
				{
					return null;
				}
				else
				{
					verifiedPoints.Add(currentPoint);
					return verifiedPoints;
				}
			}
		}
		
		return null;
	}
}
