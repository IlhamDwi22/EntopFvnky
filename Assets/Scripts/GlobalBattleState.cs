using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class RuntimeItemData
{
    public string id;
    public string nama;
    public Sprite sprite;
}

public static class GlobalBattleState
{
    public static bool kembaliDariBattle = false;
    public static bool playerMenang = false;

    public static DialogData dialogMenang;
    public static DialogData dialogKalah;
    public static string sceneSetelahMenang;
    public static string sceneOverworldAsal;
    public static string namaSceneCombat;
    public static OpponentData dataMusuhAktif;

    public static Vector3 posisiPlayerTerakhir;
    public static bool adaPosisiTersimpan = false;

    public static List<string> daftarBarangQuest = new List<string>();
    public static List<RuntimeItemData> databaseRuntime = new List<RuntimeItemData>();

    public static List<string> daftarPintuTerbuka = new List<string>();
    
    // --- MEMORI PUZZLE ---
    public static string puzzle_sceneAsal; // Variabel baru untuk mengingat tempat asal player
    public static string puzzle_idBarangWajib;
    public static string puzzle_idPintuGlobal;
    public static string puzzle_sceneTujuan;
    public static string puzzle_idPintuTujuan;

    public static void TambahBarang(string idBarang) { if (!daftarBarangQuest.Contains(idBarang)) daftarBarangQuest.Add(idBarang); }
    public static void HapusBarang(string idBarang) { if (daftarBarangQuest.Contains(idBarang)) daftarBarangQuest.Remove(idBarang); }
    public static bool CekBarang(string idBarang) { return daftarBarangQuest.Contains(idBarang); }

    public static void TambahItemRuntime(string id, Sprite sprite)
    {
        bool sudahAda = false;
        foreach (RuntimeItemData item in databaseRuntime) { if (item.id == id) { sudahAda = true; break; } }
        if (!sudahAda)
        {
            RuntimeItemData baru = new RuntimeItemData();
            baru.id = id; baru.nama = id; baru.sprite = sprite;
            databaseRuntime.Add(baru);
        }
    }
    public static RuntimeItemData CariRuntimeItem(string id)
    {
        foreach (RuntimeItemData item in databaseRuntime) { if (item.id == id) return item; }
        return null;
    }
}