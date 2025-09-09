using System.Collections.Generic;
using UnityEngine;
using DD.Web3;

[RequireComponent(typeof(LineRenderer))]
public class LiveRewardsGraph : MonoBehaviour
{
    public FakeRewardsTicker ticker;   // assign your ticker
    public int maxSeconds = 60;        // last N seconds
    public float xStep = 0.1f;         // how far each second moves on X
    public float yScale = 1f;          // adjust graph height
    public float updateInterval = 1f;  // sample rate (sec)

    private LineRenderer lr;
    private float timer;
    private List<Vector3> points = new();

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 0;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            timer = 0f;
            AddPoint();
        }
    }

    void AddPoint()
    {
        if (!ticker) return;

        float x = points.Count > 0 ? points[points.Count - 1].x + xStep : 0f;
        float y = ticker.GetCurrentRewards() * yScale;

        points.Add(new Vector3(x, y, 0));

        while (points.Count > maxSeconds)
            points.RemoveAt(0);

        lr.positionCount = points.Count;
        lr.SetPositions(points.ToArray());
    }
}
