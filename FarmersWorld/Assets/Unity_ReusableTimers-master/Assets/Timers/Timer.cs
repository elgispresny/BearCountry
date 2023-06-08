using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using TMPro;

public class Timer : MonoBehaviour
{
    [Header("Timer UI references:")]
    [SerializeField] private Image uiFillImage;
    [SerializeField] private TMP_Text uiText;

    public float Duration { get; private set; }

    public bool IsPaused { get; private set; }

    private float remainingDuration;

    // Events
    private UnityAction onTimerBeginAction;
    private UnityAction<float> onTimerChangeAction;
    private UnityAction onTimerEndAction;
    private UnityAction<bool> onTimerPauseAction;

    private void Awake()
    {
        // ResetTimer();
    }

    public void ResetTimer(Tool tool)
    {
        tool = GameDataManager.GetSelectedCharacter();
        uiText.text = tool.timerDuration.ToString("00:00.00");
        uiFillImage.fillAmount = 0f;

        Duration = remainingDuration = 0f;

        onTimerBeginAction = null;
        onTimerChangeAction = null;
        onTimerEndAction = null;
        onTimerPauseAction = null;

        IsPaused = false;
    }

    public void SetPaused(bool paused)
    {
        IsPaused = paused;

        if (onTimerPauseAction != null)
            onTimerPauseAction.Invoke(IsPaused);
    }

    public Timer SetDuration(float seconds)
    {
        Duration = remainingDuration = seconds;
        return this;
    }

    // Events
    public Timer OnBegin(UnityAction action)
    {
        onTimerBeginAction = action;
        return this;
    }

    public Timer OnChange(UnityAction<float> action)
    {
        onTimerChangeAction = action;
        return this;
    }

    public Timer OnEnd(UnityAction action)
    {
        onTimerEndAction = action;
        return this;
    }

    public Timer OnPause(UnityAction<bool> action)
    {
        onTimerPauseAction = action;
        return this;
    }

    public void Begin()
    {
        if (onTimerBeginAction != null)
            onTimerBeginAction.Invoke();

        StopAllCoroutines();
        StartCoroutine(UpdateTimer());
    }

    private IEnumerator UpdateTimer()
    {
        while (remainingDuration > 0f)
        {
            if (!IsPaused)
            {
                if (onTimerChangeAction != null)
                    onTimerChangeAction.Invoke(remainingDuration);

                UpdateUI(remainingDuration);
                remainingDuration -= Time.deltaTime;
            }
            yield return null;
        }
        End();
    }

    private void UpdateUI(float seconds)
{
    int hours = (int)seconds / 3600;
    int minutes = ((int)seconds / 60) % 60;
    float remainingSeconds = seconds % 60;
    float milliseconds = (remainingSeconds - (int)remainingSeconds) * 100f;

    uiText.text = string.Format("{0:D2}:{1:D2}:{2:D2}.{3:00}", hours, minutes, (int)remainingSeconds, (int)milliseconds);
    uiFillImage.fillAmount = Mathf.InverseLerp(0f, Duration, seconds);
}


    public void End()
    {
        if (onTimerEndAction != null)
            onTimerEndAction.Invoke();

       // ResetTimer();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    public void StartTimer(Tool tool)
{
    tool = GameDataManager.GetSelectedCharacter();
    float seconds = tool.timerDuration;
    int hours = (int)seconds / 3600;
    int minutes = ((int)seconds / 60) % 60;
    float remainingSeconds = seconds % 60;
    float milliseconds = (remainingSeconds - (int)remainingSeconds) * 100f;

    uiText.text = string.Format("{0:D2}:{1:D2}:{2:D2}.{3:00}", hours, minutes, (int)remainingSeconds, (int)milliseconds);
}

}
