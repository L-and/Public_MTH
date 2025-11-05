using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeltSamplePlayer : MonoBehaviour {

    enum State {
        None,
        Melt,
        Appear,
    }
    public GameObject meltObject;
    public MeshRenderer[] meltMeshes;
    public SkinnedMeshRenderer[] meltSkinnedMeshes;

    List<Material> meltMaterials = new List<Material>();
    float meltTime;
    State state;
    float meltDuration = 5.0f;
    float meltHeight = 2.0f;
    float spreadPower = 2.2f;
    float tiltAngle = 15.0f;

	void Start() {
        foreach (var mesh in meltMeshes) {
            foreach (var material in mesh.materials) {
                meltMaterials.Add(material);
            }
        }
        foreach (var mesh in meltSkinnedMeshes) {
            foreach (var material in mesh.materials) {
                meltMaterials.Add(material);
            }
        }
    }

    void Update() {
        meltTime += Time.deltaTime;
        float meltRate = 0.0f;
        switch (state) {
        case State.Melt:
            meltRate = Mathf.Min(meltTime / meltDuration, 1.0f);
            break;
        case State.Appear:
            meltRate = 1.0f - Mathf.Min(meltTime / meltDuration, 1.0f);
            break;
        }
        var basePos = new Vector4();
        basePos.x = meltObject.transform.position.x;
        basePos.y = meltObject.transform.position.y;
        basePos.z = meltObject.transform.position.z;
        foreach (var material in meltMaterials) {
            material.SetVector("_ObjectBasePos", basePos);
            material.SetFloat("_MeltRate", meltRate);
            material.SetFloat("_TiltAngle", tiltAngle);
            material.SetFloat("_ObjectMeltHeight", meltHeight);
            material.SetFloat("_SpreadPower", spreadPower);
        }
    }

    public void StartMelt() {
        meltTime = 0.0f;
        state = State.Melt;
    }

    public void StartAppear() {
        meltTime = 0.0f;
        state = State.Appear;
    }

    public void DurationValueChange(float value) {
        meltDuration = value;
    }

    public void MeltHeightChange(float value) {
        meltHeight = value;
    }

    public void TiltAngleChange(float value) {
        tiltAngle = value;
    }

    public void SpreadPowerChange(float value) {
        spreadPower = value;
    }
}
