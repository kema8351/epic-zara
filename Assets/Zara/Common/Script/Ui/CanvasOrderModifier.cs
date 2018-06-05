using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zara.Common.Utility;

namespace Zara.Common.Ui
{
    public interface ICanvasListHolder
    {
        IReadOnlyList<CanvasInStratum> GetCanvasList(int stratumId);
    }

    public class CanvasOrderModifier
    {
        Dictionary<int, Stratum> strata;
        ICanvasListHolder canvasListHolder;

        public CanvasOrderModifier(
            IEnumerable<StratumSetting> stratumSettings,
            ICanvasListHolder canvasListHolder)
        {
            InitStrata(stratumSettings);
            this.canvasListHolder = canvasListHolder;
        }

        void InitStrata(IEnumerable<StratumSetting> stratumSettings)
        {
            int minOrder = 0;
            strata = new Dictionary<int, Stratum>();
            foreach (var setting in stratumSettings)
            {
                var stratum = new Stratum(setting, minOrder);
                strata.Add(setting.Id, stratum);
                minOrder = stratum.ExclusiveMaxOrder;
            }
        }

        public void SetOrder(int stratumId)
        {
            if (CanHoldCanvases(stratumId))
                ModifyOrder(stratumId);
            else
                // Stratum情報を更新する必要があるのですべてのCanvasを調整する
                SetOrderAll();
        }

        // Canvas数が予定数以内か確認する
        bool CanHoldCanvases(int stratumId)
        {
            Stratum stratum = strata.GetValue(stratumId);
            if (stratum == null)
                return true;

            IReadOnlyList<CanvasInStratum> canvasList = canvasListHolder.GetCanvasList(stratumId);
            if (stratum.Setting.PlannedMaxCanvasCount >= canvasList.Count)
                return true;

            Debug.LogError($"Unexpected canvas count: stratumId={stratumId}, plannedMaxCanvasCount={stratum.Setting.PlannedMaxCanvasCount}, currentCanvasCount={canvasList.Count}");
            return false;
        }

        // OrderInLayerの値が指定順に並んでいるか確認する
        // 並んでいない場合は調整する
        void ModifyOrder(int stratumId)
        {
            Stratum stratum = strata.GetValue(stratumId);
            IReadOnlyList<CanvasInStratum> canvasList = canvasListHolder.GetCanvasList(stratumId);

            if (stratum == null && !canvasList.IsEmpty())
            {
                Debug.LogError($"Cannot get stratum: stratumId={stratumId}");
                return;
            }

            int usableMinOrder = stratum.MinOrder;
            for (int i = 0; i < canvasList.Count; i++)
            {
                CanvasInStratum canvas = canvasList[i];
                int currentOrder = canvas.CanvasOrderUpdater.CanvasOrder;

                int usableExclusiveMaxOrder = stratum.ExclusiveMaxOrder - (canvasList.Count - i - 1) * stratum.Setting.OrderInterval;
                if (currentOrder < usableMinOrder ||
                    currentOrder >= usableExclusiveMaxOrder)
                {
                    canvas.CanvasOrderUpdater.SetOrder(usableMinOrder);
                    currentOrder = usableMinOrder;
                }

                usableMinOrder = currentOrder + stratum.Setting.OrderInterval;
            }
        }

        void SetOrderAll()
        {
            ExpandCanvasCapacity();

            foreach (int stratumId in strata.Keys)
                ModifyOrder(stratumId);
        }

        void ExpandCanvasCapacity()
        {
            Debug.LogError("Change canvas stratum setting. Review canvas stratum setting.");

            StratumSetting[] newSettings = strata.Values
                .Select(stratum =>
                {
                    StratumSetting setting = stratum.Setting;
                    IReadOnlyList<CanvasInStratum> canvasList = canvasListHolder.GetCanvasList(stratum.Setting.Id);

                    return new StratumSetting
                    (
                        setting.Id,
                        setting.OrderInterval,
                        Mathf.Max(setting.PlannedMaxCanvasCount, canvasList.Count)
                    );
                })
                .ToArray();

            InitStrata(newSettings);
        }
    }
}