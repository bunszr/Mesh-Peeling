using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using System.Linq;
using UnityEngine.EventSystems;

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

    public enum DegreeSpace { xy, xz }
    public static float FindDegree(Vector3 v, DegreeSpace space = DegreeSpace.xy)
    {
        float angle = space == DegreeSpace.xy ? Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg : Mathf.Atan2(v.x, v.z) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360;
        return angle;
    }

    public static Quaternion GetSmoothXZRotation(this Transform transform, Vector3 dirXZ, float percent)
    {
        float angleY = Utility.FindDegree(dirXZ, Utility.DegreeSpace.xz);
        return Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, angleY, 0), percent);
    }

    public static void Foreach<T>(this IEnumerable<T> aaa, Action<int, T> action)
    {
        int i = 0;
        foreach (var item in aaa) action(++i, item);
    }

    public static void Foreach<T>(this IEnumerable<T> aaa, Action<T> action)
    {
        foreach (var item in aaa) action(item);
    }


    public static Vector3 GetClosestPoint(Vector3 point, List<Vector3> pointList)
    {
        float closestDst = float.MaxValue;
        Vector3 closestPoint = Vector3.zero;
        for (int i = 0; i < pointList.Count; i++)
        {
            float sqrDst = Vector3.Distance(point, pointList[i]);
            if (sqrDst < closestDst)
            {
                closestDst = sqrDst;
                closestPoint = pointList[i];
            }
        }
        return closestPoint;
    }

    // Bu şekilde kullan
    // Vector3 newVector = Utility.ClosestPointOnLineSegment(worldPos, myRopeData.startRb.position, myRopeData.endRb.position);
    // float percent = Utility.InverseLerp(myRopeData.startRb.position, myRopeData.endRb.position, newVector);
    // https://answers.unity.com/questions/1271974/inverselerp-for-vector3.html 
    public static float InverseLerp(Vector3 a, Vector3 b, Vector3 value)
    {
        Vector3 AB = b - a;
        Vector3 AV = value - a;
        float result = Vector3.Dot(AV, AB) / Vector3.Dot(AB, AB);
        return Mathf.Clamp01(result);
    }

    public static Vector3 ClosestPointOnLineSegment(Vector3 p, Vector3 a, Vector3 b)
    {
        Vector3 aB = b - a;
        Vector3 aP = p - a;
        float sqrLenAB = aB.sqrMagnitude;

        if (sqrLenAB == 0)
            return a;

        float t = Mathf.Clamp01(Vector3.Dot(aP, aB) / sqrLenAB);
        return a + aB * t;
    }

    public static int SideOfLine(Vector2 a, Vector2 b, Vector2 c)
    {
        return (int)Mathf.Sign((c.x - a.x) * (-b.y + a.y) + (c.y - a.y) * (b.x - a.x));
    }

    // Nesne solumuzda ise -1, sağda ise 1
    public static int SideOfLineXZ(Vector3 a, Vector3 b, Vector3 c)
    {
        return -(int)Mathf.Sign((c.x - a.x) * (-b.z + a.z) + (c.z - a.z) * (b.x - a.x));
        // Kullanımı = int side = Utility.SideOfLineXZ(transform.position.ToX0Z(), (transform.position + transform.forward).ToX0Z(), sideT.position.ToX0Z());
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

    public static RaycastHit RaycastWithRay(Ray ray, Camera cam, LayerMask layerMask, out RaycastHit hit)
    {
        Physics.Raycast(ray, out hit, 100, layerMask);
        return hit;
    }

    public static RaycastHit RaycastWithRay(Ray ray, Camera cam, out RaycastHit hit)
    {
        Physics.Raycast(ray, out hit, 100);
        return hit;
    }


    public static Vector3 PlaneRaycast(Vector3 inNormal, Vector3 inPoint, Camera cam)
    {
        Plane plane = new Plane(inNormal, inPoint);
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        float rayDistance;
        if (plane.Raycast(ray, out rayDistance))
            return ray.GetPoint(rayDistance);
        return Vector3.zero;
    }

    public static float Remap(this float value, float from1, float to1, float from2, float to2) => (value - from1) / (to1 - from1) * (to2 - from2) + from2;

    public static Vector3 GetRandomPointInBounds(this Bounds bounds, float y = 0)
    {
        return new Vector3(
            UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
            y,
            UnityEngine.Random.Range(bounds.min.z, bounds.max.z));
    }

    public static Vector3 GetRandomPointInBounds(this Bounds bounds)
    {
        return new Vector3(
            UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
            UnityEngine.Random.Range(bounds.min.y, bounds.max.y),
            UnityEngine.Random.Range(bounds.min.z, bounds.max.z));
    }

    public static Vector3 ClampedPointInBounds(this Vector3 v, Bounds bounds)
    {
        v.x = Mathf.Clamp(v.x, bounds.min.x, bounds.max.x);
        v.y = Mathf.Clamp(v.y, bounds.min.y, bounds.max.y);
        v.z = Mathf.Clamp(v.z, bounds.min.z, bounds.max.z);
        return v;
    }

    public static Vector3 GetPercentOfPointInBounds(this Vector3 v, Bounds bounds)
    {
        v.x = Mathf.InverseLerp(bounds.min.x, bounds.max.x, v.x);
        v.y = Mathf.InverseLerp(bounds.min.y, bounds.max.y, v.y);
        v.z = Mathf.InverseLerp(bounds.min.z, bounds.max.z, v.z);
        return v;
    }

    public static Vector3 GetPointWitPercentInBounds(Vector3 percent, Bounds bounds)
    {
        Vector3 v = Vector3.zero;
        v.x = Mathf.Lerp(bounds.min.x, bounds.max.x, percent.x);
        v.y = Mathf.Lerp(bounds.min.y, bounds.max.y, percent.y);
        v.z = Mathf.Lerp(bounds.min.z, bounds.max.z, percent.z);
        return v;
    }

    public static Tween RotateAroundTween(this Transform t, Vector3 pivot, Vector3 axis, int loops = 1, float targetAngle = 100, float duration = 1)
    {
        Vector3 rotVector = t.localPosition - pivot;
        float angle = 0;
        return DOTween.To(() => angle, x => angle = x, targetAngle, duration).SetLoops(loops).OnUpdate(() =>
        {
            t.localPosition = pivot + Quaternion.Euler(axis * angle) * rotVector;
        });
    }

    public static Tween DOTimeScale(float from, float to, float duration, Ease ease = Ease.InOutSine)
    {
        return DOTween.To(() => Time.timeScale, x => Time.timeScale = x, to, duration).From(from).SetEase(Ease.InOutSine).SetUpdate(true);
    }

    public static Tween GetEmptyTween(float duration)
    {
        float percent = 0;
        return DOTween.To(() => percent, x => percent = x, 1, duration);
    }

    public static void Shuffle<T>(System.Random rng, T[] array)
    {
        int n = array.Length;
        while (n > 1)
        {
            int k = rng.Next(n--);
            T temp = array[n];
            array[n] = array[k];
            array[k] = temp;
        }
    }

    #region COLOR COLOR COLOR COLOR COLOR COLOR COLOR COLOR 

    public static Color RandomColor => new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, 1);

    public static Color SetAlpha(this Color color, float alpha)
    {
        color.a = alpha;
        return color;
    }

    public static bool CompareColorRGB(Color colorA, Color colorB)
    {
        return Mathf.Approximately(colorA.r, colorB.r) && Mathf.Approximately(colorA.g, colorB.g) && Mathf.Approximately(colorA.b, colorB.b);
    }

    public static bool CompareColorRGBA(Color colorA, Color colorB)
    {
        return Mathf.Approximately(colorA.r, colorB.r) && Mathf.Approximately(colorA.g, colorB.g) && Mathf.Approximately(colorA.b, colorB.b) && Mathf.Approximately(colorA.a, colorB.a);
    }

    #endregion



    public static Tween SetBlendShape(this SkinnedMeshRenderer skinnedMeshRenderer, int blendShapeIndex, float endValue, float duration)
    {
        float percent = skinnedMeshRenderer.GetBlendShapeWeight(blendShapeIndex);
        return DOTween.To(() => percent, x => percent = x, endValue, duration).OnUpdate(() =>
        {
            skinnedMeshRenderer.SetBlendShapeWeight(blendShapeIndex, percent);
        });
    }

    public static void ActivateOrNotWithDelay(this GameObject gameObject, float duration, bool isActive)
    {
        Utility.GetEmptyTween(duration).OnComplete(() => gameObject.SetActive(isActive));
    }

    public static Rigidbody ResetRb(this Rigidbody rb)
    {
        rb.velocity = new Vector3(0f, 0f, 0f);
        rb.angularVelocity = new Vector3(0f, 0f, 0f);
        rb.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
        return rb;
    }


    public static Vector3 SampleParabola(Vector3 start, Vector3 end, float height, float t)
    {
        float parabolicT = t * 2 - 1;
        if (Mathf.Abs(start.y - end.y) < 0.1f)
        {
            //start and end are roughly level, pretend they are - simpler solution with less steps
            Vector3 travelDirection = end - start;
            Vector3 result = start + t * travelDirection;
            result.y += (-parabolicT * parabolicT + 1) * height;
            return result;
        }
        else
        {
            //start and end are not level, gets more complicated
            Vector3 travelDirection = end - start;
            Vector3 levelDirecteion = end - new Vector3(start.x, end.y, start.z);
            Vector3 right = Vector3.Cross(travelDirection, levelDirecteion);
            Vector3 up = Vector3.Cross(right, travelDirection);
            if (end.y > start.y) up = -up;
            Vector3 result = start + t * travelDirection;
            result += ((-parabolicT * parabolicT + 1) * height) * up.normalized;
            return result;
        }
    }

    public static Vector2 WorldToAnchoredPosition(RectTransform baseRect, Canvas canvas, Camera cam, Vector3 worldPos)
    {
        Vector2 screenPos = cam.WorldToScreenPoint(worldPos);
        Vector2 anchorPos = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(baseRect, screenPos, canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : cam, out anchorPos);
        return anchorPos;
    }

    public static void KillMine(this Tween tween)
    {
        tween.Kill();
        tween = null;
    }

    public static bool IsPlayingMine(this Tween tween)
    {
        return tween != null && tween.IsActive() && tween.IsPlaying();
    }

    public static void DeleteJsonWithKey(string key)
    {
        // Utility.DeleteJsonWithKey(SaveName);
        string file = System.IO.Directory.GetFiles(Application.persistentDataPath).FirstOrDefault(x => x.EndsWith(key));
        if (file != null) System.IO.File.Delete(file);
    }

    public static void AddTo(this Tween tween, List<Tween> tweens)
    {
        tweens.Add(tween);
    }

    public static bool HasPressedUIObject()
    {
#if UNITY_EDITOR
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return true;
#else
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(0)) return true;
#endif
        return false;
    }

}

