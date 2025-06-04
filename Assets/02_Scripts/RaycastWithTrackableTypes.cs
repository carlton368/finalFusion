using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

namespace CuteDuckGame
{
    public class RaycastWithTrackableTypes : MonoBehaviour
    {
        [Header("AR 컴포넌트")] [SerializeField] private GameObject indicator;
        [Header("유저별 맵 프리팹")] [SerializeField] private GameObject mapPrefab;
        [SerializeField] private ARRaycastManager raycastManager;

        private List<ARRaycastHit> hits = new List<ARRaycastHit>();
        private Vector3 currentSelectedPosition = Vector3.zero;
        private bool hasValidPosition = false;
        private bool indicatorEnabled = true;

        public static Action<Vector3> OnARPositionChanged;
        public static Action<bool> OnARPositionValidityChanged;

        private void Start()
        {
            indicator.SetActive(false);

            if (raycastManager == null)
                raycastManager = GetComponent<ARRaycastManager>();
        }

        private void Update()
        {
            if (indicatorEnabled)
            {
                DetectGround();

                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);
                    Debug.Log("화면 터치 발생");
                    if (touch.phase == TouchPhase.Began)
                    {
                        Debug.Log("화면 터치 시작");
                        CreateMap();
                        
                    }
                }
            }
        }

        private void CreateMap()
        {
            Debug.Log("맵 생성 시작");
            Instantiate(mapPrefab, StaticData.GetCurrentSpawnPosition(), indicator.transform.rotation);
            Debug.Log("맵 생성 완료");
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void DetectGround()
        {
            Vector2 screenPoint = new Vector2(Screen.width / 2, Screen.height / 2);

            if (raycastManager.Raycast(screenPoint, hits, TrackableType.Planes))
            {
                indicator.SetActive(true);
                indicator.transform.position = hits[0].pose.position;
                indicator.transform.rotation = hits[0].pose.rotation;
                indicator.transform.position += indicator.transform.up * 0.1f;

                Vector3 newPosition = hits[0].pose.position;
                if (Vector3.Distance(currentSelectedPosition, newPosition) > 0.1f)
                {
                    currentSelectedPosition = newPosition;
                    StaticData.SetInitialSpawnPos(currentSelectedPosition);
                    OnARPositionChanged?.Invoke(currentSelectedPosition);
                }

                if (!hasValidPosition)
                {
                    hasValidPosition = true;
                    OnARPositionValidityChanged?.Invoke(true);
                }
            }
            else
            {
                indicator.SetActive(false);

                if (hasValidPosition)
                {
                    hasValidPosition = false;
                    OnARPositionValidityChanged?.Invoke(false);
                }
            }
        }

        public void SetIndicatorEnabled(bool enabled)
        {
            indicatorEnabled = enabled;
            if (!enabled && indicator != null)
                indicator.SetActive(false);
        }

        public Vector3 GetCurrentSelectedPosition() => currentSelectedPosition;
        public bool HasValidPosition() => hasValidPosition;
    }
}