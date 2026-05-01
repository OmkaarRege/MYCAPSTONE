using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class GameIntro : MonoBehaviour
{
    [Header("UI Reference")]
    public RectTransform textRect; // assign your TMP object here

    [Header("Positions (anchored)")]
    public Vector2 startPos;
    public Vector2 middlePos;
    public Vector2 topPos;

    [Header("Timing")]
    public float moveToMiddleDuration = 2f;
    public float hoverDuration = 4f;
    public float moveToTopDuration = 2f;

    private float timer = 0f;
    private enum State { MoveToMiddle, Hover, MoveToTop, Done }
    private State currentState = State.MoveToMiddle;

    void Start()
    {
        if (textRect != null)
        {
            textRect.anchoredPosition = startPos;
        }
    }

    void Update()
    {
        if (textRect == null) return;

        timer += Time.deltaTime;

        switch (currentState)
        {
            case State.MoveToMiddle:
                Move(textRect, startPos, middlePos, moveToMiddleDuration);
                if (timer >= moveToMiddleDuration)
                {
                    NextState(State.Hover);
                }
                break;

            case State.Hover:
                if (timer >= hoverDuration)
                {
                    NextState(State.MoveToTop);
                }
                break;

            case State.MoveToTop:
                Move(textRect, middlePos, topPos, moveToTopDuration);
                if (timer >= moveToTopDuration)
                {
                    currentState = State.Done;
                    SceneManager.LoadScene("MainScene");
                }
                break;
        }
    }

    void Move(RectTransform rect, Vector2 from, Vector2 to, float duration)
    {
        float t = Mathf.Clamp01(timer / duration);
        t = Mathf.SmoothStep(0f, 1f, t); // smoother motion
        rect.anchoredPosition = Vector2.Lerp(from, to, t);
    }

    void NextState(State next)
    {
        currentState = next;
        timer = 0f;
    }
}
