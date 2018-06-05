using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zara.Common.Utility;

namespace Zara.Common.ExCamera
{
    public interface ICameraListHolder
    {
        IReadOnlyList<CameraInStudio> GetCameraList(int studioId);
    }

    public class CameraDepthModifier
    {
        Dictionary<int, Studio> studios;
        ICameraListHolder cameraListHolder;

        public CameraDepthModifier(
            IEnumerable<StudioSetting> studioSettings,
            ICameraListHolder cameraListHolder)
        {
            InitStudios(studioSettings);
            this.cameraListHolder = cameraListHolder;
        }

        void InitStudios(IEnumerable<StudioSetting> studioSettings)
        {
            int minDepth = 0;
            studios = new Dictionary<int, Studio>();
            foreach (var setting in studioSettings)
            {
                var studio = new Studio(setting, minDepth);
                studios.Add(setting.Id, studio);
                minDepth = studio.ExclusiveMaxDepth;
            }
        }

        public void SetDepth(int studioId)
        {
            if (CanHoldCameras(studioId))
                ModifyDepth(studioId);
            else
                // Strudio情報を更新する必要があるのですべてのCameraを調整する
                SetDepthAll();
        }

        // Camera数が予定数以内か確認する
        bool CanHoldCameras(int studioId)
        {
            Studio studio = studios.GetValue(studioId);
            if (studio == null)
                return true;

            IReadOnlyList<CameraInStudio> cameraList = cameraListHolder.GetCameraList(studioId);
            if (studio.Setting.PlannedMaxCameraCount >= cameraList.Count)
                return true;

            Debug.LogError($"Unexpected camera count: studioId={studioId}, plannedMaxCameraCount={studio.Setting.PlannedMaxCameraCount}, currentCameraCount={cameraList.Count}");
            return false;
        }

        // Depthの値が指定順に並んでいるか確認する
        // 並んでいない場合は調整する
        void ModifyDepth(int studioId)
        {
            Studio studio = studios.GetValue(studioId);
            IReadOnlyList<CameraInStudio> cameraList = cameraListHolder.GetCameraList(studioId);

            if (studio == null && !cameraList.IsEmpty())
            {
                Debug.LogError($"Cannot get studio: studioId={studioId}");
                return;
            }

            int usableMinDepth = studio.MinDepth;
            for (int i = 0; i < cameraList.Count; i++)
            {
                CameraInStudio camera = cameraList[i];
                float currentDepth = camera.CameraOperator.CameraDepth;

                int usableExclusiveMaxDepth = studio.ExclusiveMaxDepth - (cameraList.Count - i - 1) * studio.Setting.DepthInterval;
                if (currentDepth < (float)usableMinDepth ||
                    currentDepth >= (float)usableExclusiveMaxDepth)
                {
                    camera.CameraOperator.SetDepth((float)usableMinDepth);
                    currentDepth = usableMinDepth;
                }

                usableMinDepth = Mathf.CeilToInt(currentDepth) + studio.Setting.DepthInterval;
            }
        }

        void SetDepthAll()
        {
            ExpandCameraCapacity();

            foreach (int studioId in studios.Keys)
                ModifyDepth(studioId);
        }

        void ExpandCameraCapacity()
        {
            Debug.LogError("Change camera studio setting. Review camera studio setting.");

            StudioSetting[] newSettings = studios.Values
                .Select(studio =>
                {
                    StudioSetting setting = studio.Setting;
                    IReadOnlyList<CameraInStudio> cameraList = cameraListHolder.GetCameraList(studio.Setting.Id);

                    return new StudioSetting
                    (
                        setting.Id,
                        setting.DepthInterval,
                        Mathf.Max(setting.PlannedMaxCameraCount, cameraList.Count)
                    );
                })
                .ToArray();

            InitStudios(newSettings);
        }
    }
}