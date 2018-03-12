/// <summary>
/// UI State Manager. Implements UISM interface. Checkout that interface to see how to talk to this class.
/// 
/// Attached to UIStateManager Prefab. Use MainMenu to get to your level or drag-drop UIStateManager to your 
/// scene. Singleton pattern. It is static, unique and never destroyed.
/// 
/// Takes care of all out of game UI animations and operations. Specifically includes: the main menu and its 
/// subpanels, the pause menu, the hurt red edge blinking effect, the dead screen with handlers. More coming...
/// </summary>

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class UIStateManager : MonoBehaviour, UISM {

//	public enum State {In_Game, Loading, Main, Level_Select, Paused, Settings, About}
	public State currentState = State.Main;

	public static UIStateManager controller = null;

	[Header("Key Bindings")]
	public KeyCode backKey = KeyCode.Escape;
	public KeyCode restartKey = KeyCode.F5;
	public KeyCode upKey = KeyCode.UpArrow;
	public KeyCode downKey = KeyCode.DownArrow;
	public KeyCode confirmKey = KeyCode.Return;

	[Header("BackGround & Menu")]
	public float circleAnimationLerp = 0.4f;
	public float circleAnimationGap = 0.2f;
	public float contentAnimationLerp = 0.1f;

	private RectTransform[] bgCircles;
	private Vector3[] bgCircleScales;
	private float contentAlpha = 0f;
	private GameObject canvasPanel, canvasOverlay;
	private GameObject mainPanel, pausePanel, selectPanel, settingsPanel, aboutPanel, loadingScreen;
	private GameObject m_currentPanel;
	private RectTransform m_bg, m_content, m_deadText, m_hurtImage, m_cursor, m_dim;
	private CanvasGroup m_contentAlpha;

	[Header("Dead Screen")]
	public float deadWaitTime = 2.5f;
	public float deadTextAnimationLerp = 0.3f;
	public float deadSlowTimeScale = 0.25f;

	private Text m_deadTextt;
	private float deadAlpha = 0f;

	[Header("Hurt Screen")]
	public float hurtPromptLerp = 0.4f;
	public float hurtWaitTime = 0.5f;
	public float hurtFadeLerp = 0.1f;

	private RawImage m_hurtImagei;
	private float hurtAlpha = 0f;

	[Header("Dim Screen")]
	public float dimFadeLerp = 0.2f;
	private RawImage m_dimi;
	private float m_dimGoal = 0f;

	[Header("Cursor")]
	public Color cursorFocusColor = Color.white;
	public float cursorFocusScale = 1.5f;
	public float cursorFocusLerp = 0.3f;
	private Image m_cursori;
	public Color origCursorCol = Color.grey;
	private Color currentCursorCol = Color.grey;
	private float currentCursorScale = 1f;
	private IEnumerator focusEvent = null;

	[Header("Misc / Debug")]
	public int targetFrameRate = -1;
	public float approximateEpsilon = 0.01f;
	public bool debug = true;
	public KeyCode HurtKey = KeyCode.O;
	public KeyCode DeadKey = KeyCode.P;
	public KeyCode focusCursorKey = KeyCode.F;
	public KeyCode instructKey = KeyCode.I;
	public bool animationDone = true;

	private IEnumerator ongoingAnim = null;
	private bool m_playingGame = false;
	private bool m_inGame = false;
	public bool inGame {get {return m_playingGame;}}
	private bool m_panelOn = true;
	private int currentScene = 0;
	private bool busy = false;  // If true disable input
	private float lastTimeSinceStart = 0f;
	private RectTransform m_prompt, m_dialogue;
	private Text m_promptT, m_dialogueT;
	private EventSystem eventsystem;
	private Button restartButton;

	[HideInInspector]
	public float realDeltaTime;

	//################################ Initialization ################################
	// Singleton check.
	void Awake () {
		if (controller == null) {
			controller = this;
		} else if (controller != this) {
			Destroy (gameObject);
		}
		DontDestroyOnLoad (gameObject);

		FindReferences ();
		ReadyAnimtaions ();
		ShowPanel ();
		Application.targetFrameRate = targetFrameRate;
	}

	void FindReferences() {
		canvasPanel = GameObject.Find ("CanvasPanels");  // The menu canvas e.g. main menu, pause menu
		canvasOverlay = GameObject.Find ("CanvasOverlay");  // The game overlay canvas. e.g. cursor, damage effect
		if (canvasPanel == null || canvasOverlay == null) {
			Destroy (gameObject);
		}

		Transform canvasT = canvasPanel.transform;
		Transform canvasO = canvasOverlay.transform;

		m_bg = (RectTransform) canvasT.Find ("Background");
		m_content = (RectTransform) canvasT.Find ("Content");
		m_contentAlpha = m_content.GetComponent<CanvasGroup> ();
		m_deadText = (RectTransform) canvasT.Find ("DeadText");
		m_deadTextt = m_deadText.GetComponent<Text> ();
		m_hurtImage = (RectTransform) canvasO.Find ("HurtImage");
		m_hurtImagei = m_hurtImage.GetComponent<RawImage> ();
		m_cursor = (RectTransform) canvasO.Find ("Cursor");
		m_cursori = m_cursor.GetComponent<Image> ();
		m_dim = (RectTransform) canvasO.Find ("Dim Mask");
		m_dimi = m_dim.GetComponent<RawImage> ();

		m_prompt = (RectTransform) canvasO.Find ("Prompt Text");
		m_promptT = m_prompt.GetComponent<Text> ();
		m_dialogue = (RectTransform) canvasO.Find ("Dialogue Text");
		m_dialogueT = m_dialogue.GetComponent<Text> ();

		mainPanel = canvasT.Find ("Content/MainPanel").gameObject;
		pausePanel = canvasT.Find ("Content/PausePanel").gameObject;
		settingsPanel = canvasT.Find ("Content/SettingsPanel").gameObject;
		aboutPanel = canvasT.Find ("Content/AboutPanel").gameObject;
		selectPanel = canvasT.Find ("Content/SelectPanel").gameObject;
		//		loadingScreen = canvasT.Find ("Content/LoadingScreen").gameObject;
		m_currentPanel = mainPanel;

		eventsystem = GameObject.Find ("EventSystem").GetComponent<EventSystem> ();
		restartButton = GameObject.Find ("QuickRestart").GetComponentInChildren<Button>();
	}

	private void ReadyAnimtaions() {
		bgCircles = new RectTransform [m_bg.childCount];
		bgCircleScales = new Vector3 [m_bg.childCount];
		for (int i = 0; i < m_bg.childCount; i++) {
			bgCircles [i] = (RectTransform) m_bg.GetChild(i);
			bgCircles [i].localScale = Vector3.zero;
			bgCircleScales [i] = Vector3.zero;
		}
		origCursorCol = m_cursori.color;
		currentCursorCol = origCursorCol;
	}


	//################################ Input Navigations ################################
	// Accepts inputs other than mouse.
	void Update () {
		bool skipInstructionCheck = false;
		if (!busy) {
			if (Input.GetKeyDown (backKey)) {
				GoBack ();
			} else if (currentState != State.In_Game && Input.GetKeyDown (restartKey)) {
				LoadScene (1);
			} else if (debug) {
				if (Input.GetKeyDown (DeadKey)) {
					ShowDead (null);  // Show dead screen but do nothing (null)
				} else if (Input.GetKeyDown (HurtKey)) {
					ShowHurt (0.4f);  // Alpha = 0.4
				} else if (Input.GetKey (focusCursorKey)) {
					if (focusEvent != null)
						StopCoroutine (focusEvent);
					FocusCursor (0.5f);  // Restore in 0.5s
				} else if (Input.GetKeyDown(instructKey) && requiredKeyState == "") {
					skipInstructionCheck = true;
					PauseForInstruction("Press "+instructKey.ToString()+" to continue", instructKey, "down");
				}
			}
			NavigateMenu ();
		}
		if (!animationDone)
			panelAnimations ();
		overlayAnimations ();

		if (requiredKeyState != "" && !skipInstructionCheck) 
			InstructionFollowedCheck ();

		lastTimeSinceStart = Time.realtimeSinceStartup;
	}

	// Check inputs and navigate menu
	private List<GameObject> buttons = new List<GameObject>();
//	private GameObject activeButton;
	private ScrollRect activeScrollBody;
	void FindAllButtons(GameObject panel) {
		buttons.Clear ();
		Button[] allButtons = panel.GetComponentsInChildren<Button> ();
		List<Button> tempButtons = new List<Button> (allButtons);
		foreach (Button b in tempButtons) {
			if (b.IsInteractable())
				buttons.Add (b.gameObject);
		}
		buttons.Add(restartButton.gameObject);
		activeScrollBody = panel.GetComponentInChildren<ScrollRect> ();
//		activeButton = buttons[0];
		eventsystem.SetSelectedGameObject( buttons[0], null);
//		buttons [0].Select ();
	}

	private GameObject lastActive = null;
	public void NavigateMenu() {
//		if (Input.GetKeyDown (upKey)) {
//			activeButton -= 1;
//			if (activeButton < 0) {
//				activeButton = buttons.Count-1;
//			}
//			eventsystem.SetSelectedGameObject( buttons[activeButton].gameObject);
//		} else if (Input.GetKeyDown (downKey)) {
//			activeButton += 1;
//			if (activeButton >= buttons.Count) {
//				activeButton = 0;
//			}
//			eventsystem.SetSelectedGameObject( buttons[activeButton].gameObject);
//			activeScrollBody.verticalNormalizedPosition = Mathf.Clamp (1f- activeButton *1f  / (buttons.Count-3), 0f, 1f);
//		} else if (Input.GetKeyDown (confirmKey)) {
////			buttons [activeButton].onClick.Invoke();
//		}
//		GameObject currentActive = eventsystem.currentSelectedGameObject;
//		if (currentActive != lastActive) 
//			activeScrollBody.verticalNormalizedPosition = Mathf.Clamp (1f- buttons.IndexOf (currentActive) *1f  / (buttons.Count-3), 0f, 1f);
	}

	public void GoBack() {
		switch (currentState) {
		case State.In_Game:
			TurnPanel (State.Paused);
			break;
		case State.Paused:
			TurnPanel (State.In_Game);
			break;
		case State.Level_Select:
			TurnPanel (State.Main);
			break;
		case State.Settings:
		case State.About:
			if (m_inGame) {
				TurnPanel (State.Paused);
			} else {
				TurnPanel (State.Main);
			}
			break;
		}
	}

	// Switch from one menu to another. Show/Hide accordingly.
	void TurnPanel(State state) {
		if (currentState == state) {
			return;
		} else if (currentState == State.In_Game) {
			PauseGame ();
		} else if (state == State.In_Game) {
			ResumeGame ();
		}

		if (state == State.In_Game && m_panelOn) {
			ClearAll ();
			currentState = state;
			return;
		} else if (state != State.In_Game && state != State.Loading && !m_panelOn) {
			ShowPanel ();
		}

		m_currentPanel.SetActive (false);
		GameObject goalPanel = mainPanel;

		switch (state) {
		case State.Main:
			goalPanel = mainPanel;
			break;
		case State.Paused:
			goalPanel = pausePanel;
			break;
		case State.Settings:
			goalPanel = settingsPanel;
			break;
		case State.About:
			goalPanel = aboutPanel;
			break;
		case State.Level_Select:
			goalPanel = selectPanel;
			break;
		}

		goalPanel.SetActive (true);
		m_currentPanel = goalPanel;
		currentState = state;
		FindAllButtons (goalPanel);
	}

	// Secondary TurnPanel function, allows string input.
	public void SetPanel(string state) {
		State s;
		try{
			s = (State) System.Enum.Parse (typeof(State), state);
		} catch(Exception e){ Debug.Log ("Bad State Name: "+e);return; }

		TurnPanel (s);
	}

	//################################ Show / Hide Panel Animations ################################
	// Show the menu
	public void ShowPanel() {
		canvasPanel.SetActive (true);
		m_panelOn = true;
		CancelPanelAnimations ();
		ongoingAnim = StartBGAnimation (1, 2, (()=>{ShowContent();}));
		StartCoroutine (ongoingAnim);
		FindAllButtons (m_currentPanel);
	}

//	public void HidePanel() {
//		m_panelOn = false;
//		CancelPanelAnimations ();
//		animationDoneCallBack = (()=>{canvasPanel.SetActive (false);DisableContent();});
//		ongoingAnim = StartBGAnimation (0, 2, null);
//		contentAlpha = 0f;
//		StartCoroutine (ongoingAnim);
//	}

	// Show / hide the contenets panel
	private void ShowContent() {
		m_content.gameObject.SetActive (true);
		contentAlpha = 1f;
	}
		private void DisableContent() {
			m_content.gameObject.SetActive (false);
			contentAlpha = 0f;
		}

//	public delegate void DeadResetHandler ();
	//Show the dead screen
	public void ShowDead(DeadResetHandler drh) {
		if (busy)
			return;
		busy = true;
		SlowGame (deadSlowTimeScale);
		canvasPanel.SetActive (true);
		m_panelOn = true;
		CancelPanelAnimations ();
		ongoingAnim = StartBGAnimation (1, -1, (()=>{deadAlpha = 1f;}));
		if (drh != null)
			animationDoneCallBack = (()=>{drh();});
		StartCoroutine (ongoingAnim);
		StartCoroutine (HideDead(deadWaitTime));
	}
		// Hide the dead screen. Only called after ShowDead().
		private IEnumerator HideDead(float waitTime) {
			yield return WaitForRealSeconds (waitTime);
			ClearAll ();
		}

	// Clears all UI
	public void ClearAll() {
		m_panelOn = false;
		CancelPanelAnimations ();
		animationDoneCallBack = (()=>{
			canvasPanel.SetActive (false);
			DisableContent();
		});
		ongoingAnim = StartBGAnimation (0, -1, (()=>{busy = false;}));
		contentAlpha = 0f;
		deadAlpha = 0f;
		ResumeGame();
		StartCoroutine (ongoingAnim);
	}


	private delegate void AnimationDoneCallBack();
	AnimationDoneCallBack animationDoneCallBack = null;

	private delegate void AnimationSetCallBack();

	IEnumerator StartBGAnimation(float scale, int upto, AnimationSetCallBack cb) {
		animationDone = false;
		if (upto <= 0) {
			upto = 256;
		}
		for (int i = 0; i < Mathf.Min(bgCircles.Length, upto); i++) {
			bgCircleScales [i] = Vector3.one * scale;
			yield return WaitForRealSeconds (circleAnimationGap);
		}
		if (cb != null) {
			cb ();
		}
	}

	void panelAnimations() {
		realDeltaTime = Mathf.Min (0.1f, Time.realtimeSinceStartup - lastTimeSinceStart);
		bool stageDone = true;
		for (int i = 0; i < bgCircles.Length; i++) {
			RectTransform c = bgCircles [i];
			if (Mathf.Abs(c.localScale.magnitude - bgCircleScales [i].magnitude) > approximateEpsilon) {
				stageDone = false;
				// If lerping+realDeltaTime still not working, these two lines are plan C
//				Vector3 velocity = Vector3.zero;
//				c.localScale = Vector3.SmoothDamp (c.localScale, bgCircleScales [i], ref velocity, circleAnimationLerp, Mathf.Infinity, realDeltaTime);
				c.localScale = Vector3.Lerp (c.localScale, bgCircleScales [i], circleAnimationLerp + 6*realDeltaTime);
			}
		}
		if (Mathf.Abs(m_contentAlpha.alpha - contentAlpha) > approximateEpsilon) {
			stageDone = false;
			m_contentAlpha.alpha = Mathf.Lerp (m_contentAlpha.alpha, contentAlpha, contentAnimationLerp + 6*realDeltaTime);
		}
		if (Mathf.Abs(m_deadTextt.color.a - deadAlpha) > approximateEpsilon) {
//			stageDone = false;
			Color rgba = m_deadTextt.color;
			rgba.a = Mathf.Lerp (rgba.a, deadAlpha, deadTextAnimationLerp + 6*realDeltaTime);
			m_deadTextt.color = rgba;
		}
		animationDone = stageDone;
		if (animationDone && animationDoneCallBack != null) {
			animationDoneCallBack ();
			animationDoneCallBack = null;
		}
	}

	void CancelPanelAnimations() {
//		animationDone = true;
		animationDoneCallBack = null;
		if (ongoingAnim != null) 
			StopCoroutine (ongoingAnim);
	}


	//################################ Overlay Animations / Prompt / Dialogue ################################

	// Prompt the hurt red edge image and later clear it.
	private IEnumerator hurtEvent = null;
	public void ShowHurt(float alpha) {
		if (hurtEvent != null) {
			StopCoroutine (hurtEvent);
			animationDoneCallBack = null;
		}
		hurtEvent = Hurt (alpha);
		StartCoroutine (hurtEvent);
	}
	private IEnumerator Hurt(float alpha) {
		hurtAlpha = alpha;
		yield return new WaitForSeconds (hurtWaitTime);
		hurtAlpha = 0f;
	}

	// public methods to adjust cursor
	public void FocusCursor(Color c, float enlarge, float restoreIn) {
		currentCursorCol = c;
		currentCursorScale = enlarge;
		if (restoreIn > 0) {
			focusEvent = RestoreCursorIn (restoreIn);
			StartCoroutine (focusEvent);
		}
	}
		public void FocusCursor(float restoreIn) { FocusCursor(cursorFocusColor, cursorFocusScale, restoreIn); }

	public void RestoreCursor() {
		currentCursorCol = origCursorCol;
		currentCursorScale = 1f;
	}

	IEnumerator RestoreCursorIn(float restoreIn) {
		yield return new WaitForSeconds (restoreIn);
		RestoreCursor ();
	}

	void overlayAnimations() {
		if (Mathf.Abs(m_hurtImagei.color.a - hurtAlpha) > approximateEpsilon) {
			Color rgba = m_hurtImagei.color;
			rgba.a = Mathf.Lerp (rgba.a, hurtAlpha, (rgba.a<hurtAlpha? hurtPromptLerp:hurtFadeLerp) + 6*realDeltaTime);
			m_hurtImagei.color = rgba;
		}
		if (Mathf.Abs(m_dimi.color.a - m_dimGoal) > approximateEpsilon) {
			Color rgba = m_dimi.color;
			rgba.a = Mathf.Lerp (rgba.a, m_dimGoal, dimFadeLerp + 6*realDeltaTime);
			m_dimi.color = rgba;
		}
		Color diff = currentCursorCol - m_cursori.color;
		if (Mathf.Abs (diff.r + diff.g + diff.b + diff.a + currentCursorScale - m_cursor.localScale.magnitude) > approximateEpsilon) {
			m_cursori.color = Color.Lerp (m_cursori.color, currentCursorCol, cursorFocusLerp + 6 * realDeltaTime);
			m_cursor.localScale = Vector3.Lerp (m_cursor.localScale, Vector3.one * currentCursorScale, cursorFocusLerp + 6 * realDeltaTime);
		}
	}

	private KeyCode requiredKey;
	private string requiredKeyState = "";
	private string instructionFollowedResponse = "Good";
	private float keyHoldTime = 1.5f;
	public void PauseForInstruction(String prompt, String donePrompt, KeyCode requireKey, string requireKeyState) {
		DimScreen (0.5f);
		Prompt (prompt, -1);
		requiredKey = requireKey;
		requiredKeyState = requireKeyState;
		instructionFollowedResponse = donePrompt;
		Time.timeScale = 0f;
	}
	public void PauseForInstruction(String prompt, KeyCode requireKey, string requireKeyState) {
		PauseForInstruction (prompt, "", requireKey, requireKeyState);
	}

	public void ResumeAfterInstruction() {
		if (instructionFollowedResponse != "")
			Prompt (instructionFollowedResponse, 1f);
		else
			ClearPrompt ();
		UnDimScreen ();
		Time.timeScale = 1f;
		requiredKeyState = "";
	}

	void DimScreen(float alpha) { m_dimGoal = alpha; }
	void UnDimScreen() { m_dimGoal = 0f; }

	void InstructionFollowedCheck() {
		switch (requiredKeyState) {
		case "up":
			if (Input.GetKeyUp (requiredKey))
				ResumeAfterInstruction ();
			break;
		case "down":
			if (Input.GetKeyDown(requiredKey))
				ResumeAfterInstruction();
			break;
		case "hold":
			if (Input.GetKey (requiredKey))
				keyHoldTime -= Time.deltaTime;
			if (keyHoldTime <= 0)
				ResumeAfterInstruction ();
			if (Input.GetKeyUp (requiredKey))
				keyHoldTime = 1.5f;
			break;
		}
	}

	// Prompt
	private IEnumerator promptEvent = null;
	public void Prompt(string text, float clearIn) {
		if (Time.timeScale > 0) {
			if (promptEvent != null)
				StopCoroutine (promptEvent);
			m_promptT.text = text;
			if (clearIn > 0) {
				promptEvent = ClearPromptIn (clearIn);
				StartCoroutine (promptEvent);
			}
		}
	}

	IEnumerator ClearPromptIn(float clearIn) {
		yield return new WaitForSeconds (clearIn);
		ClearPrompt ();
	}

	public void ClearPrompt() { m_promptT.text = ""; }

	// Dialogue
	private IEnumerator dialogueEvent = null;
	public void Dialogue(string text, float clearIn) {
		if (dialogueEvent != null)
			StopCoroutine (dialogueEvent);
		m_dialogueT.text = text;
		if (clearIn > 0) {
			dialogueEvent = ClearDialogueIn (clearIn);
			StartCoroutine (dialogueEvent);
		}
	}

	IEnumerator ClearDialogueIn(float clearIn) {
		yield return new WaitForSeconds (clearIn);
		ClearDialogue ();
	}

	public void ClearDialogue() { m_dialogueT.text = ""; }


	//################################ Utilities ################################
	public void PauseGame() {
		m_playingGame = false;
		Time.timeScale = 0f;
	}

	void SlowGame(float scale) {
		Time.timeScale = scale;
	}

	public void ResumeGame() {
		m_playingGame = true;
		Time.timeScale = 1f;
	}

	public void Restart() {
		LoadScene (currentScene);
	}

	public void Exit() {
		Application.Quit ();
		//UnityEditor.EditorApplication.isPlaying = false;
	}

	public void LoadScene(int which) {
		if (which == 0) {
			TurnPanel (State.Main);
			m_inGame = false;
		} else {
			TurnPanel (State.In_Game);
			m_inGame = true;
		}
		SceneManager.LoadScene(which);
		currentScene = which;
	}

	// This function tracks its own time independent of Time scale.
	public IEnumerator WaitForRealSeconds(float time) {
		float start = Time.realtimeSinceStartup;
		while (Time.realtimeSinceStartup - start < time) {
			yield return null;
		}
	}
}