using UnityEngine;

public class PetiPassword : MonoBehaviour
{
    public static string passwordRahasiaSaatIni;

    [Header("Deteksi Player (Gizmos)")]
    [Tooltip("Atur seberapa jauh player bisa berinteraksi dengan peti ini")]
    public float radiusInteraksi = 2f;

    [Header("Pengaturan Hadiah Peti")]
    public string idBarangHadiah = "DokumenPenting";
    
    [Header("Dialog Feedback")]
    public DialogData dialogBerhasil;
    public DialogData dialogGagal;

    [Header("Visual Indikator")]
    public GameObject ikonPanah;

    private bool playerDiDekat = false;
    private bool sudahTerbuka = false;

    private void Awake()
    {
        int angkaAcak = Random.Range(0, 10000);
        passwordRahasiaSaatIni = angkaAcak.ToString("D4"); 
    }

    private void Start()
    {
        if (ikonPanah != null) ikonPanah.SetActive(false);
    }

    private void Update()
    {
        CekRadiusPlayer();

        if (playerDiDekat && !sudahTerbuka && Input.GetKeyDown(KeyCode.E))
        {
            if (ikonPanah != null) ikonPanah.SetActive(false); 

            if (UIInputPassword.Instance != null)
            {
                UIInputPassword.Instance.BukaLayarPassword(this);
            }
        }
    }

    // --- SISTEM SENSOR RADIUS ---
    private void CekRadiusPlayer()
    {
        bool terdeteksi = false;
        
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, radiusInteraksi);
        foreach (Collider2D c in cols)
        {
            if (c.CompareTag("Player")) 
            { 
                terdeteksi = true; 
                break; 
            }
        }

        if (terdeteksi && !playerDiDekat)
        {
            playerDiDekat = true;
            if (!sudahTerbuka && ikonPanah != null) ikonPanah.SetActive(true);
        }
        else if (!terdeteksi && playerDiDekat)
        {
            playerDiDekat = false;
            if (ikonPanah != null) ikonPanah.SetActive(false);
        }
    }

    // --- SISTEM GIZMOS ---
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radiusInteraksi);
    }

    public void VerifikasiPassword(string inputUser)
    {
        if (inputUser == passwordRahasiaSaatIni)
        {
            sudahTerbuka = true;
            if (ikonPanah != null) ikonPanah.SetActive(false); 
            
            GlobalBattleState.TambahBarang(idBarangHadiah);
            
            if (DialogManager.Instance != null && dialogBerhasil != null)
            {
                DialogManager.Instance.MulaiDialog(dialogBerhasil, () => { Destroy(gameObject); });
            }
            else Destroy(gameObject);
        }
        else
        {
            if (playerDiDekat && ikonPanah != null) ikonPanah.SetActive(true);

            if (DialogManager.Instance != null && dialogGagal != null)
            {
                DialogManager.Instance.MulaiDialog(dialogGagal, null);
            }
        }
    }
}