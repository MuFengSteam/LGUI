// 作者: 木枫LL
// GitHub: https://github.com/MuFengSteam/
// 小红书: https://www.xiaohongshu.com/user/profile/67c5dc1a000000000a03e5d0

using UnityEngine;

public class UIReference : UIBase
{
    public override string ComponentTypeName => "UIReference";
    public override string BindDataType => "GameObject";

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public RectTransform GetRectTransform()
    {
        return GetComponent<RectTransform>();
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    public bool IsActive()
    {
        return gameObject.activeSelf;
    }

    public void SetParent(Transform parent, bool worldPositionStays = false)
    {
        transform.SetParent(parent, worldPositionStays);
    }

    public void SetPosition(Vector3 position)
    {
        RectTransform rt = GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchoredPosition3D = position;
        }
        else
        {
            transform.localPosition = position;
        }
    }

    public void SetPosition(Vector2 position)
    {
        RectTransform rt = GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchoredPosition = position;
        }
        else
        {
            transform.localPosition = new Vector3(position.x, position.y, 0);
        }
    }

    public void SetScale(Vector3 scale)
    {
        transform.localScale = scale;
    }

    public void SetScale(float scale)
    {
        transform.localScale = new Vector3(scale, scale, scale);
    }

#if UNITY_EDITOR
    public override string GetValidationError()
    {
        if (!HasValidBindName)
        {
            return $"UIReference组件 [{gameObject.name}] 的bindName未设置";
        }
        return null;
    }
#endif
}