// DOVirtual.DelayedCall(1, Method).SetLoops(6);
// void Method()
// {
// }

// var anyDuplicate = applyForcesSpline.computerPaths.GroupBy(x => x).Any(g => g.Count() > 1);
// if (anyDuplicate) Debug.LogError("There are same before");



// Vector3 p = new Vector3(i % 4, 0, i / 4);
// Vector3 start = player.transform.position;
// float percent = 0;
// DOTween.To(() => percent, x => percent = x, 1, 1).OnUpdate(() =>
// {
//     player.transform.position = Utility.SampleParabola(start, p, 6, percent);
// });



// Collider[] colliders = new Collider[20];
// public bool HasCollision(Vector3 position, Quaternion rotation, Vector3 extents)
// {
//     for (int i = 0; i < colliders.Length; i++) colliders[i] = null;
//     Physics.OverlapBoxNonAlloc(position, extents, colliders, rotation, maskBox);
//     // Debug.Log("------------------");
//     // colliders.Foreach(x => Debug.Log(x, x));
//     return colliders[0] != null;
// }


// float delay;
// float time;
// DOTween.To(() => delay, x => delay = x, .2f, 1);
// void OnUpdate()
// {
//     time += Time.deltaTime;
//     if (time > delay)
//     {
//         time = 0;
//     }
// }



