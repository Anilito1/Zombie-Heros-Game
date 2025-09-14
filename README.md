TP3 Unity3D — Démo 3ᵉ personne (forêt, Bézier, combat, jour/nuit)

Petite démo Unity URP en 3ᵉ personne :

Intro cinématique : la caméra vole sur une courbe de Bézier dans la forêt, puis bascule sur la caméra gameplay.

Caméra gameplay : 3ᵉ personne, souris (douce, pas trop rapide), zoom molette, évite les obstacles.

Terrain/forêt : textures, arbres, buissons (URP), outils Terrain.

Gameplay : Player vs Zombie (melee avec Animation Events), barres de vie, mort + respawn (anim Die).

Jour/Nuit : script de cycle 24h virtuelles (intensité du soleil animée par code).

Enregistrement : prêt pour Unity Recorder (MP4).

🚀 Lancer le projet

Version Unity

Unity 6 (URP) recommandé.

Si nécessaire : Window → Rendering → Render Pipeline Converter → Materials Upgrade → Convert.



Play

La caméra suit la courbe de Bézier jusqu’au joueur, puis bascule sur la caméra 3ᵉ personne.

🎮 Contrôles

Déplacement : Z Q S D

Saut : Space

Attaque : E (l’impact part via Animation Event)

Caméra : souris (clic droit facultatif si activé), molette = zoom

✨ Fonctionnalités principales

Caméra Bézier fluide & constante
BezierCurve.cs (quad/cubique + LineRenderer)
BezierCameraFly.cs (reparamétrage par longueur d’arc = vitesse constante)
CameraSequenceController.cs (bascule auto Bézier → gameplay)

Caméra 3ᵉ personne (souris douce)
FollowTarget.cs (orbitale, sensibilité modérée, lissage, zoom, anti-clip par SphereCast)

Combat & UI
PlayerMelee.cs / ZombieMelee.cs (OverlapSphere + Animation_Hit)
Stats.cs (HP, AttackPower, Slider HP)
Barres de vie en World-Space Canvas (suivent les persos)

Mort + Respawn
SimpleDeathRespawn.cs (déclenche Die, attend, respawn au point de spawn, HP full, rejoue Idle)

Jour/Nuit
DayNightCycle.cs (24h virtuelles, intensité/lumière Directional Light, accélération du temps)

Forêt URP
Terrain (peinture de textures, arbres “Paint Trees”, détails “Paint Details”), billboards désactivés si besoin.

🔧 Paramétrages importants (à vérifier)
Tags & Layers

Player : Tag = Player

Zombie : Tag = Enemy

Terrain : Layer = Ground (utilisé par la caméra Bézier pour rester au-dessus du sol)

Animator (Player & Zombie)

Paramètres : Speed (float), IsGrounded (bool), Attack (trigger), Die (trigger)

Transitions :

Any State → Die (Has Exit Time OFF, Condition: Die)

Idle ↔ Run (via Speed)

Any State → Attack (Trigger Attack)

Animation Events

Dans le clip d’attaque du Player : un Event appelle Animation_Hit (script PlayerMelee).

Dans le clip d’attaque du Zombie : un Event appelle Animation_Hit (script ZombieMelee).

Barres de vie

Dans Stats (sur chaque perso), assigne Hp Slider (le Slider de sa barre).

MaxHP, AttackPower > 0.

Caméras

Intro : CameraRig avec BezierCameraFly (loop OFF), points P0..P3 posés au-dessus du terrain.

Gameplay : FollowTarget activé après la cinématique (géré par CameraSequenceController).

Désactiver tout ce qui pilote la caméra en parallèle (Cinemachine Brain ou anciens scripts) si non utilisé.

URP & Arbres

Si assets non URP → Materials = URP/Lit. Feuilles : Alpha Clipping ON + Both Faces.

Terrain Settings : Draw Instanced ON, Billboard Start très grand (p.ex. 2000) pour éviter les impostors hérités.

🕒 Jour/Nuit

DayNightCycle.cs (exemple d’options) :

Day Length (minutes) : durée d’un cycle 24h virtuel (ex. 2:00 pour l’exercice).

Sun (Directional Light) : rotation continue, intensité plus forte en journée, faible la nuit.

Skybox : optionnel, change de teinte selon l’heure.
