using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zara.Common.ExBase;

namespace Zara.Test
{
    public class Message : ExMonoBehaviour
    {
        [SerializeField]
        Text messageText = null;

        [SerializeField]
        Button okButton = null;

        [SerializeField]
        Button cancelButton = null;

        public void Init(string message, UnityAction onOk, UnityAction onCancel)
        {
            messageText.text = message;
            SetButtonAction(okButton, onOk);
            SetButtonAction(cancelButton, onCancel);
        }

        void SetButtonAction(Button button, UnityAction action)
        {
            if (action != null)
                button.onClick.AddListener(action);
            else
                button.gameObject.SetActive(false);
        }
    }
}