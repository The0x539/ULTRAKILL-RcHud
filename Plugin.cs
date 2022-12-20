﻿using System;
using System.Collections.Generic;

using BepInEx;

using HarmonyLib;

using UnityEngine;

namespace LeatHud;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public sealed class Plugin : BaseUnityPlugin {
    public void Awake() {
        LeatHud.Config.Init(this.Config);
        Harmony.CreateAndPatchAll(typeof(Plugin));
    }

    public void Start() {
        RacecarHud.SetLogger(this.Logger);
    }

    public void Update() {
        _ = RacecarHud.Instance;

        var musicManager = MusicManager.Instance;
        if (musicManager != null) {
            if (musicManager.battleTheme.volume > 0 || musicManager.bossTheme.volume > 0) {
                RefreshBothIcons();
            }
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(FistControl), "UpdateFistIcon")]
    static void RefreshFistIconOnSwitch() {
        if (LeatHud.Config.RefreshFistOnSwitch) {
            RacecarHud.Instance.RefreshFist();
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(FistControl), "UpdateFistIcon")]
    [HarmonyPatch(typeof(Punch), "PunchStart")]
    static void RefreshFistIconOnPunch() {
        if (LeatHud.Config.RefreshFistOnPunch) {
            RacecarHud.Instance.RefreshFist();
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(GunControl), "SwitchWeapon", new Type[] { typeof(int) })]
    [HarmonyPatch(typeof(GunControl), "SwitchWeapon", new Type[] { typeof(int), typeof(List<GameObject>), typeof(bool), typeof(bool) })]
    static void RefreshGunIcon() {
        if (LeatHud.Config.RefreshGunOnSwitch) {
            RacecarHud.Instance.RefreshGun();
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(BossHealthBar), "Update")]
    static void RefreshBothIcons() {
        if (LeatHud.Config.RefreshOnBattleMusic) {

            RacecarHud.Instance.RefreshFist();
            RacecarHud.Instance.RefreshGun();
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(HUDOptions), "HudFade")]
    static void SetIconFade(bool stuff) {
        RacecarHud.Instance.fadeIcons = stuff;
    }
}
