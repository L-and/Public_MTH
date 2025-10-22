using System;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// _targetRecoil에 즉각적으로 반동을 추가하고,
/// Update루프에서 _targetRotation을 지속적으로 원점으로 돌려놓으려고 하며,
/// _currentRotation을 _targetRotation을 추적하도록 하여 반동을 시각화하는 스크립트
/// </summary>
public class Recoil : MonoBehaviour
{
    // Rotations
    private Vector3 _currentRotation; // 현재 회전값
    private Vector3 _targetRotation; // 반동이 적용된 회전값
    
    [SerializeField] private float recoilX;
    [SerializeField] private float recoilY;
    [SerializeField] private float recoilZ;

    [Header("반동 적용/복귀속도 설정값")]
    [SerializeField] private float snappiness;
    [SerializeField] private float returnSpeed;
    
    private void Update()
    {
        // ApplyRecoil에서 더해진 회전값을 지속적으로 원점(Vector3.zero)로 복귀시킴
        _targetRotation = Vector3.Lerp(_targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        // 현재 회전값이 지속적으로 _targetRotation을 추적하며 반동을 시각화 함
        _currentRotation = Vector3.Slerp(_currentRotation, _targetRotation, snappiness * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(_currentRotation);
    }

    /// <summary>
    /// _targetRotation에 반동을 추가하는 메서드
    /// </summary>
    public void ApplyRecoil()
    {
        _targetRotation += new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
    }
}
