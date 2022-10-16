using UnityEngine;

public static class WxParticleSystem {
	public static void SetEmissionActive(this ParticleSystem ps, bool enable_) {
		if (!ps) return;
		var emiMod = ps.emission;
		emiMod.enabled = enable_;
	}





}