// Vector3 oldPos;
// private void Start()
// {
//     oldPos = transform.position;
// }
// private void Update()
// {
//     if (oldPos != transform.position)
//     {
//         oldPos = transform.position;
//     }

//     if (transform.position.y - oldPos.y < 0)
//     {

//     }
// }



// durations = new Queue<float>(Enumerable.Range(0, ItemData.Ins.emptySlotQueue.Count).Select(x => randomValueSetting.Duration));
//     WaitForSeconds waitForSeconds1 = new WaitForSeconds(durations.Max());



//EditorUtility.SetDirty(x);
//UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(x.gameObject.scene);,

// float defaultTexScale; tiling değiştirme
// float defaultRestLength;
// defaultTexScale = meshRenderer.material.mainTextureScale.y;
// defaultRestLength = obiRope.restLength;
// meshRenderer.material.SetTextureScale(GameData.Ins.Id_Shader_MainTex, new Vector2(1, defaultTexScale* obiRope.restLength / defaultRestLength));


// Set rect value / Modify the width and height of RectTransform
// float height = transform.GetChild(0).GetComponent<RectTransform>().rect.height * LevelInfoManager.Ins.CurrLevelInfo.stickerColors.Length +
//  space * LevelInfoManager.Ins.CurrLevelInfo.stickerColors.Length;
// rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height);



