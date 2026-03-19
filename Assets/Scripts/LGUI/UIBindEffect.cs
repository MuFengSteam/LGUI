// 作者: 木枫LL
// GitHub: https://github.com/MuFengSteam/
// 小红书: https://www.xiaohongshu.com/user/profile/67c5dc1a000000000a03e5d0

using UnityEngine;
using System.Collections.Generic;

public class UIBindEffect : UIBase
{
    [System.Serializable]
    public class EffectItem
    {
        [Tooltip("特效名称")]
        public string effectName;

        [Tooltip("特效对象（ParticleSystem或Animation）")]
        public GameObject effectObject;

        [Tooltip("是否循环播放")]
        public bool loop = false;

        [Tooltip("播放完成后是否隐藏")]
        public bool hideOnComplete = true;
    }

    [Header("特效设置")]
    [Tooltip("特效列表")]
    [SerializeField] private List<EffectItem> m_Effects = new List<EffectItem>();

    [Tooltip("是否在Start时自动收集子节点的特效")]
    [SerializeField] private bool m_AutoCollect = false;

    private Dictionary<string, EffectItem> _effectDict = new Dictionary<string, EffectItem>();
    private Dictionary<string, ParticleSystem> _particleDict = new Dictionary<string, ParticleSystem>();
    private Dictionary<string, Animation> _animationDict = new Dictionary<string, Animation>();
    private Dictionary<string, Animator> _animatorDict = new Dictionary<string, Animator>();

    public override string ComponentTypeName => "UIBindEffect";
    public override string BindDataType => "UIBindEffect";

    protected override void Awake()
    {
        base.Awake();
        BuildEffectDictionary();
    }

    private void Start()
    {
        if (m_AutoCollect)
        {
            AutoCollectEffects();
        }
    }

