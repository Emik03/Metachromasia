// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace Metachromasia;
#pragma warning disable RCS1158
using Patches = IReadOnlyCollection<Func<Patch>>;
using Strings = IReadOnlyList<string>;

public abstract partial class PlantInjector<TPlugin, TPlant, TBullet> : Localizable<TPlugin>
    where TPlugin : PlantInjector<TPlugin, TPlant, TBullet>, new()
    where TPlant : Plant
    where TBullet : Bullet
{
    public const string Prefab = nameof(Prefab);

    const string Box = "OutOfTheBox", Sub = "%s", Tag = "<color=yellow>";

    static readonly Dictionary<string, BulletType> s_bullets = new(StringComparer.Ordinal);

    [UsedImplicitly]
    static int? s_buffIndex;

    static MelonPreferences_Entry<bool>? s_box;

    static Strings? s_buffs;

    readonly PlantData _plant;

    protected PlantInjector(PlantData p, params Patches h)
        : this(p, null, null, h) { }

    protected PlantInjector(PlantData p, Strings? s = null, params Patches h)
        : this(p, s, null, h) { }

    protected PlantInjector(PlantData p, Strings? s = null, int? i = null, params Patches h)
        : base(h) =>
        (Plant, _plant, s_buffs, s_buffIndex) = (p, p, s, i);

    public static int Item { get; private set; }

    public static int Travel { get; [UsedImplicitly] private set; }

    public static BulletType Bullet { get; private set; }

    public static ParticleType Particle { get; private set; }

    [field: MaybeNull]
    public static Il2CppAssetBundle Bundle =>
        field ??= Il2CppAssetBundleManager.LoadFromMemory(
            $"{typeof(TPlugin).Assembly.GetName().Name}.bundle".Debug() is var name &&
            typeof(TPlugin).GetManifestResource<byte[]>(name) is { } bytes
                ? bytes
                : throw new InvalidOperationException(
                    $"This assembly does not contain the following required file as an embedded resource: {name}"
                )
        );

    public static IReadOnlyDictionary<string, BulletType> Bullets => s_bullets;

    [field: MaybeNull]
    public static PlantData Plant
    {
        get => field ??= new TPlugin()._plant;
        set;
    }

    public static ReadOnlySpan<float> Gatling => [-0.3f, 0, 0.3f];

    public static void AddComponent<T>(GameObject x)
        where T : MonoBehaviour
    {
        if (!ClassInjector.IsTypeRegisteredInIl2Cpp<T>())
            ClassInjector.RegisterTypeInIl2Cpp<T>();

        x.AddComponent<T>();
    }

    public static void AddSortingGroup(GameObject go)
    {
        var x = go.AddComponent<SortingGroup>();
        x.sortingOrder = ushort.MaxValue;
        x.sortingLayerName = "UI";
        x.sortAtRoot = true;
    }

    public static void PushBack(Zombie zombie, Collider2D collider, int damage, bool muted = false)
    {
        if (!collider || collider.GetComponent<TPlant>() is var plant && !plant)
            return;

        zombie.transform.Translate(Vector3.right);
        plant.TakeDamage(damage);

        if (muted)
            GameAPP.PlaySound(Random.Range(72, 75));
    }

    public static Action<Bullet> Pierce(int times) => x => x.penetrationTimes = times;

    public static Action<Bullet> Rotate(IReadOnlyList<int> cycle)
    {
        Debug.Assert(cycle is not null and not []);
        var i = -1;
        return x => x.Rotate(x.gameObject, cycle[i = (i + 1) % cycle.Count]);
    }

    public static Action<Bullet> Rotate(int min, int max) => x => x.Rotate(x.gameObject, Random.Range(min, max));

    public static void RegisterItem(GameObject go)
    {
        for (var i = 0; i < GameAPP.itemPrefab.Count; i++)
            if (!GameAPP.itemPrefab[i])
            {
                AddSortingGroup(GameAPP.itemPrefab[Item = i] = go);
                return;
            }

        GameAPP.itemPrefab = ResizeAndAdd(GameAPP.itemPrefab, go);
    }

    public static void RegisterParticle(GameObject go)
    {
        for (var i = 0; i < GameAPP.particlePrefab.Count; i++)
            if (!GameAPP.particlePrefab[i])
            {
                Particle = (ParticleType)i;
                GameAPP.particlePrefab[i] = go;
                return;
            }

        GameAPP.particlePrefab = ResizeAndAdd(GameAPP.particlePrefab, go);
    }

    public static bool HasBuff(string buff) => Lawnf.TravelAdvanced((AdvBuff)GetBuff(buff));

    public static bool HasMixBomb(CreatePlant instance, int column, int row, PlantType target)
    {
        foreach (var v in ToSpan2D<BoardGrid>(instance.board.boardGrid)[column, row].plants)
            if (v && v.thePlantType == target)
                return true;

        return false;
    }

    public static bool Jicamagic(
        MixBomb instance,
        PlantType left,
        PlantType middle,
        PlantType? right = null,
        params ReadOnlySpan<PlantType> results
    )
    {
        foreach (var p in Lawnf.Get1x1Plants(instance.thePlantColumn, instance.thePlantRow))
        {
            if (!p || p.thePlantType != middle)
                continue;

            var l = Lawnf.GetCertainPlant(p.thePlantColumn - 1, p.thePlantRow, left, instance.board);
            var r = Lawnf.GetCertainPlant(p.thePlantColumn + 1, p.thePlantRow, right ?? left, instance.board);

            if (!l || !r)
            {
                InGameText.Instance.ShowText(Localize(nameof(Jicamagic)).ToString(), 2);
                continue;
            }

            foreach (var result in results.IsEmpty ? [Plant.Type] : results)
                Lawnf.SetDroppedCard(p.transform.position + Vector3.up, result);

            l.Die();
            r.Die();
            p.Die();
            instance.Die();
            GameAPP.PlaySound(125);
            return false;
        }

        return true;
    }

    public static bool Matches(Bullet? bullet) => bullet && bullet is { theBulletType: var type } && type == Bullet;

    public static bool Matches(Bullet? bullet, string? name) =>
        bullet && bullet is { theBulletType: var type } && type == (name is null ? Bullet : Bullets[name]);

    public static bool Matches(Plant? plant) => plant && plant is { thePlantType: var type } && type == Plant.Type;

    public static int GetBuff(string buff)
    {
        if (s_buffs is null)
            return Travel;

        for (var i = 0; i < s_buffs.Count; i++)
            if (buff == s_buffs[i])
                return Travel + i;

        return Travel;
    }

    public static Action<GameObject> GetBulletRegistrator<T>(string? name = null)
        where T : Bullet =>
        x =>
        {
            var bulletType = GameAPP.resourcesManager.allBullets.Count + BulletType.Bullet_zombieBlock;
            x.AddComponent<T>().theBulletType = bulletType;
            _ = name is null ? Bullet = bulletType : s_bullets[name] = bulletType;
            GameAPP.resourcesManager.bulletPrefabs.Add(bulletType, x);
            GameAPP.resourcesManager.allBullets.Add(bulletType);
        };

    public static Action<GameObject> AddComponent<T>(Action<GameObject> action)
        where T : MonoBehaviour =>
        x =>
        {
            action(x);
            AddComponent<T>(x);
        };

    public static Action<GameObject> AddComponents<T1, T2>(Action<GameObject> action)
        where T1 : MonoBehaviour
        where T2 : MonoBehaviour =>
        AddComponent<T2>(AddComponent<T1>(action));

    public static PlantType? Fuse(PlantType a, PlantType b) => MixData.TryGetMix(a, b, out var c, false) ? c : null;

    /// <inheritdoc />
    public override void OnInitializeMelon()
    {
        base.OnInitializeMelon();
        var category = MelonPreferences.CreateCategory(nameof(Metachromasia));
        s_box = category.GetEntry<bool>(Box) ?? category.CreateEntry(Box, false);

        if (Enum.IsDefined(Plant.Type))
            LoggerInstance?.Error("Plant type conflict: {PlantType} ({PlantId})", Plant.Type, Plant.Id);

        foreach (var type in GetType().Assembly.GetExportedTypes().Where(RequiresRegistering))
            ClassInjector.RegisterTypeInIl2Cpp(type);
    }

    protected override void Patch(HarmonyLib.Harmony harmony)
    {
        const BindingFlags Flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

        base.Patch(harmony);

        if (typeof(GameAPP).GetMethod(nameof(GameAPP.Awake), Flags, []) is var gameApp &&
            harmony.GetPatchedMethods().Contains(gameApp))
        {
            LoggerInstance?.Warning("Refusing to patch because the method has already been patched before.");
            return;
        }

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        foreach (var fn in Plant.Tag)
            if (fn is not null)
                harmony.Patch(fn.Method, postfix: new(((Delegate)MatchThisPlant).Method));

        var cheatKey = typeof(CheatKey).GetMethod(nameof(CheatKey.CheckCheatCodes), []);
        // var travelManager = typeof(TravelMgr).GetMethod(nameof(TravelMgr.Awake), Flags, []);
        var seedLibrary = typeof(SeedLibrary).GetMethod(nameof(SeedLibrary.Awake), Flags, []);
        var plantMenu = typeof(AlmanacPlantMenu).GetMethod(nameof(AlmanacPlantMenu.Awake), Flags, []);
        var createPlant = typeof(CreatePlant).GetMethod(nameof(CreatePlant.LimTravel), Flags, [typeof(PlantType)]);
        var plantBank = typeof(AlmanacPlantBank).GetMethod(nameof(AlmanacPlantBank.InitNameAndInfoFromJson), Flags, []);

        harmony.Patch(gameApp, postfix: new(((Delegate)Load).Method));
        harmony.Patch(cheatKey, new(((Delegate)AddOutOfTheBox).Method));
        harmony.Patch(plantMenu, postfix: new(((Delegate)AddPlantMenu).Method));
        harmony.Patch(plantBank, postfix: new(((Delegate)InitBankInfo).Method));
        harmony.Patch(seedLibrary, postfix: new(((Delegate)AddSeedSlot).Method));

        _ = Plant.NoAdventure &&
            harmony.Patch(createPlant, postfix: new(((Delegate)LimTravel).Method)) is var _;
        // ^ s_buffs is { Count: not 0 } &&
        // harmony.Patch(travelManager, postfix: new(((Delegate)Buff).Method)) is var _;
    }

    // ReSharper disable InconsistentNaming
    static void AddOutOfTheBox(CheatKey __instance) =>
        __instance.CheatKeys.TryAdd(Box.ToLowerInvariant(), (Action)ToggleOutOfTheBox);

    static void AddPlantMenu(AlmanacPlantMenu __instance)
    {
        const string Size = "<size=36>";

        // This check isn't necessary but this saves us multiple allocations if false.
        _ = !AlmanacPlantMenu.PlantAlmanacData.ContainsKey(Plant.Type) &&
            AlmanacPlantMenu.PlantAlmanacData.TryAdd(
                Plant.Type,
                new()
                {
                    cost = (Localize("Cost", "") is var c && c.StartsWith(Size) ? c[Size.Length..] : c).ToString(),
                    info = Localize("Description").ToString(),
                    introduce = Localize("Introduce", "").ToString(),
                    name = (Localize("Name") is var name && name.IndexOf(Sub) is not -1 and var i
                        ? name[..(i - 1)].Trim()
                        : name).ToString(),
                    seedType = Plant.Id,
                }
            );
    }

    static void InitBankInfo(AlmanacPlantBank __instance)
    {
        if (__instance.theSeedType != Plant.Id)
            return;

        for (var i = 0; i < __instance.transform.childCount && __instance.transform.GetChild(i) is var c; i++)
            switch (c ? c.name : null)
            {
                case "Name":
                    (c.GetComponent<TextMeshPro>().text, c.GetChild(0).GetComponent<TextMeshPro>().text) = Name();
                    break;
                case "Info":
                    var mesh = c.GetComponent<TextMeshPro>();
                    mesh.overflowMode = TextOverflowModes.Page;
                    mesh.text = Description();

                    if (Localize("Cost", "") is not [] and var cost)
                        mesh.text += $"\n\n{cost}";

                    mesh.fontSize = 36; // 40
                    break;
                // case "Cost":
                //     c.GetComponent<TextMeshPro>().text = Localize("Cost", "").ToString();
                //     break;
            }
    }

    // static void Buff(TravelMgr __instance)
    // {
    //     Debug.Assert(s_buffs is not null);
    //
    //     if (Travel is 0)
    //         Travel = TravelMgr.advancedBuffs.Count;
    //
    //     if (s_buffIndex is null)
    //     {
    //         var upgrades = new bool[Travel + s_buffs.Count];
    //         __instance.advancedUpgrades.AsSpan().CopyTo(upgrades);
    //         __instance.advancedUpgrades = upgrades;
    //     }
    //
    //     using var e = s_buffs.GetEnumerator();
    //
    //     for (var i = s_buffIndex ?? Travel; e.MoveNext(); i++)
    //         TravelMgr.advancedBuffs[i] = Localize(e.Current).ToString();
    // }

    static void Load()
    {
        const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public;

        if (PlantDataManager.PlantData_Default.ContainsKey(Plant.Type))
            return;

        GameAPP.resourcesManager.allPlants.Add(Plant.Type);
        _ = Plant.Tag.Has(Metachromasia.Tag.AntiCrush) && TypeMgr.UncrashablePlants.Add(Plant.Type);

        PlantDataManager.PlantData_Modified[Plant.Type] = PlantDataManager.PlantData_Default[Plant.Type] =
            (PlantDataManager.PlantData)Plant;

        foreach (var asset in Bundle.LoadAllAssets())
            if (asset && asset.TryCast<GameObject>() is { } go)
                ProcessGameObject(go, Plant.Prefix);

        foreach (var (l, r) in Plant.Fusions)
        {
            var inversion = r <= PlantType.Nothing;
            var result = inversion ? ~r : Plant.Type;
            var ingredient = inversion ? Plant.Type : r;
            MixData.AddRecipe(l, ingredient, result);
        }

        if (Bullet is not BulletType.Bullet_pea &&
            typeof(Shooter).IsAssignableFrom(typeof(TPlant)) &&
            GetHarmony() is { } harmony &&
            typeof(TPlant).GetMethod(nameof(Shooter.GetBulletType), Flags) is var shooter)
            harmony.Patch(shooter, new(((Delegate)GetBulletType).Method));
    }

    // ReSharper disable once CognitiveComplexity
    static void ProcessGameObject(GameObject go, string? prefix)
    {
        prefix ??= typeof(TPlugin).Name is null or "Plugin" // ReSharper disable once NullableWarningSuppressionIsUsed
            ? typeof(TPlugin).Assembly.GetName().Name!
            : typeof(TPlugin).Name;

        new KeyValuePair<string, string>(go.GetIl2CppType().FullName, go.name).Debug();
        var span = go.name.AsSpan(go.name.StartsWith(prefix) ? prefix.Length : 0);
        var rm = GameAPP.resourcesManager;

        switch (span)
        {
            case "Bulet" or "Bullet":
                GetBulletRegistrator<TBullet>()(go);
                break;
            case "Particle":
                RegisterParticle(go);
                break;
            case Prefab or "SkinPrefab":
                RegisterPrefab(go);
                _ = span is Prefab ? rm.plantPrefabs.TryAdd(Plant.Type, go) : rm.plantSkinDic.TryAdd(Plant.Type, 0);
                break;
            case "Preview" or "SkinPreview":
                RegisterPreview(go);
                _ = span is "Preview" && rm.plantPreviews.TryAdd(Plant.Type, go);
                break;
        }

        foreach (var (key, match) in Plant)
            if (span.Equals(key, StringComparison.Ordinal))
            {
                match(go);
                break;
            }
    }

    static void Add<T>(
        Il2CppSystem.Collections.Generic.Dictionary<PlantType, Il2CppSystem.Collections.Generic.List<T>> dictionary,
        T value
    )
    {
        _ = dictionary.TryGetValue(Plant.Type, out var v) || dictionary.TryAdd(Plant.Type, v = new());
        v.Add(value);
    }

    static void RegisterPreview(GameObject go)
    {
        go.tag = "Preview";
        Add(GameAPP.resourcesManager._plantPreviews, go);
    }

    static void RegisterPrefab(GameObject go)
    {
        const string Shadow = nameof(Shadow);

        if (!go.transform.Find(Shadow))
        {
            var type = typeof(TPlant) == typeof(Plant) ? default : Enum.Parse<PlantType>(typeof(TPlant).Name, true);
            var shadow = GameAPP.resourcesManager.plantPrefabs[type].transform.Find(Shadow);
            var copy = Object.Instantiate(shadow);
            // I have no idea why, but specifying the parent in Object.Instantiate breaks all of our overrides.
            // ReSharper disable once Unity.InstantiateWithoutParent
            copy.transform.SetParent(go.transform, false);
            copy.name = Shadow;
        }

        var plant = go.AddComponent<TPlant>();
        plant.tag = nameof(Il2Cpp.Plant);
        plant.thePlantType = Plant.Type;
        plant.plantTag = Plant.Tag.ToPlantTag();

        if (Plant.AttributeCount is not 0)
            plant.attributeCount = Plant.AttributeCount;

        go.gameObject.layer = LayerMask.NameToLayer("Plant");
        Add(GameAPP.resourcesManager._plantPrefabs, go);
    }

    static void ToggleOutOfTheBox()
    {
        Debug.Assert(s_box is not null);
        var state = (s_box.Value = !s_box.Value) ? "ON" : "OFF";
        var text = $"{nameof(Metachromasia)}'s \"Out of the Box\" feature flag is now {state}";
        InGameText.Instance.ShowText(text, 5);
    }

    static void AddSeedSlot(SeedLibrary __instance)
    {
        if (s_box is not { Value: true } && !Plant.AddSeedSlot)
            return;

        var prefab = GameAPP.resourcesManager.plantPrefabs[Plant.Type];
        var head = __instance.transform.Find("ColorfulCards/Page1");
        var name = prefab.name[..^Prefab.Length];

        if (head.Find(name))
            return;

        var newSeed = Object.Instantiate(head.GetChild(0).gameObject);
        newSeed.name = name;

        for (var i = 0; i < newSeed.transform.childCount && newSeed.transform.GetChild(i) is var child; i++)
            switch (child.name.AsSpan().Trim())
            {
                case "Packet":
                    bool free = IZBottomMenu.Instance;
                    var ui = child.GetOrAddComponent<CardUI>();
                    ui.enabled = true;
                    ui.parent = newSeed;
                    ui.theSeedType = Plant.Id;
                    ui.thePlantType = Plant.Type;
                    ui.CD = free ? 0 : ui.fullCD;
                    ui.fullCD = free ? 0 : Plant.Cooldown;
                    ui.theSeedCost = free ? 0 : Plant.Price;
                    Mouse.Instance.ChangeCardSprite(Plant.Type, ui);
                    break;
                case "PacketBg":
                    var g = child.Find("Icon").GetOrAddComponent<Image>();
                    g.preserveAspect = true;
                    g.sprite = GameAPP.resourcesManager.plantPreviews[Plant.Type].GetComponent<SpriteRenderer>().sprite;
                    child.Find("Cost").GetComponent<TextMeshProUGUI>().text = Plant.Price.ToString();
                    break;
            }

        // I have no idea why, but specifying the parent in Object.Instantiate breaks all of our overrides.
        // ReSharper disable once Unity.InstantiateWithoutParent
        newSeed.transform.SetParent(head, false);
    }

    static void LimTravel(ref bool __result, ref PlantType theSeedType)
    {
        if (theSeedType != Plant.Type)
            return;

        if (GameAPP.theBoardType is not LevelType.Advanture)
        {
            __result = false;
            return;
        }

        __result = true;
        InGameText.Instance.ShowText(Localize("NoAdventure").ToString(), 4);
    }

    static bool GetBulletType(Shooter __instance, ref BulletType __result)
    {
        if (!Matches(__instance))
            return true;

        __result = Bullet;
        return false;
    }

    static bool RequiresRegistering(Type x) =>
        typeof(Il2CppObjectBase).IsAssignableFrom(x) && !ClassInjector.IsTypeRegisteredInIl2Cpp(x);

    static string Description()
    {
        var intro = Localize("Introduce", "");
        var desc = Localize("Description");
        var pad = intro.IsEmpty ? "" : "\n\n";
        var capacity = intro.Length + pad.Length + desc.Length;
        return new StringBuilder(capacity).Append(intro).Append(pad).Append(desc).ToString();
    }

    static (string WithTag, string WithoutTag) Name()
    {
        var span = Localize("Name");
        var id = Plant.Id.ToString();
        var capacity = Tag.Length + span.Length - Sub.Length + Math.Max(Sub.Length, id.Length);
        var ret = new StringBuilder(Tag, capacity).Append(span).Replace(Sub, id).ToString();
        return (ret, ret[Tag.Length..]);
    }

    // ReSharper disable once SuggestBaseTypeForParameter
    static Il2CppReferenceArray<GameObject?> ResizeAndAdd(Il2CppReferenceArray<GameObject> prefabs, GameObject go)
    {
        Il2CppReferenceArray<GameObject?> expandedPrefabs = new(prefabs.Count * 2) { [prefabs.Count] = go };

        for (var i = 0; i < prefabs.Count; i++)
            expandedPrefabs[i] = prefabs[i];

        return expandedPrefabs;
    }
}
