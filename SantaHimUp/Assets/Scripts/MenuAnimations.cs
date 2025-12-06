
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuAnimations : MonoBehaviour
{
    [SerializeField] private List<Button> buttons = new List<Button>();
    [SerializeField] private float swipeDistance = 800f;
    [SerializeField] private float swipeDuration = 0.35f;
    [SerializeField] private float stagger = 0.03f;

    private readonly List<RectTransform> _rects = new();
    private readonly List<Vector2> _baseAnchoredPos = new();
    private bool _isSwiping;

    private void Awake()
    {
        if (buttons.Count == 0)
            buttons = new List<Button>(GetComponentsInChildren<Button>());

        foreach (var btn in buttons)
        {
            if (btn == null) continue;
            var rt = btn.transform as RectTransform;
            if (rt == null) continue;

            _rects.Add(rt);
            _baseAnchoredPos.Add(rt.anchoredPosition);
            btn.onClick.AddListener(() => { if (!_isSwiping) StartCoroutine(SwipeAllRightRoutine()); });
        }
    }

    public void SwipeAllRight()
    {
        if (!_isSwiping)
            StartCoroutine(SwipeAllRightRoutine());
    }

    public void SwipeAllLeft()
    {
        if (!_isSwiping)
            StartCoroutine(SwipeAllLeftRoutine());
    }

    private IEnumerator SwipeAllRightRoutine()
    {
        _isSwiping = true;

        for (int i = 0; i < _rects.Count; i++)
        {
            var from = _baseAnchoredPos[i];
            var to = from + new Vector2(swipeDistance, 0f);
            StartCoroutine(SwipeOne(_rects[i], from, to, stagger * i));
        }

        yield return WaitTotalTime();
        _isSwiping = false;
    }

    private IEnumerator SwipeAllLeftRoutine()
    {
        _isSwiping = true;

        for (int i = 0; i < _rects.Count; i++)
        {
            int idx = _rects.Count - 1 - i;
            var from = _baseAnchoredPos[idx] + new Vector2(swipeDistance, 0f);
            var to = _baseAnchoredPos[idx];
            StartCoroutine(SwipeOne(_rects[idx], from, to, stagger * i));
        }

        yield return WaitTotalTime();
        _isSwiping = false;
    }

    private IEnumerator SwipeOne(RectTransform rt, Vector2 from, Vector2 to, float delay)
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        float elapsed = 0f;
        while (elapsed < swipeDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / swipeDuration);
            float eased = EaseInOutCubic(progress);
            rt.anchoredPosition = Vector2.Lerp(from, to, eased);
            yield return null;
        }

        rt.anchoredPosition = to;
    }

    private IEnumerator WaitTotalTime()
    {
        float totalTime = (stagger * (_rects.Count - 1)) + swipeDuration;
        yield return new WaitForSeconds(totalTime);
    }

    private float EaseInOutCubic(float x)
    {
        return x < 0.5f ? 4f * x * x * x : 1f - Mathf.Pow(-2f * x + 2f, 3f) / 2f;
    }

    [ContextMenu("Reset To Base Positions")]
    public void ResetToBasePositions()
    {
        for (int i = 0; i < _rects.Count; i++)
            if (_rects[i] != null)
                _rects[i].anchoredPosition = _baseAnchoredPos[i];
    }
}
