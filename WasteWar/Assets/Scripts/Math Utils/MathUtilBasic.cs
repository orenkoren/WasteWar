using UnityEngine;

public static class MathUtilBasic {



    /*p1 and p2 are two points that have certain (different) rotation, the function
     interpolates rotation inbetween these two points and gives us a Quaternion to smoothly transition from p1 state to p2 rotation state on our object
     */
    public static Quaternion CalcRotationChangeAlongTheCurve(float t, Transform p1,Transform p2)
    {
        return Quaternion.Lerp(
                   p1.rotation,
                   p2.rotation,
                   t);
    }
      /*a bezier curve is a parametric curve  given by a set of points pX and an equation
     *a point pX consists of in our case 3 coordinates  
     *by inserting a parameter t (from 0 to 1) inside the equation we are calculating
     *a Vec3 coordinate of which will be located in a location based on the specified t
     *for example t=0, we'll get a point at the start of the curve specified by the equation
     *t=1 endpoint
     *t between 0 and 1, somewhere along the curve
     *simple visualization @ https://christopherchudzicki.github.io/MathBox-Demos/parametric_curves_3D.html
     */
    public static Vector3 CalcCurrPosAlongTheCurve(float t, Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        return Mathf.Pow(1 - t, 3) * p1 +
            3 * Mathf.Pow(1 - t, 2) * t * p2 +
            3 * (1 - t) * Mathf.Pow(t, 2) * p3 +
            Mathf.Pow(t, 3) * p4;
    }

}
