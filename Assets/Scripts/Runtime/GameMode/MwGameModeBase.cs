using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MwGameModeBase : MonoBehaviour {

	//Singleton

	static MwGameModeBase _current = null;
	public static MwGameModeBase current {
		get {
			if (_current == null) {
				_current = new GameObject("MwGameMode").AddComponent<MwGameModeBase>();
			}
			return _current;
		}
	}

	//Players
	protected static List<MwPlayerController>				_playerList	 = new List<MwPlayerController>();
	protected static Dictionary<int, MwPlayerController>	_playerIDMap = new Dictionary<int, MwPlayerController>();

	[Header("GameMode Config")]
	[SerializeField] MwCharacter		_startingCharacter	= null;
	[SerializeField] MwPlayerController _startingPlayer		= null;

	public static void RegisterPlayer(MwPlayerController pc_) {
		if (_playerList == null) return;

		if(!_playerIDMap.ContainsValue(pc_)) {
			_playerList.Add(pc_);
		}

		if(!_playerIDMap.ContainsValue(pc_)) {
			_playerIDMap[pc_.playerID] = pc_;
		}

		WxLogger.Log($"<color=#16c444>Player</color> [ <color=#16c444>{pc_.playerID}</color> ] has joined the Game");
	}
	public static void UnRegisterPlayer(MwPlayerController pc_) {
		if (_playerList == null) return;


		if (_playerList.Contains(pc_)) {
			_playerList.Remove(pc_);
		}

		if(_playerIDMap.ContainsValue(pc_)) {
			_playerIDMap.Remove(pc_.playerID);
		}

		WxLogger.Log($"<color=#16c444>Player</color> [ <color=#16c444>{pc_.playerID}</color> ] has left the Game");

	}

	//MonoFunc
	void Start() {
		//Prevent Instant MouseInput Reading Error
		StartCoroutine(GameAutoStartCour(0.2f));
	}

	private void GameAutoStart() {

		if(!_startingCharacter) _startingPlayer = FindObjectOfType<MwPlayerController>();

		if (_startingCharacter && _startingPlayer) {
			_startingPlayer.Possess(_startingCharacter);
		}
	}

	IEnumerator GameAutoStartCour(float delay) {
		WaitForSeconds wait = new WaitForSeconds(delay);
		yield return wait;

		GameAutoStart();
	}
}