//https://forum.unity.com/threads/c-color-make-it-lighter.536026/#:~:text=3%2C919-,Depending%20on%20what%20you%20specifically%20want%20to%20do%20with%20the%20colors%2C%20one%20simple%20approach%20is%20just%20to%20multiply%20the%20color%20by%20a%20value%20higher%20than%201%20(lighter)%20or%20lower%20than%20one%20(darker).%20For%20example%3A,-Code%20(CSharp)%3A
// Depending on what you specifically want to do with the colors, one simple approach is just to multiply the color by a value higher than 1 (lighter) or lower than one (darker). For example:
// Code (CSharp):
//var red = Color.red;
//var lightRed = red * 1.5f;
//var darkRed = red * 0.5f;

//Correct alpha.
//lightRed.a = 1;
//darkRed.a = 1;



// how to prevent kill an invalid tween? https://github.com/Demigiant/dotween/issues/285 
// rotTween.Kill();
// rotTween = null;
// rotTween = CurrMold.Value.transform.DORotate(Vector3.right * -25f, .5f);



//  if (EventSystem.current.currentSelectedGameObject == rightButton.gameObject || EventSystem.current.currentSelectedGameObject == leftButton.gameObject)
// {
//     return;
// }



// MoneyInfo.Ins.FlatMoney += 10;
// MoneyInfo.Ins.RunMoneyAnim(1);




// if (SOHolder.Ins.ItemManagerSO.Ins.coliderItemDictionary.TryGetValue(other, out Item item))
// {
// }
// public Dictionary<Collider, Item> coliderItemDictionary;
// coliderItemDictionary = new Dictionary<Collider, Item>();




// GameManager.Ins.onWin += OnWin;
// GameManager.Ins.onFail += OnFail;
// GameManager.Ins.onClickAndLevelStart += OnStart;
// protected virtual void OnStart()
// {
//     this.enabled = true;
// }
// protected virtual void OnWin()
// {
//     this.enabled = false;
// }
// protected virtual void OnFail()
// {
//     this.enabled = false;
// }



// [OnValueChanged("ChangeLength")]
// public float length = 5;
// [ReadOnly, InlineEditor(InlineEditorModes.LargePreview)]
// public Material Material;
// private void ChangeLength()
// {
//     cursor.ChangeLength(length);
// }


// IEnumerator coroutine;
// coroutine = PoolLoop();
// StartCoroutine(coroutine);
// StopCoroutine(coroutine);



// if (EventSystem.current.currentSelectedGameObject == rightButton.gameObject || EventSystem.current.currentSelectedGameObject == leftButton.gameObject)
// {
//     return;
// }


// #if UNITY_EDITOR
//         var anyDuplicate = computerPaths.GroupBy(x => x).Any(g => g.Count() > 1);
//         if (anyDuplicate) Debug.LogError("There are same");
// #endif

//  Observable.Timer(TimeSpan.FromSeconds(1)).RepeatUntilDestroy(gameObject).Subscribe(x => Debug.Log("Her 1 saniyede bir " + Time.time)); //Oyunu durdursan bile çalışmaya devam ediyor.

// if (Input.GetMouseButtonDown(0))
// {
// }
// else if (Input.GetMouseButton(0))
// {
// }
// else if (Input.GetMouseButtonUp(0))
// {
// }
