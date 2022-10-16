# Unity_VFXGraphBulletSystem
A test project of VFX Graph based Bullet System.

Purpose of this Project:
Build a bullet system that does not require a GameObject, Transform & Mesh Renderer Component per each projectile instance.
Hence reduce draw calls and GameObject / MonoBehaviour overhead.
A single VFX system instance handles every Bullet Spawn requests from player at different locations.
All the Bullets will be simulated by the same VFX system instance with compute Shader.


