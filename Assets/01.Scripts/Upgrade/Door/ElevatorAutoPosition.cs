using _01.Scripts.PlayerControll;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ElevatorAutoPosition : MonoBehaviour
{
    [Header("플레이어 태그")]
    public string playerTag = "Player";

    [Header("플레이어가 이동할 목표 위치")]
    public Transform targetPosition; // Player elv position 

    [Header("플레이어가 바라볼 방향(문쪽)")]
    public Transform doorFront; // elv door position 

    [Header("이동 설정")]
    public float moveSpeed = 3f;
    public float rotateSpeed = 5f;

    private bool isMoving = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag) && !isMoving)
        {
            StartCoroutine(MoveAndTurnPlayer(other.gameObject));
        }
    }

    private IEnumerator MoveAndTurnPlayer(GameObject player)
    {
        isMoving = true;

        // 플레이어 조작 잠금
        var playerController = player.GetComponent<PlayerController>();
        var aimLook = player.GetComponentInChildren<AimLook>(); 

        if (playerController != null) playerController.enabled = false;
        if (aimLook != null) aimLook.enabled = false;

        // 리지드바디 정지
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        Transform cameraRoot = aimLook != null ? aimLook.transform : player.transform;

        // 목표 위치 고정 및 문쪽으로 플레이어 몸 회전
        Debug.Log("플레이어 목표 위치 이동");
        Vector3 startPos = player.transform.position;
        Quaternion fixedRot = player.transform.rotation;
        Vector3 endPos = new Vector3(targetPosition.position.x, startPos.y, targetPosition.position.z);

        float moveT = 0f;
        while (moveT < 1f)
        {
            moveT += Time.deltaTime * moveSpeed;
            player.transform.position = Vector3.Lerp(startPos, endPos, moveT);
            player.transform.rotation = fixedRot;
            cameraRoot.rotation = fixedRot;
            yield return null;
        }

        // 위치 고정
        player.transform.position = endPos;

        // 도착 후 문 쪽으로 몸, 시점 회전
        Quaternion startRot = player.transform.rotation;
        Quaternion endRot = Quaternion.LookRotation(endPos -doorFront.position);

        float rotT = 0f;
        while (rotT < 1f)
        {
            rotT += Time.deltaTime * rotateSpeed;
            player.transform.rotation = Quaternion.Slerp(startRot, endRot, rotT);
            cameraRoot.rotation = Quaternion.Slerp(startRot, endRot, rotT);
            yield return null;
        }
        
        // 회전 정확히 맞추기
        player.transform.rotation = endRot;
        cameraRoot.rotation = endRot;

        // 커서만 활성화 
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        yield return new WaitForSeconds(0.5f);

        // 플레이어 이동,시점 조작 다시 활성화
        if (playerController != null) playerController.enabled = true;
        if (aimLook != null) aimLook.enabled = true;

        isMoving = false;
    }
}
