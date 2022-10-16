using UnityEngine;

public static class ShaderPropertyUtil {
	public static readonly int emitEvent		= Shader.PropertyToID("Emit");
	public static readonly int bulletliveCount	= Shader.PropertyToID("Bullet Head");
	public static readonly int spawnCountAtt = Shader.PropertyToID("spawnCount");
	public static readonly int startPosAtt = Shader.PropertyToID("startPosition");
	public static readonly int targetPosAtt = Shader.PropertyToID("targetPosition");
	public static readonly int velocityAtt = Shader.PropertyToID("velocity");
	public static readonly int targetNormalAtt = Shader.PropertyToID("targetNormal");
	public static readonly int lifeTimeAtt = Shader.PropertyToID("lifetime");
	public static readonly int aliveMapAtt = Shader.PropertyToID("aliveMap");
}