    private void BuildEffectDictionary()
    {
        _effectDict.Clear();
        _particleDict.Clear();
        _animationDict.Clear();
        _animatorDict.Clear();

        foreach (var item in m_Effects)
        {
            if (string.IsNullOrEmpty(item.effectName) || item.effectObject == null)
                continue;

            _effectDict[item.effectName] = item;

            ParticleSystem ps = item.effectObject.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                _particleDict[item.effectName] = ps;
            }

            Animation anim = item.effectObject.GetComponent<Animation>();
            if (anim != null)
            {
                _animationDict[item.effectName] = anim;
            }

            Animator animator = item.effectObject.GetComponent<Animator>();
            if (animator != null)
            {
                _animatorDict[item.effectName] = animator;
            }
        }
    }

    private void AutoCollectEffects()
    {

        ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>(true);
        foreach (var ps in particles)
        {
            string name = ps.gameObject.name;
            if (!_effectDict.ContainsKey(name))
            {
                var item = new EffectItem
                {
                    effectName = name,
                    effectObject = ps.gameObject,
                    loop = ps.main.loop,
                    hideOnComplete = true
                };
                m_Effects.Add(item);
                _effectDict[name] = item;
                _particleDict[name] = ps;
            }
        }

        Animation[] animations = GetComponentsInChildren<Animation>(true);
        foreach (var anim in animations)
        {
            string name = anim.gameObject.name;
            if (!_effectDict.ContainsKey(name))
            {
                var item = new EffectItem
                {
                    effectName = name,
                    effectObject = anim.gameObject,
                    loop = false,
                    hideOnComplete = true
                };
                m_Effects.Add(item);
                _effectDict[name] = item;
                _animationDict[name] = anim;
            }
        }
    }

    public void Play(string effectName)
    {
        if (string.IsNullOrEmpty(effectName))
        {
            return;
        }

        if (!_effectDict.TryGetValue(effectName, out var item))
        {
            return;
        }

        item.effectObject.SetActive(true);

        if (_particleDict.TryGetValue(effectName, out var ps))
        {
            ps.Play(true);

            if (!item.loop && item.hideOnComplete)
            {
                StartCoroutine(HideAfterParticleComplete(effectName, ps));
            }
        }

        if (_animationDict.TryGetValue(effectName, out var anim))
        {
            anim.Play();

            if (!item.loop && item.hideOnComplete)
            {
                StartCoroutine(HideAfterAnimationComplete(effectName, anim));
            }
        }

        if (_animatorDict.TryGetValue(effectName, out var animator))
        {
            animator.enabled = true;
            animator.Play(effectName, 0, 0);
        }
    }

    public void PlayByIndex(int index)
    {
        if (index >= 0 && index < m_Effects.Count)
        {
            Play(m_Effects[index].effectName);
        }
    }

    public void Stop(string effectName)
    {
        if (string.IsNullOrEmpty(effectName))
            return;

        if (!_effectDict.TryGetValue(effectName, out var item))
            return;

        if (_particleDict.TryGetValue(effectName, out var ps))
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

        if (_animationDict.TryGetValue(effectName, out var anim))
        {
            anim.Stop();
        }

        if (_animatorDict.TryGetValue(effectName, out var animator))
        {
            animator.enabled = false;
        }

        if (item.hideOnComplete)
        {
            item.effectObject.SetActive(false);
        }
    }

    public void StopAll()
    {
        foreach (var item in m_Effects)
        {
            Stop(item.effectName);
        }
    }

    public void Pause(string effectName)
    {
        if (_particleDict.TryGetValue(effectName, out var ps))
        {
            ps.Pause(true);
        }

        if (_animatorDict.TryGetValue(effectName, out var animator))
        {
            animator.speed = 0;
        }
    }

    public void Resume(string effectName)
    {
        if (_particleDict.TryGetValue(effectName, out var ps))
        {
            ps.Play(true);
        }

        if (_animatorDict.TryGetValue(effectName, out var animator))
        {
            animator.speed = 1;
        }
    }

    public bool IsPlaying(string effectName)
    {
        if (_particleDict.TryGetValue(effectName, out var ps))
        {
            return ps.isPlaying;
        }

        if (_animationDict.TryGetValue(effectName, out var anim))
        {
            return anim.isPlaying;
        }

        return false;
    }

    public void SetSpeed(string effectName, float speed)
    {
        if (_particleDict.TryGetValue(effectName, out var ps))
        {
            var main = ps.main;
            main.simulationSpeed = speed;
        }

        if (_animatorDict.TryGetValue(effectName, out var animator))
        {
            animator.speed = speed;
        }
    }

    public void AddEffect(string effectName, GameObject effectObject, bool loop = false, bool hideOnComplete = true)
    {
        if (string.IsNullOrEmpty(effectName) || effectObject == null)
            return;

        var item = new EffectItem
        {
            effectName = effectName,
            effectObject = effectObject,
            loop = loop,
            hideOnComplete = hideOnComplete
        };

        m_Effects.Add(item);
        _effectDict[effectName] = item;

        ParticleSystem ps = effectObject.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            _particleDict[effectName] = ps;
        }

        Animation anim = effectObject.GetComponent<Animation>();
        if (anim != null)
        {
            _animationDict[effectName] = anim;
        }

        Animator animator = effectObject.GetComponent<Animator>();
        if (animator != null)
        {
            _animatorDict[effectName] = animator;
        }
    }

    public void RemoveEffect(string effectName)
    {
        if (string.IsNullOrEmpty(effectName))
            return;

        _effectDict.Remove(effectName);
        _particleDict.Remove(effectName);
        _animationDict.Remove(effectName);
        _animatorDict.Remove(effectName);

        m_Effects.RemoveAll(e => e.effectName == effectName);
    }

    private System.Collections.IEnumerator HideAfterParticleComplete(string effectName, ParticleSystem ps)
    {
        yield return new WaitUntil(() => !ps.isPlaying);

        if (_effectDict.TryGetValue(effectName, out var item) && item.hideOnComplete)
        {
            item.effectObject.SetActive(false);
        }
    }

    private System.Collections.IEnumerator HideAfterAnimationComplete(string effectName, Animation anim)
    {
        yield return new WaitUntil(() => !anim.isPlaying);

        if (_effectDict.TryGetValue(effectName, out var item) && item.hideOnComplete)
        {
            item.effectObject.SetActive(false);
        }
    }

#if UNITY_EDITOR
    public override string GetValidationError()
    {
        if (!HasValidBindName)
        {
            return $"UIBindEffect组件 [{gameObject.name}] 的bindName未设置";
        }
        return null;
    }

    [ContextMenu("自动收集子节点特效")]
    private void EditorAutoCollect()
    {
        AutoCollectEffects();
        BuildEffectDictionary();
    }
#endif
}
