using System.Collections;
using UnityEngine;

public class DoorClose : MonoBehaviour
{
    [Header("엘리베이터 문 오브젝트 연결")]
    public Transform leftDoor;
    public Transform rightDoor;

    [Header("문 닫히는 거리 및 속도")]
    public float moveDistance = 1.5f;  // 문이 얼마나 열릴지
    public float closeSpeed = 2f; // 문 열림닫힘 속도
    public float autoCloseDelay = 2f; //문 열림 후 닫히기까지 대기 시간

    public Vector3 leftDoorOpenPos;
    public Vector3 rightDoorOpenPos;
    public Vector3 leftDoorClosedPos;
    public Vector3 rightDoorClosedPos;
    
    private Coroutine currentRoutine;

    void Start()
    {
        // 처음 위치 저장
        leftDoorClosedPos = leftDoor.localPosition;
        rightDoorClosedPos = rightDoor.localPosition;

        // 열림 위치 계산 (닫힌 위치에서 반대)
        leftDoorOpenPos = leftDoorClosedPos + Vector3.forward * moveDistance;
        rightDoorOpenPos = rightDoorClosedPos + Vector3.back * moveDistance;

        // 시작 시 문 닫힌 상태 유지
        leftDoor.localPosition = leftDoorClosedPos;
        rightDoor.localPosition = rightDoorClosedPos;
    }
     
    public void OpenDoors()
    {
        if (currentRoutine != null) StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(DoorRoutine(true));
    }

    public void CloseDoors()
    {
        if (currentRoutine != null) StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(DoorRoutine(false));
    }

    IEnumerator DoorRoutine(bool open)
    { 
        float t = 0f;

        // 현재 문 위치 기준으로 열림닫힘
        Vector3 leftStart = leftDoor.localPosition;
        Vector3 rightStart = rightDoor.localPosition;

        // open이 true, false에 따라 open,closed 쓰기
        Vector3 leftTarget = open ? leftDoorOpenPos : leftDoorClosedPos;
        Vector3 rightTarget = open ? rightDoorOpenPos : rightDoorClosedPos;

        while (t < 1f)
        {
            t += Time.deltaTime * closeSpeed;
            leftDoor.localPosition = Vector3.Lerp(leftStart, leftTarget, t);
            rightDoor.localPosition = Vector3.Lerp(rightStart,rightTarget, t);
            
            yield return null;
        }

        // 열렸을 때 일정 시간 후 닫기
        if (open)
        {
            yield return new WaitForSeconds(autoCloseDelay);
            CloseDoors();
        }
    }
}
