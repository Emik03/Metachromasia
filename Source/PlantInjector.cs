// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace Metachromasia;

using Patches = IReadOnlyCollection<Func<Patch>>;

/// <summary>A class responsible for registering one modded plant.</summary>
/// <remarks><para>Multiple plants have to be registered using separate classes.</para></remarks>
/// <typeparam name="TPlugin">
/// The type implementing the modded plant. Referencing <see cref="Plant"/> such as through multiple plants
/// in the same assembly to look up values from one another can cause a temporary instance to be created of
/// <typeparamref name="TPlugin"/>. It is therefore imperative that the parameterless constructor should not have
/// any side effects, and these classes cannot have a circular dependency on each other through <see cref="Plant"/>.
/// </typeparam>
/// <typeparam name="TPlant">
/// The type of <see cref="Il2Cpp.Plant"/> that the modded plant is based on. This will typically be the plant with the
/// most similar behavior to the plant being added, especially if this plant is part of the ingredient list of its
/// fusions. The specified <see cref="Il2Cpp.Plant"/> automatically gets added to the prefab, and patches done on the
/// constructor can infer <typeparamref name="TPlant"/> to refer to this specific modded plant, removing the need to
/// validate the plant type. If no vanilla plant fits this use case, you can simply specify <see cref="Il2Cpp.Plant"/>,
/// although this will not instantiate any components onto the registered prefab.
/// </typeparam>
/// <typeparam name="TBullet">
/// The type of <see cref="Il2Cpp.Bullet"/> that the modded bullet is based on. Instantiation and patching work
/// identically to <typeparamref name="TPlant"/>, except <see cref="Il2Cpp.Bullet"/> should be specified if no vanilla
/// bullet fits this use case. If a bullet is registered, <see cref="Bullet"/> will contain the registered bullet type,
/// and can be assumed to contain a <typeparamref name="TBullet"/> component. Unlike <typeparamref name="TPlant"/>, it
/// is possible to register multiple bullets using <see cref="GetBulletRegistrator"/>, however the instantiation and
/// patching will only apply to the main bullet being registered, which also includes the value of <see cref="Bullet"/>.
/// This generic can therefore be thought of as the "default" bullet if specified, and any additional bullets need to
/// use manual checks for patching and will be contained in <see cref="Bullets"/>.
/// </typeparam>
public abstract partial class PlantInjector<TPlugin, TPlant, TBullet> : Localizable<TPlugin>
    where TPlugin : PlantInjector<TPlugin, TPlant, TBullet>, new()
    where TPlant : Plant
    where TBullet : Bullet
{
    /// <summary>The text <c>Prefab</c> which is the suffix used to register the prefab.</summary>
    public const string Prefab = nameof(Prefab);

    /// <summary>Used for string searching.</summary>
    const string Box = "OutOfTheBox", Sub = "%s", Tag = "<color=yellow>";

    /// <summary>The bullets registered using <see cref="GetBulletRegistrator"/>.</summary>
    static readonly Dictionary<string, BulletType> s_bullets = [with(StringComparer.Ordinal)];

    /// <summary>
    /// The starting index for the list of buffs. If <see langword="null"/>, these buffs are additions.
    /// If specified, these override the existing <see cref="AdvBuff"/>.
    /// </summary>
    static int? s_buffIndex;

    /// <summary>Keeps the <c>outofthebox</c> cheat code persistent across sessions.</summary>
    static MelonPreferences_Entry<bool>? s_box;

    /// <summary>Contains the buffs to register.</summary>
    static IReadOnlyList<string>? s_buffs;

    /// <summary>Contains the plant data.</summary>
    readonly PlantData _plant;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlantInjector{TPlugin, TPlant, TBullet}"/> class.
    /// </summary>
    /// <param name="p">The plant data.</param>
    /// <param name="h">The patches.</param>
    protected PlantInjector(PlantData p, params Patches h)
        : this(p, null, null, h) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PlantInjector{TPlugin, TPlant, TBullet}"/> class.
    /// </summary>
    /// <param name="p">The plant data.</param>
    /// <param name="s">The buffs.</param>
    /// <param name="h">The patches.</param>
    protected PlantInjector(PlantData p, IReadOnlyList<string>? s = null, params Patches h)
        : this(p, s, null, h) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PlantInjector{TPlugin, TPlant, TBullet}"/> class.
    /// </summary>
    /// <param name="p">The plant data.</param>
    /// <param name="s">The buffs.</param>
    /// <param name="i">
    /// The starting index for the list of buffs. If <see langword="null"/>, these buffs are additions.
    /// If specified, these override the existing <see cref="AdvBuff"/>.
    /// </param>
    /// <param name="h">The patches.</param>
    protected PlantInjector(PlantData p, IReadOnlyList<string>? s = null, int? i = null, params Patches h)
        : base(h) =>
        (Plant, _plant, s_buffs, s_buffIndex) = (p, p, s, i);

    /// <summary>Contains the index in <see cref="GameAPP.itemPrefab"/> of the registered item, if any.</summary>
    public static int Item { get; private set; }

    /// <summary>Contains the starting buff index used for <see cref="Lawnf.TravelAdvanced"/>, if any.</summary>
    public static int Travel { get; [UsedImplicitly] private set; }

    /// <summary>Contains the implicitly registered <see cref="BulletType"/>.</summary>
    public static BulletType Bullet { get; private set; }

    /// <summary>Contains the implicitly registered <see cref="ParticleType"/>.</summary>
    public static ParticleType Particle { get; private set; }

    /// <summary>
    /// Contains the explicitly registered <see cref="BulletType"/>
    /// instances registered with <see cref="GetBulletRegistrator"/>.
    /// </summary>
    public static IReadOnlyDictionary<string, BulletType> Bullets => s_bullets;

    /// <summary>Gets the plant data.</summary>
    /// <remarks><para>
    /// Accessing this property may cause a temporary instantiation of <typeparamref name="TPlugin"/>.
    /// </para></remarks>
    [field: MaybeNull]
    public static PlantData Plant
    {
        get => field ??= new TPlugin()._plant;
        set;
    }

    /// <summary>Contains the heights for three bullets for the height of super gatling shooters.</summary>
    public static ReadOnlySpan<float> Gatling => [-0.3f, 0, 0.3f];

    /// <summary>Pushes back the zombie if collided with this plant.</summary>
    /// <param name="zombie">The zombie to potentially push back.</param>
    /// <param name="collider">The collider.</param>
    /// <param name="damage">The amount of damage the plant should take.</param>
    /// <param name="muted">Whether to mute the sound.</param>
    public static void PushBack(Zombie zombie, Collider2D collider, int damage, bool muted = false)
    {
        if (!collider || collider.GetComponent<TPlant>() is var plant && !Matches(plant))
            return;

        zombie.transform.Translate(Vector3.right);
        plant.TakeDamage(damage);

        if (muted)
            GameAPP.PlaySound(Random.Range(72, 75));
    }

    /// <summary>Gets the <see cref="Action{T}"/> for assigning <see cref="Bullet.penetrationTimes"/>.</summary>
    /// <param name="times">The value to assign.</param>
    /// <returns>
    /// The <see cref="Action{T}"/> that assigns <see cref="Bullet.penetrationTimes"/>
    /// to the parameter <paramref name="times"/>.
    /// </returns>
    public static Action<Bullet> Pierce(int times) => b => b.penetrationTimes = times;

    /// <summary>Gets the <see cref="Action{T}"/> for rotating a bullet in a cycle.</summary>
    /// <param name="cycle">The cycle of angles to rotate the bullet by.</param>
    /// <returns>
    /// The <see cref="Action{T}"/> that rotates a bullet in the
    /// cycle as defined by the parameter <paramref name="cycle"/>.
    /// </returns>
    public static Action<Bullet> Rotate(IReadOnlyList<int> cycle)
    {
        if (cycle is null or [])
            throw new InvalidOperationException($"{nameof(Rotate)}'s {nameof(cycle)} should not be empty or null.");

        var i = -1;
        return x => x.Rotate(x.gameObject, cycle[i = (i + 1) % cycle.Count]);
    }

    /// <summary>Gets the <see cref="Action{T}"/> for rotating a bullet within a random range.</summary>
    /// <param name="min">The minimum angle to rotate by.</param>
    /// <param name="max">The maximum angle to rotate by.</param>
    /// <returns>
    /// The <see cref="Action{T}"/> that rotates a bullet within the range of
    /// the parameters <paramref name="min"/> and <paramref name="max"/>.
    /// </returns>
    public static Action<Bullet> Rotate(int min, int max) => x => x.Rotate(x.gameObject, Random.Range(min, max));

    /// <summary>Registers an item into <see cref="Item"/>.</summary>
    /// <param name="go">The <see cref="GameObject"/> to register as an item.</param>
    public static void RegisterItem(GameObject go)
    {
        for (var i = 0; i < GameAPP.itemPrefab.Count; i++)
            if (!GameAPP.itemPrefab[i])
            {
                AddSortingGroup(GameAPP.itemPrefab[Item = i] = go);
                return;
            }

        Item = GameAPP.itemPrefab.Count;
        GameAPP.itemPrefab = ResizeAndAdd(GameAPP.itemPrefab, go);
    }

    /// <summary>Registers a particle into <see cref="Particle"/>.</summary>
    /// <param name="go">The <see cref="GameObject"/> to register as a particle.</param>
    public static void RegisterParticle(GameObject go)
    {
        for (var i = 0; i < GameAPP.particlePrefab.Count; i++)
            if (!GameAPP.particlePrefab[i])
            {
                Particle = (ParticleType)i;
                GameAPP.particlePrefab[i] = go;
                return;
            }

        Particle = (ParticleType)GameAPP.particlePrefab.Length;
        GameAPP.particlePrefab = ResizeAndAdd(GameAPP.particlePrefab, go);
    }

    /// <summary>Determines whether the buff is active.</summary>
    /// <param name="buff">The buff to determine.</param>
    /// <returns>Whether the parameter <paramref name="buff"/> is active.</returns>
    public static bool HasBuff(string buff) => Lawnf.TravelAdvanced((AdvBuff)GetBuff(buff));

    /// <summary>Check for <see cref="CreatePlant.MixBombCheck"/>.</summary>
    /// <param name="instance">The instance.</param>
    /// <param name="column">The column.</param>
    /// <param name="row">The row.</param>
    /// <param name="target">The target.</param>
    /// <returns>Whether there is the parameter <paramref name="target"/> at the location.</returns>
    public static bool HasMixBomb(CreatePlant instance, int column, int row, PlantType target)
    {
        foreach (var v in ToSpan2D<BoardGrid>(instance.board.boardGrid)[column, row].plants)
            if (v && v.thePlantType == target)
                return true;

        return false;
    }

    /// <summary>Check for <see cref="MixBomb.AttributeEvent"/>.</summary>
    /// <param name="instance">The instance.</param>
    /// <param name="left">The plant to the left of the planted Jicamagic.</param>
    /// <param name="middle">The plant on the planted Jicamagic.</param>
    /// <param name="right">
    /// The plant to the right of the planted Jicamagic.
    /// Infers to be the parameter <paramref name="left"/> if <see langword="null"/>.
    /// </param>
    /// <param name="results">The resulting plants to drop. Infers to be this plant type if empty.</param>
    /// <returns>Whether Jicamagic merged the plants.</returns>
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

    /// <summary>Determines whether the <see cref="Bullet"/> matches the parameter.</summary>
    /// <param name="bullet">The bullet to match.</param>
    /// <returns>Whether the parameter <paramref name="bullet"/> has the same type as <see cref="Bullet"/>.</returns>
    public static bool Matches(Bullet? bullet) => bullet && bullet is { theBulletType: var type } && type == Bullet;

    /// <summary>Determines whether the <see cref="Bullets"/> matches the parameter.</summary>
    /// <param name="bullet">The bullet to match.</param>
    /// <param name="name">The bullet to match from <see cref="Bullets"/>.</param>
    /// <returns>Whether the parameter <paramref name="bullet"/> has the same type as <see cref="Bullets"/>.</returns>
    public static bool Matches(Bullet? bullet, string? name) =>
        bullet && bullet is { theBulletType: var type } && type == (name is null ? Bullet : Bullets[name]);

    /// <summary>Determines whether the <see cref="Plant"/> matches the parameter.</summary>
    /// <param name="plant">The plant to match.</param>
    /// <returns>Whether the parameter <paramref name="plant"/> has the same type as <see cref="Plant"/>.</returns>
    public static bool Matches(Plant? plant) => plant && plant is { thePlantType: var type } && type == Plant.Type;

    /// <summary>Gets the buff index from the buff string.</summary>
    /// <param name="buff">The buff string to get the buff index of.</param>
    /// <returns>The index of the parameter <paramref name="buff"/>.</returns>
    public static int GetBuff(string buff)
    {
        if (s_buffs is null)
            return Travel;

        for (var i = 0; i < s_buffs.Count; i++)
            if (buff == s_buffs[i])
                return Travel + i;

        return Travel;
    }

    /// <summary>Gets the bullet registrator.</summary>
    /// <typeparam name="T">The type of bullet to add.</typeparam>
    /// <param name="name">The name of the bullet, which if specified will append it to <see cref="Bullets"/>.</param>
    /// <returns>The <see cref="Action{T}"/> that registers the bullet.</returns>
    public static Action<GameObject> GetBulletRegistrator<T>(string? name = null)
        where T : Bullet =>
        go =>
        {
            var bulletType = GameAPP.resourcesManager.allBullets.Count + BulletType.Bullet_zombieBlock;
            go.AddComponent<T>().theBulletType = bulletType;
            _ = name is null ? Bullet = bulletType : s_bullets[name] = bulletType;
            GameAPP.resourcesManager.bulletPrefabs.Add(bulletType, go);
            GameAPP.resourcesManager.allBullets.Add(bulletType);
        };

    /// <summary>Wraps the <see cref="Action{T}"/> with adding the component.</summary>
    /// <typeparam name="T">The type of component to add.</typeparam>
    /// <param name="action">The <see cref="Action{T}"/> to wrap.</param>
    /// <returns>
    /// The wrapped <see cref="Action{T}"/> that invokes <paramref name="action"/>
    /// and adds the component <typeparamref name="T"/>.
    /// </returns>
    public static Action<GameObject> AddComponent<T>(Action<GameObject> action)
        where T : MonoBehaviour =>
        go =>
        {
            action(go);
            AddComponent<T>(go);
        };

    /// <summary>Wraps the <see cref="Action{T}"/> with adding 2 components.</summary>
    /// <typeparam name="T1">The first type of component to add.</typeparam>
    /// <typeparam name="T2">The second type of component to add.</typeparam>
    /// <param name="action">The <see cref="Action{T}"/> to wrap.</param>
    /// <returns>
    /// The wrapped <see cref="Action{T}"/> that invokes <paramref name="action"/>
    /// and adds the component <typeparamref name="T1"/> and <typeparamref name="T2"/>.
    /// </returns>
    public static Action<GameObject> AddComponents<T1, T2>(Action<GameObject> action)
        where T1 : MonoBehaviour
        where T2 : MonoBehaviour =>
        AddComponent<T2>(AddComponent<T1>(action));

    /// <summary>Gets the fusion result of two plants.</summary>
    /// <param name="a">The first plant.</param>
    /// <param name="b">The second plant.</param>
    /// <returns>The resulting fusion, if it exists.</returns>
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

    /// <inheritdoc />
    protected override void Patch(HarmonyLib.Harmony harmony)
    {
        const BindingFlags Flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

        base.Patch(harmony);

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        foreach (var fn in Plant.Tag)
            if (fn is not null)
                harmony.Patch(fn.Method, postfix: new(GetMatcher(fn).Method));

        var gameApp = typeof(GameAPP).GetMethod(nameof(GameAPP.Awake), Flags, []);
        var cheatKey = typeof(CheatKey).GetMethod(nameof(CheatKey.CheckCheatCodes), []);
        var seedLibrary = typeof(SeedLibrary).GetMethod(nameof(SeedLibrary.Awake), Flags, []);
        var plantMenu = typeof(AlmanacPlantMenu).GetMethod(nameof(AlmanacPlantMenu.Awake), Flags, []);
        var createPlant = typeof(CreatePlant).GetMethod(nameof(CreatePlant.LimTravel), Flags, [typeof(PlantType)]);
        var plantBank = typeof(AlmanacPlantBank).GetMethod(nameof(AlmanacPlantBank.InitNameAndInfoFromJson), Flags, []);

        harmony.Patch(gameApp, postfix: new(((Delegate)Load).Method));
        harmony.Patch(cheatKey, new(((Delegate)AddOutOfTheBox).Method));
        harmony.Patch(plantBank, postfix: new(((Delegate)InitBankInfo).Method));
        harmony.Patch(plantMenu, postfix: new(((Delegate)AddPlantMenu).Method));
        harmony.Patch(seedLibrary, postfix: new(((Delegate)AddSeedSlot).Method));

        _ = Plant.NoAdventure && harmony.Patch(createPlant, postfix: new(((Delegate)LimTravel).Method)) is var _;
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

        (__instance.plantName.text, __instance.plantName_shadow.text) = Name();
        __instance.introduce.overflowMode = TextOverflowModes.Page;
        __instance.introduce.fontSize = 36; // 40
        __instance.introduce.text = Description();
        // __instance.cost.text = Localize("Cost", "").ToString();

        if (Localize("Cost", "") is not "" and var cost)
            __instance.introduce.text += $"\n\n{cost}";
    }

    static void Load()
    {
        const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public;

        if (PlantDataManager.PlantData_Default.ContainsKey(Plant.Type))
            return;

        GameAPP.resourcesManager.allPlants.Add(Plant.Type);
        _ = Plant.Tag.Has(Metachromasia.Tag.AntiCrush) && TypeMgr.UncrashablePlants.Add(Plant.Type);

        PlantDataManager.PlantData_Modified[Plant.Type] = PlantDataManager.PlantData_Default[Plant.Type] =
            (PlantDataManager.PlantData)Plant;

        foreach (var asset in PlantData.GetAssets<TPlugin>())
            if (asset && asset.TryCast<GameObject>() is { } go && go)
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

        if (s_buffs is not { Count: 0 })
            return;

        if (Travel is 0)
            Travel = TravelMgr.AdvBuffData.Count;

        using var e = s_buffs.GetEnumerator();

        for (var i = s_buffIndex ?? Travel; e.MoveNext(); i++)
            TravelMgr.AdvBuffData[(AdvBuff)i] = new ModdedBuff(i, Localize(e.Current).ToString());
    }

    static void ProcessGameObject(GameObject go, string? prefix)
    {
        prefix ??= typeof(TPlugin).Name is null or "Plugin"
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
        var head = __instance.cardPagesContainer.Find("ColorfulCards/Page1");
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
}
