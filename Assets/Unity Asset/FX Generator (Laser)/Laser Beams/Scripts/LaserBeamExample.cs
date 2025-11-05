using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace LaserBeams
{
    public class LaserBeamExample : MonoBehaviour
    {
        public List<GameObject> laserBeams; // 存储所有可能的激光预制体
        public Dropdown beamTypeDropdown; // 下拉框用于选择 BeamType
        public Button nextBeamButton; // 按钮用于切换下一个激光束预制体
        private int currentBeamIndex = 0; // 当前激光束预制体索引

        public LaserBeamEmitter laserBeamEmitter;

        void Start()
        {
            // 初始化下拉框
            InitializeBeamTypeDropdown();
            beamTypeDropdown.onValueChanged.AddListener(OnBeamTypeChanged);

            // 初始化按钮
            nextBeamButton.onClick.AddListener(OnNextBeamButtonClicked);

            // 设置初始激光束预制体
            if (laserBeams.Count > 0)
            {
                SetLaserBeamPrefab(laserBeams[currentBeamIndex]);
            }
        }

        // 初始化下拉框
        void InitializeBeamTypeDropdown()
        {
            beamTypeDropdown.ClearOptions();
            List<string> options = new List<string>();
            foreach (BeamType beamType in System.Enum.GetValues(typeof(BeamType)))
            {
                options.Add(beamType.ToString());
            }
            beamTypeDropdown.AddOptions(options);
        }

        // 当下拉框值变化时调用
        void OnBeamTypeChanged(int index)
        {
            BeamType selectedBeamType = (BeamType)index;
            laserBeamEmitter.beamType = selectedBeamType;
            laserBeamEmitter.InstantiateLaserBeam(laserBeamEmitter.beamType, laserBeamEmitter.laserBeamPrefab);
        }

        // 当按钮被点击时调用
        void OnNextBeamButtonClicked()
        {
            currentBeamIndex = (currentBeamIndex + 1) % laserBeams.Count;
            SetLaserBeamPrefab(laserBeams[currentBeamIndex]);
        }

        // 设置激光束预制体
        void SetLaserBeamPrefab(GameObject laserBeamPrefab)
        {
            laserBeamEmitter.InstantiateLaserBeam(laserBeamEmitter.beamType, laserBeamPrefab);
        }
    }
}