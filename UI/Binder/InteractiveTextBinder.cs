using System;
using System.Collections.Generic;

using HUtil.Runtime.Observable;

using TMPro;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace HUtil.UI.Binder
{
    /// <summary>
    /// 클릭, 호버 이벤트를 처리하는 TMP_Text 바인딩 컴포넌트
    /// </summary>
    public class InteractiveTextBinder : MonoBinder, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
    {
        [SerializeField] private TMP_Text _target;
        [SerializeField] private TextMeshProUGUI _targetMesh;
        [SerializeField] private PropertyBindingPort<string> _baseText_prop = new PropertyBindingPort<string>(BindingType.OfType(BindingBaseType.String), BindingDirectionFlags.ToUI);
        [SerializeField] private CommandBindingPort _onHoverIn_prop = new CommandBindingPort(BindingDirectionFlags.ToData);
        [SerializeField] private CommandBindingPort _onHoverOut_prop = new CommandBindingPort(BindingDirectionFlags.ToData);
        [SerializeField] private CommandBindingPort _onClick_prop = new CommandBindingPort(BindingDirectionFlags.ToData);
        [SerializeField] private UnityEvent<string> _onHoverIn = new UnityEvent<string>();
        [SerializeField] private UnityEvent<string> _onHoverOut = new UnityEvent<string>();
        [SerializeField] private UnityEvent<string> _onClick = new UnityEvent<string>();

        private int _lastHoveredLinkIdx = -1;

        protected override void Reset()
        {
            base.Reset();
            _target = GetComponent<TMP_Text>();
            _targetMesh = GetComponent<TextMeshProUGUI>();
        }

        protected virtual void OnValidate()
        {
            _baseText_prop.Validate(this);
            _onHoverIn_prop.Validate(this);
            _onHoverOut_prop.Validate(this);
            _onClick_prop.Validate(this);
        }

        protected override void BindInternal(Dictionary<string, IViewModelProperty> bindMap, CompositeDisposable disposable)
        {
            _baseText_prop.Bind(bindMap, disposable, SetText);
            _onHoverIn_prop.Bind(bindMap, disposable, _onHoverIn);
            _onHoverOut_prop.Bind(bindMap, disposable, _onHoverOut);
            _onClick_prop.Bind(bindMap, disposable, _onClick);
        }

        protected virtual void SetText(string value)
        {
            _target.text = value;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            int id = TMP_TextUtilities.FindIntersectingLink(_targetMesh, eventData.position, eventData.enterEventCamera);
            if(id != -1)
            {
                string keyword = _targetMesh.textInfo.linkInfo[id].GetLinkID();
                _onClick.Invoke(keyword);
            }
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            // hoverin 감지가 안 되는 핵심 이유를 해결합니다.
            CheckLinkInteraction(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // 영역을 완전히 벗어나면 현재 호버 중인 링크도 해제
            ClearLastHover();
        }

        private void CheckLinkInteraction(PointerEventData eventData)
        {
            // Enter 시점의 카메라를 사용
            Camera eventCamera = eventData.enterEventCamera;
            int currentLinkIdx = TMP_TextUtilities.FindIntersectingLink(_targetMesh, eventData.position, eventCamera);

            // 동일한 링크 위에 있으면 무시
            if (currentLinkIdx == _lastHoveredLinkIdx) return;

            // 1. 이전에 호버 중이던 링크가 있었다면 HoverOut 호출
            if (_lastHoveredLinkIdx != -1)
            {
                string oldKeyword = _targetMesh.textInfo.linkInfo[_lastHoveredLinkIdx].GetLinkID();
                _onHoverOut.Invoke(oldKeyword);
            }

            // 2. 새로운 링크 위에 올라왔다면 HoverIn 호출
            if (currentLinkIdx != -1)
            {
                string newKeyword = _targetMesh.textInfo.linkInfo[currentLinkIdx].GetLinkID();
                _onHoverIn.Invoke(newKeyword);
            }

            _lastHoveredLinkIdx = currentLinkIdx;
        }

        private void ClearLastHover()
        {
            if (_lastHoveredLinkIdx != -1)
            {
                string keyword = _targetMesh.textInfo.linkInfo[_lastHoveredLinkIdx].GetLinkID();
                _onHoverOut.Invoke(keyword);
            }
            _lastHoveredLinkIdx = -1;
        }

        // PointerEnter는 이제 단순히 Move로 로직을 넘기거나 비워둬도 됩니다.
        public void OnPointerEnter(PointerEventData eventData) => CheckLinkInteraction(eventData);
    }
}