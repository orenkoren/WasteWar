using UnityEngine;

public class ZoomRouteDraw : MonoBehaviour
{
    [SerializeField]
    private Transform[] controlPoints;
    
    private Vector3 gizmosPosition;
    private const float INCREMENT_BY= 0.02f;

    private void OnDrawGizmos()
    {
        for (float t=0; t <= 1; t += INCREMENT_BY)
        {
            gizmosPosition = MathUtilBasic.CalcCurrPosAlongTheCurve(t, controlPoints[0].position, controlPoints[1].position, controlPoints[2].position, controlPoints[3].position);
            Gizmos.DrawSphere(gizmosPosition, 0.15f);
        }
    }
}
