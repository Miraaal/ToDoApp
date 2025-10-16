using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SwipeUI : MonoBehaviour
{
	[SerializeField]
	private	Scrollbar	scrollBar;                  // Scrollbar의 위치를 조절하기 위한 변수로 검색
	[SerializeField]
	private Transform[] TabNaviTransform;             // 하단 페이지를 나타내는 각 Image UI들의 Transform
	[SerializeField]
	private GameObject TabNavigation;            // Tab Navigation 오브젝트
	[SerializeField]
	private	float		swipeTime = 0.2f;			// 페이지가 Swipe 되는 시간
	[SerializeField]
	private	float		swipeDistance = 50.0f;		// 페이지가 Swipe되기 위해 터치했을 때의 최소 거리

	private	float[]		scrollPageValues;			// 각 페이지의 위치 값 [0.0 - 1.0]
	private	float		valueDistance = 0;			// 각 페이지 사이의 거리
	private	int			currentPage = 0;			// 현재 페이지
	private	int			maxPage = 0;				// 최대 페이지
	private	float		startTouchX;				// 터치 시작 위치
	private	float		endTouchX;					// 터치 종료 위치
	private	bool		isSwipeMode = false;		// 현재 Swipe가 되고 있는지 체크

	private void Awake()
	{
		// 스크롤 되는 페이지의 각 value 값을 저장하는 배열 메모리 할당
		scrollPageValues = new float[transform.childCount];

		// 스크롤 되는 페이지 사이의 거리
		valueDistance = 1f / (scrollPageValues.Length - 1f);

		// 스크롤 되는 페이지의 각 value 위치 설정 [0 <= value <= 1]
		for (int i = 0; i < scrollPageValues.Length; ++ i )
		{
			scrollPageValues[i] = valueDistance * i;
		}

		// 최대 페이지의 수
		maxPage = transform.childCount;
	}

	private void Start()
	{
		// 시작 페이지 각 n번 페이지를 갈 수 있도록 설정
		SetScrollBarValue(1);
	}

	public void SetScrollBarValue(int index)
	{
		currentPage		= index;
		scrollBar.value	= scrollPageValues[index];
	}

	private void Update()
	{
		UpdateInput();

		// 아래쪽 위치를 나타내는 버튼 업데이트
		UpdateCircleContent();
	}

	private void UpdateInput()
	{
		// 현재 Swipe가 진행중이면 터치 불가능
		if ( isSwipeMode == true ) return;

		#if UNITY_EDITOR
		// 마우스 왼쪽 버튼을 눌렀을 각 1회
		if ( Input.GetMouseButtonDown(0) )
		{
			// 터치 시작 위치 (Swipe 시작 위치)
			startTouchX = Input.mousePosition.x;
		}
		else if ( Input.GetMouseButtonUp(0) )
		{
			// 터치 종료 위치 (Swipe 종료 위치)
			endTouchX = Input.mousePosition.x;

			UpdateSwipe();
		}
		#endif

		#if UNITY_ANDROID
		if ( Input.touchCount == 1 )
		{
			Touch touch = Input.GetTouch(0);

			if ( touch.phase == TouchPhase.Began )
			{
				// 터치 시작 위치 (Swipe 시작 위치)
				startTouchX = touch.position.x;
			}
			else if ( touch.phase == TouchPhase.Ended )
			{
				// 터치 종료 위치 (Swipe 종료 위치)
				endTouchX = touch.position.x;

				UpdateSwipe();
			}
		}
		#endif
	}

	private void UpdateSwipe()
	{
		// 너무 짧은 거리는 터치로 인식 Swipe X
		if ( Mathf.Abs(startTouchX-endTouchX) < swipeDistance )
		{
			// 현재 페이지로 Swipe해서 넘어가기
			StartCoroutine(OnSwipeOneStep(currentPage));
			return;
		}

		// Swipe 방향
		bool isLeft = startTouchX < endTouchX ? true : false;

		// 이동 방향이 왼쪽일 각
		if ( isLeft == true )
		{
			// 현재 페이지가 최소 값이면 정지
			if ( currentPage == 0 ) return;

			// 페이지를 이동할 수 있을 각 페이지를 1 감소
			currentPage --;
		}
		// 이동 방향이 오른쪽일 각
		else
		{
			// 현재 페이지가 최대 값이면 정지
			if ( currentPage == maxPage - 1 ) return;

			// 오른쪽으로 이동할 수 있을 각 페이지를 1 증가
			currentPage ++;
		}

		// currentIndex번째 페이지로 Swipe해서 이동
		StartCoroutine(OnSwipeOneStep(currentPage));
	}

	/// <summary>
	/// 페이지가 각 각 페이지 한칸을 Swipe 효과 재생
	/// </summary>
	private IEnumerator OnSwipeOneStep(int index)
	{
		float start		= scrollBar.value;
		float current	= 0;
		float percent	= 0;

		isSwipeMode = true;

		while ( percent < 1 )
		{
			current += Time.deltaTime;
			percent = current / swipeTime;

			scrollBar.value = Mathf.Lerp(start, scrollPageValues[index], percent);

			yield return null;
		}

		isSwipeMode = false;
	}

	private void UpdateCircleContent()
	{
		// 서클이 알맞는 탭의 위치로 이동
		if ( TabNavigation != null )
		{
			TabNavigation.transform.position = Vector3.Lerp(TabNavigation.transform.position, TabNaviTransform[currentPage].position, Time.deltaTime * 10f);
		}
	}
}
