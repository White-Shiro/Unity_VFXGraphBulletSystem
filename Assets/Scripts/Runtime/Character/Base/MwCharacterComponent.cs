using UnityEngine;

public abstract class MwCharacterComponent : MwComponent {

	public MwCharacter character { get; protected set; }
	public bool enableUpdate = true;

	public void Construct(MwCharacter character_) {
		if (character_ == null) {
			WxLogger.Error("Character is missing",this);
			return;
		}

		character = character_;
		OnConstruct(character_);
	}

	public void OnUpdate() { if (enableUpdate) onUpdate(); }
	public void OnFixedUpdate() { if (enableUpdate) onFixedUpdate(); }
	public void OnLateUpdate() { if (enableUpdate) onLateUpdate(); }

	protected virtual void OnConstruct(MwCharacter character_) { }
	protected virtual void onUpdate() { }
	protected virtual void onFixedUpdate() { }
	protected virtual void onLateUpdate() { }
}