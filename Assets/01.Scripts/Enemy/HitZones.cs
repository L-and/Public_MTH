using UnityEngine;

public enum HitZones { Weak, Body }

public class HitZone: MonoBehaviour
{
    public HitZones zone = HitZones.Body;
}