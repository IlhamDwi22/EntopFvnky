using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class RuntimeItemData
{
    public string id;
    public string nama;
    public Sprite sprite;
    public string isiTeks;
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
    public static List<string> daftarPetiTerbuka = new List<string>();

    public static void BukaPeti(string idPeti) { if (!daftarPetiTerbuka.Contains(idPeti)) daftarPetiTerbuka.Add(idPeti); }
    public static bool CekPetiTerbuka(string idPeti) { return daftarPetiTerbuka.Contains(idPeti); }
    
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
        TambahItemRuntime(id, sprite, "");
    }

    public static void TambahItemRuntime(string id, Sprite sprite, string isiTeks)
    {
        bool sudahAda = false;
        foreach (RuntimeItemData item in databaseRuntime) { if (item.id == id) { sudahAda = true; break; } }
        if (!sudahAda)
        {
            RuntimeItemData baru = new RuntimeItemData();
            baru.id = id; baru.nama = id; baru.sprite = sprite; baru.isiTeks = isiTeks;
            databaseRuntime.Add(baru);
        }
    }
    public static RuntimeItemData CariRuntimeItem(string id)
    {
        foreach (RuntimeItemData item in databaseRuntime) { if (item.id == id) return item; }
        return null;
    }
}