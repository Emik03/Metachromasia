# Metachromasia

[![License](https://img.shields.io/github/license/Emik03/Metachromasia.svg?color=6272a4&style=for-the-badge)](https://github.com/Emik03/Metachromasia/blob/main/LICENSE)

Plants vs. Zombies: Fusion library to create modded plants tersely. Documentation is far from finished, and future versions may introduce breaking changes as the game frequently updates.

This project has a dependency to [Emik.Morsels](https://github.com/Emik03/Emik.Morsels), if you are building this project, refer to its [README](https://github.com/Emik03/Emik.Morsels/blob/main/README.md) first.

---
- [Assets](#assets)
- [Localization](#localization)
- [Examples](#examples)
- [Contribute](#contribute)
- [License](#license)
---

## Assets

The mod expects `<ProjectName>.bundle` as an `EmbeddedResource` for all plants you register.

The ones required by the in the `AssetBundle` are `Prefab` for the model in-game, and `Preview` for the almanac. Additionally, `SkinPrefab`, `SkinPreview`, `Bullet`, and `Particle` may be optionally specified. You can also process any other game object outside of this naming convention by using specifying in the constructor `["FooBar"] = (GameObject foobar) => { /* do stuff */ }`.

If your mod contains multiple plants, you can prefix each game object with the plant name. This means that `PlantNamePrefab` only applies to `PlantName`, and `Particle` would apply to all registered plants in the mod.

## Localization

The library also expects `<PlantName>.loc.txt` as an `EmbeddedResource` for every plant you register. The plant name is inferred by type name or namespace, either `Namespace.<PlantName>` or `<PlantName>.Plugin`.

The ones required in the almanac are `Name` and `Description`, and optionally `Cost`. You may also grab these localized strings with the `Localize` method.

Here's the format for localization files, with multiple languages supported. Note that the localization mod is not a hard dependency, and the native language is used if it isn't installed.

```cs
{Name}
native name

{Description}
native
description

{Name.English}
english name

{Description.English}
english
description

{Name.Ukrainian}
ukrainian name

{Description.Ukrainian}
ukrainian
description
```

For formatting, since these strings go directly to a `TextMeshPro` instance, it means that you can use `<size>` or `<color>` just as you would normally.

## Examples

The following are incomplete reimplementations of existing mods from LibraHP.
Assets and some of the source code are omitted to prevent redistribution.

#### BrainBest

```cs
public sealed class Plugin() : PlantInjector<Plugin, PeaShooter, Bullet>(
	new(507, true) { Price = 404, Cooldown = 100, Damage = 200, [Prefab] = AddComponent<BrainBestScript> },
	[TypeMgr.IsIcePlant, TypeMgr.IsMagnetPlants, Lawnf.IsUltiPlant]
);
```

#### CabbagePultShroom

```cs
public sealed class Plugin()
    : PlantInjector<Plugin, Cabbage, Bullet_cabbage>(
        new(2001, (PlantType.Cabbagepult, (PlantType)2000)) { Price = 125, Cooldown = 15, Damage = 90 },
        [TypeMgr.IsPuff],
        Prefix<Zombie>(
            x => x.HitZombie,
            (x, ref z) =>
            {
                if (!z)
                    return;

                z.TakeDamage(DmgType.Shieldless, x.Damage);
                x.Particle(ParticleType.PeaSplat);
                x.PlaySound(z);
                x.RotateUpdate();
                x.Die();
            }
        )
    );
```

#### DestroyCannon

```cs
public sealed class Plugin() : PlantInjector<Plugin, FireCannon, Bullet_cannon>(
	new(556, (PlantType.DoomShroom, PlantType.CobCannon))
	{
		Price = 600,
		AttackInterval = 36,
		ProductionInterval = 0,
		Cooldown = 20,
		Damage = 1800,
	},
	[TypeMgr.DoubleBoxPlants],
	Prefix(x => x.AnimShoot, x => x.Launch(Bullet)),
	Prefix(
		x => x.HitLand,
		x =>
		{
			var columnFromX = Mouse.Instance.GetColumnFromX(x.transform.position.x);
			var rowFromY = Mouse.Instance.GetRowFromY(x.transform.position.x, x.transform.position.y);
			Vector2 vector = x.transform.GetChild(0).GetChild(0).position;
			Board.Instance.SetDoom(columnFromX, rowFromY, false, false, vector, x.Damage);

			foreach (var p in Lawnf.Get3x3Plants(columnFromX, rowFromY))
				if (p && !p.dying && !TypeMgr.IsPot(p.thePlantType))
					CreatePlant.Instance.SetPlant(p.thePlantColumn, p.thePlantRow, PlantType.DoomShroom);
		}
	)
);
```

#### SuperObsidianChomper

```cs
public sealed class Plugin() : PlantInjector<Plugin, UltimateChomper, Bullet_ultimateCactus>(
    new(999, (PlantType.UltimateChomper, PlantType.UltimateTallNut))
        { Health = 99999, AttackInterval = 0, ProductionInterval = 0, Damage = 9999 },
    [Lawnf.IsUltiPlant, TypeMgr.IsTallNut],
    Prefix(
        x => x.AnimShoot,
        x =>
        {
            x.attributeCountdown = 0;
            x.theStatus = PlantStatus.Defalut;
            x.shoot = x.transform.Find("Shoot");
            x.Shoot(Bullet, 300);
            GameAPP.PlaySound(Random.Range(3, 5));
        }
    ),
    Postfix<Zombie>(x => x.Bite, (x, ref _) => x.theStatus = PlantStatus.Defalut),
    Prefix(
        x => x.HitZombie,
        (x, ref z) =>
        {
            z.KnockBack(Time.deltaTime);
            z.TakeDamage(DmgType.Normal, x.Damage);

            switch (Random.Range(0, 3))
            {
                case 0:
                    Board.Instance.CreateFreeze(x.transform.position);
                    break;
                case 1:
                    Board.Instance.CreateFireLine(x.theBulletRow, x.Damage);
                    break;
                case 2:
                    var column = Mouse.Instance.GetColumnFromX(x.transform.position.x);
                    Board.Instance.SetDoom(column, x.theBulletRow, false, false, default, x.Damage);
                    break;
            }
        }
    ),
    Prefix<int, int>(x => x.TakeDamage, (_, ref _, ref _) => s_vulnerable),
    Prefix<int, int, Zombie>(x => x.Crashed, (_, ref _, ref _, ref z) => z.PushBack()),
    Prefix<UltimateFootballZombie>(
        x => x.AttackEffect,
        (x, ref p) =>
        {
            s_vulnerable = true;
            p.TakeDamage(x.theAttackDamage);
            s_vulnerable = false;
        }
    ),
    Prefix<SubmarineZombie>(x => x.OnTriggerStay2D, (x, ref z) => PushBack(x, z, 500))
)
{
    static bool s_vulnerable;
}
```

#### BarleyNuts

```cs
public sealed class Plugin() : PlantInjector<Plugin, WallNut, Bullet>(
    new(810)
    {
        Health = 4000,
        Price = 150,
        AttackInterval = 0,
        ProductionInterval = 0,
        Cooldown = 30,
        Damage = 100,
        [Prefab] = AddComponent<BarleyNuts>,
    },
    [TypeMgr.IsNut]
)
{
    sealed class ImitatorsBarleyNuts() : PlantInjector<ImitatorsBarleyNuts, ObsidianWallNut, Bullet>(
        new(811)
        {
            Health = 12000,
            Price = 500,
            AttackInterval = 0,
            ProductionInterval = 0,
            Cooldown = 30,
            Damage = 100,
            [Prefab] = AddComponent<BarleyNuts>,
        },
        [TypeMgr.IsFirePlant, TypeMgr.IsNut]
    );

    sealed class ObsidianBarleyNuts() : PlantInjector<ObsidianBarleyNuts, ObsidianWallNut, Bullet>(
        new(812)
        {
            Health = 12000,
            Price = 800,
            AttackInterval = 0,
            ProductionInterval = 0,
            Cooldown = 30,
            Damage = 100,
            [Prefab] = AddComponent<BarleyNuts>,
        },
        [TypeMgr.IsFirePlant, TypeMgr.IsNut],
        PrefixUnsafe<CreatePlant, System.Func<int, int, PlantType, Plant, Vector2, bool, bool, Plant, GameObject>,
            ActCreatePlantSetPlantPatch>(
            x => x.SetPlant,
            (_, ref _, ref _, ref _, ref p, ref _, ref _, ref _, ref _, ref _) =>
            {
                if (p == Plant.Type && Random.value < 0.5)
                    p = ImitatorsBarleyNuts.Plant.Type;
            }
        )
    );

    static readonly System.Collections.Generic.List<PlantType> s_nutFusions = [],
        s_ultiNutFusions = [PlantType.UltimateChomper, PlantType.GarlicUltimateChomper];

    public static System.Collections.Generic.IReadOnlyList<PlantType> NutFusions => s_nutFusions;

    public static System.Collections.Generic.IReadOnlyList<PlantType> UltiNutFusions => s_ultiNutFusions;

    public static void FindNutPlants()
    {
        if (s_nutFusions.Count is not 0)
            return;

        var fusions = Fusions;

        foreach (var type in GameAPP.resourcesManager.allPlants)
            if ((fusions.Get(type, PlantType.WallNut) ?? fusions.Get(type, PlantType.TallNut)) is { } fusion)
                (Lawnf.IsUltiPlant(fusion) ? s_ultiNutFusions : s_nutFusions).Add(fusion);
    }

    /// <inheritdoc />
    public override void OnInitializeMelon()
    {
        base.OnInitializeMelon();
        new ImitatorsBarleyNuts().OnInitializeMelon();
        new ObsidianBarleyNuts().OnInitializeMelon();
    }
}
```

## Contribute

Issues and pull requests are welcome to help this repository be the best it can be.

## License

This repository falls under the [MPL-2 license](https://www.mozilla.org/en-US/MPL/2.0/).
