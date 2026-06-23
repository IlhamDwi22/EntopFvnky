public static class GlobalBattleState
{
    public static bool kembaliDariBattle = false;
    public static bool playerMenang = false;
    
    // Data untuk diproses saat kembali ke Overworld
    public static DialogData dialogMenang;
    public static DialogData dialogKalah;
    public static string sceneSetelahMenang;
    public static string sceneOverworldAsal;
    public static string namaSceneCombat;
    public static OpponentData dataMusuhAktif;
}