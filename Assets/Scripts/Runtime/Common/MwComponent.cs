using UnityEngine;


//Dodo direct inherit from Component instead of Mono to avoid updates Callbacks
public abstract class MwComponent : MonoBehaviour {

    public void Construct() { OnConstruct(); }
    protected virtual void OnConstruct() { }

}

