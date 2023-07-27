using UnityEngine;

public static class Utility
{
    public static Vector3 ToRound(this Vector3 v3, float mul) => new Vector3(RoundTo(v3.x, mul), RoundTo(v3.y, mul), RoundTo(v3.z, mul));
    public static float RoundTo(this float value, float mul = 1) => Mathf.Round(value / mul) * mul;
    public static Vector3 RotateXYPlane(this Vector3 vector) => new Vector3(vector.y, -vector.x, 0);
    public static Vector3 RotateXZPlane(this Vector3 vector) => new Vector3(vector.z, 0, -vector.x);
    public static Vector3 RotateYZPlane(this Vector3 vector) => new Vector3(0, vector.z, -vector.y);
    public static Vector2 ToXZ(this Vector3 vector) => new Vector2(vector.x, vector.z);

    public static Vector3 V2ToX0Z(this Vector2 vector) => new Vector3(vector.x, 0, vector.y);
    public static Vector3 _X0Z(this Vector3 v) => new Vector3(v.x, 0, v.z);

    public static void _SetMaterialColorAlpha(this Material mat, float alpha) => mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, alpha);
    public static Color _SetColorAlpha(this Color c, float alpha) => new Color(c.r, c.g, c.b, alpha);

    public static Vector3 Abs(this Vector3 v) => new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));

    public static void SetLocalPosX(this Transform tra, float x) => tra.localPosition = new Vector3(x, tra.localPosition.y, tra.localPosition.z);
    public static void SetLocalPosY(this Transform tra, float y) => tra.localPosition = new Vector3(tra.localPosition.x, y, tra.localPosition.z);
    public static void SetLocalPosZ(this Transform tra, float z) => tra.localPosition = new Vector3(tra.localPosition.x, tra.localPosition.y, z);

    public static void SetPosX(this Transform tra, float x) => tra.position = new Vector3(x, tra.position.y, tra.position.z);
    public static void SetPosY(this Transform tra, float y) => tra.position = new Vector3(tra.position.x, y, tra.position.z);
    public static void SetPosZ(this Transform tra, float z) => tra.position = new Vector3(tra.position.x, tra.position.y, z);

    public static void SetLocalEulerX(this Transform tra, float x) => tra.localEulerAngles = new Vector3(x, tra.localEulerAngles.y, tra.localEulerAngles.z);
    public static void SetLocalEulerY(this Transform tra, float y) => tra.localEulerAngles = new Vector3(tra.localEulerAngles.x, y, tra.localEulerAngles.z);
    public static void SetLocalEulerZ(this Transform tra, float z) => tra.localEulerAngles = new Vector3(tra.localEulerAngles.x, tra.localEulerAngles.y, z);

    public static void SetEulerX(this Transform tra, float x) => tra.eulerAngles = new Vector3(x, tra.eulerAngles.y, tra.eulerAngles.z);
    public static void SetEulerY(this Transform tra, float y) => tra.eulerAngles = new Vector3(tra.eulerAngles.x, y, tra.eulerAngles.z);
    public static void SetEulerZ(this Transform tra, float z) => tra.eulerAngles = new Vector3(tra.eulerAngles.x, tra.eulerAngles.y, z);

    public static Vector3 SetPosX(this Vector3 v, float x) => new Vector3(x, v.y, v.z);
    public static Vector3 SetPosY(this Vector3 v, float y) => new Vector3(v.x, y, v.z);
    public static Vector3 SetPosZ(this Vector3 v, float z) => new Vector3(v.x, v.y, z);

    public static float Sign(this float value)
    {
        if (value < 0) return -1;
        else if (value > 0) return 1;
        return 0;
    }

    public static int Sign(this int value)
    {
        if (value < 0) return -1;
        else if (value > 0) return 1;
        return 0;
    }

    public static RaycastHit RaycastWithCam(Camera cam, LayerMask layerMask, out RaycastHit hit)
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit, 100, layerMask);
        return hit;
    }

    public static RaycastHit RaycastWithCam(Camera cam, out RaycastHit hit)
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit, 100);
        return hit;
    }

    public static RaycastHit RaycastWithRay(Ray ray, LayerMask layerMask, out RaycastHit hit)
    {
        Physics.Raycast(ray, out hit, 100, layerMask);
        return hit;
    }

    public static RaycastHit RaycastWithRay(Ray ray, out RaycastHit hit)
    {
        Physics.Raycast(ray, out hit, 100);
        return hit;
    }

    public static float Remap(this float value, float from1, float to1, float from2, float to2) => (value - from1) / (to1 - from1) * (to2 - from2) + from2;

    public static float GetAngleSpeedFromSpeed(float speed, float radius)
    {
        float circumferenceOfCircle = 2 * Mathf.PI * radius;
        return speed * 360f / circumferenceOfCircle;
    }
}