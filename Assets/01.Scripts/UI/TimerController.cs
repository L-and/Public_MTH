using System;
using TMPro;
using UnityEngine;

public class TimerController : MonoBehaviour
{
    [Header("타이머 설정")]
    [Tooltip("타이머가 시작할 시간(초)")]
    [SerializeField] private float startTime = 60f;

    [Header("UI 연결")]
    [Tooltip("시간을 표시할 TextMeshProUGUI 컴포넌트")]
    [SerializeField] private TextMeshProUGUI timerText;

    private float currentTime; // 현재 남은 시간을 저장하는 변수
    private bool isTimerRunning = true; // 타이머 실행 상태

    void Start()
    {
        // 시작 시간을 현재 시간으로 설정
        currentTime = startTime;

        if (timerText == null)
        {
            isTimerRunning = false; // 타이머 실행 중지
        }
    }

    void Update()
    {
        if (isTimerRunning)
        {
            currentTime -= Time.deltaTime;

            if (currentTime > 0)
            {
                DisplayTime(currentTime);
            }
            else
            {
                currentTime = 0; // 정확히 0으로 설정
                isTimerRunning = false; // 타이머 정지
                DisplayTime(currentTime); // 최종 시간 표시 (00:00)

                // 시간 종료 시 실행할 함수 호출
                OnTimerEnd();
            }
        }
    }


    void DisplayTime(float timeToDisplay)
    {
        // 시간을 TimeSpan 객체로 변환하여 포맷팅을 쉽게 합니다.
        // timeToDisplay는 초 단위이므로 1000을 곱해 밀리초 단위로 변환합니다.
        TimeSpan time = TimeSpan.FromSeconds(timeToDisplay);

        // "mm\:ss" 형식으로 포맷팅합니다. (예: 01:30)
        string timeFormat = string.Format("{0:00}:{1:00}", time.Minutes, time.Seconds);

        // UI Text에 표시합니다.
        timerText.text = timeFormat;
    }

    void OnTimerEnd()
    {

    }

    public void StartTimer(float newStartTime)
    {
        startTime = newStartTime;
        currentTime = startTime;
        isTimerRunning = true;
    }
}
