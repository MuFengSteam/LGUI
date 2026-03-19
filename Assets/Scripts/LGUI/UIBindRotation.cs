using UnityEngine;

public class UIBindRotation : UIBase
{
    public enum RotationMode
    {
        [Tooltip("仅Z轴旋转（2D UI常用）")]
        ZAxis,
        [Tooltip("完整的3D旋转")]
        Full3D
    }

    [Header("旋转设置")]
    [Tooltip("旋转模式")]
    public RotationMode rotationMode = RotationMode.ZAxis;

    [Tooltip("默认旋转角度（Z轴）")]
    public float defaultRotation = 0f;

    [Tooltip("是否在Start时设置默认值")]
    public bool setDefaultOnStart = true;

    private RectTransform _rectTransform;

    public override string ComponentTypeName => "UIBindRotation";
    public override string BindDataType => "float";

#if UNITY_EDITOR
    protected override void Reset()
    {
        base.Reset();
        EnsureRectTransform();
    }
#endif

    private void Start()
    {
        if (setDefaultOnStart)
        {
            SetRotationZ(defaultRotation);
        }
    }

    protected override void Initialize()
    {
        if (_initialized) return;
        EnsureRectTransform();
        base.Initialize();
    }

    private void EnsureRectTransform()
    {
        _rectTransform = GetComponent<RectTransform>();
        if (_rectTransform == null)
        {
            _rectTransform = gameObject.AddComponent<RectTransform>();
        }
    }

    public void SetRotationZ(float angle)
    {
        EnsureInitialized();

        if (_rectTransform != null)
        {
            Vector3 euler = _rectTransform.localEulerAngles;
            euler.z = angle;
            _rectTransform.localEulerAngles = euler;
        }
    }

    public float GetRotationZ()
    {
        EnsureInitialized();

        if (_rectTransform != null)
        {
            return _rectTransform.localEulerAngles.z;
        }
        return 0f;
    }

    public void SetRotation(Vector3 eulerAngles)
    {
        EnsureInitialized();

        if (_rectTransform != null)
        {
            _rectTransform.localEulerAngles = eulerAngles;
        }
    }

    public Vector3 GetRotation()
    {
        EnsureInitialized();

        if (_rectTransform != null)
        {
            return _rectTransform.localEulerAngles;
        }
        return Vector3.zero;
    }

    public void SetQuaternion(Quaternion rotation)
    {
        EnsureInitialized();

        if (_rectTransform != null)
        {
            _rectTransform.localRotation = rotation;
        }
    }

    public Quaternion GetQuaternion()
    {
        EnsureInitialized();

        if (_rectTransform != null)
        {
            return _rectTransform.localRotation;
        }
        return Quaternion.identity;
    }

    public void RotateToZ(float targetAngle, float duration = 0.3f)
    {
        StartCoroutine(RotateCoroutine(targetAngle, duration));
    }

    private System.Collections.IEnumerator RotateCoroutine(float targetAngle, float duration)
    {
        float startAngle = GetRotationZ();
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            SetRotationZ(Mathf.LerpAngle(startAngle, targetAngle, t));
            yield return null;
        }

        SetRotationZ(targetAngle);
    }

    public void StartContinuousRotation(float speed = 90f)
    {
        StartCoroutine(ContinuousRotateCoroutine(speed));
    }

    private System.Collections.IEnumerator ContinuousRotateCoroutine(float speed)
    {
        while (true)
        {
            float currentZ = GetRotationZ();
            SetRotationZ(currentZ + speed * Time.deltaTime);
            yield return null;
        }
    }

    public void StopRotation()
    {
        StopAllCoroutines();
    }

#if UNITY_EDITOR
    public override string GetValidationError()
    {
        if (!HasValidBindName)
        {
            return $"UIBindRotation组件 [{gameObject.name}] 的bindName未设置";
        }
        return null;
    }
#endif
}
